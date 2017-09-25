using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Technician.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;
using SmartArch.Data;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Technician.Controllers
{
    using SmartArch.Web.Attributes;

    [PDRAuthorize(Roles = "Technician")]
    public class RepairOrdersController : Common.Controllers.RepairOrdersController
    {
        public RepairOrdersController(
            IRepositoryFactory repositoryFactory,
            IGridMasterForStoredProcedure<RepairOrder, RepairOrderJsonModel, RepairOrderViewModel> repairOrderGridMaster,
            ICompanyRepository<Customer> customersRepository,
            ICompanyRepository<Estimate> estimates,
            ICurrentWebStorage<Employee> userStorage,
            ICompanyRepository<Insurance> insuranceRepository,
            ICompanyRepository<Car> carRepository,
            ITempImageManager tempImageManager,
            ICompanyRepository<Domain.Model.Users.Manager> managers,
            ICompanyRepository<RepairOrder> repairOrdersRepository,
            ICompanyRepository<Invoice> invoicesRepository,
            ICompanyRepository<Team> teamsRepository,
            ILogger logger, 
            IPdfConverter pdfConverter)
            : base(
                repositoryFactory, repairOrderGridMaster, customersRepository, estimates, userStorage,
                insuranceRepository, carRepository, tempImageManager, managers, repairOrdersRepository,
                invoicesRepository, teamsRepository, logger, pdfConverter)
        {
        }

        [HttpGet]
        public ActionResult AssignMoreTechnicians(long? ids)
        {
            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(ids));

            var customerTeams = repairOrder.Customer.Teams;

            var technician = ((Domain.Model.Users.Technician)this.userStorage.Get());

            var admins = technician.Company.Employees.Where(x => x.Role == UserRoles.Admin && x.Status == Statuses.Active).Select(x => (TeamEmployee)x).Distinct().ToList();

            var managerTeams = technician.Teams;

            var teams = customerTeams.Count == 0 ? managerTeams : customerTeams.Intersect(managerTeams);

            var model = GetRiOperations(repairOrder);

            var teamEmps = teams.SelectMany(x => x.Employees
                .Where(y => y.Role == UserRoles.Technician
                        || y.Role == UserRoles.Manager || y.Role == UserRoles.RITechnician))
                .Where(x => x.Status == Statuses.Active).Distinct().ToList();
            teamEmps.AddRange(admins);
            var teamEmpsTemp = new List<TeamEmployee>();

            teamEmpsTemp.AddRange(model.TeamEmployees.Count > 0
                                      ? teamEmps.Where(teamEmployee => !model.TeamEmployees.Contains(teamEmployee))
                                      : teamEmps);


            model.Technicians = teamEmpsTemp
                .OrderBy(x => x.Name)
                .Select(
                        r => new SelectListItem
                        {
                            Text = r.Name,
                            Value = string.Concat(r.Id.ToString(), ":", r.Role.ToString())
                        }).ToList();
            if (model.Technicians.Count != 0)
            {
                model.Technicians.First().Selected = true;
            }

            return this.PartialView(model);
        }

        [Transaction]
        [HttpGet]
        public JsonResult RequestForEditRepairOrder(string ids)
        {
            if (!String.IsNullOrEmpty(ids))
            {
                var orderIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
                var orders = this.repairOrdersRepository.Where(x => orderIds.Contains(x.Id)).ToList();
                foreach (var repairOrder in orders)
                {
                    repairOrder.EditedStatus = EditedStatuses.EditPending;
                    this.repairOrdersRepository.Save(repairOrder);
                    logger.LogRequestEdit(repairOrder);
                }
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
