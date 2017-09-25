using PDR.Domain.Model.Matrixes;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiMatrixModel : BaseIPhoneModel
    {
        public ApiMatrixModel()
        {
        }

        public ApiMatrixModel(Matrix baseMatrix)
        {
            if (baseMatrix != null)
            {
                this.Id = baseMatrix.Id;
                this.Name = baseMatrix.Name;
                this.Description = baseMatrix.Description;
                this.Maximum = baseMatrix.Maximum;
                this.LimitForBodyPartEstimate = baseMatrix.Company.LimitForBodyPartEstimate;
                this.MaxCorrosionProtection = baseMatrix.MaxCorrosionProtection;
                this.CorrosionProtectionPart = baseMatrix.CorrosionProtectionPart;
                this.OversizedDents = baseMatrix.OversizedDents;
                this.AluminiumPanel = baseMatrix.AluminiumPanel;
                this.DoubleLayeredPanels = baseMatrix.DoubleLayeredPanels;
                this.OversizedRoof = baseMatrix.OversizedRoof;
                this.IsDefault = baseMatrix is DefaultMatrix;
                if (this.IsDefault)
                {
                    this.Maximum = baseMatrix.Company.DefaultMatrix.Maximum;
                    this.DefaultHourlyRate = baseMatrix.Company.DefaultHourlyRate;
                }
            }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Maximum { get; set; }

        public bool IsDefault { get; set; }

        public int DefaultHourlyRate { get; set; }

        public int LimitForBodyPartEstimate { get; set; }

        public double MaxCorrosionProtection { get; set; }

        public double CorrosionProtectionPart { get; set; }

        public double OversizedDents { get; set; }

        public int AluminiumPanel { get; set; }

        public int DoubleLayeredPanels { get; set; }

        public int OversizedRoof { get; set; }
    }
}