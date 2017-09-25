using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class Car : CompanyEntity
    {
		public virtual string VIN { get; set; }

		public virtual string Model { get; set; }

		public virtual string Make { get; set; }

		public virtual int Year { get; set; }

		public virtual string Trim { get; set; }

        public virtual int Mileage { get; set; }

		public virtual string Color { get; set; }

		public virtual string LicensePlate { get; set; }

		public virtual StatesOfUSA? State { get; set; }

		public virtual string CustRO { get; set; }

		public virtual string Stock { get; set; }

        public virtual VehicleTypes? Type { get; set; }

        public Car(){}

        public Car(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual string GetYearMakeModelInfo()
        {
            return string.Format("{0}/{1}/{2}", this.Year, this.Make, this.Model);
        }
    }
}
