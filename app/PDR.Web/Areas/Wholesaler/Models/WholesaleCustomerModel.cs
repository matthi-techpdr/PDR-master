using PDR.Domain.Model.Customers;

namespace PDR.Web.Areas.Wholesaler.Models
{
    public class WholesaleCustomerModel///TODO: make creating WholesaleCustomerModel using automapper
    {
        public bool Insurance { get; set; }
        public bool WorkByThemselve { get; set; }
        public double LaborRate { get; set; }
        public double HourlyRate { get; set; }
        public bool EstimateSignature { get; set; }
        public int Discount { get; set; }

        public WholesaleCustomerModel()
        {
        }

        public WholesaleCustomerModel(WholesaleCustomer wholesalerCustomer)
        {
            this.Insurance = wholesalerCustomer.Insurance;
            this.WorkByThemselve = !wholesalerCustomer.WorkByThemselve;
            this.LaborRate = wholesalerCustomer.LaborRate;
            this.HourlyRate = wholesalerCustomer.HourlyRate;
            this.EstimateSignature = wholesalerCustomer.EstimateSignature;
            this.Discount = wholesalerCustomer.Discount;
        }
    }
}