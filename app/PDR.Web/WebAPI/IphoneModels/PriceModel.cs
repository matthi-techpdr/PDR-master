using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class PriceModel
    {
        public PartOfBody PartOfBody { get; set; }

        public AverageSize AverageSize { get; set; }

        public TotalDents TotalDents { get; set; }

        public long MatrixId { get; set; }

        public double Cost { get; set; }
    }
}