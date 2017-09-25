using System.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Core.Helpers
{
    public static class LinkRouteResolver
    {
        public static Dictionary<string, object> ResolveRoute(UserRoles role)
        {
            var routeDictionary = new Dictionary<string, object>();

            switch (role)
            {
                case UserRoles.Estimator:
                    routeDictionary.Add("area", "Estimator");
                    routeDictionary.Add("controller", "Reports");
                    routeDictionary.Add("action", "Details");
                    break;
                case UserRoles.Technician:
                    routeDictionary.Add("area", "Technician");
                    routeDictionary.Add("controller", "EstimateReports");
                    routeDictionary.Add("action", "Details");
                    break;
            }

            return routeDictionary;
        }
    }
}