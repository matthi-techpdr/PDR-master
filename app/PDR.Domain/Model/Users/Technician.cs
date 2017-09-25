using PDR.Domain.Model.Enums;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Users
{
    public class Technician : TeamEmployee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Technician;
            }
        }

        public Technician(){}

        public Technician(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
