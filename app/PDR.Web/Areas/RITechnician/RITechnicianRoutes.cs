using System.Web.Mvc;
using System.Web.Routing;

using PDR.Web.Areas.RITechnician.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.RITechnician
{
    public class RITechnicianRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<RepairOrdersController>(
                "RITechnician",
                t => t.Route(
                    new Route("{company}/RI_Technician/{controller}/{action}/{id}",
                                                                   new RouteValueDictionary(
                                                                   new
                                                                   {
                                                                       area = "RITechnician",
                                                                       action = "Index",
                                                                       id = UrlParameter.Optional
                                                                   }),
                                                                   new MvcRouteHandler())));
        }
    }
}
