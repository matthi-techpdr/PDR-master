using System.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleDefaultSectionModel
    {
        public long Id { get; set; }

        public PartOfBody Name { get; set; }

        public double NewSectionPrice { get; set; }

        public List<VehicleDefaultEffortItemModel> EffortItems { get; set; }

        public VehicleDefaultSectionModel()
        {
            this.EffortItems = new List<VehicleDefaultEffortItemModel>();
        } 
    }
}