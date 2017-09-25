using System.Collections.Generic;
using System.Web.Mvc;

using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
    public class AffiliatesViewModel : IViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string LaborRate { get; set; }

        public string PartRate { get; set; }

        public string HourlyRate { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public IEnumerable<string> TeamsNames { get; set; }

        public IList<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> TeamsList { get; set; }

       public IEnumerable<long> TeamsIds { get; set; }

       public int AmountOfOpenEstimates { get; set; }

       public string SumOfOpenEstimates { get; set; }

       public string SumOfOpenWorkOrders { get; set; }

       public string SumOfPaidInvoices { get; set; }

       public string SumOfUnpaidInvoices { get; set; }

    }
}