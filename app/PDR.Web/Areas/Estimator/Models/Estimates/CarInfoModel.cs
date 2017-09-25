using System.Collections.Generic;
using System.Web.Mvc;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class CarInfoModel
    {
        public long? Id { get; set; }

        public string VIN { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Trim { get; set; }

        public string Year { get; set; }

        public string Mileage { get; set; }

        public string Color { get; set; }

        public string LicensePlate { get; set; }

        public StatesOfUSA State { get; set; }

        public VehicleTypes Type { get; set; }

        public string CustRO { get; set; }

        public string Stock { get; set; }

        public IList<SelectListItem> States { get; set; }

        public IList<SelectListItem> VehicleTypes { get; set; }
    }
}