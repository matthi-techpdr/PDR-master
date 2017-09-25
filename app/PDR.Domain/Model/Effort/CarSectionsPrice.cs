using System.Collections;
using System.Xml.Serialization;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Effort
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CarSectionsPrice : CompanyEntity
    {
        public virtual PartOfBody Name { get; set; }

        public virtual CarModel CarModel { get; set; }

        public virtual double NewSectionPrice { get; set; }

        public virtual ISet<EffortItem> EffortItems { get; set; }

        public CarSectionsPrice()
        {
            this.EffortItems = new HashedSet<EffortItem>();
        }

        public CarSectionsPrice(bool isNewEntity = false)
        {
            this.EffortItems = new HashedSet<EffortItem>();
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

    }
}
