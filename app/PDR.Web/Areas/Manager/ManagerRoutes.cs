using System.Web.Mvc;
using System.Web.Routing;

using PDR.Web.Areas.Manager.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.Manager
{
    public class ManagerRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<InvoicesController>(
                "Manager",
                t => t.Route(
                    new Route("{company}/Manager/{controller}/{action}/{id}",
                                                                   new RouteValueDictionary(
                                                                   new
                                                                   {
                                                                       area = "Manager",
                                                                       action = "Index",
                                                                       id = UrlParameter.Optional
                                                                   }),
                                                                   new MvcRouteHandler())));
        }
    }
}