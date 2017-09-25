using System.Collections.Generic;
using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Estimator.Models.Affiliates
{
    public class AffiliatesCustomerViewModel : IViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string ContactName { get; set; }

        public string Comment { get; set; }
    }
}