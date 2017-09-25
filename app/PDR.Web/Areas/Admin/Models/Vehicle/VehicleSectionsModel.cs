using System.Collections.Generic;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleSectionsModel
    {
        public VehicleSectionsModel()
        {
            this.Sections = new List<VehicleSectionModel>();
        }

        public IList<VehicleSectionModel> Sections { get; set; }
    }
}