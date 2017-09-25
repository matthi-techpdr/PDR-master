using System.Globalization;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.StoredProcedureHelpers;

namespace PDR.Web.Areas.Estimator.Models.Customers
{
    using System;

    using PDR.Domain.Model.Enums;

    public class WholesaleCustomerJsonModel : IJsonModel
    {
        private readonly Employee currentEmployee;

        private readonly ICompanyRepository<Estimate> estimateRepository;

        private readonly ICompanyRepository<RepairOrder> repairOrderRepository;

        private readonly ICompanyRepository<Invoice> invoiceRepository;

        private readonly ICompanyRepository<Team> teamRepository;

		public WholesaleCustomerJsonModel(WholesaleCustomer customer, long? team)
		{
		    this.TeamId = team;
            this.teamRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();

            var onlyOwn = this.TeamId == 0;

		    this.currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
		    this.estimateRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.repairOrderRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            this.invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentEmployee.Id, team, customer.Id, onlyOwn, isGetAllEstimates: true, isGetTotalSum: true);

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(this.currentEmployee.Id, team, customer.Id, onlyOwn, isGetAllRo:true);
		    var openRo = repairOrdersSpHelper.RepairOrders;
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(this.currentEmployee.Id, team, customer.Id, onlyOwn, getPaidUnpaidInvoicesSum: true);
		    var paidInvSum = invoicesSpHelper.PaidInvoicesSum;
		    var unpaidInvSum = invoicesSpHelper.UnpaidInvoicesSum;

            var openEstimatesCount = estimatesSpHelper.TotalCountRows;
            var openEstSum = estimatesSpHelper.TotalAmountSum;
            var openRoSum = openRo.ToList().Sum(e => e.TotalAmount);

		    this.Id = customer.Id.ToString();
		    this.Name = customer.GetCustomerName();
		    this.State = customer.State.ToString();
            this.City = customer.City;
		    this.Email = customer.Email;
        }

        public WholesaleCustomerJsonModel(CustomerModel model,  long? team)
        {
            Id = model.Id;
            Name = model.Name;
            State = model.State.ToString();
            Phone = model.Phone;
            City = model.City;
            Email = model.Email;
            Email2 = model.Email2;
            Email3 = model.Email3;
            Email4 = model.Email4;
            currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            TeamId = team;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string Email2 { get; set; }

        public string Email3 { get; set; }

        public string Email4 { get; set; }

        public long? TeamId { get; set; }

        private static string RenderSum(double sum)
        {
           return sum.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
	}
}