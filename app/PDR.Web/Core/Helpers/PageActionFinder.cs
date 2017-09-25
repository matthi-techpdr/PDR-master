using System;
using System.Linq.Expressions;
using System.Web.Mvc;

using SmartArch.Core.Helpers;

namespace PDR.Web.Core.Helpers
{
    public static class PageActionFinder
    {
         public static bool IsExecuting<TController>(this WebViewPage page, Expression<Action<TController>> action)
         {
             var controllerName = typeof(TController).Name.Replace("Controller", string.Empty);
             var noMatchController = page.GetExecutingControllerName() != controllerName;
             if (noMatchController)
             {
                 return false;
             }

             var actionName = Reflector.Method(action).Name;
             var noMatchAction = page.GetExecutingActionName() != actionName;
             if (noMatchAction)
             {
                 return false;
             }

             return true;
         }

         public static string GetExecutingControllerName(this WebViewPage page)
         {
             return page.ViewContext.RouteData.GetRequiredString("controller");
         }

        public static string GetExecutingActionName(this WebViewPage page)
         {
             return page.ViewContext.RouteData.GetRequiredString("action");
         }
    }
}