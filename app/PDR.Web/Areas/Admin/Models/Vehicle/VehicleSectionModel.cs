using System.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleSectionModel
    {
        public VehicleSectionModel()
        {
            this.EffortItems = new List<VehicleEffortModel>();
        }

        public long? Id { get; set; }

        public PartOfBody Name { get; set; }

        public string FullName { get; set; }

        public string Price { get; set; }

        public IList<VehicleEffortModel> EffortItems { get; set; }
    }
}