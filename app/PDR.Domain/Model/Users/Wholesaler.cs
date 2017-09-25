using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Users
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class Wholesaler : Employee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Wholesaler;
            }
        }

        public Wholesaler()
        {
            CanCreateEstimates = false;
        }

        public Wholesaler(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                CanCreateEstimates = false;
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual bool CanCreateEstimates { get; set; }
    }
}
