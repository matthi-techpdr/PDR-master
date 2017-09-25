using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Estimator.Controllers
{
    [PDRAuthorize(Roles = "Estimator")]
    public class CustomersController : Common.Controllers.CustomersController
    {
        public CustomersController(IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, WholesaleCustomerViewModel> wholesaleCustomersGridMaster,
            ICurrentWebStorage<Employee> currentEmployee,
            ICompanyRepository<Estimate> estimatesRepository,
            IRepositoryFactory repositoryFactory) :
            base(wholesaleCustomersGridMaster, currentEmployee, estimatesRepository, repositoryFactory)
        {
        }

        public override ActionResult GetCustomerDetails(long? id)
        {
            WholesaleCustomerViewModel model = this.GetCustomerViewModel(id);
            return this.PartialView("CustomerDetails", model);
        }
    }
}
