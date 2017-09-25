namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class EstimateCustomerModel
    {
        public EstimateCustomerModel()
        {
            this.Retail = new RetailCustomerModel();
            this.Wholesale = new EstimateWholesaleCustomerModel();
            this.CustomerType = EstimateCustomerType.Retail;
        }

        public RetailCustomerModel Retail { get; set; }
        
        public EstimateWholesaleCustomerModel Wholesale { get; set; }

        public EstimateCustomerType CustomerType { get; set; }
    }
}