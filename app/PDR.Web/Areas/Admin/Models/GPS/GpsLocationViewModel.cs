using PDR.Domain.Model.Logging;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class GpsLocationViewModel : IViewModel
    {
        public GpsLocationViewModel()
        {
        }

        public GpsLocationViewModel(Location location)
        {
            this.Id = location.Id.ToString();
            this.Date = location.Date.ToString();
            this.Lat = location.Lat;
            this.Lng = location.Lng;
            this.Name = location.License.Employee.Name;
        }

        public string Id { get; set; }

        public string Date { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public string Name { get; set; }
    }
}
