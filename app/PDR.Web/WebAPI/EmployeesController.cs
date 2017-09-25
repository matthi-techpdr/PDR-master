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
using PDR.Domain.Services.Account;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

namespace PDR.Web.WebAPI
{    
    public class EmployeesController : BaseWebApiController<Employee, ApiEmployeeModel>
    {
        private readonly HashGenerator hashGenerator;

        private readonly IRepositoryFactory repositoryFactory;

        public EmployeesController()
        {
            this.hashGenerator = new HashGenerator();
            this.repositoryFactory = ServiceLocator.Current.GetInstance<IRepositoryFactory>();
        }

        protected ICurrentWebStorage<Employee> EmployeeWebStorage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>();
            }
        }

        [ApiAuthorize(Ignore = true)]
        [WebApiTransaction]
        public HttpResponseMessage Get(string login, string password, string company, string deviceId, string token)//, string version
        {
            var nativeRepository = ServiceLocator.Current.GetInstance<INativeRepository<Employee>>();
            var employee = nativeRepository.Where(x => x.Status != Statuses.Removed).SingleOrDefault(e => e.Login == login && e.Password == password && e.Company.Url == company);
            if (employee == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("Not found");
                throw new HttpResponseException(message);
            }
           
            ////==================================UNCOMMIT=========================================

            if (employee.Company.Status == Statuses.Suspended)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Error", "The employee's company is suspended.");
                return response;
            }

            if (employee.Status == Statuses.Suspended)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Error", "The employee is suspended.");
                return response;
            }

            var activeLicense = employee.Licenses.SingleOrDefault(x => x.Status == LicenseStatuses.Active);
            if (activeLicense == null)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Error", "No license for this employee or license is inactive.");
                return response;
            }

            //var activeLicenses = employee.Licenses.Where(x => x.Status == LicenseStatuses.Active).ToList();
            //if (activeLicenses.Count > 0)
            //{
            //    var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            //    response.Headers.Add("Error", "No license for this employee or license is inactive.");
            //    return response;
            //}

            string errorMessage;
            if (!this.CheckDeviceId(activeLicense, deviceId, company, out errorMessage, token))
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Error", errorMessage);
                return response;
            }

            //foreach (var license in activeLicenses)
            //{
            //if (!this.CheckDeviceId(activeLicense, deviceId, company, out errorMessage, token))
            //{
            //    var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            //    response.Headers.Add("Error", errorMessage);
            //    return response;
            //}
            //}
            
            HttpContext.Current.Response.Cookies.Clear();
            HttpContext.Current.Response.Cookies.Add(this.EmployeeWebStorage.GetAuthCookie(employee, true));//, version
  
            var model = new ApiEmployeeModel
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Login = employee.Login,
                    Role = employee.Role.ToString(),    
                    Email = employee.Email,
                    SignatureName = employee.SignatureName,
                    License = new ApiLicenseModel(employee.Licenses.Single(x => x.DeviceToken == token && x.DeviceId == deviceId && x.Status == LicenseStatuses.Active))
                };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        private bool CheckDeviceId(License license, string deviceId, string company, out string message, string token)
        {
            message = null;

            if (!string.IsNullOrEmpty(deviceId))
            {
                if (license != null)
                {
                    if (token != license.DeviceToken)
                    {
                        license.DeviceToken = token;
                        this.Repository.Save(license.Employee);
                    }

                    if (license.DeviceId == null)
                    {
                        var licenses = this.repositoryFactory.CreateNative<License>()
                            .Where(x => x.Company.Url == company)
                            .Select(x => x.DeviceId)
                            .ToList();

                        if (licenses.Contains(deviceId))
                        {
                            message = "This device id already exist in the system.";
                            return false;
                        }

                        license.DeviceId = deviceId;
                        this.Repository.Save(license.Employee);

                        return true;
                    }

                    if (license.DeviceId == deviceId)
                    {
                        return true;
                    }
                }
            }

            message = "Incorrect device ID.";
            return false;
        }
    }
}
