using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Users
{
    public class Accountant : Employee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Accountant;
            }
        }

        public Accountant(){}

        public Accountant(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
