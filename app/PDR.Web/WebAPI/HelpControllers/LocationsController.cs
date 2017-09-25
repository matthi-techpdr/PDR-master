
using System.Web.Mvc;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Webstorage;
using PDR.Web.WebAPI.Authorization;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.IphoneModels;
using SmartArch.Data.Proxy;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize]
    public class LocationsController : Controller
    {
        private readonly ICurrentWebStorage<Employee> webStorage;

        private readonly ICompanyRepository<Affiliate> affiliates;

        public LocationsController(ICurrentWebStorage<Employee> webStorage)
        {
            this.webStorage = webStorage;
            this.affiliates = ServiceLocator.Current.GetInstance<ICompanyRepository<Affiliate>>();
        }

        [WebApiTransaction]
        [System.Web.Mvc.HttpGet]
        public JsonResult Get()
        {
            var currentUser = this.webStorage.Get();
            if (currentUser != null)
            {
                var tmp = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
                        ? ((TeamEmployee)currentUser).Teams.Where(x => x.Status == Statuses.Active).SelectMany(x => x.Customers).Distinct()
                        .Select(x => x.ToPersist<Affiliate>()).ToList()
                        : this.affiliates.ToList();
                var locations = tmp.Where(x => x!= null && x.Status == Statuses.Active).Select(x => new ApiLocationModel() { Id = x.Id, NameLocation = x.Name });

                return this.Json(locations, JsonRequestBehavior.AllowGet);
            }
            return this.Json(null, JsonRequestBehavior.AllowGet);
        }


    }
}