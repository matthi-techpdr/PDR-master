using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Users
{
    public class Admin : TeamEmployee
    {
        public override UserRoles Role
        {
            get
            {
                return UserRoles.Admin;
            }
        }
    }
}
