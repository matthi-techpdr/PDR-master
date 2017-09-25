using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize]
    public class TeamsController : Controller
    {
        private readonly Employee employee;

        private readonly ICompanyRepository<Team> teamsRepository;

        public TeamsController(ICompanyRepository<Team> teamsRepository, ICurrentWebStorage<Employee> storage)
        {
            this.teamsRepository = teamsRepository;
            this.employee = storage.Get();
        }

        public JsonResult GetAll()
        {
            var teamEmployee = this.employee as TeamEmployee;
            List<ApiTeamModel> teams = null;
            if (teamEmployee != null)
            {
                if ((teamEmployee is Admin) || (teamEmployee is Manager && teamEmployee.IsShowAllTeams))
                {
                    teams = this.teamsRepository.OrderBy(x => x.Title).Select(x => new ApiTeamModel(x)).ToList();
                    //teams = teams.OrderBy(x => x.Title).ToList();
                }
                else
                {
                    teams = teamEmployee.Teams.OrderBy(x => x.Title).Select(x => new ApiTeamModel(x)).ToList();
                    //teams = teams.OrderBy(x => x.Title).ToList();
                }
            }
            return this.Json(teams, JsonRequestBehavior.AllowGet);
        }
    }
}