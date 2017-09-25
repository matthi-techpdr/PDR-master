using System.Linq;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Web.Areas.Wholesaler.Models;

namespace PDR.Web.Areas.Wholesaler.Managers
{
    public class EstimateManager
    {
        public WholesalerEstimateModel GetEstimateModel(WholesaleCustomer customer)
        {
            var employees = (customer).Teams.SelectMany(x => x.Employees).Where(x => x is PDR.Domain.Model.Users.Manager).Where(x => x.Status == Statuses.Active).Distinct<PDR.Domain.Model.Users.Employee>().ToList();
            if (employees.Count() == 0)
            {
                employees = customer.Company.Employees.Where(x => x is PDR.Domain.Model.Users.Admin).ToList();
            }

            var model = WholesalerEstimateModel.Get(customer, employees);
            return model;
        }

    }
}