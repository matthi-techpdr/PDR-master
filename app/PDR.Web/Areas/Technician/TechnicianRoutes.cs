using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.Technician.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.Technician
{
    public class TechnicianRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<RepairOrderReportsController>(
                "Technician",
                t => t.Route(
                    new Route("{company}/Technician/{controller}/{action}/{id}",
                                                                   new RouteValueDictionary(
                                                                   new
                                                                   {
                                                                       area = "Technician",
                                                                       action = "Index",
                                                                       id = UrlParameter.Optional
                                                                   }),
                                                                   new MvcRouteHandler())));
        }
    }
}
