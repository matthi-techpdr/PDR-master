using System;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Reports;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Estimator.Models.Reports;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Common.Controllers.ReportsControllers
{
    public abstract class ReportsController<T> : Controller where T : Report, new()
    {
        protected readonly ICompanyRepository<T> currentReportTypeRepository; 

        protected readonly Employee currentEmployee;

        protected readonly ICompanyRepository<Customer> customersRepository;

        protected readonly ICompanyRepository<Team> teamsRepository;

        protected readonly IPdfConverter pdfConverter;

        protected ReportsController()
        {
            this.currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            this.customersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>();
            this.teamsRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();
            this.pdfConverter = ServiceLocator.Current.GetInstance<IPdfConverter>();
            this.currentReportTypeRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<T>>();
        }

        public abstract ActionResult Index();

        [Transaction]
        public void Save(ReportModel model)
        {
            var report = new T
                {
                    Title = model.Title,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TeamId = model.TeamId,
                    CustomerId = model.CustomerId,
                    Commission = model.Commission,
                    Employee = this.currentEmployee,
                    Company = this.currentEmployee.Company
                };

            ReportLogger.New(report);
            this.currentReportTypeRepository.Save(report);
        }

        [Transaction]
        public void Delete(long id)
        {
            var report = this.currentReportTypeRepository.Get(id);
            ReportLogger.Delete(report);
            this.currentReportTypeRepository.Remove(report);
        }

        public ActionResult Details(long id)
        {
            var estimateReport = this.currentReportTypeRepository.Get(id);
            var customerName = estimateReport.CustomerId.HasValue ? this.customersRepository.Get(estimateReport.CustomerId.Value).GetCustomerName() : "All customers";
            var teamName = estimateReport.TeamId.HasValue ? estimateReport.TeamId.Value != 0 ? this.teamsRepository.Get(estimateReport.TeamId.Value).Title : "My activity only" : "All teams";
            var startDate = estimateReport.StartDate;
            var endDate = estimateReport.EndDate;
            var commission = estimateReport.Commission;

            var model = new ReportModel
            {
                Id = id,
                Title = estimateReport.Title,
                CustomerName = customerName,
                TeamName = teamName,
                Commission = commission,
                StartDate = startDate,
                EndDate = endDate,
                TeamId = estimateReport.TeamId,
                CustomerId = estimateReport.CustomerId,
                Role = this.currentEmployee.Role.ToString()
            };

            return this.PartialView(model);
        }

        public abstract ActionResult SaveToPdf(string id, string from, string to, string customer, string team, bool commission, bool report = false);

        protected string GetTeamForReports(string team)
        {
            return this.currentEmployee.Role != UserRoles.Manager
                   && this.currentEmployee.Role != UserRoles.Admin
                       ? string.Empty
                       : team == "All teams" || team == string.Empty
                             ? "All teams"
                             : team == "0"
                                   ? "My activity only"
                                   : this.teamsRepository.Single(x => x.Id == Convert.ToInt64(team)).Title;
        }

        protected string GetCustomerForReports(string customer)
        {
            return customer == "All customers" || customer == string.Empty
                       ? "All customers"
                       : this.customersRepository.FirstOrDefault(x => x.Id == Convert.ToInt64(customer)).GetCustomerName();
        }
    }
}
