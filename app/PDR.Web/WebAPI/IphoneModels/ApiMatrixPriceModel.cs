using PDR.Domain.Model.Matrixes;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiMatrixPriceModel : BaseIPhoneModel
    {
        public ApiMatrixPriceModel()
        {
        }

        public ApiMatrixPriceModel(Price price)
        {
            if (price != null)
            {
                this.Id = price.Id;
                this.PartOfBody = price.PartOfBody.ToString();
                this.AverageSize = price.AverageSize.ToString();
                this.TotalDents = price.TotalDents.ToString();
                this.Cost = price.Cost;
            }
        }

        public string PartOfBody { get; set; }

        public string AverageSize { get; set; }

        public string TotalDents { get; set; }

        public double Cost { get; set; } 
    }
}