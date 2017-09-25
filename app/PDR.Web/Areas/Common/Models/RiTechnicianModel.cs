namespace PDR.Web.Areas.Common.Models
{
    public class RiTechnicianModel
    {
        public bool IsVisible { get; set; }

        public bool? IsFlatFee { get; set; }

        public double? PaymentFlatFee { get; set; }

        public double? PaymentRiOperations { get; set; }

        public RiTechnicianModel()
        {
            this.IsVisible = false;
            this.IsFlatFee = null;
            this.PaymentFlatFee = 0;
        }
    }
}