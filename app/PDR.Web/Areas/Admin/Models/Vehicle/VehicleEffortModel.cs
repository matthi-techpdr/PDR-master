namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleEffortModel
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public bool HasR_R { get; set; }

        public bool HasR_I { get; set; }

        public string HoursR_R { get; set; }
        
        public string HoursR_I { get; set; } 
    }
}