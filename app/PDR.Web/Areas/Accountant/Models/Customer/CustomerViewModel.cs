using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
    public class CustomerViewModel : IViewModel
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

        public string Email2 { get; set; }

        public string Email3 { get; set; }

        public string Email4 { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Discount { get; set; }

        public string Password { get; set; }

        public string LaborRate { get; set; }

        public string PartRate { get; set; }

        public string HourlyRate { get; set; }

        public bool Insurance { get; set; }

        public bool EstimateSignature { get; set; }

        public bool OrderSignature { get; set; }

        public bool WorkByThemselve { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public IEnumerable<string> MatricesNames { get; set; }

        public IEnumerable<string> TeamsNames { get; set; }

        public IList<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> TeamsList { get; set; }

        public IEnumerable<SelectListItem> MatricesList { get; set; }

        public IEnumerable<long> MatricesIds { get; set; }

        public IEnumerable<long> TeamsIds { get; set; }

        public int AmountOfOpenEstimates { get; set; }

        public string SumOfOpenEstimates { get; set; }

        public string SumOfOpenWorkOrders { get; set; }

        public string SumOfPaidInvoices { get; set; }

        public string SumOfUnpaidInvoices { get; set; }

        [DisplayName("Customer can create Estimates")]
        public bool CanCreateEstimates { get; set; }
    }
}