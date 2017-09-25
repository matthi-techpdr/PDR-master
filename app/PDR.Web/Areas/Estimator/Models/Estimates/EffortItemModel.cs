using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class EffortItemModel
    {
        public long? Id { get; set; }

        public string Name { get; set; }

        public double? HoursR_R { get; set; } //probe

        public double? HoursR_I { get; set; } //probe

        public double? Hours { get; set; }

        public Operations? Operations { get; set; }

        public bool? Choosed { get; set; }

        public ChosenEffortType EffortType { get; set; }
    }
}