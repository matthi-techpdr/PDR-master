using System.Web;
using System.Web.Mvc;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Wrappers
{
    public static class ManagerTabWrapper
    {
        private static string CanEditTeamMembersKey = "CanEditTeamMembers";

        public static bool IsCompanyTabAvaliable
        {
            get
            {
                if ((HttpContext.Current.User != null) && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.Session[CanEditTeamMembersKey] == null)
                    {
                        User currentUser = DependencyResolver.Current.GetService<ICurrentWebStorage<Employee>>().Get() ?? DependencyResolver.Current.GetService<ICurrentWebStorage<User>>().Get();
                        if (currentUser is Manager)
                        {
                            HttpContext.Current.Session[CanEditTeamMembersKey] = (currentUser as Employee).CanEditTeamMembers.GetValueOrDefault();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return (bool)HttpContext.Current.Session[CanEditTeamMembersKey];
                }
                return false;
            }
            set { HttpContext.Current.Session[CanEditTeamMembersKey] = value; }
        }
    }
}