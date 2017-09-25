using PDR.Domain.Model.Base;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class Supplement : CompanyEntity
    {
        public virtual string Description { get; set; }

        public virtual double Sum { get; set; }

        public virtual RepairOrder RepairOrder { get; set; }

        public Supplement(){}

        public Supplement(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}