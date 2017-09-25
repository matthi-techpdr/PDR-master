using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class InsuranceModel
    {
        [AllowHtml]
        public string InsuredName { get; set; }
        [AllowHtml]
        public string CompanyName { get; set; }
        [AllowHtml]
        public string Policy { get; set; }

        public string Claim { get; set; }

        public DateTime? ClaimDate { get; set; }

        public DateTime? AccidentDate { get; set; }

        public string Phone { get; set; }

        public string ContactName { get; set; }

        public IList<SelectListItem> AllCompanyNames { get; set; }
    }
}