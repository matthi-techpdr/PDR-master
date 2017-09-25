using PDR.Domain.Model.Enums;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Users
{
    public class Manager : TeamEmployee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Manager;
            }
        }

        public Manager()
        {
            this.CanEditTeamMembers = false;
        }

        public Manager(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                this.CanEditTeamMembers = false;
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
