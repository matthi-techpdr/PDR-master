using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model;

using PDR.Web.Core.Attributes;
using PDR.Web.Core.NLog.FileLoggers;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.Helper;
using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Data;

namespace PDR.Web.WebAPI
{
    using PDR.Domain.Model.Enums.LogActions;
    using PDR.Domain.Services.Logging;

    [ApiAuthorize]
    public class EstimateController : BaseWebApiController<Estimate, ApiEstimateModel>
    {
        private readonly EstimateModelToEstimateConverter estimateModelToEstimateConverter;

        protected readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        private readonly ICompanyRepository<Affiliate> affiliates;

        protected readonly ILogger logger;

        public EstimateController()
        {
            this.estimateModelToEstimateConverter = new EstimateModelToEstimateConverter();
            this.repairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            this.logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        public override HttpResponseMessage Get(long id)
        {
            var entity = this.Repository.Get(id);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = new ApiEstimateModel(entity);
            CommonLogger.View(entity);

            var result = Request.CreateResponse(HttpStatusCode.OK, model);

            return result;
        }

        [WebApiTransaction]
        public override HttpResponseMessage Post(ApiEstimateModel model)
        {
            if (model == null || !model.EmployeeId.HasValue)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                message.Content = new StringContent("Entity can not be null.");
                throw new HttpResponseException(message);
            }
            try
            {
                var estimate = this.estimateModelToEstimateConverter.GetEstimate(model);
                this.Repository.Save(estimate);

                var ro = repairOrdersRepository.SingleOrDefault(x => x.Estimate.Id == estimate.Id);
                if (ro != null)
                {
                    ro.EditedStatus = EditedStatuses.EditingReject;
                    repairOrdersRepository.Save(ro);
                    logger.Log(ro, RepairOrderLogActions.Edit);
                    CommonLogger.EditRepairOrder(ro);
                }
                else
                {
                    if (model.Id == null)
                    {
                        EstimateLogger.Create(estimate);
                        logger.Log(estimate, EstimateLogActions.Create);
                    }
                    else
                    {
                        EstimateLogger.Edit(estimate);
                        logger.Log(estimate, EstimateLogActions.Edit);
                    }
                }

                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri(Request.RequestUri, estimate.Id.ToString());
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public override void Put(ApiEstimateModel model)
        {
            if (model.Id == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                message.Content = new StringContent("EstimateId can not be null.");
                throw new HttpResponseException(message);
            }

            var estimate = this.estimateModelToEstimateConverter.GetEstimate(model);
            this.Repository.Save(estimate);

            var ro = repairOrdersRepository.SingleOrDefault(x => x.Estimate.Id == estimate.Id);
            
            if (ro == null)
            {
                return;
            }

            ro.EditedStatus = EditedStatuses.EditingReject;
            repairOrdersRepository.Save(ro);
        }
    }
}
