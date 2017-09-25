using System;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

using PDR.Web.Areas.Accountant.Models.Team;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Common.Controllers
{
    [PDRAuthorize]
    public class TeamsController : Controller //TODO: add validator that if manager uses this actions - he has CanEditTeamMembers permission
    {
        private readonly ICompanyRepository<Team> teamRepository;

        private readonly ICompanyRepository<TeamEmployee> teamEmployeeRepository;

        private readonly ICompanyRepository<RepairOrder> repairOrderRepository;

        private readonly ICompanyRepository<Invoice> invoiceRepository;

        private readonly IGridMaster<Team, TeamJsonModel, TeamViewModel> teamGridMaster;

        public TeamsController(
            ICompanyRepository<Team> teamRepository,
            ICompanyRepository<TeamEmployee> teamEmployeeRepository,
            ICompanyRepository<RepairOrder> repairOrderRepository,
            ICompanyRepository<Invoice> invoiceRepository,
            IGridMaster<Team, TeamJsonModel, TeamViewModel> teamGridMaster)
        {
            this.teamRepository = teamRepository;
            this.teamEmployeeRepository = teamEmployeeRepository;
            this.repairOrderRepository = repairOrderRepository;
            this.invoiceRepository = invoiceRepository;
            this.teamGridMaster = teamGridMaster;
        }

        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult GetTeam(long? id, bool edit)
        {
            TeamViewModel model;
            if (id == null)
            {
                model = new TeamViewModel
                {
                    Employees =
                        this.teamEmployeeRepository
                        .Where(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Admin))
                        .OrderBy(x => x.Name)
                        .Select(t => new SelectListItem
                                         {
                                             Text = t.Name,
                                             Value = t.Id.ToString()
                                         })
                };
            }
            else
            {
                var team = this.teamRepository.Get(id.Value);
                model = new TeamViewModel(team)
                    {
                        Employees =
                            this.teamEmployeeRepository
                            .Where(x => x.Status == Statuses.Active && !(x is Domain.Model.Users.Admin))
                            .OrderBy(x => x.Name)
                            .Select(t => new SelectListItem
                                {
                                    Text = t.Name,
                                    Value = t.Id.ToString(),
                                    Selected = team.Employees.Contains(t)
                                })
                };
                if (!edit)
                {
                    model.OpenEstimates = team.Employees.Sum(e => e.Estimates.Count(es => es.EstimateStatus == EstimateStatus.Open && es.Customer.Teams.Contains(team)));
                    model.OpenRO =
                        this.repairOrderRepository.Count(
                            ro =>
                                ro.RepairOrderStatus == RepairOrderStatuses.Open && ro.Customer.Teams.Contains(team)
                               && ro.TeamEmployeePercents.All(x => x.TeamEmployee is Domain.Model.Users.Admin || x.TeamEmployee.Teams.Contains(team)));
                    model.OpenInvoices =
                        this.invoiceRepository.Count(
                            inv =>
                               inv.IsDiscard == false && inv.Status == InvoiceStatus.Unpaid && inv.Customer.Teams.Contains(team) &&
                                inv.RepairOrder.TeamEmployeePercents.All(
                                    x =>
                                        x.TeamEmployee is Domain.Model.Users.Admin ||
                                        x.TeamEmployee.Teams.Contains(team)));
                }
            }

            return this.PartialView(edit ? "EditTeam" : "ViewTeam", model);
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows)
        {
            var data = this.teamGridMaster.GetData(page, rows, sidx, sord);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return json;
        }

        [HttpPost]
        [Transaction]
        public void CreateTeam(TeamViewModel model)
        {
            var team = new Team(true)
            {
                Title = model.Title,
                Comment = model.Comments
            };
            foreach (var empId in model.EmployeesList)
            {
                var teamEmployee = this.teamEmployeeRepository.Get(empId);
                team.Employees.Add(teamEmployee);
            }

            TeamLogger.Create(team);
            this.teamRepository.Save(team);
        }

        [HttpPost]
        [Transaction]
        public void EditTeam(TeamViewModel model)
        {
            var team = this.teamRepository.Get(Convert.ToInt64(model.Id));
            
            team.Title = model.Title;
            team.Comment = model.Comments;
            team.Employees.Clear();
            foreach (var empId in model.EmployeesList)
            {
                var teamEmployee = this.teamEmployeeRepository.Get(empId);
                team.Employees.Add(teamEmployee);
            }

            TeamLogger.Edit(team);
            this.teamRepository.Save(team);
        }

        [HttpPost]
        [Transaction]
        public ContentResult Suspend(long id)
        {
            var team = this.teamRepository.Get(id);
            var employees = team.Employees;
            if (employees.Any(employee => employee.Teams.Count() == 1))
            {
                return this.Content("You need to assign all employees to other teams.");
            }

            team.Status = Statuses.Suspended;
            TeamLogger.Suspend(team);
            this.teamRepository.Save(team);
            return this.Content("Success.");
        }

        [HttpPost]
        [Transaction]
        public void ReactivateTeam(string ids)
        {
            var teamIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var teams = this.teamRepository.Where(x => teamIds.Contains(x.Id)).ToList();
            foreach (var team in teams)
            {
                team.Status = Statuses.Active;
                TeamLogger.Reactivate(team);
                this.teamRepository.Save(team);
            }
        }
    }
}