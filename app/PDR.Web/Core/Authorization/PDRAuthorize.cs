using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Core.Authorization
{
    public class PDRAuthorize : AuthorizeAttribute
    {
        protected User CurrentUser()
        {
            return DependencyResolver.Current.GetService<ICurrentWebStorage<Employee>>().Get() ?? DependencyResolver.Current.GetService<ICurrentWebStorage<User>>().Get();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var user = this.CurrentUser();
            if (user != null)
            {
                if (user is Employee)
                {
                    var employee = user as Employee;
                    HttpContext.Current.Request.RequestContext.RouteData.Values["company"] = employee.Company.Url;
                    if (employee.Status != Statuses.Active || employee.Company.Status != Statuses.Active)
                    {
                        return false;
                    }
                }

                if (this.Roles == string.Empty || this.Roles.Split(',').Contains(user.Role.ToString()))
                {
                    return true;
                }                
            }
            
            var dictionary = HttpContext.Current.Application["Authorize"] as Dictionary<Guid, long>;
            if (HttpContext.Current.Request.QueryString["token"] == null)
            {
                return false;
            }

            var guid = Guid.Parse(HttpContext.Current.Request.QueryString["token"]);
            if (dictionary == null)
            {
                return false;
            }
            
            var keys = dictionary.Keys;
            foreach (var key in keys)
            {
                if (key != guid)
                {
                    continue;
                }

                var ticks = dictionary[key];
                if(DateTime.Now.Ticks - ticks < new TimeSpan(2, 0, 0).Ticks)
                {
                    return true;
                }

                dictionary.Remove(key);
                return false;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var handler = new UnauthorizeHandler();
            handler.Handle401Error(HttpContext.Current.Request, HttpContext.Current.Response);

            string logOnUrl = handler.GetLogOnUrl(HttpContext.Current.Request);
            filterContext.Result = new RedirectResult(logOnUrl);
        }
    }
}