using System;
using System.Web;
using System.Web.Mvc;

namespace PDR.Web.HttpModules
{
    public class UnauthorizeAjaxRequestHandler : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += this.HandleUnauthorizeAjaxRequest;
        }

        private void HandleUnauthorizeAjaxRequest(object sender, EventArgs e)
        {
            var currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                var context = new HttpContextWrapper(currentContext);
                if (context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}