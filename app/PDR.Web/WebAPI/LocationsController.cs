using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Logging;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.Authorization;

namespace PDR.Web.WebAPI
{
    [ApiAuthorize]
    public class LocationsController : ApiController
    {
        private Employee CurrentEmployee
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            }
        }

        private ICompanyRepository<Location> LocationRepository
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICompanyRepository<Location>>();
            }
        }

        [WebApiTransaction]
        public HttpResponseMessage Post(double lat, double lng)
        {
            var location = new Location(true) { Lat = lat, Lng = lng, License = this.CurrentEmployee.Licenses.Single(x => x.Status == LicenseStatuses.Active) };
            this.LocationRepository.Save(location);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}