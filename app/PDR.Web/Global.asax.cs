using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FluentValidation.Mvc;
using PDR.Domain.Services.PushNotification;
using PDR.Init.FluentValidation;
using PDR.Init.IoC;
using PDR.Resources.PDR.Domain.Model;
using PDR.Web.Areas.Accountant;
using PDR.Web.Areas.Default;
using PDR.Web.Areas.Estimator;
using PDR.Web.Areas.Estimator.Controllers;
using PDR.Web.Areas.Manager;
using PDR.Web.Areas.RITechnician;
using PDR.Web.Areas.SuperAdmin;
using PDR.Web.Areas.Technician;
using PDR.Web.Areas.Wholesaler;
using PDR.Web.Areas.Worker;
using PDR.Web.Core;
using PDR.Web.Core.Attributes;
using PDR.Web.Core.Formatters;
using PDR.Web.Core.NLog;
using PDR.Web.Core.PushNotification;
using SmartArch.Core.Helpers;
using SmartArch.Core.Helpers.EntityLocalization;

namespace PDR.Web
{
    using PDR.Web.Areas.Admin;
    using PDR.Web.Core.NLog.FileLoggers;

    using RestfulRouting;

    public class MvcApplication : HttpApplication
    {

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes()
        {
            var routes = RouteTable.Routes;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(jpg|gif|jpeg|png|js|css|htm|html|htc|txt)$" });
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            //web api routes
            routes.MapHttpRoute("PdrApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            routes.MapRoute("ApiRoute", "api/help/{controller}/{action}/{id}", new { id = UrlParameter.Optional }, new[] { "PDR.Web.WebAPI.HelpControllers" });
            
            routes.MapRoutes<WorkerRoutes>();
            routes.MapRoutes<SuperAdminRoutes>();
            routes.MapRoutes<DefaultRoutes>();
            routes.MapRoutes<AccountantRoutes>();
            routes.MapRoutes<EstimatorRoutes>();
            routes.MapRoutes<TechnicianRoutes>();
            routes.MapRoutes<RITechnicianRoutes>();
            routes.MapRoutes<ManagerRoutes>();
            routes.MapRoutes<AdminRoutes>();
            routes.MapRoutes<WholesalerRoutes>();
            
        }

        protected void Application_Start()
        {
            Application["Authorize"] = new Dictionary<Guid, long>();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CustomViewEngine());
            JsonNetFormatterInitializer.Init();
            AutomapperInitializer.RegisterAutoMappers();
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            EntityLocalizationHelper.Engine = new EntityLocalizationEngine(typeof(EntityNamesRes));
            AssemblyVersionHelper.TypeFromAssembly = typeof(EstimatesController);
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes();

            DependencyResolverInitializer.Initialize(x => x.For(typeof(IPushNotification)).Use(typeof(IphonePushNotification)));

            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new StructureMapValidatorFactory();
            });

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            NLogConfigurator.Init();
            GlobalFilters.Filters.Add(new TimerActionFilter());
            GlobalConfiguration.Configuration.Filters.Add(new TimerWebApiActionFilter());
        }

        protected void Application_BeginRequest()
        {
            Response.Buffer = true;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        }

        protected void Application_EndRequest()
        {
            if (HttpContext.Current.Handler is MvcHandler)
            {
                DependencyResolverInitializer.ReleaseAndDisposeAllHttpScopedObjects();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            var exc = lastError as HttpException;
            string innerException = String.Empty;
            if (lastError.InnerException != null)
            {
                innerException =
                    String.Format(
                        "InnerExceptionMessage: " + exc.InnerException.Message + Environment.NewLine
                        + "InnerExceptionSource: " + exc.InnerException.Source + Environment.NewLine
                        + "InnerExceptionStackTrace: " + exc.InnerException.StackTrace + Environment.NewLine);
            }

            if (exc != null)
            {
                var errorMessage = string.Format(Environment.NewLine + "-----------------------------" + Environment.NewLine +
                                "HtmlErrorMessage: " + exc.GetHtmlErrorMessage() + Environment.NewLine +
                                "HttpCode: " + exc.GetHttpCode() + Environment.NewLine +
                                "Message: " + exc.Message + Environment.NewLine +
                                "ErrorCode: " + exc.ErrorCode + Environment.NewLine +
                                "WebEventCode: " + exc.WebEventCode + Environment.NewLine + 
                                "Source: " + exc.Source + Environment.NewLine +
                                "Data: " + exc.Data + Environment.NewLine +
                                "TargetSite: " + exc.TargetSite + Environment.NewLine + 
                                "StackTrace: " + exc.StackTrace + Environment.NewLine +
                                "BaseExceptionMessage: " + exc.GetBaseException().Message + Environment.NewLine +
                                "GetType: " + exc.GetType() + Environment.NewLine +
                                "HelpLink: " + exc.HelpLink + Environment.NewLine +
                                innerException);
                ErrorLogger.LogError(errorMessage);
            }
            if (exc == null)
            {
                var msg = string.Format(Environment.NewLine + "-----------------------------" + Environment.NewLine +
                            "Message: " + lastError.Message + Environment.NewLine +
                            "Source: " + lastError.Source + Environment.NewLine +
                            "Data: " + lastError.Data + Environment.NewLine +
                            "TargetSite: " + lastError.TargetSite + Environment.NewLine +
                            "BaseExceptionMessage: " + lastError.GetBaseException().Message + Environment.NewLine +
                            "StackTrace: " + lastError.StackTrace + Environment.NewLine +
                            "GetType: " + lastError.GetType() + Environment.NewLine +
                            "HelpLink: " + lastError.HelpLink + Environment.NewLine +
                            innerException
                            );
                ErrorLogger.LogError(msg);
            }
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authenticateCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authenticateCookie != null && authenticateCookie.Value != null)
            {
                FormsAuthenticationTicket authTicket;
                try
                {
                    // Extract the forms authentication cookie
                    authTicket = FormsAuthentication.Decrypt(authenticateCookie.Value);
                }
                catch (Exception)
                {
                    return;
                }

                if (authTicket.Expired)
                {
                    return;
                }

                var newAuthTicket = FormsAuthentication.RenewTicketIfOld(authTicket);
                if (newAuthTicket == null)
                {
                    return;
                }

                if (authTicket != newAuthTicket)
                {
                    var newAuthenticateCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(newAuthTicket));
                    HttpContext.Current.Response.Cookies.Remove(authenticateCookie.Name);
                    HttpContext.Current.Response.Cookies.Add(newAuthenticateCookie);
                }

                // Create an Identity object
                var id = new FormsIdentity(newAuthTicket);

                // Create an IPrincipal object
                var newUser = new GenericPrincipal(id, new string[0]);

                // Sets user authentication info
                Context.User = newUser;
            }
        }

        private void SetNoCache()
        {
            this.Response.Cache.SetNoStore();
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.AppendHeader("pragma", "no-cache");
            this.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        }
    }
}