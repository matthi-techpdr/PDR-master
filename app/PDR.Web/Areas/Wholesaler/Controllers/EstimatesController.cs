using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MvcContrib;
using NLog;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Resources;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.SMSSending;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Areas.Wholesaler.Managers;
using PDR.Web.Areas.Wholesaler.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;
using SmartArch.Data;
using SmartArch.Web.Attributes;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;
using Logger = NLog.Logger;

namespace PDR.Web.Areas.Wholesaler.Controllers
{
    [PDRAuthorize(Roles = "Wholesaler")]
    public class EstimatesController : Common.Controllers.EstimatesController
    {
        private readonly IMailService mailService;

        public EstimatesController(IRepositoryFactory repositoryFactory, IGridMasterForStoredProcedure<Estimate, EstimateJsonModel,
            EstimateViewModel> estimateGridMaster, ICurrentWebStorage<Employee> userStorage, ITempImageManager tempImageManager,
            IPdfConverter pdfConverter, ILogger logger, ICompanyRepository<CarInspection> carInspectionsRepository,
            ICompanyRepository<EffortItem> effortItemRepository, ReassignHelper reassignHelper,
            ICompanyRepository<RepairOrder> repairOrdersRepository, ICompanyRepository<Invoice> invoicesRepository)
            : base(repositoryFactory, estimateGridMaster, userStorage, tempImageManager, pdfConverter, logger, carInspectionsRepository,
            effortItemRepository, reassignHelper, repairOrdersRepository, invoicesRepository)
        {
            this.mailService = ServiceLocator.Current.GetInstance<IMailService>();
        }

        #region Actions

        [Transaction]
        [HttpGet]
        public override FileContentResult PrintEstimates(string id, bool? isBasic = true)
        {
            return base.PrintEstimates(id, isBasic);
        }

        #region New
        [HttpGet]
        public override ActionResult New()
        {
            Employee currentUser = this.userStorage.Get();

            EstimateManager estimateManager = new EstimateManager();
            WholesalerEstimateModel model = estimateManager.GetEstimateModel(this.wholesaleCustomersRepository.First(x => x.Email == currentUser.Login));

            this.InitViewBags(currentUser);

            return this.View(model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult NewWholesalerEstimate(WholesalerEstimateModel wholesalerEstimateModel)
        {
            try
            {
                wholesalerEstimateModel.Insurance.CompanyName = wholesalerEstimateModel.Insurance.CompanyName ?? string.Empty;
                Employee currentUser = this.userStorage.Get();
                WholesaleCustomer currentCustomer = this.wholesaleCustomersRepository.First(x => x.Email == currentUser.Login);
                if (this.ModelState.IsValid && currentUser != null)
                {
                    Estimate estimate = this.Save(wholesalerEstimateModel, currentCustomer);
                    logger.Log(estimate, EstimateLogActions.Create);
                    if (wholesalerEstimateModel.IsExistVin)
                    {
                        logger.Log(estimate, EstimateLogActions.RepeatVehicleEntryConfirmed);
                    }
                    EstimateLogger.Create(estimate);
                    return this.RedirectToAction(x => x.Index(null, null, null, true));
                }

                EstimateManager estimateManager = new EstimateManager();
                WholesalerEstimateModel model = estimateManager.GetEstimateModel(this.wholesaleCustomersRepository.First(x => x.Email == currentUser.Login));
                this.InitViewBags(currentUser);

                wholesalerEstimateModel.CarInspectionsModel.CarInspections[14].TotalDents = (TotalDents)(-1);
                wholesalerEstimateModel.CarInspectionsModel.CarInspections[14].AverageSize = (AverageSize)(-1);
                wholesalerEstimateModel.CarInspectionsModel.CarInspections[15].TotalDents = (TotalDents)(-1);
                wholesalerEstimateModel.CarInspectionsModel.CarInspections[15].AverageSize = (AverageSize)(-1);

                return this.View("New", model);
            }
            catch (Exception exception)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                var msg = String.Format("Save new estimate failed: {0}", exception.Message);
                logger.Error(msg);
            }
            return this.View("New");
        }

        #endregion


        #endregion

        [HttpGet]
        public JsonResult GetMatrixData(long? matrix)
        {
            if (matrix != 0)
            {
                var matrixprice = this.repositoryFactory.CreateForCompany<Matrix>().Get(Convert.ToInt64(matrix)) ??
                                      this.repositoryFactory.CreateForCompany<DefaultMatrix>().First();

                return Json(
                    new
                    {
                        MaxCorProtect = matrixprice.MaxCorrosionProtection,
                        Aluminium = matrixprice.AluminiumPanel,
                        DoubleMetall = matrixprice.DoubleLayeredPanels,
                        matrixprice.OversizedRoof,
                        matrixprice.OversizedDents,
                        CorProtectPart = matrixprice.CorrosionProtectionPart,
                        matrixprice.Maximum
                    },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        protected Estimate Save(WholesalerEstimateModel wholesalerEestimateModel, WholesaleCustomer currentCustomer)
        {
            SaveEstimateManager saveEstimatemanager = new SaveEstimateManager(this.repositoryFactory, this.tempImageManager);
            Estimate estimate = saveEstimatemanager.Save(wholesalerEestimateModel, currentCustomer);
            this.estimatesRepository.Save(estimate);
            WholesaleCustomer customer = (WholesaleCustomer)estimate.Customer;
            string smsMessage = String.Format(SmsTemplatesRes.NewEstimateByWholesaler, customer.Name);
            new SMSSending().Send(estimate.Employee.PhoneNumber, smsMessage);
            string emailSubject = String.Format(EmailTemplatesRes.NewEstimateByWholesalerSubject, customer.Name);
            string emailMessage =
                String.Format(EmailTemplatesRes.NewEstimateByWholesalerMsg, customer.Name, estimate.Car.GetYearMakeModelInfo(), customer.Address1, customer.City, customer.State, customer.Phone);
            if (estimate.Employee.Email != null)
            {
                this.mailService.Send(estimate.Employee.Company.Email, estimate.Employee.Email, emailSubject, emailMessage);
            }
            return estimate;
        }

        protected void InitViewBags(Employee currentUser)
        {
            var company = this.userStorage.Get().Company;
            var matrix = this.repositoryFactory.CreateForCompany<Matrix>().SingleOrDefault(x => x is DefaultMatrix && x.Status == Statuses.Active && x.Company.Id == company.Id);
            ViewBag.CurrentUser = currentUser;
            ViewBag.LimitForBodyPart = company.LimitForBodyPartEstimate;
            ViewBag.DefaultLimit = company.LimitForBodyPartEstimate;
            ViewBag.DefaultCar = true;
            if (matrix != null)
            {
                ViewBag.DefaultMatrix = matrix.Id;
                ViewBag.DefaultMaxCorProtect = matrix.MaxCorrosionProtection;
                ViewBag.DefaultAluminium = matrix.AluminiumPanel;
                ViewBag.DefaultDoubleMetall = matrix.DoubleLayeredPanels;
                ViewBag.DefaultOversizedRoof = matrix.OversizedRoof;
                ViewBag.DefaultOversizedDents = matrix.OversizedDents;
                ViewBag.DefaultCorProtectPart = matrix.CorrosionProtectionPart;
                ViewBag.DefaultMaxPercentPart = matrix.Maximum;
            }
            ViewBag.Status = EstimateStatus.Open;
        }
    }
}
