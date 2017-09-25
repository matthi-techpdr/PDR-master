using PDR.Domain.Model.Logging;

namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class LastLocationModel 
    {
        protected LastLocationModel()
        {
        }

        public LastLocationModel(Location location)
        {
            this.Name = location.License.Employee.Name;
            this.Location = new LocationJsonModel(location);
        }

        public string Name { get; set; }

        public LocationJsonModel Location { get; set; }
    }
}
