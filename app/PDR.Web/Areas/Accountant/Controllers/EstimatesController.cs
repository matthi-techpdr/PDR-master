using System.Web.Mvc;

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

namespace PDR.Web.Areas.Accountant.Controllers
{
    public class EstimatesController : Common.Controllers.EstimatesController
    {
        public EstimatesController(IRepositoryFactory repositoryFactory, IGridMaster<Estimate, EstimateJsonModel, EstimateViewModel> estimateGridMaster, ICurrentWebStorage<Employee> userStorage, ITempImageManager tempImageManager, IPdfConverter pdfConverter, ILogger logger, ICompanyRepository<CarInspection> carInspectionsRepository, ICompanyRepository<EffortItem> effortItemRepository, ReassignHelper reassignHelper)
            : base(repositoryFactory, estimateGridMaster, userStorage, tempImageManager, pdfConverter, logger, carInspectionsRepository, effortItemRepository, reassignHelper)
        {
        }
    }
}
