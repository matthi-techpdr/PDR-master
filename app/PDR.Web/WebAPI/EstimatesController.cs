using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;
using System.Configuration;
using PDR.Domain.Model.Enums;
using PDR.Domain.StoredProcedureHelpers;


namespace PDR.Web.WebAPI
{
    [ApiAuthorize]
    public class EstimatesController : BaseWebApiController<Estimate, ApiEstimateLightModel>
    {
        private readonly ICurrentWebStorage<Employee> storage;

        private readonly ICompanyRepository<Team> teams; 

        protected readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        private readonly ICompanyRepository<Affiliate> affiliates;

        private const string SortByColumn = "CreationDate";

        private int CountRows
        {
            get
            {
                int number;
                bool result = Int32.TryParse(ConfigurationManager.AppSettings["CountDocumentRowsForIPhone"], out number);
                if (result)
                {
                    return number;
                }
                else
                {
                    return 50;
                }
            }
        }
        
        public EstimatesController()
        {
            this.storage = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>();
            this.teams = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();
            this.repairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
        }

        public override IList<ApiEstimateLightModel> Get()
        {
            var collection = this.Request.RequestUri.ParseQueryString();
            var employee = this.storage.Get();

            var selectCommand = collection["select"];
            var teamSelector = collection["team"];
            var archivedSelector = collection["archived"];
            var vin = collection["vin"];
            var stock = collection["stock"];
            var custRo = collection["custRo"];
            var page = collection["page"];
            var sortByDateDesc = collection["isDesc"] == "true";
            var statusEstimate = collection["statusEstimate"];

            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            var onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;
            var archived = String.IsNullOrEmpty(archivedSelector) ? (bool?)null : archivedSelector.ToLower() == "true";
            var numPage = Convert.ToInt32(page);
            var status = GeteEstimateStatus(statusEstimate);
            var sort = sortByDateDesc ? "DESC" : "ASC";

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(employee.Id, teamId, isOnlyOwn: onlyOwn, rowsPerPage: this.CountRows, pageNumber: numPage + 1,
                        sortByColumn: SortByColumn, sortType: sort, estimatesStatus: status, isForReport: false, isArchived: archived, vinCode: vin, stock: stock, custRo: custRo);


            var estimates = estimatesSpHelper.Estimates;

            var model = estimates.Select(e => new ApiEstimateLightModel(e)).AsQueryable();
            
            if (selectCommand == null)
            {
                return model.ToList();
            }

            var properties = selectCommand.Split(',');
            var result = model.Select(m => this.GetModelWithSelectedProperties(m, properties));

            return result.ToList();
        }

        private EstimateStatus? GeteEstimateStatus(string status)
        {
            if (status == null)
            {
                return null;
            }
            switch (status.ToLower())
            {
                case "open":
                    return EstimateStatus.Open;
                case "completed":
                    return EstimateStatus.Completed;
                case "approved":
                    return EstimateStatus.Approved;
                case "converted":
                    return EstimateStatus.Converted;
                default:            //ALL estimates
                    return null;
            }
        }
    }
}
