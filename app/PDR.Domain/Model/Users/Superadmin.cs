using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Users
{
    public class Superadmin : User
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Superadmin;
            }
        }
    }
}
