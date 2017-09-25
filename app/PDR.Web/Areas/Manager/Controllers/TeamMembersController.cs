using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Scripting.Utils;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Resources;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.GeneratePassword;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.SMSSending;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers.Manager;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Manager.Controllers
{
    [PDRAuthorize(Roles = "Manager")]
    public class TeamMembersController : Controller
    {
        private readonly IMailService mailing;

        private readonly ICompanyRepository<TeamEmployee> teamEmployeesRepository;

        private readonly IGridMaster<TeamEmployee, EmployeeJsonData, EmployeeViewModel> employeesGridMaster;

        private readonly TeamEmployee currentUser;

        private readonly ICompanyRepository<Team> teamRepository;

        private readonly HashGenerator hashGenerator;

        public TeamMembersController(
            IGridMaster<TeamEmployee, EmployeeJsonData, EmployeeViewModel> employeesGridMaster,
            ICurrentWebStorage<Employee> storage,
            ICompanyRepository<Team> teamRepository,
            ICompanyRepository<TeamEmployee> teamEmployeesRepository,
            IMailService mailing)
        {
            this.mailing = mailing;
            this.teamEmployeesRepository = teamEmployeesRepository;
            this.employeesGridMaster = employeesGridMaster;
            this.teamRepository = teamRepository;
            this.currentUser = storage.Get() as TeamEmployee;
            this.hashGenerator = new HashGenerator();
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows, long? teamId, int? roleId)
        {
            var grid = this.employeesGridMaster;
            Expression<Func<TeamEmployee, bool>> teamExpression = null;
            
            if (this.currentUser.Teams.Count == 0)
            {
                var dat = grid.GetData(page, rows, sidx, sord);
                dat.rows = new List<EmployeeJsonData>();
                return this.Json(dat, JsonRequestBehavior.AllowGet);
            }

            foreach (var currentManagerTeam in this.currentUser.Teams)
            {
                var team = currentManagerTeam;
                Expression<Func<TeamEmployee, bool>> exp = x => x.Teams.Contains(team);
                teamExpression = teamExpression == null
                    ? exp :
              Expression.Lambda<Func<TeamEmployee, bool>>(Expression.OrElse(teamExpression.Body, new ExpressionParameterReplacer(exp.Parameters, teamExpression.Parameters).Visit(exp.Body)), teamExpression.Parameters);
            }

            grid.ExpressionFilters.Add(teamExpression);

            if (teamId.HasValue)
            {
                var currentTeam = this.teamRepository.Get(teamId.Value);
                grid.ExpressionFilters.Add(x => x.Teams.Contains(currentTeam));
            }

            if (roleId.HasValue)
            {
                switch (roleId.Value)
                {
                    case (int)UserRoles.Manager:
                        grid.ExpressionFilters.Add(x => x is Domain.Model.Users.Manager);
                        break;
                    case (int)UserRoles.Technician:
                        grid.ExpressionFilters.Add(x => x is Domain.Model.Users.Technician);
                        break;
                }
            }

            var data = grid.GetData(page, rows, sidx, sord);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            dynamic model = new ExpandoObject();
            model.Roles = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = @"All roles", Value = null
                    },
                    new SelectListItem
                    {
                        Text = UserRoles.Manager.ToString(), Value = ((int)UserRoles.Manager).ToString() 
                    },
                    new SelectListItem
                    {
                        Text = UserRoles.Technician.ToString(), Value = ((int)UserRoles.Technician).ToString() 
                    } 
                };

            var teamsList = this.currentUser.Teams.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).OrderBy(x=>x.Text).ToList();
            teamsList.Insert(0, new SelectListItem { Text = @"All teams", Value = null });
            model.Teams = teamsList;
            return this.View(model);
        }

        public ActionResult View(long id)
        {
            var teamEmployee = this.teamEmployeesRepository.Get(id);
            if (teamEmployee != null)
            {
                dynamic model = new ExpandoObject();
                model.Name = teamEmployee.Name;
                model.Phone = teamEmployee.PhoneNumber;
                model.Email = teamEmployee.Email;
                model.Address = teamEmployee.Address;
                model.OpenEstimates = teamEmployee.Estimates.Where(x => x.EstimateStatus == EstimateStatus.Open).Count();
                model.OpenRo = teamEmployee.RepairOrders.Where(x => x.RepairOrderStatus == RepairOrderStatuses.Open).Count();
                return this.PartialView(model);
            }

            return this.Content("Employee can not be found");
        }

        [Transaction]
        public ContentResult ResetPassword(long[] ids)
        {
            var employees = this.currentUser.Teams.SelectMany(x => x.Employees).Where(x => ids.Contains(x.Id)).ToList();
            foreach (var employee in employees)
            {
                var password = PasswordGenerator.Generate();
                employee.Password = password;
                EmployeeLogger.ResetPassword(employee);
                this.teamEmployeesRepository.Save(employee);
                string message = String.Format(SmsTemplatesRes.NewPassword, password);
                new SMSSending().Send(employee.PhoneNumber, message);
            }

            return this.Content("Password(s) has been successfully changed");
        }

        public void SendEmail(SendEmailViewModel model)
        {
            var from = this.currentUser.Company.Email;
            new Task(
                () =>
                    {
                        this.mailing.Send(from, model.To, model.Subject, model.Message);
                        EmployeeLogger.SendEmailToEmployee(model);
                    });
        }

        public ActionResult GetEmailDialog(long id)
        {
            var user = this.teamEmployeesRepository.Get(id);
            if (user != null)
            {
                return this.PartialView("~/Areas/Common/Views/Estimates/GetEmailDialog.cshtml", new SendEmailViewModel { To = user.Email });
            }

            return this.Content("Employee can not be found");
        }
    }
}
