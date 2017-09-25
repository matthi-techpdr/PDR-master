using PDR.Domain.Model.Customers;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiRetailCustomerModel : ApiCustomerModel
    {
        public ApiRetailCustomerModel()
        {
        }

        public ApiRetailCustomerModel(RetailCustomer customer)
            : base(customer)
        {
            if (customer != null)
            {
                this.FirstName = customer.FirstName;
                this.LastName = customer.LastName;
                this.DefaultMatrixId = customer.DefaultMatrix.Id;
                this.HourlyRate = customer.Company.DefaultHourlyRate;
                this.LimitForBodyPartEstimate = customer.Company.LimitForBodyPartEstimate;
            }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int HourlyRate { get; set; }

        public long DefaultMatrixId { get; set; }

        public int LimitForBodyPartEstimate { get; set; }
    }
}