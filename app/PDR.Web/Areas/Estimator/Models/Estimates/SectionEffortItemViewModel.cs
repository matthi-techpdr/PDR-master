using System.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class SectionEffortItemViewModel
    {
        public IList<EffortItemModel> EffortItems { get; set; }

        public PartOfBody Name { get; set; }

        public double Cost { get; set; }
    }
}