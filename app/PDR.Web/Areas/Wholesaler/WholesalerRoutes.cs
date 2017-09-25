using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.Wholesaler.Controllers;
using RestfulRouting;

namespace PDR.Web.Areas.Wholesaler
{
    public class WholesalerRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<InvoiceReportsController>(
                "Wholesaler",
                t => t.Route(
                    new Route("{company}/Wholesaler/{controller}/{action}/{id}",
                                                                   new RouteValueDictionary(
                                                                   new
                                                                   {
                                                                       area = "Wholesaler",
                                                                       action = "Index",
                                                                       id = UrlParameter.Optional
                                                                   }),
                                                                   new MvcRouteHandler())));
        }
    }
}