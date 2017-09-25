using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Sheduler;
using PDR.Domain.Services.XLS;

using PDR.Web.Areas.Admin.Models.Vehicle;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Areas.SuperAdmin.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers.Manager;
using PDR.Web.Core.NLog.FileLoggers.Superadmin;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.SuperAdmin.Controllers
{
    using Domain.Model.Base;
    using Domain.Model.Users;
    using Domain.Services.GeneratePassword;
    using SmartArch.Data;

    [PDRAuthorize(Roles = "Superadmin")]
    public class CompaniesController : Controller
    {
        private readonly ISuperadminGridMaster<Company, CompanyJsonModel, CompanyViewModel> companyGridMaster;

        private readonly HashGenerator hashGenerator;

        private readonly MailService mailService;

        private readonly ISheduler sheduler;

        private readonly INativeRepository<Matrix> maricesRepository;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly IXLSGenerator xlsGenerator;

        private readonly INativeRepository<Company> companies;

        public CompaniesController(
            ISuperadminGridMaster<Company, CompanyJsonModel, CompanyViewModel> companyGridMaster,
            INativeRepository<Estimate> estimates,
            IPdfConverter pdfConverter,
            ISheduler sheduler,
            INativeRepository<Matrix> maricesRepository,
            IXLSGenerator xlsGenerator,
            IRepositoryFactory repositoryFactory,
            INativeRepository<Company> companies)
        {
            this.companyGridMaster = companyGridMaster;
            this.sheduler = sheduler;
            this.maricesRepository = maricesRepository;
            this.xlsGenerator = xlsGenerator;
            this.hashGenerator = new HashGenerator();
            this.mailService = new MailService();
            this.repositoryFactory = repositoryFactory;
            this.companies = companies;
        }

        public ActionResult Index()
        {
            var states = ListsHelper.GetStates(null).ToList();
            states.Insert(0, new SelectListItem { Text = @"All states", Selected = true, Value = null });
            return this.View(states);
        }

        public ActionResult GetCompany(long? id, bool edit)
        {
            var model = this.companyGridMaster.GetEntityViewModel(id);
            return this.PartialView(edit ? "EditCompany" : "ViewCompany", model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult CreateCompany(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var password = PasswordGenerator.Generate();
                var newAdmin = new Admin
                                   {
                                       Login = model.AdminLogin,
                                       Name = model.AdminName,
                                       Password = password,
                                       PhoneNumber = model.PhoneNumber,
                                       Email = model.Email
                                   };

                var company = this.companyGridMaster.CreateEntity(
                    model,
                    x =>
                        {
                           
                            x.AddEmployee(newAdmin);
                            ////create messages templates
                            this.sheduler.RunProcessByTime(
                                DateTime.Now.AddSeconds(1),
                                () =>
                                this.mailService.Send(
                                    string.Empty,
                                    model.Email,
                                    string.Format("{0}'s admin credentials", x.Name),
                                    string.Format("Login: {0}\nPassword: {1}\n", model.AdminLogin, password)));
                            x.AddLogo(new CompanyLogo(Server.MapPath("~/Content/images/defaultLogo.jpg"), null, "image/jpeg", newAdmin, true));
                            this.CreateDefaultMatrix(x, newAdmin);
                        });

                this.CreateDefaultVehicle(company);
                this.repositoryFactory.Create<Company>().Save(company);
                GreateDefaultOffice(company);

                CompanyLogger.New(model);
                return this.Content("success");
            }

            return this.PartialView("EditCompany", model);
        }

        [HttpPost]
        [Transaction]
        [ValidateInput(false)] 
        public ActionResult EditCompany(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                CompanyLogger.Edit(this.companyGridMaster.GetEntityViewModel(Convert.ToInt64(model.Id)), model);
                this.companyGridMaster.EditEntity(model, x => this.CustomizeEdit(x, model));
                return this.Content("success");
            }

            return this.PartialView("EditCompany", model);
        }

        [HttpPost]
        [Transaction]
        public void SuspendCompany(string ids)
        {
            var companiesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var currentCompanies = this.companies.Where(x => companiesIds.Contains(x.Id)).ToList();
            foreach (var currentCompany in currentCompanies)
            {
                currentCompany.Status = Statuses.Suspended;
                this.companies.Save(currentCompany);
                CompanyLogger.Suspend(currentCompany);
            }
        }

        [HttpPost]
        [Transaction]
        public void ReactivateCompany(string ids)
        {
            var companiesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var currentCompanies = this.companies.Where(x => companiesIds.Contains(x.Id)).ToList();
            foreach (var currentCompany in currentCompanies)
            {
                currentCompany.Status = Statuses.Active;
                this.companies.Save(currentCompany);
                CompanyLogger.Reactivate(currentCompany);
            }
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows, int? state)
        {
            if (state != null)
            {
                this.companyGridMaster.ExpressionFilters.Add(x => x.State == (StatesOfUSA)state);
            }

            var data = this.companyGridMaster.GetData(
                page,
                rows,
                sidx,
                sord);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return json;
        }

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool company = false)
        {
            var to = string.Empty;
            if (company)
            {
                to = this.repositoryFactory.Create<Company>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }

            return this.PartialView(new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids });
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                new MailService().Send(string.Empty, model.To, model.Subject, model.Message, null);
                EmployeeLogger.SendEmailToEmployee(model);
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

            return this.Json(false, JsonRequestBehavior.AllowGet);
        }

        private void CreateDefaultMatrix(Company company, Admin newAdmin = null)
        {
            var defaultMatrix = (DefaultMatrix)this.xlsGenerator.ImportMatrixFromXLS(Server.MapPath("~/Content/DefaultMatrix/Default.xls"), new DefaultMatrix(true, newAdmin), newAdmin);
            company.SetDefaultMatrix(defaultMatrix);
        }

        private void CreateDefaultVehicle(Company company)
        {
            var deserializer = new XmlSerializer(typeof(VehicleDefaultModel));
            var textReader = new StreamReader(this.Server.MapPath(@"~\Content\DefaultVehicle\default_vehicle.xml"));
            var defaultVehicle = (VehicleDefaultModel)deserializer.Deserialize(textReader);
            textReader.Close();

            var model = this.GetDefaultCarModel(defaultVehicle, company);

            company.SetDefaultVehicle(model);
        }

        private CarModel GetDefaultCarModel(VehicleDefaultModel model, Company company)
        {
            var carModel = new CarModel
                               {
                                   Make = model.Make,
                                   Model = model.Model,
                                   YearFrom = model.YearFrom,
                                   YearTo = model.YearTo,
                                   Company = company
                               };
            this.repositoryFactory.Create<CarModel>().Save(carModel);
            foreach (var part in model.CarParts)
            {
                var section = new CarSectionsPrice
                                       {
                                           Name = part.Name,
                                           NewSectionPrice = part.NewSectionPrice,
                                           Company = company,
                                           CarModel = carModel
                                       };
                this.repositoryFactory.Create<CarSectionsPrice>().Save(section);
                foreach (var item in part.EffortItems)
                {
                    var effort = new EffortItem
                                     {
                                         HoursR_I = item.HoursR_I,
                                         HoursR_R = item.HoursR_R,
                                         Name = item.Name,
                                         Company = company,
                                         CarSectionsPrices = section
                                     };
                    this.repositoryFactory.Create<EffortItem>().Save(effort);
                    section.EffortItems.Add(effort);
                }

                carModel.CarParts.Add(section);
            }

            return carModel;
        }

        private void CustomizeEdit(Company x, CompanyViewModel model)
        {
            var admin = x.Employees.Where(a => a.Role == UserRoles.Admin).OrderBy(a => a.Id).FirstOrDefault() as Admin ?? new Admin();
            admin.Login = model.AdminLogin;
            admin.PhoneNumber = model.PhoneNumber;
            admin.Name = model.AdminName;
            x.AddEmployee(admin);
            if (x.DefaultMatrix == null)
            {
                this.CreateDefaultMatrix(x);
            }

            if (x.DefaultVehicle == null)
            {
                this.CreateDefaultVehicle(x);
            }
        }

        private void GreateDefaultOffice(Company company)
        {
                    
        }


    }
}
