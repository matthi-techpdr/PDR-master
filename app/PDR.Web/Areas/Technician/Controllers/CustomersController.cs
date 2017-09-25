using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Core.Authorization;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Technician.Controllers
{
    [PDRAuthorize(Roles = "Technician")]
    public class CustomersController : Common.Controllers.CustomersController
    {
        public CustomersController(
            IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, WholesaleCustomerViewModel> wholesaleCustomersGridMaster,
            ICurrentWebStorage<Employee> currentEmployee,
            ICompanyRepository<Estimate> estimatesRepository,
            IRepositoryFactory repositoryFactory) :
            base(wholesaleCustomersGridMaster, currentEmployee, estimatesRepository, repositoryFactory)
        {
        }
    }
}
