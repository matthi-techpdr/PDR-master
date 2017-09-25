using System.Collections.Generic;
using System.Web.Mvc;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class CarInspectionsModel
    {
        public CarInspectionsModel()
        {
            this.CarInspections = new List<CarInspectionsInfo>();
            this.PartsNames = new List<SelectListItem>();
            this.AverageSize = new List<SelectListItem>();
            this.TotalDents = new List<SelectListItem>();
            this.FullPartsNames = new List<string>();
            this.EstimateCustomLines = new List<EstimateCustomLineModel>();
        }

        public IList<CarInspectionsInfo> CarInspections { get; set; }

        public IList<SelectListItem> PartsNames { get; set; }

        public IList<SelectListItem> AverageSize { get; set; }

        public IList<SelectListItem> TotalDents { get; set; }

        public IList<string> FullPartsNames { get; set; }
        [AllowHtml]
        public string PriorDamages { get; set; }

        public IList<EstimateCustomLineModel> EstimateCustomLines { get; set; }

        public double Total { get; set; }

        public double CalculatedSum { get; set; }

        public double? ExtraQuickCost { get; set; }
    }
}