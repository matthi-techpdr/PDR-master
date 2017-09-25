using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PDR.Domain.Model.Effort;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiEffortItemModel
    {
        public ApiEffortItemModel()
        {
        }

        public ApiEffortItemModel(EffortItem item)
        {
            //item.CarSectionsPrices.
        }

        public string Name { get; set; }

        public double? HoursR_R { get; set; }

        public double? HoursR_I { get; set; }

        public CarSectionsPrice CarSectionsPrices { get; set; }
    }
}