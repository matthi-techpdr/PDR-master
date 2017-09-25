using System.Globalization;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Areas.Estimator.Models.Affiliates
{
    using PDR.Domain.StoredProcedureHelpers;

    public class AffiliatesCustomerJsonModel : IJsonModel
    {
        private readonly Employee currentEmployee;

        public AffiliatesCustomerJsonModel(Affiliate affiliate, long? team)
		{
		    this.TeamId = team;
            var onlyOwn = this.TeamId == 0;
		    this.currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, affiliateId: affiliate.Id, isGetAllEstimates: true, isGetTotalSum: true);
            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(this.currentEmployee.Id, team, isOnlyOwn: onlyOwn, affiliateId: affiliate.Id, isGetAllRo: true);
            var openRo = repairOrdersSpHelper.RepairOrders;

            var openEstimatesCount = estimatesSpHelper.TotalCountRows;
            var openEstSum = openEstimatesCount > 0 ? estimatesSpHelper.TotalAmountSum : 0;
            var openRoSum = openRo != null ? openRo.Sum(e => e.TotalAmount) : 0;

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(this.currentEmployee.Id, team, isOnlyOwn: onlyOwn, affiliateId: affiliate.Id, 
                                                                        getPaidUnpaidInvoicesSum:true);
            var paidInvSum = invoicesSpHelper.PaidInvoicesSum;
            var unpaidInvSum = invoicesSpHelper.UnpaidInvoicesSum;

		    this.Id = affiliate.Id.ToString();
		    this.Name = affiliate.GetCustomerName();
		    this.State = affiliate.State.ToString();
            this.City = affiliate.City;
            Phone = affiliate.Phone;
		    this.Email = affiliate.Email;

            //this.AmountOfOpenEstimates = openEstimatesCount;
            //this.SumOfOpenEstimates = RenderSum(openEstSum);
            //this.SumOfOpenWorkOrders = RenderSum(openRoSum);
            //this.SumOfPaidInvoices = RenderSum(paidInvSum);
            //this.SumOfUnpaidInvoices = RenderSum(unpaidInvSum);
        }

        public AffiliatesCustomerJsonModel(CustomerModel model, long? team)
        {
            Id = model.Id;
            Name = model.Name;
            State = model.State.ToString();
            City = model.City;
            Phone = model.Phone;
            Email = model.Email;
            //AmountOfOpenEstimates = model.AmountOfOpenEstimates;
            //SumOfOpenEstimates = RenderSum(model.SumOfOpenEstimates);
            //SumOfOpenWorkOrders = RenderSum(model.SumOfOpenWorkOrders);
            //SumOfPaidInvoices = RenderSum(model.SumOfPaidInvoices);
            //SumOfUnpaidInvoices = RenderSum(model.SumOfUnpaidInvoices);
            currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            TeamId = team;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        //public int AmountOfOpenEstimates { get; set; }

        //public string SumOfOpenEstimates { get; set; }

        //public string SumOfOpenWorkOrders { get; set; }

        //public string SumOfPaidInvoices { get; set; }

        //public string SumOfUnpaidInvoices { get; set; }

        public long? TeamId { get; set; }

        private static string RenderSum(double sum)
        {
           return sum.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
	}
}