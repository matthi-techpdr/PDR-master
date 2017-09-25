using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.WebAPI.IphoneModels;

namespace PDR.Web.WebAPI
{
    public class WholesaleCustomersController : BaseWebApiController<WholesaleCustomer, ApiWholesaleCustomerModel>
    {
        private readonly Employee employee;

        public WholesaleCustomersController()
        {
            this.employee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
        }

        public override IList<ApiWholesaleCustomerModel> Get()
        {
            var team = this.Request.RequestUri.ParseQueryString()["team"];

            var customers = ((this.employee is Technician || this.employee is Manager) ?
                ((TeamEmployee)this.employee).Teams.SelectMany(x => x.Customers).Distinct().Where(x => x.CustomerType == CustomerType.Wholesale ).Cast<WholesaleCustomer>()
                : this.Repository).Where(x => x.Status == Statuses.Active);

            if (team != null)
            {
                var teamId = int.Parse(team);
                if (teamId != 0)
                {
                    customers = customers.Where( x => x != null).Where(x => (x.Teams.Select(i => i.Id)).Contains(teamId));
                }
            }
            var result = customers.Select(x => new ApiWholesaleCustomerModel(x)).AsQueryable();
            return result.ToList();
        }
    }
}
