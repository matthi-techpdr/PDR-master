using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;

using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Technician.Models;
using PDR.Web.Core.Authorization;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.RITechnician.Controllers
{
    [PDRAuthorize(Roles = "RITechnician")]
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
    }
}
