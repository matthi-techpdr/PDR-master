using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiSupplementModel
    {
        public ApiSupplementModel()
        {
        }

        public ApiSupplementModel(Supplement supplement)
        {
            this.Id = supplement.Id;
            this.Description = supplement.Description;
            this.Cost = supplement.Sum;
        }

        public long Id { get; set; }

        public string Description { get; set; }

        public double Cost { get; set; }
    }
}