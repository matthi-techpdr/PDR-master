using System.Web;
using System.Web.Mvc;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Wrappers
{
    public static class WholesalerTabWrapper
    { 
        private static string CanCreateEstimatesKey = "CanCreateEstimates";

        public static bool IsAddEstimateTabAvaliable
        {
            get
            {
                if ((HttpContext.Current.User != null) && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.Session[CanCreateEstimatesKey] == null)
                    {
                        User currentUser = DependencyResolver.Current.GetService<ICurrentWebStorage<Employee>>().Get() ?? DependencyResolver.Current.GetService<ICurrentWebStorage<User>>().Get();
                        if (currentUser is Wholesaler)
                        {
                            HttpContext.Current.Session[CanCreateEstimatesKey] = (currentUser as Wholesaler).CanEditTeamMembers.GetValueOrDefault();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return (bool)HttpContext.Current.Session[CanCreateEstimatesKey];
                }
                return false;
            }
            set { HttpContext.Current.Session[CanCreateEstimatesKey] = value; }
        }
    }
}