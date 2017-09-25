using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;

using SmartArch.Data;
using SmartArch.Web.Attributes;
using PDR.Domain.Helpers;
using PDR.Domain.StoredProcedureHelpers;

using SmartArch.Data.Proxy;

namespace PDR.Web.Areas.Common.Controllers
{
    using PDR.Web.Core.Helpers;

    [PDRAuthorize]
    public class InvoicesController : Controller
    {
        protected readonly IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelBase, InvoiceViewModelBase> invoiceGridMaster;

        protected readonly ICompanyRepository<Invoice> invoicesRepository;

        protected readonly ICompanyRepository<RepairOrder> roRepository; 

        protected readonly ICompanyRepository<Customer> customersRepository;

        protected readonly ICompanyRepository<Estimate> estimates;

        protected readonly Employee currentEmployee;

        protected readonly ICompanyRepository<Team> teamsRepository; 

        protected readonly ILogger logger;

        protected readonly IPdfConverter pdfConverter;

        protected readonly bool withEmployeeName;

        public InvoicesController(
            ICompanyRepository<Invoice> invoicesRepository,
            IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelBase, InvoiceViewModelBase> invoiceGridMaster,
            ICurrentWebStorage<Employee> userStorage,
            ICompanyRepository<Customer> customersRepository,
            ICompanyRepository<Estimate> estimates,
            ICompanyRepository<Team> teamsRepository, 
            ILogger logger,
            IPdfConverter pdfConverter,
            ICompanyRepository<RepairOrder> roRepository)
        {
            this.invoiceGridMaster = invoiceGridMaster;
            this.invoicesRepository = invoicesRepository;
            this.currentEmployee = userStorage.Get();
            this.customersRepository = customersRepository;
            this.estimates = estimates;
            this.teamsRepository = teamsRepository;
            this.logger = logger;
            this.pdfConverter = pdfConverter;
            this.roRepository = roRepository;
            this.withEmployeeName = true;
        }

        [ValidateInput(false)]
        public virtual ActionResult Index(string vin = null, string stock = null, string custRo = null, bool withFilters = true)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            FilterModel filters = null;
            if (withFilters)
            {
                var teamCookie = HttpContext.Request.Cookies.Get("team");

                filters = new FilterModel(typeof (Invoice), false, false, teamCookie);
            }

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }

            var currentUser = this.currentEmployee;
            if (currentUser == null)
            {
                return null;
            }

            return this.View(filters);
        }

        [ValidateInput(false)]
        public ActionResult ArchiveInvoices(string vin = null, string stock = null, string custRo = null)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            this.ViewBag.currenUserRole = currentEmployee.Role.ToString();
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }

            var filters = new FilterModel(typeof(Invoice), true, false, teamCookie);
            ViewBag.IsRITechnician = this.currentEmployee is Domain.Model.Users.RITechnician;
            return this.View(filters);
        }

        [ValidateInput(false)]
        public ActionResult DiscardedInvoices(string vin = null, string stock = null, string custRo = null)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            this.ViewBag.currenUserRole = currentEmployee.Role.ToString();
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }

            var filters = new FilterModel(typeof(Invoice), true, false, teamCookie, null, true);
            
            ViewData["statuses"] = ListsHelper.GetStatuses(null, Enum.GetValues(typeof(InvoiceStatus)).Cast<object>()); 

            return this.View(filters);
        }

        [Transaction]
        public void ArchiveUnarchive(string ids, bool toArchived)
        {
            var invoicesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var invoices = this.invoicesRepository.Where(x => invoicesIds.Contains(x.Id)).ToList();
            foreach (var invoice in invoices)
            {
                invoice.Archived = toArchived;
                var logActions = toArchived ? InvoiceLogActions.Archive : InvoiceLogActions.Unarchive;
                this.logger.Log(invoice, logActions);
                this.invoicesRepository.Save(invoice);
            }
        }

        [ValidateInput(false)]
        public virtual JsonResult GetData(string sidx, string sord, int page, int rows, long? customer, long? team, byte? status, bool? archived,
            bool? isStartSearch = null, string vin = null, string stock = null, string custRo = null, bool discarded = false, bool isNeedFilter = false)
        {
            if (this.currentEmployee == null)
            {
                return null;
            }

            var customerCookie = HttpContext.Request.Cookies.Get("customer");

            var onlyOwn = team == 0;
            var invoiceStatus = status.HasValue ? status == 1 ? InvoiceStatus.Unpaid : status == 0 ? InvoiceStatus.Paid : InvoiceStatus.PaidInFull : (InvoiceStatus?)null;

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(this.currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord,
                                                        archived, vin, stock, custRo, invoiceStatus, isDiscarded: discarded, isForCustomerFilter: isNeedFilter);
            var data = this.invoiceGridMaster.GetData(invoicesSpHelper, rows, page, isCustomerFilter: isNeedFilter, customerCookie: customerCookie);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHistory(string id)
        {
            var invoice = invoicesRepository.Get(Convert.ToInt64(id));
            var model = new List<string>();
            var historyManager = new DocumentHistoryManager();
            model = historyManager.GenerateHistory(invoice, withEmployeeName).ToList();

            return this.PartialView(model);
        }

        [HttpGet]
        [Transaction]
        public virtual ActionResult Print(string ids, bool? isBasic)
        {
            bool isDetailed = !(isBasic ?? currentEmployee.IsBasic);
            var invoice = this.invoicesRepository.Get(Convert.ToInt64(ids));
            var pdf = this.ConvertToPdf(invoice, isDetailed);
            this.logger.Log(invoice, InvoiceLogActions.Print);
            return new FileContentResult(pdf, "application/pdf");
        }
        
        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            var subject = this.currentEmployee.Company.InvoicesEmailSubject;

            if (customer)
            {
                to = this.customersRepository.Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }
            else
            {
                var invoicesIds = ids.Split(',');

                if (this.CheckInvoicesForTheSameCustomer(invoicesIds) && invoicesIds.Length > 1)
                {
                    return new JsonResult { Data = "Error" };
                }

                foreach (var id in invoicesIds)
                {
                    var estimate = this.invoicesRepository.Get(Convert.ToInt64(id)).RepairOrder.Estimate;

                    var email = this.invoicesRepository.Get(Convert.ToInt64(id)).Customer.Email ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        to += email + ", ";
                    }

                    if (!String.IsNullOrEmpty(estimate.Car.Stock))
                    {
                        subject += " " + estimate.Car.Stock + ",";
                    }
                        else if (!String.IsNullOrEmpty(estimate.Car.CustRO))
                        {
                            subject += " " + estimate.Car.CustRO + ",";
                        }
                            else if (!String.IsNullOrEmpty(estimate.Car.Make) && !String.IsNullOrEmpty(estimate.Car.Model))
                            {
                                subject += " " + estimate.Car.Year + "/" + estimate.Car.Make + "/" + estimate.Car.Model + ",";
                            }
                }

                to = string.Join(", ", to.Split(new[] { ',', ' ' }).Distinct());
            }
            var idWh = ids.Split(',')[0];

            var wholesale = this.invoicesRepository.Get(Convert.ToInt64(idWh)).Customer;
            var isWholesale = wholesale.ToPersist<WholesaleCustomer>() != null;
            var additionalEmails = new string[3];
            if (isWholesale)
            {
                additionalEmails[0] = wholesale.Email2 ?? string.Empty;
                additionalEmails[1] = wholesale.Email3 ?? string.Empty;
                additionalEmails[2] = wholesale.Email4 ?? string.Empty;
            }

            subject = subject.TrimEnd(',');
            var result = isWholesale
                ? new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids, Subject = subject, 
                    IsWholesale = true, To2 = additionalEmails[0], To3 = additionalEmails[1], To4 = additionalEmails[2], IsBasic = true}
                : new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids, Subject = subject, IsBasic = true};

            return this.PartialView(result);
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var invoiceIds = model.IDs.Split(',');
                IList<Attachment> attachments = new List<Attachment>();
                var from = this.currentEmployee.Company.Email;
                foreach (var id in invoiceIds)
                {
                    var invoice = this.invoicesRepository.Get(Convert.ToInt64(id));
                    if (invoice != null)
                    {
                        var pdf = this.ConvertToPdf(invoice, !model.IsBasic);
                        var fileName = string.Format("Invoice #{0}_{1}.pdf", invoice.Id, invoice.CreationDate.ToString("MM-dd-yyyy"));
                        attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                        this.logger.Log(invoice, InvoiceLogActions.Email, model.To);
                    }
                }

                var to = model.To +
                    (String.IsNullOrEmpty(model.To2) ? "" : "," + model.To2) +
                    (String.IsNullOrEmpty(model.To3) ? "" : "," + model.To3) +
                    (String.IsNullOrEmpty(model.To4) ? "" : "," + model.To4);

                var mes = new MailService().Send(from, to, model.Subject, model.Message, attachments.Count > 0 ? attachments : null);
                return this.Json(mes, JsonRequestBehavior.AllowGet);
            }
            
            return this.Json(false, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public JsonResult Discard(string ids, bool open = false)
        {
            if(string.IsNullOrEmpty(ids))
            {
                return Json("Query string is empty", JsonRequestBehavior.AllowGet);
            }

            try
            {
                foreach (var invoice in ids.Split(',').Select(id => this.invoicesRepository.Get(Convert.ToInt64(id))))
                {
                    invoice.IsDiscard = true;

                    var ro = invoice.RepairOrder;
                    ro.RepairOrderStatus = open ? RepairOrderStatuses.Open: RepairOrderStatuses.Discard;
                    ro.New = true;
                    ro.IsInvoice = false;
                    ro.IsConfirmed = false;
                    
                    this.logger.Log(invoice, InvoiceLogActions.Discard);
                    this.logger.Log(ro, open ? RepairOrderLogActions.ReOpen : RepairOrderLogActions.Discard);
                    this.roRepository.Save(ro);
                    this.invoicesRepository.Save(invoice);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json(open ? "Corresponded repair order(s) now Open" : "Success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public JsonResult ReinstateInvoice(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            try
            {
                foreach (var invoice in ids.Split(',').Select(id => this.invoicesRepository.Get(Convert.ToInt64(id))))
                {
                    invoice.IsDiscard = false;
                    invoice.Archived = false;
                    if (invoice.RepairOrder != null)
                    {
                        var ro = invoice.RepairOrder;
                        ro.RepairOrderStatus = RepairOrderStatuses.Finalised;
                        ro.EditedStatus = EditedStatuses.EditingReject;
                        ro.New = true;
                        ro.IsInvoice = true;
                        ro.IsConfirmed = true;
                        this.logger.Log(invoice, InvoiceLogActions.Undiscard);
                        this.logger.Log(ro, RepairOrderLogActions.Undiscard);
                        this.roRepository.Save(ro);
                    }
                    this.invoicesRepository.Save(invoice);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private byte[] ConvertToPdf(Invoice invoice, bool isDetailed)
        {
            return invoice != null ? this.pdfConverter.ConvertInvoice(invoice, isDetailed) : null;
        }

        private bool CheckInvoicesForTheSameCustomer(IList<string> ids)
        {
            var first = this.invoicesRepository.Get(Convert.ToInt64(ids[0])).Customer.Id;
            return ids.Any(id => this.invoicesRepository.Get(Convert.ToInt64(id)).Customer.Id != first);
        }
    }
}
