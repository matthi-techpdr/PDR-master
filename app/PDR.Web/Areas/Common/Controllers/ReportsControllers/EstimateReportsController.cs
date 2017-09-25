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
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;

namespace PDR.Web.Areas.Common.Controllers.ReportsControllers
{
    using PDR.Domain.StoredProcedureHelpers;

    [PDRAuthorize]
    public class EstimateReportsController : ReportsController<EstimateReport>
    {
        protected readonly IGridMasterForStoredProcedure<Estimate, EstimateForReportJsomModel, EstimatesForReportViewModel> estimatesGridMaster;
        
        protected readonly ICompanyRepository<Estimate> estimatesRepository;

        private readonly ICompanyRepository<Team> teamsRepository; 

        public EstimateReportsController()
        {
            this.estimatesGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Estimate, EstimateForReportJsomModel, EstimatesForReportViewModel>>();
            this.estimatesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.teamsRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();
        }

        public override ActionResult Index()
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            var currentUser = this.currentEmployee;
            var filter = new FilterModel(typeof(Estimate), false, true, teamCookie);
            this.ViewData["customers"] = filter.Customers;
            var teams = filter.Teams;

            //if (this.currentEmployee.Role == UserRoles.Admin)
            //{
            //    teams.Insert(0, new SelectListItem { Text = "My activity only", Selected = true, Value = 0.ToString() });
            //}
            
            this.ViewData["teams"] = teams;

            if (this.currentEmployee.Role == UserRoles.Wholesaler)
            {
                var customer =
                this.estimatesRepository.Where(e => e.Customer.Email == currentUser.Login).Select(
                    c => c.Customer).Distinct().FirstOrDefault();
                if (customer != null)
                {
                    ViewBag.CustomerId = customer.Id;
                }
            }

            var userReports = this.currentReportTypeRepository.Where(x => x.Employee == this.currentEmployee).ToList();

            var model = userReports.ToList().Select(x => new ReportModel { Title = x.Title, Id = x.Id });
                
            ViewBag.ExistReportNames = userReports.Select(x => x.Title).ToList();
            return this.View(model);
        }

        #region JsonDataGeters

        public virtual JsonResult GetEstimatesForReports(string sidx, string sord, int page, int rows, int? customer, long? team, string startDate, 
                            string endDate, bool isNeedFilter = false)
        {
            if (this.currentEmployee == null)
            {
                return null;
            }
            var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var onlyOwn = team == 0;
            DateTime s;
            var result = DateTime.TryParse(startDate, out s);
            var start = result ? s : (DateTime?)null;

            DateTime e;
            result = DateTime.TryParse(endDate, out e);
            var end = result ? e : (DateTime?)null;

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(this.currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord, isForReport: true,
                                dateFrom: start, dateTo: end, isForCustomerFilter: isNeedFilter);
            var data = this.estimatesGridMaster.GetData(estimatesSpHelper, rows, page, isCustomerFilter: isNeedFilter, customerCookie: customerCookie);
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public override ActionResult SaveToPdf(string id, string from, string to, string customer, string team, bool commission, bool report = false)
        {
            var currentUser = this.currentEmployee;
            var currentTeam = !string.IsNullOrWhiteSpace(team) && team != "All teams" ? (long?)Convert.ToInt64(team) : null;
            var onlyOwn = team == "0";

            DateTime? enddate = null;
            DateTime? startdate = null;

            if (!string.IsNullOrWhiteSpace(from))
            {
                startdate = Convert.ToDateTime(from).Date;
            }

            if (!string.IsNullOrWhiteSpace(to))
            {
                enddate = Convert.ToDateTime(to).Date.AddDays(1).AddSeconds(-1);
            }

            long? cust = null;
            if (!string.IsNullOrWhiteSpace(customer) && customer != "All customers")
            {
                cust = (long?)Convert.ToInt64(customer);
            }

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentUser.Id, currentTeam, cust, onlyOwn, isForReport: true, dateFrom: startdate,
                                    dateTo: enddate, isGetAllEstimates: true);

            var dateFrom = string.IsNullOrWhiteSpace(from)
                               ? estimatesSpHelper.TotalCountRows > 0
                                     ? estimatesSpHelper.Estimates.Min(x => x.CreationDate).ToString("MM/dd/yyyy")
                                     : DateTime.Now.ToString("MM/dd/yyyy")
                               : from;

            var dateTo = string.IsNullOrWhiteSpace(to)
                             ? estimatesSpHelper.TotalCountRows > 0
                                   ? estimatesSpHelper.Estimates.Max(x => x.CreationDate).ToString("MM/dd/yyyy")
                                   : DateTime.Now.ToString("MM/dd/yyyy")
                             : to;

            var currentCustomer = this.GetCustomerForReports(customer);
            var curTeam = this.GetTeamForReports(team);

            var data = new DataForReports
                           {
                               Commission = commission, 
                               Customer = currentCustomer, 
                               Employee = this.currentEmployee, 
                               Team = curTeam, 
                               EntityType = "Estimate",
                               Entities = estimatesSpHelper.Estimates.AsEnumerable(), 
                               DateFrom = dateFrom, 
                               DateTo = dateTo
                           };

            if (!string.IsNullOrEmpty(id))
            {
                var existReport = this.currentReportTypeRepository.Get(Convert.ToInt64(id));
                ReportLogger.Print(existReport);
            }
            else
            {
                ReportLogger.PrintNonSaved("estimate", dateFrom, dateTo, currentCustomer, curTeam);
            }
            
            var pdf = this.pdfConverter.ReportConvert(data, currentUser.Company);
            return new FileContentResult(pdf, "application/pdf");
        }
    }
}
