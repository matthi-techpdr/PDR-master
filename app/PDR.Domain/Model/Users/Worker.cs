using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Users
{
    public class Worker : User
    {
        public override UserRoles Role
        {
            get { return UserRoles.Worker; }

        }

    }
}