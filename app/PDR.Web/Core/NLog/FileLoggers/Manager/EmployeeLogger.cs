using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using NLog;
using NLog.LogReceiverService;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.ObjectsComparer;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Areas.Estimator.Models.Estimates;

namespace PDR.Web.Core.NLog.FileLoggers.Manager
{
    public class EmployeeLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void SendEmailToEmployee(SendEmailViewModel view)
        {
            var msg = string.Format("Send e-mail to employee - {0}; {1}; {2}.", view.To, view.Subject, view.Message);
            Loggger.Info(msg);
        } 

        public static void ResetPassword(Employee employee)
        {
            var msg = string.Format("Reset password - {0}({1}).", employee.Name, employee.Login);
            Loggger.Info(msg);
        }

        public static void Suspend(Employee employee)
        {
            var msg = string.Format("Suspend employee - {0}({1}).", employee.Name, employee.Login);
            Loggger.Info(msg);
        }

        public static void Reactivate(Employee employee)
        {
            var msg = string.Format("Re-activate employee - {0}({1}).", employee.Name, employee.Login);
            Loggger.Info(msg);
        }

        public static void Create(Employee employee)
        {
            var info = new List<string>
                {
                    employee.Name,
                    employee.City,
                    employee.Address,
                    employee.PhoneNumber,
                    employee.Email,
                    employee.Company.Name,
                    employee.TaxId,
                    employee.Login,
                    employee.Comment,
                    employee.Role.ToString()
                };

            var teamEmployee = employee as TeamEmployee;
            if (teamEmployee != null)
            {
                info.Add(teamEmployee.Commission.ToString());
                info.AddRange(teamEmployee.Teams.Select(team => team.Title));
            }

            if (employee.CanExtraQuickEstimate)
            {
                info.Add("Employee is allowed to make extra quick estimates");
            }

            if (employee.CanQuickEstimate)
            {
                info.Add("Employee is allowed to make quick estimates");
            }

            var msg = string.Format("Create employee - {0}.", string.Join("; ", info));
            Loggger.Info(msg);
        }

        public static void Edit(Employee employee, EmployeeViewModel model)
        {
            var msg = new StringBuilder(string.Format("Edit employee - {0}({1}). ", employee.Name, employee.Login));
            var changes = new LogHelper().Compare(
                new EmployeeViewModel(employee, true),
                model,
                new StringCollection
                    {
                        "Roles", "TeamsList", "LogMessages", "FullLocation", "Active", "State", "States", "Commission", "Role"
                    });

            if ((int)employee.State != int.Parse(model.State))
            {
                changes = changes + string.Format("state: {0};", ((StatesOfUSA)int.Parse(model.State)).ToString());
            }

            if ((int)employee.Role != model.Role)
            {
                changes = changes + string.Format("role: {0};", ((UserRoles)model.Role).ToString());
            }

            var newRole = (UserRoles)model.Role;
            if (newRole == UserRoles.Technician || newRole == UserRoles.Manager)
            {
                var teamEmployee = employee as TeamEmployee;
                if (teamEmployee != null)
                {
                        if (teamEmployee.Commission != model.Commission)
                        {
                            changes = changes + string.Format("commission: {0};", model.Commission);
                        }

                        var teamRepo = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();
                    var teams = teamRepo.Where(x => model.TeamsList.Contains(x.Id)).Select(x => x.Title).ToList();
                    changes = changes + string.Format("teams: {0};", string.Join(", ", teams));
                }
            }

            Loggger.Info(msg + changes);
        }

        public static void RemovedFromTeam(TeamEmployee employee, Team team)
        {
            var msg = string.Format("Employee {0} removed from the team ({1}).", employee.Name, team.Title);
            Loggger.Info(msg);
        }

    }
}