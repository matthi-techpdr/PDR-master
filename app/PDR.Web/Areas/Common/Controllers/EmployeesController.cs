using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Scripting.Metadata;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Resources;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.GeneratePassword;
using PDR.Domain.Services.Grid;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.SMSSending;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers.Manager;
using Rhino.Mocks.Constraints;
using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Common.Controllers
{
    using NHibernate;

    [PDRAuthorize]
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository<Employee> employeeRepository;

        private readonly ICompanyRepository<Invoice> invoiceRepository;
        
        private readonly ICompanyRepository<License> licenseRepository;

        private readonly ICurrentWebStorage<Employee> storage;

        private readonly ICompanyRepository<Team> teamRepository;

        private readonly ICompanyRepository<FormerRI> formerRITechnicianRepository;

        private readonly IGridMaster<Employee, EmployeeJsonData, EmployeeViewModel> employeeGridMaster;

        private readonly HashGenerator hashGenerator;

        private readonly ReassignHelper reasignHelper;

        private readonly bool isAdmin;

        private readonly bool isAccountant;

        public readonly IMailService mailService;

        public EmployeesController(
            ICompanyRepository<Employee> employeeRepository,
            ICompanyRepository<Team> teamRepository,
            IGridMaster<Employee, EmployeeJsonData, EmployeeViewModel> employeeGridMaster,
            ICurrentWebStorage<Employee> storage,
            ReassignHelper reasignHelper,
            ICompanyRepository<Invoice> invoiceRepository,
            ICompanyRepository<FormerRI> formerRITechnicianRepository)
        {
            this.employeeRepository = employeeRepository;
            this.teamRepository = teamRepository;
            this.employeeGridMaster = employeeGridMaster;
            this.storage = storage;
            this.reasignHelper = reasignHelper;
            this.mailService = ServiceLocator.Current.GetInstance<IMailService>();
            this.licenseRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<License>>();
            this.hashGenerator = new HashGenerator();
            this.isAdmin = this.storage.Get() is Domain.Model.Users.Admin;
            this.isAccountant = this.storage.Get() is Domain.Model.Users.Accountant;
            this.invoiceRepository = invoiceRepository;
            this.formerRITechnicianRepository = formerRITechnicianRepository;
        }

        public ActionResult Index()
        {
            var roles = ListsHelper.GetAllRoles(null, addAdmin: this.isAdmin || this.isAccountant).ToList();
            roles.Insert(0, new SelectListItem { Text = @"All members", Selected = true, Value = null });
            this.ViewBag.Allowed = this.storage.Get().Company.ActiveUsersNumber;
            this.ViewBag.IsAdmin = this.isAdmin;
            return this.View(roles);
        }

        public ActionResult GetEmployee(long? id, bool edit)
        {
            var activeEmpCount = this.storage.Get().Company.Employees.Count(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Wholesaler));
            if (activeEmpCount >= this.storage.Get().Company.ActiveUsersNumber && id == null)
            {
                return new HttpStatusCodeResult(204);
            }

            var employees = this.employeeRepository.ToList();
            var teams = this.teamRepository.Where(x => x.Status == Statuses.Active);
            EmployeeViewWithUserModel model;
            if (id == null)
            {
                model = new EmployeeViewWithUserModel(this.isAdmin, this.storage.Get())
                    {
                        EmployeeTeams =
                            teams.OrderBy(x => x.Title).Select(
                                t =>
                                new SelectListItem
                                    {
                                        Text = t.Title, 
                                        Value = t.Id.ToString()
                                    })
                    };
            }
            else
            {
                Employee employee = this.employeeRepository.Get(id.Value);
                model = new EmployeeViewWithUserModel(employee, this.isAdmin, this.storage.Get());
                this.FillEmployeeTeams(employee, model);
                employees.Remove(employee);
            }

            this.ViewData["employeesLogins"] = string.Join(",", employees.Where(x => x.Status != Statuses.Removed).Select(x => x.Login).ToList());
            return this.PartialView(edit ? "EditEmployee" : "ViewEmployee", model);
        }

        [HttpPost]
        [Transaction]
        public void CreateEmployee(EmployeeViewModel model)
        {
            var activeEmpCount = this.storage.Get().Company.Employees.Count(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Wholesaler));
            if (activeEmpCount + 1 <= this.storage.Get().Company.ActiveUsersNumber)
            {
                if (ModelState.IsValid)
                {
                    var customGridMaster = this.GetGridMasterByRole(model.Role);
                    Action<Employee> customizer = employee =>
                        {
                            var role = (UserRoles)model.Role;
                            if (role == UserRoles.Manager || role == UserRoles.Technician || role == UserRoles.RITechnician)
                            {
                                foreach (var teamId in model.TeamsList)
                                {
                                    var team = this.teamRepository.Get(teamId);
                                    ((TeamEmployee)employee).AssignTeam(team);
                                }
                            }

                            string message = String.Format(SmsTemplatesRes.NewPassword, model.Password);
                            new SMSSending().Send(model.PhoneNumber, message);
                        };
                    var newEmployee = customGridMaster.CreateEntity(model, customizer);
                    EmployeeLogger.Create(newEmployee);
                }
            }
        }

        [HttpPost]
        [Transaction]
        public void EditEmployee(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = this.employeeRepository.Get(Convert.ToInt64(model.Id));
                //var licences = employee.Licenses;
                EmployeeLogger.Edit(employee, model);
                var oldUserRole = employee.Role;

                var customGridMaster = this.GetGridMasterByEmployee(Convert.ToInt32(model.Role));
                var newRole = (UserRoles)model.Role;

                Action<Employee> customizer  = e =>
                {
                    var role = (UserRoles)model.Role;
                    //e.Class = role.ToString();
                    if (role == UserRoles.Manager || role == UserRoles.Technician || role == UserRoles.RITechnician)
                    {
                        var candidatesForDelete = ((TeamEmployee)e).Teams.ToList().Select(x => x.Id).ToList().Except(model.TeamsList).ToList();
                        if (candidatesForDelete.Any())
                        {
                            foreach (var teamId in candidatesForDelete)
                            {
                                var team = this.teamRepository.Get(teamId);
                                ((TeamEmployee)e).UnAssignTeam(team);
                            }
                        }
                        foreach (var teamId in model.TeamsList)
                        {
                            var team = this.teamRepository.Get(teamId);
                            if (!((TeamEmployee)e).Teams.Contains(team))
                            {
                                ((TeamEmployee)e).AssignTeam(team);
                            }
                        }
                    }

                    if (employee.Password != model.Password)
                    {
                        string message = String.Format(SmsTemplatesRes.NewPassword, model.Password);
                        new SMSSending().Send(model.PhoneNumber, message);
                    }

                    //if (oldUserRole != newRole)
                    //{
                    //    var licensesArr = this.licenseRepository.Where(x => x.Employee.Id == employee.Id);
                    //    foreach (var license in licensesArr)
                    //    {
                    //        license.Employee = e;
                    //    }
                    //    e.Licenses.AddAll(licences);
                    //    var invoices = this.invoiceRepository.Where(x => x.TeamEmployee.Id == employee.Id);
                        //var invoices = this.invoiceRepository.Where(x => x.TeamEmployee.Id == employee.Id);
                        //foreach (var invoice in invoices)
                        //{
                        //    invoice.TeamEmployee = (TeamEmployee)e;
                        //    invoice.RepairOrder.TeamEmployee = (TeamEmployee)e;
                        //}
                    //}
                };

                //if (oldUserRole != newRole)
                //{
                    //employee.Status = Statuses.Removed;
                    //customGridMaster.CreateEntity(model, customizer);

                    //if (employee is TeamEmployee)
                    //{
                    //    ((TeamEmployee)employee).Teams.Clear();
                    //    foreach (var team in ((TeamEmployee)employee).Teams.ToList())
                    //    {
                    //        team.Employees.Remove(((TeamEmployee)employee));
                    //    }
                    //}
                //}
                //else
                //{
                //    customGridMaster.EditEntity(model, customizer);
                //}
                customGridMaster.EditEntity(model, customizer);
                if (oldUserRole != newRole)
                {
                    try
                    {
                        var session = ServiceLocator.Current.GetInstance<ISession>();
                        var query = String.Format("UPDATE Users SET Class = '{0}' WHERE Id = {1}", newRole, employee.Id);
                        session.CreateSQLQuery(query).ExecuteUpdate();

                        if (oldUserRole == UserRoles.RITechnician)
                        {
                            var formerRITechnician = new FormerRI()
                            {
                                Company = employee.Company,
                                Employee = employee,
                                RoleChangeDate = DateTime.Now
                            };
                            formerRITechnicianRepository.Save(formerRITechnician);
                            //query = String.Format("UPDATE Users SET WasHeRITechnician = 1 WHERE Id = {0}", employee.Id);
                            //session.CreateSQLQuery(query).ExecuteUpdate();
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
        }

        [Transaction]
        public ActionResult GetManagers(long id)
        {
            var employee = this.employeeRepository.Get(id);
            var estimates = employee.Estimates.Where(x => x.EstimateStatus != EstimateStatus.Converted).ToList();

            var teamEmployee = employee as TeamEmployee;
            if (teamEmployee != null)
            {
                var repairOrdersEstimates = teamEmployee.TeamEmployeePercents
                    .Where(x => x.RepairOrder.RepairOrderStatus != RepairOrderStatuses.Finalised)
                    .Select(t => t.RepairOrder.Estimate)
                    .ToList();
                estimates.AddRange(repairOrdersEstimates);
            }

            this.ViewData["empId"] = employee.Id;

            if (!(employee is Domain.Model.Users.Accountant))
            {
                var managers = estimates.SelectMany(x => x.Customer.Teams).SelectMany(x => x.Employees).Where(x => x is Domain.Model.Users.Manager && x != employee).Distinct().ToList().Cast<Employee>().ToList();

                if (managers.Count() != 0)
                {
                    var admins = this.employeeRepository.Where(x => x is Domain.Model.Users.Admin && x.Status == Statuses.Active && x != employee).ToList();
                    managers.AddRange(admins);
                    var managersSelectList = managers.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                    return this.PartialView(managersSelectList);
                }
            }

            if (teamEmployee != null)
            {
                this.RemoveFromTeam(teamEmployee);
            }

            employee.Status = Statuses.Suspended;
            EmployeeLogger.Suspend(employee);
            this.employeeRepository.Save(employee);
            return new HttpStatusCodeResult(204);
        }

        [HttpPost]
        [Transaction]
        public void SuspendEmployee(long managerId, long empId)
        {
            Employee employee = this.employeeRepository.Get(empId);
            var manager = this.employeeRepository.Get(managerId);
            if (employee != null && manager != null)
            {
                var allEstimates = employee.Estimates.Where(x => x.EstimateStatus != EstimateStatus.Converted).ToList();

                var teamEmployee = employee as TeamEmployee;
                if (teamEmployee != null)
                {
                    var allRo = teamEmployee.AssignedAndPersonalRepairOrders.Where(x => !x.IsInvoice && x.RepairOrderStatus != RepairOrderStatuses.Finalised);
                    foreach (var ro in allRo)
                    {
                        var percents = ro.TeamEmployeePercents;

                        if (ro.TeamEmployee == teamEmployee)
                        {
                            ((TeamEmployee)manager).AddRepairOrder(ro);
                        }

                        if (percents.Select(x => x.TeamEmployee).Contains(teamEmployee))
                        {
                            if (percents.Count() > 1)
                            {
                                this.reasignHelper.RemoveEmployeesFromRepairOrder(ro, new List<TeamEmployee> { teamEmployee }); 
                            }
                            else if (percents.Count() == 1)
                            {
                                ro.TeamEmployeePercents.Single().TeamEmployee = (TeamEmployee)manager;
                            }
                        }
                    }

                    //var allInvoices = teamEmployee.Invoices.ToList();
                    //foreach (var i in allInvoices)
                    //{
                    //    ((TeamEmployee)manager).AddInvoice(i);
                    //}
                    
                    this.RemoveFromTeam(teamEmployee);
                }

                foreach (var estimate in allEstimates)
                {
                    manager.AddEstimate(estimate);
                }
                
                employee.Status = Statuses.Suspended;
                EmployeeLogger.Suspend(employee);
                this.employeeRepository.Save(employee);
            }
        }

        private void RemoveFromTeam(TeamEmployee teamEmployee)
        {
            foreach (var team in teamEmployee.Teams.ToList())
            {
                teamEmployee.Teams.Remove(team);
                team.Employees.Remove(teamEmployee);
            }
        }

        [HttpPost]
        [Transaction]
        public JsonResult ReactivateEmployee(string ids)
        {
            string msg = string.Empty;
            var activeEmpCount = this.storage.Get().Company.Employees.Count(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Wholesaler));
            var allowEmp = this.storage.Get().Company.ActiveUsersNumber;
            var forActive = ids.Split(',');
            if (allowEmp >= activeEmpCount + forActive.Count())
            {
                foreach (var id in forActive)
                {
                    var employee = this.employeeRepository.Get(Convert.ToInt64(id));
                    employee.Status = Statuses.Active;
                    this.employeeRepository.Save(employee);
                    EmployeeLogger.Reactivate(employee);
                }
            }
            else
            {
                msg = string.Format("Your company must contain no more than {0} active employees", allowEmp);
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public ContentResult ResetPassword(string ids)
        {
            var empIds = ids.Split(',').Select(x => Convert.ToInt64(x));
            var employees = this.employeeRepository.Where(x => empIds.Contains(x.Id));
            foreach (var employee in employees)
            {
                var password = PasswordGenerator.Generate();
                employee.Password = password;
                EmployeeLogger.ResetPassword(employee);
                this.employeeRepository.Save(employee);
                string message = String.Format(SmsTemplatesRes.NewPassword, password);
                new SMSSending().Send(employee.PhoneNumber, message);
                this.mailService.Send(employee.Company.Email, employee.Email, "Login and password", string.Format("Login and password: {0}; {1}.", employee.Login, password));
            }

            return this.Content("Password(s) has been successfully changed");
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows, int? role)
        {
            var grid = this.GetGridMasterByRole(role);
            var data = (GridModel<EmployeeJsonData>)grid.GetData(page, rows, sidx, sord);
            
            data.active = this.storage.Get().Company.Employees.Count(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Wholesaler));
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        private dynamic GetGridMasterByRole(int? role)
        {
            if (!role.HasValue)
            {
                this.employeeGridMaster.ExpressionFilters.Add(x => !(x is Domain.Model.Users.Wholesaler));
                this.employeeGridMaster.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return this.employeeGridMaster;
            }

            var userRole = (UserRoles)role;
            switch (userRole)
            {
                case UserRoles.Technician:
                    var gridT = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Technician, EmployeeJsonData, EmployeeViewModel>>();
                    gridT.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridT;
                case UserRoles.RITechnician:
                    var gridRIT = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.RITechnician, EmployeeJsonData, EmployeeViewModel>>();
                    gridRIT.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridRIT;
                case UserRoles.Estimator:
                    var gridE = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Estimator, EmployeeJsonData, EmployeeViewModel>>();
                    gridE.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridE;
                case UserRoles.Manager:
                    var gridM = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Manager, EmployeeJsonData, EmployeeViewModel>>();
                    gridM.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridM;
                case UserRoles.Accountant:
                    var gridA = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Accountant, EmployeeJsonData, EmployeeViewModel>>();
                    gridA.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridA;
                case UserRoles.Admin:
                    var gridAd = ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Admin, EmployeeJsonData, EmployeeViewModel>>();
                    gridAd.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return gridAd;
                default:
                    var grid = this.employeeGridMaster;
                    grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                    return grid;
            }
        }

        private dynamic GetGridMasterByEmployee(int? role)
        {
            if (!role.HasValue)
            {
                this.employeeGridMaster.ExpressionFilters.Add(x => !(x is Domain.Model.Users.Wholesaler));
                this.employeeGridMaster.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return this.employeeGridMaster;
            }

            return EmployeeGridHelper.GetEmployeeGrid((UserRoles) role);
        }

        private void FillEmployeeTeams(Employee employee, EmployeeViewModel model)
        {
            var teams = this.teamRepository.Where(x => x.Status == Statuses.Active);
            if (employee == null || !(employee is TeamEmployee))
            {
                model.EmployeeTeams = teams.OrderBy(x => x.Title).Select(t => new SelectListItem
                    {
                        Text = t.Title,
                        Value = t.Id.ToString()
                    });
            }
            else
            {
                var teamEmployee = ((TeamEmployee) employee);
                var teamsEmp = teamEmployee.Teams.ToList();
                model.EmployeeTeams = teams.OrderBy(x => x.Title).Select(t => new SelectListItem { Text = t.Title, Value = t.Id.ToString(), Selected = teamsEmp.Contains(t) });
                model.Commission = teamEmployee.Commission;
            }
        }

        private string FormatTeamName(string name)
        {
            return name.Length < 25 ? name : name.Insert(25, "</br>");
        }

        public void SendEmail(SendEmailViewModel model)
        {
            var from = this.storage.Get().Company.Email;
            this.mailService.Send(from, model.To, model.Subject, model.Message);
            EmployeeLogger.SendEmailToEmployee(model);
        }

        public ActionResult GetEmailDialog(long id)
        {
            var user = this.employeeRepository.Get(id);
            if (user != null)
            {
                return this.PartialView("~/Areas/Common/Views/Estimates/GetEmailDialog.cshtml", new SendEmailViewModel { To = user.Email });
            }

            return this.Content("Employee can not be found");
        }
    }
}


