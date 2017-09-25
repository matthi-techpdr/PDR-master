using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

using PDR.Web.Areas.Default.Controllers;

namespace PDR.Web.Core.Authorization
{
    public class UnauthorizeHandler
    {
        public void Handle401Error(HttpRequest request, HttpResponse response)
        {
            string redirectUrl = this.GetLogOnUrl(request);
            if (redirectUrl != null)
            {
                response.Redirect(redirectUrl);
            }
        }

        private const string COMPANY_KEY = "company";

        public string GetLogOnUrl(HttpRequest request)
        {
            var routeCollection = RouteTable.Routes;
            var defRouteData = routeCollection.GetRouteData(new HttpContextWrapper(HttpContext.Current)) ?? new RouteData();
            var requestContext = new RequestContext(new HttpContextWrapper(HttpContext.Current), defRouteData);
            var company = this.GetCompanyUrlPart(request);
            var returnUrl = this.GetReturnUrl(request, company);
            Expression<Action<AccountController>> action = a => a.LogOn(returnUrl);
            RouteValueDictionary routeValuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
            routeValuesFromExpression.Add("area", "Default");
            if (company != null)
            {
                routeValuesFromExpression.Add(COMPANY_KEY, company);
            }

            VirtualPathData virtualPathData = routeCollection.GetVirtualPathForArea(requestContext, routeValuesFromExpression);
            var redirectUrl = virtualPathData != null ? virtualPathData.VirtualPath : null;
            if(redirectUrl != null)
            {
                if (redirectUrl.ToLower().Contains("superadmin/account"))
                {
                    redirectUrl = redirectUrl.Replace("Superadmin/Account", "Account");
                }
            }
            return redirectUrl;
        }

        private string GetCompanyUrlPart(HttpRequest request)
        {
            var routeValues = request.RequestContext.RouteData.Values;
            object company;
            routeValues.TryGetValue(COMPANY_KEY, out company);

            return (string)company;
        }

        private string GetReturnUrl(HttpRequest request, string companyUrlPart)
        {
            var companyUrl = @"/" + (companyUrlPart ?? string.Empty);
            var currentUrl = request.Url.LocalPath;
            var isDefaultUrl = currentUrl == @"/" || currentUrl == companyUrl;
            var returnUrl = isDefaultUrl ? null : currentUrl;

            return returnUrl;
        }
    }
}