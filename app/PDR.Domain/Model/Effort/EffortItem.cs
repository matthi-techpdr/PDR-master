using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.Effort
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class EffortItem : CompanyEntity
	{
		public virtual string Name { get; set; }

		public virtual double? HoursR_R { get; set; }

        public virtual double? HoursR_I { get; set; }

        public virtual CarSectionsPrice CarSectionsPrices { get; set; }

        public EffortItem(){}

        public EffortItem(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
	}
}
