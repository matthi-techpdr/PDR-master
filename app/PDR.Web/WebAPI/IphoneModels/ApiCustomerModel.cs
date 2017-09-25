using PDR.Domain.Helpers;
using PDR.Domain.Model.Customers;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCustomerModel : BaseIPhoneModel
    {
        public ApiCustomerModel()
        {
        }

        public ApiCustomerModel(Customer customer)
        {
            if (customer == null)
            {
                return;
            }
            this.Id = customer.Id;
            this.Address1 = customer.Address1;
            this.Address2 = customer.Address2;
            this.State = customer.State.ToString();
            this.Zip = customer.Zip;
            this.Phone = customer.Phone;
            this.Email = customer.Email;
            this.Email2 = customer.Email2;
            this.Email3 = customer.Email3;
            this.Email4 = customer.Email4;
            this.Fax = customer.Fax;
            this.CustomerType = customer.CustomerType.ToString();
            this.Name = customer.GetCustomerName();
            this.EstimateEmailSubject = customer.Company.EstimatesEmailSubject;
            this.InvoiceEmailSubject = customer.Company.InvoicesEmailSubject;
            this.RepairOrderEmailSubject = customer.Company.RepairOrdersEmailSubject;
            this.City = customer.City;
        }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string Email2 { get; set; }

        public string Email3 { get; set; }

        public string Email4 { get; set; }

        public string CustomerType { get; set; }

        public string City { get; set; }

        public string Name { get; set; }

        public string EstimateEmailSubject { get; set; }

        public string InvoiceEmailSubject { get; set; }

        public string RepairOrderEmailSubject { get; set; }
    }
}