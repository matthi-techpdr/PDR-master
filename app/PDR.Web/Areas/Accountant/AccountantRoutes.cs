using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.Accountant.Controllers;
using PDR.Web.Areas.Common.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.Accountant
{
    public class AccountantRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<Controllers.TeamsController>(
                "Accountant",
                accountant => accountant.Route(new Route("{company}/Accountant/{controller}/{action}",
                                  new RouteValueDictionary(
                                  new
                                  {
                                      area = "Accountant",
                                      action = "Index"
                                  }),
                                  new MvcRouteHandler())));
        }
    }
}