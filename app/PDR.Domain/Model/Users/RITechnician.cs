using PDR.Domain.Model.Enums;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Users
{
    public class RITechnician : TeamEmployee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.RITechnician;
            }
        }

        public RITechnician(){}

        public RITechnician(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}