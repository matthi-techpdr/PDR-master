using PDR.Domain.Helpers;
using PDR.Domain.Model.Customers;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
	public class CustomerJsonModel : IJsonModel
	{
        public CustomerJsonModel()
        {
        }

		public CustomerJsonModel(WholesaleCustomer customer)
        {
            this.Id = customer.Id.ToString();
		    this.Name = customer.GetCustomerName();
            this.CreatingDate = customer.CreatingDate.ToString("MM/dd/yyyy");
			this.Email = customer.Email;
			this.Phone = customer.Phone;
			this.Status = customer.Status.ToString();
        }

        public string Id { get; set; }

        public string CreatingDate { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }
	}
}