using System.Web.Mvc;
using System.Web.Routing;

using PDR.Web.Areas.Estimator.Controllers;

using RestfulRouting;

namespace PDR.Web.Areas.Estimator
{
    public class EstimatorRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<EstimatesController>(
                "Estimator",
                admin => admin.Route(new Route("{company}/Estimator/{controller}/{action}/{id}",
                                     new RouteValueDictionary(new
                                         {
                                             area = "Estimator",
                                             action = "Index",
                                             id = UrlParameter.Optional
                                         }),
                                     new MvcRouteHandler())));
        }
    }
}
