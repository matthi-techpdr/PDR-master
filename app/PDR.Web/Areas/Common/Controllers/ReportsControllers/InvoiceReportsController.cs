using System;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Reports;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Specifications;

using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Reports;
using PDR.Web.Areas.Technician.Models.Reports;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;

namespace PDR.Web.Areas.Common.Controllers.ReportsControllers
{
    using System.Globalization;

    using PDR.Domain.StoredProcedureHelpers;

    [PDRAuthorize]
    public class InvoiceReportsController : ReportsController<InvoiceReport>
    {
        protected readonly IGridMasterForStoredProcedure<Invoice, InvoiceForReportJsonModel, InvoiceForReportViewModel> invoiceGridMaster;

        protected readonly ICompanyRepository<Invoice> invoicesRepository;
     
        private const string SortByColumn = "CreationDate";

        public InvoiceReportsController()
        {
            this.invoiceGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Invoice, InvoiceForReportJsonModel, InvoiceForReportViewModel>>();
            this.invoicesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
        }

        public override ActionResult Index()
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            var currentUser = this.currentEmployee;
            var filter = new FilterModel(typeof(Invoice), false, true, teamCookie);
            this.ViewData["customers"] = filter.Customers;
            this.ViewData["teams"] = filter.Teams;

            if (this.currentEmployee.Role == UserRoles.Wholesaler)
            {
                var customer =
                this.invoicesRepository.Where(e => e.Customer.Email == currentUser.Login).Select(
                    c => c.RepairOrder.Estimate.Customer).Distinct().FirstOrDefault();
                if (customer != null)
                {
                    ViewBag.CustomerId = customer.Id;
                }
            }

            var userReports = this.currentReportTypeRepository.Where(x => x.Employee == this.currentEmployee);
            
            var model = userReports.ToList().Select(x => new ReportModel { Title = x.Title, Id = x.Id }).ToList();

            ViewBag.ExistReportNames = userReports.Select(x => x.Title).ToList();

            return this.View(model);
        }

        public virtual JsonResult GetInvoicesForReports(string sidx, string sord, int page, int rows, int? customer, int? team, string startDate,
                                    string endDate, bool isNeedFilter = false)
        {
            var currentUser = this.currentEmployee;
            if (currentUser == null)
            {
                return null;
            }
            var onlyOwn = team == 0;
            var customerCookie = HttpContext.Request.Cookies.Get("customer");

            DateTime s;
            var result = DateTime.TryParse(startDate, out s);
            var start = result ? s : (DateTime?)null;

            DateTime e;
            result = DateTime.TryParse(endDate, out e);
            var end = result ? e : (DateTime?)null;

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(this.currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord, dateFrom: start,
                                        dateTo: end, isForCustomerFilter: isNeedFilter);
            var data = this.invoiceGridMaster.GetData(invoicesSpHelper, rows, page, isCustomerFilter: isNeedFilter, customerCookie: customerCookie);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public override ActionResult SaveToPdf(string id, string from, string to, string customer, string team, bool commission, bool report = false)
        {
            var data = new DataForReports
            {
                Commission = commission, 
                Customer = GetCustomerForReports(customer), 
                Employee = this.currentEmployee, 
                Team = GetTeamForReports(team), 
                EntityType = "Invoice"
            };
            InvoiceReport ireport = null;
            var currentUser = this.currentEmployee;
            var currentTeam = !string.IsNullOrWhiteSpace(team) && team != "All teams" ? (long?) Convert.ToInt64(team) : null;
            var onlyOwn = team == "0";

            DateTime? enddate = null;
            DateTime? startdate = null;

            if (!string.IsNullOrWhiteSpace(from))
            {
                startdate = report ? Convert.ToDateTime(ireport.StartDate).Date : Convert.ToDateTime(from).Date;
            }

            if (!string.IsNullOrWhiteSpace(to))
            {
                enddate = report ? Convert.ToDateTime(ireport.EndDate).Date.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(to).Date.AddDays(1).AddSeconds(-1);
            }

            long? cust = null;
            if (!string.IsNullOrWhiteSpace(customer) && customer != "All customers")
            {
                cust = (long?)Convert.ToInt64(customer);
            }

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, currentTeam, isOnlyOwn: onlyOwn, filterByCustomers: cust, isGetAllInvoices:true,
                                    dateFrom: startdate, dateTo: enddate);

            data.Entities = invoicesSpHelper.Invoices.AsEnumerable();

            data.DateFrom = string.IsNullOrWhiteSpace(from)
                ? data.Entities.Any()
                    ? invoicesSpHelper.Invoices.Min(x => x.CreationDate).ToString("MM/dd/yyyy")
                    : DateTime.Now.ToString("MM/dd/yyyy")
                : from;

            data.DateTo = string.IsNullOrWhiteSpace(to)
                ? data.Entities.Any()
                    ? invoicesSpHelper.Invoices.Max(x => x.CreationDate).ToString("MM/dd/yyyy")
                    : DateTime.Now.ToString("MM/dd/yyyy")
                : to;

            if (report || !string.IsNullOrEmpty(id))
            {
                ireport = this.currentReportTypeRepository.Get(Convert.ToInt64(id));
                ReportLogger.Print(ireport);
            }
            else
            {
                ReportLogger.PrintNonSaved("invoice", data.DateFrom, data.DateTo, customer, team);
            }
            
            var pdf = this.pdfConverter.ReportConvert(data, currentUser.Company);
            return new FileContentResult(pdf, "application/pdf");
        }
    }
}