using System.Collections.Generic;

namespace PDR.Web.Areas.Technician.Models.Reports
{
    public class InvoiceReportModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<long> InvoicesIds { get; set; }
    }
}