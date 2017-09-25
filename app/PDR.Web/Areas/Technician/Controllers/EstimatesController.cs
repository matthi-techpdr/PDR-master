using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Technician.Controllers
{
    [PDRAuthorize(Roles = "Technician")]
    public class EstimatesController : Common.Controllers.EstimatesController
    {
        public EstimatesController(IRepositoryFactory repositoryFactory,
            IGridMasterForStoredProcedure<Estimate, EstimateJsonModel, EstimateViewModel> estimateGridMaster,
            ICurrentWebStorage<Employee> userStorage,
            ITempImageManager tempImageManager,
            IPdfConverter pdfConverter,
            ILogger logger,
            ICompanyRepository<CarInspection> carInspectionsRepository,
            ICompanyRepository<EffortItem> effortItemsRepository,
            ReassignHelper reassignHelper,
            ICompanyRepository<RepairOrder> repairOrdersRepository,
            ICompanyRepository<Invoice> invoicesRepository)
            : base(
                repositoryFactory,
                estimateGridMaster,
                userStorage,
                tempImageManager,
                pdfConverter,
                logger,
                carInspectionsRepository,
                effortItemsRepository,
                reassignHelper,
                repairOrdersRepository,
                invoicesRepository)             
        {            
        }
    }
}