using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;
using PDR.Web.WebAPI.WebApiRepoExtensions;
using SmartArch.Data;
using SmartArch.Web.Attributes;
using System.Collections.Generic;
using System.Configuration;
using PDR.Domain.StoredProcedureHelpers;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize]
    public class InvoicesController : Controller
    {
        private readonly ICompanyRepository<Invoice> invoiceRepository;

        private readonly ICompanyRepository<RepairOrder> roRepository;

        private readonly ICurrentWebStorage<Employee> webStorage;

        private readonly ICompanyRepository<Team> teams;

        private readonly ILogger logger;

        private const string SortByColumn = "CreationDate";

        protected readonly bool withEmployeeName;

        private int CountRows
        {
            get
            {
                int number;
                bool result = Int32.TryParse(ConfigurationManager.AppSettings["CountDocumentRowsForIPhone"], out number);
                if (result)
                {
                    return number;
                }
                else
                {
                    return 50;
                }
            }
        }

        public InvoicesController(
            ICompanyRepository<Invoice> invoiceRepository,
            ICompanyRepository<RepairOrder> roRepository,
            ICurrentWebStorage<Employee> webStorage,
            ICompanyRepository<Team> teams,
            ILogger logger
            )
        {
            this.invoiceRepository = invoiceRepository;
            this.roRepository = roRepository;
            this.webStorage = webStorage;
            this.teams = teams;
            this.logger = logger;
            this.withEmployeeName = true;
        }

        public JsonResult GetAll()
        {
            var employee = this.webStorage.Get();
            var teamSelector = this.Request.Url.ParseQueryString()["team"];
            var onlyOwn = false;
            var archivedSelector = this.Request.Url.ParseQueryString()["archived"];
            var page = this.Request.Url.ParseQueryString()["page"];
            var sortByDateDesc = this.Request.Url.ParseQueryString()["isDesc"] == "true";
            var statusInvoices = this.Request.Url.ParseQueryString()["statusInvoices"];

            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;
            var archived = archivedSelector != null; ;
            var numPage = Convert.ToInt32(page);

            var status = GetInvoicesStatus(statusInvoices);
            var sort = sortByDateDesc ? "DESC" : "ASC";
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(employee.Id, teamId, isOnlyOwn: onlyOwn, rowsPerPage: this.CountRows,
                filterByCustomers: null, pageNumber: numPage + 1, sortByColumn: SortByColumn, sortType: sort, isArchive: archived, invoicesStatus: status);
            var invoices = GetInvoices(employee, invoicesSpHelper.Invoices.ToList(), employee);
            return this.Json(new { invoices }, JsonRequestBehavior.AllowGet);
        }

        private InvoiceStatus? GetInvoicesStatus(string status)
        {
            if (status == null)
            {
                return null;
            }
            switch (status.ToLower())
            {
                case "paid":
                    return InvoiceStatus.Paid;
                case "unpaid":
                    return InvoiceStatus.Unpaid;
                default:            //ALL estimates
                    return null;
            }
        }

        public JsonResult GetAllForSearch()
        {
            var employee = this.webStorage.Get();
            var teamSelector = this.Request.Url.ParseQueryString()["team"];
            var vin = this.Request.Url.ParseQueryString()["vin"];
            var stock = this.Request.Url.ParseQueryString()["stock"];
            var custRo = this.Request.Url.ParseQueryString()["custRo"];
            var page = this.Request.Url.ParseQueryString()["page"];

            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            var onlyOwn = false;
            onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;
            var numPage = Convert.ToInt32(page);

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(employee.Id, teamId, isOnlyOwn: onlyOwn, rowsPerPage: this.CountRows,
                filterByCustomers: null, pageNumber: numPage + 1, sortByColumn: SortByColumn, isArchive: null,
                vinCode: vin, stock: stock, custRo: custRo);
            var invoices = GetInvoices(employee, invoicesSpHelper.Invoices.ToList(), employee);

            return this.Json(new { invoices }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable GetInvoices(Employee employee, IEnumerable<Invoice> invoices, Employee currentEmployee)
        {
            var result =
                invoices.Select(
                    i =>
                    new
                        {
                            i.Id,
                            CreationDate = i.CreationDate.ToShortDateString(),
                            PaidDate = i.PaidDate.ToShortDateString(),
                            Status = i.Status == InvoiceStatus.PaidInFull
                                                ? InvoiceStatus.Paid.ToString()
                                                : i.Status.ToString(),
                            i.InvoiceSum,
                            Customer = new ApiCustomerDocumentModel<Invoice>(i),
                            AssignedEmployees = i.RepairOrder.TeamEmployeePercents.Select(a => new ApiAssignedEmployee(a)),
                            i.New,
                            IsRiPaymentAmount = employee.Role == UserRoles.RITechnician,
                            RiPaymentAmount = GetRiCommission(employee.Role, i.RepairOrder),
                            isArchive = i.Archived,
                            year = i.RepairOrder.Estimate.Car.Year,
                            make = i.RepairOrder.Estimate.Car.Make,
                            model = i.RepairOrder.Estimate.Car.Model,
                            stock = i.RepairOrder.Estimate.Car.Stock,
                            custRO = i.RepairOrder.Estimate.Car.CustRO,
                            isDiscard = i.IsDiscard,
                            Commission = InvoiceHelper.GetCommission(i, currentEmployee)
                        });
            return result;
        }

        private double GetRiCommission(UserRoles userRoles, RepairOrder rOrder)
        {
            double result = 0;
            if (userRoles != UserRoles.RITechnician)
            {
                return result;
            }

            var countRi = rOrder.TeamEmployeePercents.Count(x => x.TeamEmployee.Role == UserRoles.RITechnician);
            double sum = 0;
            if (rOrder.IsFlatFee.HasValue)
            {
                var totalGetLaborSum = rOrder.Estimate.CarInspections.Sum(x => x.GetLaborSum());
                var riPayment = totalGetLaborSum <= 0
                    ? 0
                    : totalGetLaborSum * (1 - rOrder.Estimate.Discount);

                sum = rOrder.IsFlatFee.Value
                    ? (rOrder.Payment.HasValue ? rOrder.Payment.Value : 0)
                    : riPayment;
            }
            result = (sum / countRi);
            return result;
        }

        [Transaction]
        [HttpPost]
        public void IsNotNew(long id)
        {
            var invoice = this.invoiceRepository.GetIfExist(id);
            invoice.New = false;
            this.invoiceRepository.Save(invoice);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Discard(long id, bool open = false)
        {
            var invoice = this.invoiceRepository.Get(id);

            if (invoice == null)
            {
                return new HttpStatusCodeResult(404, "Invoice doesn't exist");
            }

            var employee = this.webStorage.Get();
            if ((employee.Role == UserRoles.Manager || employee.Role == UserRoles.Technician || employee.Role == UserRoles.RITechnician) && (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.PaidInFull))
            {
                return new HttpStatusCodeResult(417, "This action cannot be done for entries with \"Paid\" status for roles Manager and Technician");
            }

            invoice.IsDiscard = true;

            var ro = invoice.RepairOrder;
            ro.RepairOrderStatus = open ? RepairOrderStatuses.Open : RepairOrderStatuses.Discard;
            ro.New = true;
            ro.IsInvoice = false;
            ro.IsConfirmed = false;

            this.logger.Log(invoice, InvoiceLogActions.Discard);
            this.logger.Log(ro, open ? RepairOrderLogActions.ReOpen : RepairOrderLogActions.Discard);
            this.roRepository.Save(ro);
            this.invoiceRepository.Save(invoice);

            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public JsonResult GetHistory(string id)
        {
            var invoice = invoiceRepository.Get(Convert.ToInt64(id));
            var historyManager = new DocumentHistoryManager();
            HistoryListModel historyList = new HistoryListModel(historyManager.GenerateHistory(invoice, withEmployeeName).ToList());
            var result = this.Json(new { historyList }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [Transaction]
        public ActionResult Pdf(long id, bool detailed = false)
        {
            var invoice = this.invoiceRepository.Get(id);

            if (invoice == null)
            {
                return new HttpStatusCodeResult(404, "Invoice doesn't exist");
            }

            var pdfConverter = ServiceLocator.Current.GetInstance<IPdfConverter>();

            this.logger.Log(invoice, InvoiceLogActions.Print);
            var pdf = pdfConverter.ConvertInvoice(invoice, detailed);

            return this.File(pdf, "application/pdf");
        }

        [HttpPost]
        [Transaction]
        public ActionResult ArchiveUnarchive(long id, bool toArchived)
        {
            var invoice = this.invoiceRepository.SingleOrDefault(x => x.Id == id);

            if (invoice == null)
            {
                return new HttpStatusCodeResult(404, "Invoice doesn't exist");
            }

            invoice.Archived = toArchived;
            var logActions = toArchived ? InvoiceLogActions.Archive : InvoiceLogActions.Unarchive;
            this.logger.Log(invoice, logActions);
            this.invoiceRepository.Save(invoice);

            return new HttpStatusCodeResult(200);
        }
    }
}