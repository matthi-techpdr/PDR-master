using PDR.Web.Areas.Default;

namespace PDR.Web.Core.Helpers
{
    public static class LayoutHelper
    {
         public static void SetLayout(this object pageObj)
         {
             var page = (dynamic)pageObj;

             var areaName = page.Request.RequestContext.RouteData.Values["area"] ?? DefaultRoutes.AREA_NAME;

             page.Layout = string.Format("~/Areas/{0}/Views/Shared/_{0}Layout.cshtml", areaName);
         }

        //public static void SetEstimateLayout(this object pageObj)
        //{
        //    var page = (dynamic)pageObj;

        //    var areaName = page.Request.RequestContext.RouteData.Values["area"] ?? DefaultRoutes.AREA_NAME;
        //    if (areaName == "Wholesaler")
        //    {
        //        page.Layout = string.Format("~/Areas/Wholesaler/Views/Estimates/Partial/OnlyEstimateLayout.cshtml");                
        //    }
        //    else
        //    {
        //        page.Layout = string.Format("~/Areas/Сommon/Views/Shared/Partial/OnlyEstimateLayout.cshtml");
        //    }
        //}
    }
}