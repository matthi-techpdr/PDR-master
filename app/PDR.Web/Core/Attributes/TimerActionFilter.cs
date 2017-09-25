using System;
using System.Diagnostics;
using System.Web.Mvc;
using NLog;

namespace PDR.Web.Core.Attributes
{
    public class TimerActionFilter : System.Web.Mvc.ActionFilterAttribute
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        private readonly Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.stopwatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            this.stopwatch.Stop();
            var result = this.stopwatch.ElapsedMilliseconds;
            var msg = string.Format("{0} - {1} milliseconds", filterContext.HttpContext.Request.Url, result.ToString());
            Loggger.Log(LogLevel.Debug, msg);
        }
    }

    public class TimerWebApiActionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        private readonly Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            this.stopwatch.Start();
        }

        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            this.stopwatch.Stop();
            var result = this.stopwatch.ElapsedMilliseconds;
            var msg = string.Format("{0} - {1} milliseconds", actionExecutedContext.Request.RequestUri, result.ToString());
            Loggger.Log(LogLevel.Debug, msg);
        }
    }
}