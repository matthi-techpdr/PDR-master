using System.Collections.Generic;
using System.Web.Mvc;

using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleInfoModel : IViewModel
    {
        public string Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string YearTo { get; set; }

        public string YearFrom { get; set; }

        public VehicleTypes VehicleType { get; set; }

        public IList<SelectListItem> VehicleTypes { get; set; }

        public IList<SelectListItem> AllMakeNames { get; set; }
    }
}