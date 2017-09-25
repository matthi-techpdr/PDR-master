using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCarModel : BaseIPhoneModel
    {
        public ApiCarModel()
        {
        }

        public ApiCarModel(Car car)
        {
            if (car != null)
            {
                this.Color = car.Color;
                this.Model = car.Model;
                this.VIN = car.VIN;
                this.Make = car.Make;
                this.State = car.State.ToString();
                this.LicensePlate = car.LicensePlate;
                this.Trim = car.Trim;
                this.Year = car.Year;
                this.Mileage = car.Mileage;
                this.CustRO = car.CustRO;
                this.Stock = car.Stock;
                this.Type = car.Type.HasValue ? car.Type.ToString() : string.Empty;
            }
        }

        public string VIN { get; set; }

        public string Model { get; set; }

        public string Make { get; set; }

        public int Year { get; set; }

        public string Trim { get; set; }

        public int Mileage { get; set; }

        public string Color { get; set; }

        public string LicensePlate { get; set; }

        public string State { get; set; }

        public string CustRO { get; set; }

        public string Stock { get; set; }

        public string Type { get; set; }
    }
}