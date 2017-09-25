using System.Web.Http;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Services.Logging;
using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Data;

namespace PDR.Web.WebAPI
{
    [ApiAuthorize]
    public class ChangeEstimateStatusController : ApiController
    {
        private readonly ICompanyRepository<Estimate> estimateRepository;

        private readonly ILogger logger;

        public ChangeEstimateStatusController()
        {
            this.estimateRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        [WebApiTransaction]
        public void Post(ChangeStatusModel model)
        {
            var estimate = this.estimateRepository.Get(model.Id);
            switch (model.Name)
            {
                case "New":
                    estimate.New = model.Value;
                    break;
                case "Archived":
                    estimate.Archived = model.Value;
                    var logAction = model.Value ? EstimateLogActions.Archive : EstimateLogActions.Unarchive;
                    this.logger.Log(estimate, logAction);
                    break;
            }

            this.estimateRepository.Save(estimate);
        }
    }
}