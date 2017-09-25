using System.Collections.Generic;
using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Estimator.Models.Customers
{
    public class WholesaleCustomerViewModel : IViewModel
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

        public string Email2 { get; set; }

        public string Email3 { get; set; }

        public string Email4 { get; set; }
        
        public string ContactName { get; set; }

        public string Comment { get; set; }

        public int AmountOfOpenEstimates { get; set; }

        public string SumOfOpenEstimates { get; set; }

        public string SumOfOpenWorkOrders { get; set; }

        public string SumOfPaidInvoices { get; set; }

        public string SumOfUnpaidInvoices { get; set; }
    }
}