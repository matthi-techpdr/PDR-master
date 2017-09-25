using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Admin.Models.MobileLicense;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class MobileLicenseController : Controller
    {
        private readonly ICompanyRepository<License> licensesRepository;

        private readonly IGridMaster<License, LicenseJsonModel, LicenseViewModel> licenseGridMaster;

        private readonly Employee currentUser;

        private readonly ICompanyRepository<Employee> employeesRepository;

        public MobileLicenseController(
            ICompanyRepository<License> licensesRepository,
            IGridMaster<License, LicenseJsonModel, LicenseViewModel> licenseGridMaster,
            ICompanyRepository<Employee> employeesRepository,
            ICurrentWebStorage<Employee> userStorage)
        {
            this.licenseGridMaster = licenseGridMaster;
            this.licensesRepository = licensesRepository;
            this.currentUser = userStorage.Get();
            this.employeesRepository = employeesRepository;
        }

        public ActionResult Index()
        {
            ViewData["statuses"] = ListsHelper.GetStatuses(null, Enum.GetValues(typeof(LicenseStatuses)).Cast<object>());
            var activity = this.licensesRepository.Count(license => license.Status == LicenseStatuses.Active);

            ViewData["activeLicense"] = activity;
            ViewData["allowedLicense"] = this.currentUser.Company.MobileLicensesNumber;
            
            return View();
        }

        public JsonResult GetData(string sidx, string sord, int page, int rows, int? status)
        {
            if (status != null)
            {
                this.licenseGridMaster.ExpressionFilters.Add(x => x.Status == (status == 1 ? LicenseStatuses.Active : LicenseStatuses.Suspended));
            }

            var data = this.licenseGridMaster.GetData(page, rows, sidx, sord);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MobileLicenseDialog(long? ids, bool edit)
        {
            var model = new MobileLicenseModel();
            model.GpsReportFrequencies = new List<SelectListItem>
                                             {
                                                 new SelectListItem { Selected = false, Text = @"5 min", Value = "5" },
                                                 new SelectListItem { Selected = false, Text = @"10 min", Value = "10" },
                                                 new SelectListItem { Selected = false, Text = @"15 min", Value = "15" },
                                                 new SelectListItem { Selected = false, Text = @"30 min", Value = "30" },
                                             };

            model.Employees = this.employeesRepository
                .Where(x => x.Status == Statuses.Active
                            && (x is Domain.Model.Users.Estimator
                                || x is Domain.Model.Users.Technician
                                || x is Domain.Model.Users.Admin
                                || x is Domain.Model.Users.Manager
                                || x is Domain.Model.Users.RITechnician)).OrderBy(x=> x.Name).Select(y => new SelectListItem { Text = y.Name, Value = y.Id.ToString() }).ToList();

            if (edit)
            {
                model.Edit = true;
                var license = this.licensesRepository.Get(Convert.ToInt64(ids));
                model.Id = license.Id;
                model.DeviceName = license.DeviceName;
                model.LicenseNumber = license.LicenseNumber;
                model.PhoneNumber = license.PhoneNumber;
                model.DeviceType = license.DeviceType == DeviceTypes.iPad ? DeviceTypes.iPad : DeviceTypes.iPhone;
                var employee = license.Employee;
                
                if (employee != null)
                {
                    model.Employees.Add(new SelectListItem { Text = employee.Name, Value = employee.Id.ToString(), Selected = true });
                    model.EmployeeName = employee.Name;
                    model.Employee = employee.Id;
                }
                else
                {
                    model.Employees.Insert(0, new SelectListItem { Text = @"Choose...", Value = null, Selected = true });
                }

                model.GpsReportFrequencies.SingleOrDefault(x => x.Value == license.GpsReportFrequency.ToString()).Selected = true;
            }
            else
            {
                model.Edit = false;
                model.Employees.Insert(0, new SelectListItem { Text = @"Choose...", Value = null, Selected = true });
                model.GpsReportFrequencies.SingleOrDefault(x => x.Value == "30").Selected = true;
                model.PhoneNumber = string.Empty;
            }
            
            return this.PartialView(model);
        }

        [HttpPost]
        [Transaction]
        public void Save(MobileLicenseModel model)
        {
            if (model.Edit)
            {
                var license = this.licensesRepository.Get(model.Id);
                license.DeviceName = model.DeviceName;
                license.DeviceType = model.DeviceType;
                license.Employee = this.employeesRepository.SingleOrDefault(x => x.Id == model.Employee);
                license.GpsReportFrequency = Convert.ToInt32(model.GpsFrequency);
                license.PhoneNumber = model.PhoneNumber;
                LicenseLogger.Edit(license);
                this.licensesRepository.Save(license);
            }
            else
            {
                var license = new License
                                  {
                                      CreationDate = DateTime.Now,
                                      DeviceName = model.DeviceName,
                                      DeviceType = model.DeviceType,
                                      PhoneNumber = model.PhoneNumber,
                                      Employee = this.employeesRepository.SingleOrDefault(x => x.Id == model.Employee),
                                      GpsReportFrequency = Convert.ToInt32(model.GpsFrequency),
                                      Company = this.currentUser.Company
                                  };

                license.Status = license.Employee.Licenses.Count(x => x.Status == LicenseStatuses.Active) > 0
                                     ? LicenseStatuses.Suspended
                                     : LicenseStatuses.Active;

                this.licensesRepository.Save(license);
                LicenseLogger.Create(license);
            }
        }

        [HttpPost]
        [Transaction]
        public void SuspendLicense(string ids)
        {
            foreach (var id in ids.Split(','))
            {

                var license = this.licensesRepository.Get(Convert.ToInt64(id));
                license.Status = LicenseStatuses.Suspended;
                LicenseLogger.Suspend(license);
                this.licensesRepository.Save(license);
            }
        }

        [HttpPost]
        [Transaction]
        public JsonResult ReactivateLicense(string ids)
        {
            var message = string.Empty;
            foreach (var id in ids.Split(','))
            {
                var license = this.licensesRepository.Get(Convert.ToInt64(id));
                if (license.Employee.Licenses.Count(x => x.Status == LicenseStatuses.Active) == 0)
                {
                    license.Status = LicenseStatuses.Active;
                    LicenseLogger.Reactivate(license);
                    this.licensesRepository.Save(license);
                    message = "Success";
                }
                else
                {
                    message = "You can make for a single user license only one active.\nOne or several your selected licenses were not activated.";
                }
            }

            return this.Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewLicense(long? id)
        {
            var model = new LicenseViewModel();
            var license = this.licensesRepository.Get(Convert.ToInt64(id));
            model.Id = license.Id.ToString();
            model.DeviceId = license.DeviceId;
            model.DeviceName = license.DeviceName;
            model.LicenseNumber = license.Id.ToString();
            model.PhoneNumber = license.PhoneNumber;
            model.DeviceType = license.DeviceType;
            model.Employee = license.Employee.Name;
            model.GpsFrequency = license.GpsReportFrequency + " min";
                
            return this.PartialView(model);
        }

        public JsonResult GetCountLicense()
        {
            var activity = this.licensesRepository.ToList().Count(license => license.Status == LicenseStatuses.Active);
            var data = new { active = activity, allowed = this.currentUser.Company.MobileLicensesNumber };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Transaction]
        public JsonResult ClearDeviceId(string ids)
        {
            try
            {
                foreach (var id in ids.Split(','))
                {
                    var license = this.licensesRepository.Get(Convert.ToInt64(id));
                    license.DeviceId = null;
                    license.DeviceToken = null;
                    LicenseLogger.ClearDeviceId(license);
                    this.licensesRepository.Save(license);
                }
            }catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
