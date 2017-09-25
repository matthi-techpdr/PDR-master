using System.Web.Mvc;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class CustomLineModel
    {
        public long? Id { get; set; }
        [AllowHtml]
        public string Name { get; set; }
        
        public double? Cost { get; set; }
    }
}