using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Services.VersionStorage;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace PDR.Web.WebAPI.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ApiAuthorize : AuthorizeAttribute
    {
        public bool Ignore { get; set; }

        protected User CurrentUser()
        {
            return ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get() ?? ServiceLocator.Current.GetInstance<ICurrentWebStorage<User>>().Get();
        }

        protected bool IsWorkingCurrentVersionApp
        {
            get { return ServiceLocator.Current.GetInstance<IVersionStorage<VersioniPhoneApp>>().IsWorking(); }
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (this.Ignore || this.AuthorizeCore(new HttpContextWrapper(HttpContext.Current)))
            {
                return;
            }

            this.HandleUnauthorizedRequest(actionContext);
        }

        protected bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //if(!this.IsWorkingCurrentVersionApp)
            //{
            //    return false;
            //}

            var user = this.CurrentUser();
            if (user != null)
            {
                if (user is Employee)
                {
                    var employee = user as Employee;
                    HttpContext.Current.Request.RequestContext.RouteData.Values["company"] = employee.Company.Url;
                }

                if (this.Roles == string.Empty || this.Roles.Split(',').Contains(user.Role.ToString()))
                {
                    ////==================================UNCOMMIT=========================================

                    var employee = (Employee)user;
                    var activeLicense = employee.Licenses.SingleOrDefault(x => x.Status == LicenseStatuses.Active); //Where(x => x.Status == LicenseStatuses.Active)
                    if (activeLicense == null || employee.Status != Statuses.Active || employee.Company.Status != Statuses.Active) // activeLicense.Count == 0
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}