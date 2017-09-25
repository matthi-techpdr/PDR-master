using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.Admin.Controllers;
using RestfulRouting;

namespace PDR.Web.Areas.Admin
{
    public class AdminRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Area<CompanyInfoController>(
                "Admin",
                admin => admin.Route(
                    new Route("{company}/Admin/{controller}/{action}/{id}",
                                                                   new RouteValueDictionary(
                                                                   new
                                                                   {
                                                                       area = "Admin",
                                                                       action = "Index",
                                                                       id = UrlParameter.Optional
                                                                   }),
                                                                   new MvcRouteHandler())));
        }
    }
}
