using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Effort
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CarModel : CompanyEntity
    {
        public virtual string Make { get; set; }

        public virtual string Model { get; set; }

        public virtual int YearFrom { get; set; }

        public virtual int YearTo { get; set; }

        public virtual VehicleTypes Type { get; set; }

        public virtual ISet<CarSectionsPrice> CarParts { get; set; }

        public CarModel()
        {
            this.CarParts = new HashedSet<CarSectionsPrice>();
        }

        public CarModel(bool isNewEntity = false)
        {
            this.CarParts = new HashedSet<CarSectionsPrice>();
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

    }
}
