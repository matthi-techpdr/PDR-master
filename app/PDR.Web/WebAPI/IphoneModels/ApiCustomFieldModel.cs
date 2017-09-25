using PDR.Domain.Model.CustomLines;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCustomFieldModel
    {
        public ApiCustomFieldModel()
        {
        }

        public ApiCustomFieldModel(CustomLine line)
        {
            if (line != null)
            {
                this.Id = line.Id;
                this.Name = line.Name;
                this.Cost = line.Cost;
            }
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }
    }
}