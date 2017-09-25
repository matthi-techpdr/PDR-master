using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class CustomersController : Common.Controllers.CustomerManagementController
    {
        public CustomersController(IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, CustomerViewModel> wholesaleCustomerGridMaster, 
            ICompanyRepository<Team> teamRepository, ICompanyRepository<PriceMatrix> matricesRepository, 
            ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository, 
            IRepositoryFactory repositoryFactory)
            : base(wholesaleCustomerGridMaster, teamRepository, matricesRepository, wholesalersRepository, repositoryFactory)
        {
        }
    }
}
