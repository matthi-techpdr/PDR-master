using PDR.Domain.Model.Logging;

namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class LocationJsonModel
    {
        public LocationJsonModel()
        {
        }

        public LocationJsonModel(Location location)
        {
            this.Date = location.Date.ToString("MM/dd/yyyy HH:mm");
            this.LatLng = new LatLng(location.Lat, location.Lng);
        }

        public string Date { get; set; }

        public LatLng LatLng { get; set; }
    }
}