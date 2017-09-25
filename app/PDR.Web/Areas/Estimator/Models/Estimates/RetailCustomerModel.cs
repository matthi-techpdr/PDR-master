using System.Collections.Generic;
using System.Web.Mvc;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Automapper;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    using System.Linq;

    using PDR.Domain.Model.Matrixes;

    public class RetailCustomerModel
	{
	    public RetailCustomerModel()
	    {
	    }

	    public RetailCustomerModel(RetailCustomer customer)
	    {
	        CustomAutomapper.Map(customer, this);
	    }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

        public string Zip { get; set; }
        
        public StatesOfUSA State { get; set; }

        public string City { get; set; }

		public string Phone { get; set; }

		public string Fax { get; set; }

		public string Email { get; set; }

        public string AffiliateId { get; set; }

        public IList<SelectListItem> States { get; set; }

        public IList<SelectListItem> Affiliates { get; set; }

	}
}