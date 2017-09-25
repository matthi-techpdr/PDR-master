using Newtonsoft.Json;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Effort
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class ChosenEffortItem : CompanyEntity
	{
        [JsonIgnore]
		public virtual EffortItem EffortItem { get; set; }

		public virtual bool Choosed { get; set; }

        public virtual double Hours { get; set; }

        public virtual Operations Operations { get; set; }

        public virtual CarInspection CarInspection { get; set; }

        public virtual double CalculateEffort()
        {
            if (this.EffortItem != null)
            {
                return this.Operations == Operations.R_I ? this.EffortItem.HoursR_I.Value : this.EffortItem.HoursR_R.Value;
            }

            return 0;
        }

        public virtual ChosenEffortType ChosenEffortType { get; set; }

        public ChosenEffortItem(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public ChosenEffortItem()
        {
            
        }
    }
}
