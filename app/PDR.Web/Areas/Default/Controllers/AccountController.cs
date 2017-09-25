using System;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Services.Account;

using PDR.Web.Areas.Default.Models;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Default.Controllers
{
    using NLog;

    using PDR.Domain.Model.Enums;
    using PDR.Domain.Model.Users;

    public class AccountController : Controller
    {
        private readonly IAuthorizationProvider authorizationProvider;

        private readonly INativeRepository<Company> companyRepository;

        private readonly INativeRepository<Employee> employeesRepository;

        private int AttemptCount
        {
            get
            {
                return this.Session["Attempt"] != null ? (int)this.Session["Attempt"] : 0;
            }

            set
            {
                this.Session["Attempt"] = value;
            }
        }

        public AccountController(IAuthorizationProvider authorizationProvider,
            INativeRepository<Company> companyRepository,
            INativeRepository<Employee> employeesRepository)
        {
            this.authorizationProvider = authorizationProvider;
            this.companyRepository = companyRepository;
            this.employeesRepository = employeesRepository;
        }

        //// GET: /Account/Login
        //[HttpGet]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        [HttpGet]
        public ActionResult LogOn(string returnUrl)
        {
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.AddHeader("Last-Modified", "Mon, 01 Sep 1997 01:03:33 GMT");
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, must-revalidate");
            System.Web.HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            var companyRoute = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["company"];
            this.Session["company"] = null;
            if(companyRoute != null)
            {
                var companyName = companyRoute.ToString();
                var company = this.companyRepository.SingleOrDefault(x => x.Url.ToLower() == companyName);

                this.Session["company"] = company != null
                                              ? System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["company"]
                                              : null;
            }

            if (Request.UserAgent != null)
            {
                if (Request.UserAgent.ToLower().Contains("iphone"))
                {
                    return View("LogOnMobile");
                }
                if (Request.UserAgent.ToLower().Contains("android")
                    && Request.UserAgent.ToLower().Contains("mobile")
                    && !Request.UserAgent.ToLower().Contains("firefox")
                    && !Request.UserAgent.ToLower().Contains("tablet"))
                {
                    return View("LogOnMobile");
                }
            }
            return View();
        }

        public FileContentResult RenderLogo(string date)
        {
            if (this.Session["company"] != null)
            {
                var companyName = this.Session["company"].ToString();
                var company = this.companyRepository.SingleOrDefault(x => x.Url.ToLower() == companyName);
                if (company != null)
                {
                    if (company.Logo != null)
                    {
                        var logo = company.Logo;
                        return this.File(logo.PhotoFull, logo.ContentType);
                    }
                }
            }

            var path = AppDomain.CurrentDomain.BaseDirectory + "Content\\css\\images\\PDRManage_logo_v1.png";

            var defaultLogo = System.IO.File.ReadAllBytes(path);
            
            return this.File(defaultLogo, "image/png");
        }

        [HttpPost]
        public ActionResult LogOn(LoginModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var authorizationResult = model.Company != null
                        ? this.authorizationProvider.LogOnEmployee(this.Response, model.Company, model.Login.Trim(), model.Password.Trim(), this.AttemptCount)
                        : this.authorizationProvider.LogOnSuperadmin(this.Response, model.Login.Trim(), model.Password.Trim());
                    if (authorizationResult.Success)
                    {
                        this.AttemptCount = 0;

                        if (returnUrl != null)
                        {
                            var role = returnUrl.Split('/')[2];
                            if (role == authorizationResult.User.Role.ToString())
                            {
                                return this.Redirect(returnUrl);
                            }
                        }

                        return this.RedirectToAction("Index", "Home");
                    }

                     var user = this.employeesRepository.Where(x => x.Status != Statuses.Removed).FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                     this.AttemptCount = this.AttemptCount <= 5
                         ? user!=null ? this.AttemptCount : this.AttemptCount + 1 
                         : 0;
                    ModelState.AddModelError("Login", authorizationResult.Message);
                    ModelState.AddModelError("Password", authorizationResult.Message);
                }

                if (Request.UserAgent != null)
                {
                    if (Request.UserAgent.ToLower().Contains("iphone"))
                    {
                        return View("LogOnMobile", model);
                    }
                    else if (Request.UserAgent.ToLower().Contains("android")
                        && Request.UserAgent.ToLower().Contains("mobile")
                        && !Request.UserAgent.ToLower().Contains("firefox")
                        && !Request.UserAgent.ToLower().Contains("tablet"))
                    {
                        return View("LogOnMobile", model);
                    }
                }
            }
            catch (Exception exception)
            {
                NLog.Logger logger = LogManager.GetCurrentClassLogger();
                var msg = String.Format("Log On failed: {0}", exception.Message);
                logger.Error(msg);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult LogOut(string token)
        {
            this.authorizationProvider.LogOut(token);

            return this.RedirectToAction("LogOn");
        }
    }
}