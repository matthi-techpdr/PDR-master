using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.SuperAdmin.Controllers;
using RestfulRouting;

namespace PDR.Web.Areas.SuperAdmin
{
    public class SuperAdminRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<CompaniesController>(
                "SuperAdmin",
                admin => admin.Route(
                    new Route("Superadmin/{controller}/{action}",
                    new RouteValueDictionary(
                    new
                    {
                        area = "SuperAdmin",
                        action = "Index"
                    }),
                    new MvcRouteHandler())));
        }
    }
}
