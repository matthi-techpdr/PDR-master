using System.Web.Mvc;
using System;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Services.Logging;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;
using SmartArch.Data;
using SmartArch.Web.Attributes;
using System.Linq;
using System.Collections.Generic;
using PDR.Domain.Helpers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize(Roles = "Manager,Technician,Admin,Estimator")]
    public class EstimatesController : Controller
    {
        private readonly ICompanyRepository<Estimate> estimatesRepository;

        private readonly ICurrentWebStorage<Employee> webStorage;

        private readonly ICompanyRepository<Team> teams;
        
        private readonly ILogger logger;

        protected readonly bool withEmployeeName;

        public EstimatesController(ILogger logger, ICurrentWebStorage<Employee> webStorage, ICompanyRepository<Team> teams)
        {
            this.logger = logger;
            this.estimatesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.webStorage = webStorage;
            this.teams = teams;
            this.withEmployeeName = true;
        }

        [Transaction]
        [HttpPost]
        public ActionResult Discard(long id)
        {
            var estimate = this.estimatesRepository.Get(id);
            
            if(estimate == null)
            {
                return new HttpStatusCodeResult(404, "Estimate doesn't exist");
            }

            estimate.EstimateStatus = EstimateStatus.Discard;
            this.logger.Log(estimate, EstimateLogActions.Discard);
            this.estimatesRepository.Save(estimate);

            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public JsonResult GetHistory(string id)
        {
            var estimate = estimatesRepository.Get(Convert.ToInt64(id));
            var historyManager = new DocumentHistoryManager();
            HistoryListModel historyList = new HistoryListModel(historyManager.GenerateHistory(estimate, withEmployeeName).ToList());
             var result = this.Json(new { historyList }, JsonRequestBehavior.AllowGet);
            return result;
        }


        //public JsonResult GetAllForSearch()
        //{
        //    var selectCommand = this.Request.Url.ParseQueryString()["select"];
        //    var employee = this.webStorage.Get();
        //    var teamSelector = this.Request.Url.ParseQueryString()["team"];
        //    var vin = this.Request.Url.ParseQueryString()["vin"];
        //    var stock = this.Request.Url.ParseQueryString()["stock"];
        //    var custRo = this.Request.Url.ParseQueryString()["custRo"];
        //    Team team = null;
        //    var onlyOwn = false;
        //    if (teamSelector != null)
        //    {
        //        var teamId = Convert.ToInt64(teamSelector);
        //        onlyOwn = teamId == 0;
        //        team = this.teams.Get(teamId);
        //    }

        //    var est = this.estimatesRepository.Find()
        //        .All(new EstimatesByUserForSearch(employee, team: team, onlyOwn: onlyOwn, vin: vin, stock: stock, custRo: custRo))
        //        .ToList()
        //        .OrderByDescending(x => x.CreationDate);

        //    var estimates = est.Select(e => new ApiEstimateModel(e, true)).ToList().AsQueryable();

        //    if (selectCommand != null)
        //    {
        //        var properties = selectCommand.Split(',');
        //        var tmp = estimates.Select(m => this.GetModelWithSelectedProperties(m, properties));
        //        return this.Json(new { tmp }, JsonRequestBehavior.AllowGet);
        //    }
        //    return this.Json(new { estimates }, JsonRequestBehavior.AllowGet);
        //}

        //protected ApiEstimateModel GetModelWithSelectedProperties(ApiEstimateModel model, IEnumerable<string> requestProperties)
        //{
        //    var modelProperties = typeof(ApiEstimateModel).GetProperties();
        //    foreach (var modelProperty in modelProperties)
        //    {
        //        if (!requestProperties.Contains(modelProperty.Name))
        //        {
        //            modelProperty.SetValue(model, null, null);
        //        }
        //    }

        //    return model;
        //}

    }
}