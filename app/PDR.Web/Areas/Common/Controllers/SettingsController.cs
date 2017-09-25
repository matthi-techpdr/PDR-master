using System.Web.Mvc;
using PDR.Web.Core.Authorization;
using System.Linq;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Common.Models;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Web.Core.NLog.FileLoggers;
using SmartArch.Web.Attributes;
using System;
using SmartArch.Data;

namespace PDR.Web.Areas.Common.Controllers
{
    [PDRAuthorize]
    public class SettingsController : Controller
    {
        private readonly Employee currentEmployee;

        private readonly ICompanyRepository<Employee> employeeRepository;

        private readonly ICompanyRepository<License> licensesRepository;

        public SettingsController(ICurrentWebStorage<Employee> storage, ICompanyRepository<License> licensesRepository, ICompanyRepository<Employee> employeeRepository)
        {
            this.employeeRepository = employeeRepository;
            this.currentEmployee = storage.Get();
            this.licensesRepository = licensesRepository;
        }

        public ActionResult Index()
        {
            var model = new SettingsViewModel(currentEmployee);
            this.FillEmployeeTeams(currentEmployee, model);
            return View(model);
        }

        [HttpPost]
        [Transaction]
        public JsonResult Index(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                long Id;
                var result = Int64.TryParse(model.Id, out Id);
                if (result)
                {
                    var emloyee = employeeRepository.Get(Id);
                    emloyee.Name = model.Name;
                    emloyee.SignatureName = model.SignatureName;
                    emloyee.Address = model.Address;
                    emloyee.City = model.City;
                    var state = Convert.ToInt32(model.State);
                    currentEmployee.State = (StatesOfUSA)state;
                    emloyee.Zip = model.Zip;
                    emloyee.PhoneNumber = model.PhoneNumber;
                    emloyee.Email = model.Email;
                    emloyee.TaxId = model.TaxId;
                    emloyee.IsBasic = model.IsBasic;
                    emloyee.Login = model.Login;
                    emloyee.Password = model.Password;
                    this.employeeRepository.Save(emloyee);
                    return Json("Settings have been saved successfully", JsonRequestBehavior.AllowGet);
                }
            }
            return this.Json("Error! Settings have not been saved successfully", JsonRequestBehavior.AllowGet);
        }

        private void FillEmployeeTeams(Employee employee, SettingsViewModel model)
        {
            if (employee is TeamEmployee)
            {
                var teams = ((TeamEmployee)employee).Teams.Where(x => x.Status == Statuses.Active).OrderBy(x => x.Title).ToList();
                model.EmployeeTeams = teams.Select(x => x.Title).ToList();
            }
        }

        [HttpPost]
        [Transaction]
        public JsonResult ClearDeviceId()
        {
            var licenses = currentEmployee.Licenses.ToList();
            try
            {
                foreach (var license in licenses)
                {
                    license.DeviceId = null;
                    license.DeviceToken = null;
                    LicenseLogger.ClearDeviceId(license);
                    this.licensesRepository.Save(license);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
