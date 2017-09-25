using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleJsonModel : IJsonModel
    {
        public VehicleJsonModel(Domain.Model.Effort.CarModel carModel)
        {
            this.Id = carModel.Id;
            this.Make = carModel.Make.ToLower() == "defaultcar" ? "Default" : carModel.Make;
            this.Model = this.Make.ToLower() == "default" ? "Default" : carModel.Model;
            this.YearFrom = this.Make.ToLower() == "default" ? string.Empty : carModel.YearFrom.ToString();
            this.YearTo = this.Make.ToLower() == "default" ? string.Empty : carModel.YearTo.ToString();
            this.VehicleType = carModel.Type.ToString();
        }

        public long Id { get; set; }

        public string Model { get; set; }

        public string Make { get; set; }

        public string YearTo { get; set; }

        public string YearFrom { get; set; }

        public string VehicleType { get; set; }
    }
}