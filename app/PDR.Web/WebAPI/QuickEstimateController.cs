using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Services.Logging;
using PDR.Web.Core.Attributes;
using PDR.Web.Core.NLog.FileLoggers;
using PDR.Web.WebAPI.Helper;
using PDR.Web.WebAPI.IphoneModels;

namespace PDR.Web.WebAPI
{
    public class QuickEstimateController : ApiController
    {
        private readonly ICompanyRepository<Estimate> estimateRepository;

        private readonly EstimateModelToEstimateConverter estimateModelToEstimateConverter;

        protected readonly ILogger logger;

        public QuickEstimateController()
        {
            this.estimateRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.estimateModelToEstimateConverter = new EstimateModelToEstimateConverter();
            this.logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        [WebApiTransaction]
        public HttpResponseMessage Post(EstimatesList model)
        {
            try
            {
                foreach (var estimateModel in model.Estimates)
                {
                    var estimate = this.estimateModelToEstimateConverter.GetEstimate(estimateModel);
                    this.estimateRepository.Save(estimate);

                    if (estimateModel.Id == null)
                    {
                        EstimateLogger.Create(estimate);
                        logger.Log(estimate, EstimateLogActions.Create);
                    }
                    else
                    {
                        EstimateLogger.Edit(estimate);
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}