using System.Web.Mvc;
using System.Web.Routing;

using PDR.Web.Areas.Worker.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.Worker
{
    public class WorkerRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<DownloadController>(
                "Worker",
                admin => admin.Route(
                    new Route("Worker/{controller}/{action}",
                    new RouteValueDictionary(
                    new
                    {
                        area = "Worker",
                        action = "Index"
                    }),
                    new MvcRouteHandler())));
        }
    }
}
