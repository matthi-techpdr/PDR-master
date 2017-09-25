namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class LatLng
    {
        public LatLng(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        public double lat { get; set; }

        public double lng { get; set; } 
    }
}