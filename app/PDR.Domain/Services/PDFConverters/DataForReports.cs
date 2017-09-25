using System.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.PDFConverters
{
    public class DataForReports
    {
        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public string Customer { get; set; }

        public string Team { get; set; }

        public IEnumerable<ICompanyEntity> Entities { get; set; }

        public Employee Employee { get; set; }

        public bool Commission { get; set; }

        public string EntityType { get; set; }
    }
}
