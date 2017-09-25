using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Logging;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.PushNotification;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Services.XLS;
using PDR.Resources.Web.WebApi;
using PDR.Web.Areas.Accountant.Models.Invoice;
using PDR.Web.Areas.Accountant.Models.Log;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Accountant.Controllers
{
    using PDR.Domain.StoredProcedureHelpers;

    [PDRAuthorize(Roles = "Accountant")]
    public class InvoicesController : Common.Controllers.InvoicesController
    {
        private readonly ICompanyRepository<InvoiceLog> invoiceLogsRepository;

        private readonly new IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelForAccountant, InvoiceViewModelForAccountant> invoiceGridMaster;

        private readonly IPushNotification push;

        public InvoicesController(
            ICompanyRepository<Invoice> invoicesRepository,
            IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelForAccountant, InvoiceViewModelForAccountant> invoiceGridMaster,
            ICurrentWebStorage<Employee> userStorage,
            ICompanyRepository<Customer> customersRepository,
            ICompanyRepository<Estimate> estimates,
            ICompanyRepository<Team> teamsRepository,
            ICompanyRepository<InvoiceLog> invoiceLogsRepository,
            ILogger logger,
            IPdfConverter pdfConverter,
            ICompanyRepository<RepairOrder> roRepository)
            : base(
                invoicesRepository,
                invoiceGridMaster as IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelBase, InvoiceViewModelBase>,
                userStorage,
                customersRepository,
                estimates,
                teamsRepository,
                logger,
                pdfConverter,
                roRepository)
        {
            this.invoiceLogsRepository = invoiceLogsRepository;
            this.invoiceGridMaster = invoiceGridMaster;
            this.push = ServiceLocator.Current.GetInstance<IPushNotification>();
        }


        public override ActionResult Index(string vin = null, string stock = null, string custRo = null, bool withFilters=true)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            //var customer = customerCookie != null ? customerCookie.Value : null;
            var teamCookie = HttpContext.Request.Cookies.Get("team");
            var team = teamCookie != null ? teamCookie.Value : null;

            var listTeams = ListsHelper.GetTeamsSelectListForInvoices(this.invoicesRepository, this.teamsRepository).ToList();
            if (team != null)
            {
                var item = listTeams.FirstOrDefault(x => x.Value == team);
                if (item != null)
                {
                    listTeams.Single(x => x.Selected).Selected = false;
                    listTeams.Remove(item);
                    item.Selected = true;
                    listTeams.Insert(0, item);
                }
            }


            ViewData["customers"] = new List<SelectListItem> { new SelectListItem { Text = @"All customers", Selected = true, Value = null } };
            ViewData["teams"] = listTeams;
            ViewData["statuses"] = ListsHelper.GetStatuses(null, Enum.GetValues(typeof(InvoiceStatus)).Cast<object>()); 

            return View();
        }

         [ValidateInput(false)]
        public JsonResult GetDataForAccountant(string sidx, string sord, int page, int rows, long? customer, long? team, int? status, string startDate, string endDate,
            bool? archived, bool? isStartSearch = null, string vin = null, string stock = null, string custRo = null, bool discarded = false, bool isNeedFilter = false)
        {

            DateTime s;
            var res = DateTime.TryParse(startDate, out s);
            var start = res ? s : (DateTime?)null;

            DateTime e;
            res = DateTime.TryParse(endDate, out e);
            var end = res ? e : (DateTime?)null;

            var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var custom = customerCookie != null ? customerCookie.Value : null;
            var listCustomers = ListsHelper.GetCustomersSelectListForInvoices(this.invoicesRepository, null).ToList();
            if (customer != null)
            {
                var item = listCustomers.FirstOrDefault(x => x.Value == custom);

                if (item != null)
                {
                    listCustomers.Single(x => x.Selected).Selected = false;
                    listCustomers.Remove(item);
                    item.Selected = true;
                    listCustomers.Insert(0, item);
                }
            }

            var invoiceStatus = status.HasValue ? status == 1 ? InvoiceStatus.Unpaid : status == 0 ? InvoiceStatus.Paid : InvoiceStatus.PaidInFull : (InvoiceStatus?)null;
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(this.currentEmployee.Id, team, customer, null, rows, page, sidx, sord,
                                                        archived, vin, stock, custRo, invoiceStatus, start, end, isDiscarded: discarded);
            
             
            var data = this.invoiceGridMaster.GetData(invoicesSpHelper, rows, page, additionalModelParam: team.HasValue ? team.Value : -1);
            data.customersFilter = listCustomers;
            var result = this.Json(data, JsonRequestBehavior.AllowGet);
            return result;
        }

        public ActionResult SetData(string PaidSum, string id, string oper)
        {
            var reg = new Regex(@"\d+(\.\d+)?");

            var sum = PaidSum;

            if (!reg.IsMatch(sum))
            {
                return this.Json(new { Message = "Invalid data format. Can enter only digits." }, JsonRequestBehavior.AllowGet);
            }

            var invoice = this.invoicesRepository.SingleOrDefault(x => x.Id == Convert.ToInt64(id));
            
            if (Convert.ToDouble(sum) > invoice.InvoiceSum)
            {
                return this.Json(new { Message = string.Format("Amount paid can not be greater than the sum of the invoice #{0}", id) }, JsonRequestBehavior.AllowGet);
            }

            Response.StatusCode = 303;
            Response.RedirectLocation = Url.Action("PaidSave", "Invoices", new { area = "Accountant", id, sum, paiddate = Convert.ToDouble(PaidSum) == 0 ? "01/01/1753" : DateTime.Now.ToShortDateString() });
            Response.End();

            return RedirectToAction("PaidSave", "Invoices", new { area = "Accountant", id, sum, paiddate = Convert.ToDouble(PaidSum) == 0 ? "01/01/1753" : DateTime.Now.ToShortDateString() });
        }

        [HttpPost]
        public ActionResult GetDialog(string id)
        {
            return this.PartialView(new PaidDialogViewModel
                                        {
                                            InvoiceId = Convert.ToInt64(id),
                                            InvoiceMessage = string.Format("Define the payment status of invoice #{0}", id),
                                            PaymentDate = DateTime.Now
                                        });
        }

        [HttpPost]
        public JsonResult PaidInvoice(string id, string sum, string paiddate)
        {
            var invoice = this.invoicesRepository.SingleOrDefault(x => x.Id == Convert.ToInt64(id));
            if (Convert.ToDateTime(paiddate) > DateTime.Now)
            {
                return this.Json("The date of payment can not be greater than current date");  
            }

            if (!string.IsNullOrWhiteSpace(sum))
            {
                if (Convert.ToDouble(sum) > invoice.InvoiceSum)
                {
                    return this.Json(string.Format("Amount paid can not be greater than the sum of the invoice #{0}", id));
                }
            }

            return this.Json(string.Empty);
        }

        [Transaction]
        public void PaidSave(string id, string sum, string paiddate, string invoiceStatus)
        {
            var invoice = this.invoicesRepository.SingleOrDefault(x => x.Id == Convert.ToInt64(id));

            if (!string.IsNullOrWhiteSpace(sum))
            {
                invoice.Status = Convert.ToDouble(sum) == 0 ? InvoiceStatus.Unpaid : invoiceStatus == "PaidInFull" ? InvoiceStatus.PaidInFull : InvoiceStatus.Paid;
                invoice.PaidSum = Convert.ToDouble(sum) == 0 ? 0 : Convert.ToDouble(sum);
                invoice.PaidDate = invoice.PaidSum == 0.0 ? Convert.ToDateTime("01/01/1753") : Convert.ToDateTime(paiddate);
                if (invoice.InvoiceSum == Convert.ToDouble(sum))
                {
                    CommonLogger.MarkInvoiceAsPaid(invoice);
                }
                else
                {
                    CommonLogger.UpdateInvoicePaid(invoice);
                }
            }
            else
            {
                invoice.Status = InvoiceStatus.PaidInFull;
                invoice.PaidSum = Math.Round(invoice.InvoiceSum, 2);
                invoice.PaidDate = string.IsNullOrWhiteSpace(paiddate) ? DateTime.Now : Convert.ToDateTime(paiddate);
                CommonLogger.MarkInvoiceAsPaid(invoice);
            }

            invoice.PaidSum = string.IsNullOrWhiteSpace(sum) ? invoice.InvoiceSum : Convert.ToDouble(sum);
            invoice.New = true;

            this.invoicesRepository.Save(invoice);
            logger.LogPaid(invoice);
            
            invoice.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee).ToList()
                .ForEach(x => this.push.Send(x, string.Format(NotificationMessages.PaidInvoice, invoice.Id)));
        }

        public ActionResult GetLogData(long id)
        {
            var invoiceLogs = this.invoiceLogsRepository.Where(x => x.EntityId == id);

            var model = new InvoiceLogViewModel
                            {
                                Id = id.ToString(),
                                InvoiceLogs = new Dictionary<InvoiceLog, string>()
                            };

            foreach (var log in invoiceLogs)
            {
                model.InvoiceLogs.Add(
                    log,
                    log.Action == InvoiceLogActions.Email
                        ? !string.IsNullOrWhiteSpace(log.Emails)
                              ? log.Emails.Replace(" ", string.Empty).Replace(",", ",\n")
                              : string.Empty
                        : string.Empty);
            }
            
            return this.PartialView("LogDetails", model);
        }

        public ActionResult GenerateExcel(string ids)
        {
            var invoiceIds = ids.Split(',');
            var gen = new InvoiceToXLSConverter();

            var invoices = invoiceIds.Select(invoiceId => this.invoicesRepository.FirstOrDefault(x => !x.IsImported && x.Id == Convert.ToInt64(invoiceId))).ToList();
            
            if (invoices.All(m => m == null))
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }

            gen.Invoices = invoices.Where(m => m != null);
            gen.Invoices.ToList().ForEach(CommonLogger.ExportInvoiceToQuickBook);
            var filename = gen.Create();
            var path = @"~/Content/" + filename;
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));
            Response.AddHeader("Pragma", "no-cache");
            Response.ContentType = "application/force-download";

            return this.Content(Url.Content(path));
        }
        
        [Transaction]
        public void SetImportedStatus(string ids)
        {
            var invoiceIds = ids.Split(',');

            foreach (var invoice in invoiceIds.Select(invoiceId => this.invoicesRepository.FirstOrDefault(x => !x.IsImported && x.Id == Convert.ToInt64(invoiceId))).Where(m => m != null))
            {
                invoice.IsImported = true;
                this.invoicesRepository.Save(invoice);
            }
        }
    }
}
