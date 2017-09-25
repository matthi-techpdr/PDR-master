using System.Web.Mvc;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class SendEmailViewModel
    {
        [AllowHtml]
        public string Message { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }
         
        public string To2 { get; set; }

        public string To3 { get; set; }

        public string To4 { get; set; }

        public bool IsWholesale { get; set; }
        
        public string IDs { get; set; }

        public bool IsBasic { get; set; }
    }
}