using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Core.Authorization;


namespace PDR.Web.Areas.Accountant.Controllers
{
    [PDRAuthorize(Roles = "Accountant")]
    public class CustomersController : Common.Controllers.CustomerManagementController
    {
        public CustomersController(
            IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, CustomerViewModel> wholesaleCustomerGridMaster,
            ICompanyRepository<Team> teamRepository, ICompanyRepository<PriceMatrix> matricesRepository,
            ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository,
            IRepositoryFactory repositoryFactory)
            : base(
                wholesaleCustomerGridMaster, teamRepository, matricesRepository, wholesalersRepository, repositoryFactory)
        {
        }

        [HttpGet]
        public override JsonResult GetData(string sidx, string sord, int page, int rows, int? state, long? team)
        {
            var gridMaster = ServiceLocator.Current.GetInstance<IGridMaster<WholesaleCustomer, CustomerJsonModel, CustomerViewModel>>();

            if (state != null)
            {
                gridMaster.ExpressionFilters.Add(x => x.State == (StatesOfUSA)state.Value);
            }

            var data = gridMaster.GetData(page, rows, sidx, sord);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return json;
        }
    }
}