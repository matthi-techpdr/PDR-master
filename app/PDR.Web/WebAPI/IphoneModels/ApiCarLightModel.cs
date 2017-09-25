using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCarLightModel : BaseIPhoneModel
    {
        public ApiCarLightModel()
        {
        }

        public ApiCarLightModel(Car car)
        {
            if (car != null)
            {
                this.Model = car.Model;
                this.Make = car.Make;
                this.Year = car.Year;
            }
        }

        public string Model { get; set; }

        public string Make { get; set; }

        public int Year { get; set; }
    }
}