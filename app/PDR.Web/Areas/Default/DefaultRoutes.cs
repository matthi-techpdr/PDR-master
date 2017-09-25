using System.Web.Mvc;
using System.Web.Routing;
using PDR.Web.Areas.Default.Controllers;
using RestfulRouting;

namespace PDR.Web.Areas.Default
{
    public class DefaultRoutes : RouteSet
    {
        public const string AREA_NAME = "Default";
        
        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");
            map.Area<AccountController>(
                AREA_NAME,
                def =>
                {
                    def.Route(new Route(string.Empty, new RouteValueDictionary(new { area = AREA_NAME, controller = "Home", action = "Index" }), new MvcRouteHandler()));
                    def.Route(new Route("{company}/Account/LogOn", new RouteValueDictionary(new { area = AREA_NAME, controller = "Account", action = "LogOn" }), new RouteValueDictionary(new { company = @"^[A-Za-z0-9]+([_-]?[A-Za-z0-9]+)+$" }), new MvcRouteHandler()));
                    def.Route(new Route("{company}/Account/LogOut", new RouteValueDictionary(new { area = AREA_NAME, controller = "Account", action = "LogOut" }), new RouteValueDictionary(new { company = @"^[A-Za-z0-9]+([_-]?[A-Za-z0-9]+)+$" }), new MvcRouteHandler()));
                    def.Route(new Route("{company}", new RouteValueDictionary(new { area = AREA_NAME, controller = "Home", action = "Index" }), new RouteValueDictionary(new { company = @"^[A-Za-z0-9]+([_-]?[A-Za-z0-9]+)+$" }), new MvcRouteHandler()));
                    def.Route(new Route("{controller}/{action}", new RouteValueDictionary(new { area = AREA_NAME, action = "Index" }), new MvcRouteHandler()));
                    def.Route(new Route("{company}/Common/{controller}/{action}", new RouteValueDictionary(new { area = AREA_NAME }), new MvcRouteHandler()));  
                });
        }
    }
}