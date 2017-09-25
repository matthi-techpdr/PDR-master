using System.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleDefaultModel
    {
        public long Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int YearFrom { get; set; }

        public int YearTo { get; set; }

        public VehicleTypes Type { get; set; }

        public List<VehicleDefaultSectionModel> CarParts { get; set; }

        public VehicleDefaultModel()
        {
            this.CarParts = new List<VehicleDefaultSectionModel>();
        } 
    }
}