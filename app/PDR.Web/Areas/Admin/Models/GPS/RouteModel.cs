using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Logging;

namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class RouteModel
    {
        public RouteModel()
        {
        }

        public RouteModel(IList<Location> locations)
        {
            this.LicenseId = locations.First().License.Id;
            this.Name = locations.First().License.Employee.Name;
            this.Locations = locations.Select(x => new LocationJsonModel(x)).ToList();
        }

        public long LicenseId { get; set; }

        public string Name { get; set; }

        public IList<LocationJsonModel> Locations { get; set; }
    }
}