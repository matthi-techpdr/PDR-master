using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Accountant.Models.Team;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Manager.Controllers
{
    [PDRAuthorize(Roles = "Manager")]
    public class TeamsController : Common.Controllers.TeamsController
    {
        public TeamsController(
            ICompanyRepository<Team> teamRepository,
            ICompanyRepository<TeamEmployee> teamEmployeeRepository,
            ICompanyRepository<RepairOrder> repairOrderRepository,
            ICompanyRepository<Invoice> invoiceRepository,
            IGridMaster<Team, TeamJsonModel, TeamViewModel> teamGridMaster)
            : base(teamRepository, teamEmployeeRepository, repairOrderRepository, invoiceRepository, teamGridMaster)
        {
        }

    }
}
