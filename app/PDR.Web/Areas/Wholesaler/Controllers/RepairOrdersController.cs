using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Technician.Models;
using PDR.Web.Core.Authorization;
using SmartArch.Data;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Wholesaler.Controllers
{
    [PDRAuthorize(Roles = "Wholesaler")]
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
                repositoryFactory,
                repairOrderGridMaster,
                customersRepository,
                estimates,
                userStorage,
                insuranceRepository,
                carRepository,
                tempImageManager,
                managers,
                repairOrdersRepository,
                invoicesRepository,
                teamsRepository,
                logger,
                pdfConverter)
        {
        }

        [HttpGet]
        public ActionResult AssignMoreTechnicians(long? ids)
        {
            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(ids));

            var adminEmployees = this.userStorage.Get().Company.Employees
                                                        .Where(x => x is TeamEmployee)
                                                        .Where(x => x.Status == Statuses.Active)
                                                        .Select(x => (TeamEmployee)x).ToList();

            var model = GetRiOperations(repairOrder);

            var teamEmps = adminEmployees.Where(x => x.Status == Statuses.Active).Distinct().OrderBy(x => x.Name).ToList();
            var teamEmpsTemp = new List<TeamEmployee>();

            teamEmpsTemp.AddRange(model.TeamEmployees.Count > 0
                                      ? teamEmps.Where(teamEmployee => !model.TeamEmployees.Contains(teamEmployee))
                                      : teamEmps);


            model.Technicians = teamEmpsTemp
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

        [HttpGet]
        public override ActionResult Print(string id, bool? isBasic = true)
        {
           return base.Print(id, isBasic);
        }
    }
}
