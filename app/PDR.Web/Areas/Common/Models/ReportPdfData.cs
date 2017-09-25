using System.Collections.Generic;

using PDR.Domain.Model.Users;

namespace PDR.Web.Areas.Common.Models
{
    public class ReportPdfData
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string Customer { get; set; }

        public string Team { get; set; }

        public IEnumerable<ReportItem> ReportItems { get; set; }

        public Employee Employee { get; set; }

        //public string LogoUrl { get; set; }
    }
}