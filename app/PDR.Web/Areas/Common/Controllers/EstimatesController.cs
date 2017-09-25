using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using MvcContrib;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.VINDecoding;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Data.Fetching;
using SmartArch.Data.Proxy;
using SmartArch.Web.Attributes;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Common.Controllers
{
    using NLog;

    using PDR.Domain.StoredProcedureHelpers;
    using PDR.Web.Core.Helpers;

    [PDRAuthorize]
    public class EstimatesController : Controller
    {
        #region Protected Fields

        protected virtual IGridMasterForStoredProcedure<Estimate, EstimateJsonModel, EstimateViewModel> estimateGridMaster { get; set; }

        protected readonly IRepositoryFactory repositoryFactory;

        protected readonly ICompanyRepository<Estimate> estimatesRepository;

        protected readonly ICompanyRepository<Invoice> invoicesRepository;

        private readonly IRepository<Affiliate> affiliatesRepository;

        protected readonly ICompanyRepository<WholesaleCustomer> wholesaleCustomersRepository;

        protected readonly ICompanyRepository<CarInspection> carInspectionsRepository;

        protected readonly ITempImageManager tempImageManager;

        protected readonly ICurrentWebStorage<Employee> userStorage;

        protected readonly ReassignHelper reassignHelper;

        protected readonly IPdfConverter pdfConverter;

        protected readonly ILogger logger;

        protected readonly ICompanyRepository<EffortItem> effortItemRepository;

        protected readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        protected readonly bool withEmployeeName;

        private readonly ICompanyRepository<WholesaleCustomer> wholesaleCustomers;

        #endregion

        #region Constructor

        public EstimatesController(
            IRepositoryFactory repositoryFactory,
            IGridMasterForStoredProcedure<Estimate, EstimateJsonModel, EstimateViewModel> estimateGridMaster,
            ICurrentWebStorage<Employee> userStorage,
            ITempImageManager tempImageManager,
            IPdfConverter pdfConverter,
            ILogger logger,
            ICompanyRepository<CarInspection> carInspectionsRepository,
            ICompanyRepository<EffortItem> effortItemRepository,
            ReassignHelper reassignHelper,
            ICompanyRepository<RepairOrder> repairOrdersRepository,
            ICompanyRepository<Invoice> invoicesRepository)
        {
            this.repositoryFactory = repositoryFactory;
            this.estimateGridMaster = estimateGridMaster;
            this.reassignHelper = reassignHelper;
            this.userStorage = userStorage;
            this.tempImageManager = tempImageManager;
            this.pdfConverter = pdfConverter;
            this.logger = logger;
            this.carInspectionsRepository = carInspectionsRepository;
            this.wholesaleCustomersRepository = this.repositoryFactory.CreateForCompany<WholesaleCustomer>();
            this.affiliatesRepository = this.repositoryFactory.CreateForCompany<Affiliate>();
            this.estimatesRepository = this.repositoryFactory.CreateForCompany<Estimate>();
            this.effortItemRepository = effortItemRepository;
            this.repairOrdersRepository = repairOrdersRepository;
            this.invoicesRepository = invoicesRepository;
            this.withEmployeeName = true;
            this.wholesaleCustomers = ServiceLocator.Current.GetInstance<ICompanyRepository<WholesaleCustomer>>();
        }

        #endregion

        #region Actions

        [ValidateInput(false)]
        public virtual ActionResult Index(string vin = null, string stock = null, string custRo = null, bool withFilters=true)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            FilterModel filters = null;
            if (withFilters)
            {
                var teamCookie = HttpContext.Request.Cookies.Get("team");

                Type type = typeof (Estimate);
                filters = new FilterModel(type, false, false, teamCookie);
            }

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }
            return this.View(filters);
        }

        [ValidateInput(false)]
        public virtual ActionResult ArchiveEstimates(string vin = null, string stock = null, string custRo = null)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            var currentUser = this.userStorage.Get();
            var filters = new FilterModel(typeof(Estimate), true, false, teamCookie);
            var adminOrManager = currentUser is Domain.Model.Users.Admin || currentUser is Domain.Model.Users.Manager;
            this.ViewBag.AddEmployeeColumn = adminOrManager;
            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }

            return this.View(filters);
        }

        [Transaction]
        public void ArchiveUnarchive(string ids, bool toArchived)
        {
            var estimatesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var estimates = this.estimatesRepository.Where(x => estimatesIds.Contains(x.Id)).ToList();
            foreach (var estimate in estimates)
            {
                estimate.Archived = toArchived;
                this.estimatesRepository.Save(estimate);
                var logAction = toArchived ? EstimateLogActions.Archive : EstimateLogActions.Unarchive;
                this.logger.Log(estimate, logAction);
            }
        }

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            var subject = this.userStorage.Get().Company.EstimatesEmailSubject;
            if (customer)
            {
                to = this.repositoryFactory.CreateForCompany<Customer>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }
            else
            {
                var estimatesIds = ids.Split(',');

                if (this.CheckEstimatesForTheSameCustomer(estimatesIds) && estimatesIds.Length > 1)
                {
                    return new JsonResult { Data = "Error" };
                }

                foreach (var id in estimatesIds)
                {
                    var estimate = this.estimatesRepository.Get(Convert.ToInt64(id));
                    var email = estimate.Customer.Email ?? string.Empty;
                    var pattern = email;
                    var reg = new Regex(pattern, RegexOptions.IgnoreCase);
                    var matches = reg.Matches(to);

                    if (!string.IsNullOrWhiteSpace(email) && !(matches.Count > 0))
                    {
                        to += email + ", ";
                    }
                    if (!String.IsNullOrEmpty(estimate.Car.Stock))
                    {
                        subject += " " + estimate.Car.Stock + ",";
                    }
                    else if (!String.IsNullOrEmpty(estimate.Car.CustRO))
                    {
                        subject += " " + estimate.Car.CustRO + ",";
                    }
                    else if (!String.IsNullOrEmpty(estimate.Car.Make) && !String.IsNullOrEmpty(estimate.Car.Model))
                    {
                        subject += " " + estimate.Car.Year + "/" + estimate.Car.Make + "/" + estimate.Car.Model + ",";
                    }
                }
                
                to = string.Join(", ", to.Split(new[] { ',', ' ' }).Distinct());
            }

            var idWh = ids.Split(',')[0];
            var wholesale = this.estimatesRepository.Get(Convert.ToInt64(idWh)).Customer;
            var isWholesale = wholesale.ToPersist<WholesaleCustomer>() != null;
            var additionalEmails = new string[3];
            if (isWholesale)
            {
                additionalEmails[0] = wholesale.Email2 ?? string.Empty;
                additionalEmails[1] = wholesale.Email3 ?? string.Empty;
                additionalEmails[2] = wholesale.Email4 ?? string.Empty;
            }

           subject = subject.TrimEnd(',');
            var result = isWholesale
                ? new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids, Subject = subject, 
                    IsWholesale = true, To2 = additionalEmails[0], To3 = additionalEmails[1], To4 = additionalEmails[2], IsBasic = true}
                : new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids, Subject = subject, IsBasic = true};
            return this.PartialView(result);
        }

        [Transaction]
        public ActionResult GetManagers(string ids, bool forConvert)
        {
            var user = this.userStorage.Get();
            if (user is Domain.Model.Users.Technician)
            {
                this.ConvertToRepairOrder(user.Id, ids);
                return new HttpStatusCodeResult(201);
            }

            this.ViewData["convert"] = forConvert;
            long[] estimatesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var employees = this.reassignHelper.GetReassignEmployees(this.userStorage.Get(), estimatesIds, forConvert)
                    .Where(x => x.Role != UserRoles.RITechnician);

            employees = employees.OrderBy(x => x.Name);
            var selectedList = employees.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            selectedList.Insert(0, new SelectListItem { Text = @" ", Value = "-1" });
            return this.PartialView(selectedList);
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                string _msg = model.Message;
                var estimateIds = model.IDs.Split(',');
                IList<Attachment> attachments = new List<Attachment>();
                var from = this.userStorage.Get().Company.Email;
                foreach (var id in estimateIds)
                {
                    var estimate = this.estimatesRepository.Get(Convert.ToInt64(id));
                    if (estimate != null)
                    {
                        var pdf = this.ConvertToPdf(estimate, !model.IsBasic); //Todo  
                        var fileName = string.Format("Estimate #{0}_{1}.pdf", estimate.Id, estimate.CreationDate.ToString("MM-dd-yyyy"));
                        attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                        this.logger.Log(estimate, EstimateLogActions.Email, model.To);

                        if (estimate.Customer.GetCustomerName().Contains("Caliber"))
                        {
                            string _disclimer = "Note: This is an initial estimate and NOT a final invoice." + Environment.NewLine;
                            _msg = _disclimer + model.Message;
                        }
                    }                   
                }
                var to = model.To + 
                    (String.IsNullOrEmpty(model.To2) ? "" : "," + model.To2) + 
                    (String.IsNullOrEmpty(model.To3) ? "" : "," + model.To3) + 
                    (String.IsNullOrEmpty(model.To4) ? "" : "," + model.To4);

                var message = new MailService().Send(from, to, model.Subject, _msg , attachments.Count > 0 ? attachments : null);
                return this.Json(message, JsonRequestBehavior.AllowGet);
            }

            return this.Json(false, JsonRequestBehavior.AllowGet);
        }
        
        [Transaction]
        [HttpPost]
        public JsonResult Discard(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            try
            {
                foreach (var estimate in ids.Split(',')
                                            .Select(id => this.estimatesRepository.SingleOrDefault(x => x.Id == Convert.ToInt64(id)))
                                            .Where(estimate => estimate != null))
                {
                    estimate.EstimateStatus = EstimateStatus.Discard;
                    this.logger.Log(estimate, EstimateLogActions.Discard);
                    this.estimatesRepository.Save(estimate);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        public void Reassign(string estimatesIds, long managerId)
        {
            var estimateIdsList = estimatesIds.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            this.reassignHelper.ReassignEstimate(estimateIdsList, managerId, this.userStorage.Get());
        }

        [Transaction]
        public void ConvertToRepairOrders(string estimatesIds, long? managerId)
        {
            this.ConvertToRepairOrder(managerId, estimatesIds);
        }

        [ValidateInput(false)]
        public virtual JsonResult GetData(string sidx, string sord, int page, int rows, long? customer, long? team, bool archived,
            bool? isStartSearch = null, string vin = null, string stock = null, string custRo = null, bool isNeedFilter = false)
        {
            var currentUser = this.userStorage.Get();
            if (currentUser == null)
            {
                return null;
            }
            var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var onlyOwn = team == 0;

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentUser.Id, team, customer, onlyOwn, rows, page, sidx, sord, archived,
                        vin, stock, custRo, false, isForCustomerFilter: isNeedFilter);
            var data = this.estimateGridMaster.GetData(estimatesSpHelper, rows, page, isCustomerFilter: isNeedFilter, customerCookie: customerCookie);

            return Json( data , JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult GetHistory(string id)
        {
            var estimate = estimatesRepository.Get(Convert.ToInt64(id));
            var model = new List<string>();
            var historyManager = new DocumentHistoryManager();
            model = historyManager.GenerateHistory(estimate, withEmployeeName).ToList();
            return this.PartialView(model);
        }

        #region New
        [HttpGet]
        public virtual ActionResult New()
        {
            var customerResolver = new CustomersResolver(this.repositoryFactory, this.userStorage);
            var model = customerResolver.CustomerResolve(null, null, "new");
            var affiliateId = Convert.ToInt64(model.Customer.Retail.Affiliates.First().Value);

            this.InitViewBags(null, model, null, "new", affiliateId);

            var allCustomers = this.wholesaleCustomers.ToList();
            //var allRetailCustomers = this.retailCustomers.ToList();

            //if (model.Id != null)
            //{
            //    allCustomers.Remove(this.wholesaleCustomers.Get(Convert.ToInt64(model.Id)));
            //}

            //var ar = allCustomers.Select(x => x.Email).ToList().AddRange(allRetailCustomers.Select(x => x.Email.ToList()));
            //this.ViewData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList());// + "," + (string.Join(",", allRetailCustomers.Select(x => x.Email).ToList()));//.AddRange(allRetailCustomers.Select(x => x.Email.ToList(string))));
            
            //Using TempData because ViewData becomes null going through RenderPartialWithPrefix
            this.TempData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList());// + "," + (string.Join(",", allRetailCustomers.Select(x => x.Email).ToList()));//.AddRange(allRetailCustomers.Select(x => x.Email.ToList(string))));

            return this.View(model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult New(EstimateModel estimateModel)
        {
            try
            {
                estimateModel.Insurance.CompanyName = estimateModel.Insurance.CompanyName ?? string.Empty;
                var employee = this.userStorage.Get();
                if (this.ModelState.IsValid && employee != null)
                {
                    var estimate = new Estimate(true);
                    this.Save(estimate, estimateModel);
                    logger.Log(estimate, EstimateLogActions.Create);
                    if (estimateModel.IsExistVin)
                    {
                        logger.Log(estimate, EstimateLogActions.RepeatVehicleEntryConfirmed);
                    }
                    EstimateLogger.Create(estimate);
                    return this.RedirectToAction(x => x.Index(null, null, null, true));
                }

                var customerResolver = new CustomersResolver(this.repositoryFactory, this.userStorage);
                var model = customerResolver.CustomerResolve();
                var affiliateId = Convert.ToInt64(model.Customer.Retail.Affiliates.First().Value);

                this.InitViewBags(null, estimateModel, null, "new", affiliateId);
                estimateModel.CarInspectionsModel.CarInspections[14].TotalDents = (TotalDents)(-1);
                estimateModel.CarInspectionsModel.CarInspections[14].AverageSize = (AverageSize)(-1);
                estimateModel.CarInspectionsModel.CarInspections[15].TotalDents = (TotalDents)(-1);
                estimateModel.CarInspectionsModel.CarInspections[15].AverageSize = (AverageSize)(-1);

                return this.View(model);
            }
            catch (Exception exception)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                var msg = String.Format("Save new estimate failed: {0}", exception.Message);
                logger.Error(msg);
            }
            return this.View();
        }
        #endregion

        #region Edit

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var estimate = this.estimatesRepository.Get(id);
            var customerResolver = new CustomersResolver(this.repositoryFactory, this.userStorage);
            var model = customerResolver.CustomerResolve(estimate);

            var ro = repairOrdersRepository.SingleOrDefault(x => x.Estimate.Id == estimate.Id);
            if (ro != null)
            {
                this.ViewBag.isEditable = ro.EditedStatus == EditedStatuses.Editable;
            }

            this.InitViewBags(id, model, estimate, "edit");

            return this.View(model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(long id, EstimateModel estimateModel)
        {
            var estimate = this.estimatesRepository.Get(id);
            if (estimate == null)
            {
                throw new HttpException(404, "Not found action");
            }

            estimateModel.Insurance.CompanyName = estimateModel.Insurance.CompanyName ?? string.Empty;

            if (this.ModelState.IsValid)
            {
                this.Save(estimate, estimateModel);
                if (estimateModel.IsRoEditable)
                {
                    logger.Log(this.repairOrdersRepository.Single(x => x.Estimate.Id == estimateModel.Id), RepairOrderLogActions.Edit);
                }
                else
                {
                    logger.Log(estimate, EstimateLogActions.Edit);
                }
                EstimateLogger.Edit(estimate);
                
                var ro = repairOrdersRepository.SingleOrDefault(x => x.Estimate.Id == estimate.Id);
                if (ro != null)
                {
                    ro.EditedStatus = EditedStatuses.EditingReject;
                    repairOrdersRepository.Save(ro);
                    return RedirectToAction("View", "RepairOrders", new { id = ro.Id});
                }

                return this.RedirectToAction(x => x.Index(null, null, null, true));
            }

            this.InitViewBags(id, estimateModel, estimate, "edit");
            estimateModel.CarInspectionsModel.CarInspections[14].TotalDents = (TotalDents)(-1);
            estimateModel.CarInspectionsModel.CarInspections[14].AverageSize = (AverageSize)(-1);
            estimateModel.CarInspectionsModel.CarInspections[15].TotalDents = (TotalDents)(-1);
            estimateModel.CarInspectionsModel.CarInspections[15].AverageSize = (AverageSize)(-1);

            var currentUser = this.userStorage.Get();
                var affilates = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
            ? ((TeamEmployee)currentUser).Teams.Where(x => x.Status == Statuses.Active).SelectMany(x => x.Customers).Distinct()
            .Select(x => x.ToPersist<Affiliate>()).Where(x => x.Status == Statuses.Active).ToList()
            : this.affiliatesRepository.Where(x => x.Status == Statuses.Active).ToList();
            if (estimate.Affiliate != null && !affilates.Contains(estimate.Affiliate))
            {
                affilates.Add(estimate.Affiliate);
            }
            

            estimateModel.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates, estimate.Affiliate);

            return this.View(EstimateModel.Get(model: estimateModel));
        }

        #endregion

        [HttpGet]
        [Transaction]
        public ActionResult View(long? id)
        {
            var currentUser = this.userStorage.Get();
            var estimate = this.estimatesRepository.Get(Convert.ToInt64(id));
            estimate.New = false;
            this.estimatesRepository.Save(estimate);
            var customers = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
                               ? ((TeamEmployee)currentUser).Teams.SelectMany(x => x.Customers).Distinct()
                               .Select(x => x.ToPersist<WholesaleCustomer>()).Where(x => x != null).Where(x => x.Status == Statuses.Active).ToList()
                               : this.wholesaleCustomersRepository.Where(x => x.Status == Statuses.Active).ToList();
            var affilates = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
                    ? ((TeamEmployee)currentUser).Teams.Where(x => x.Status == Statuses.Active).SelectMany(x => x.Customers).Distinct()
                    .Select(x => x.ToPersist<Affiliate>()).Where(x => x != null).Where(x => x.Status == Statuses.Active).ToList()
                    : this.affiliatesRepository.Where(x => x.Status == Statuses.Active).ToList();
            if (estimate.Affiliate != null && !affilates.Contains(estimate.Affiliate))
            {
                affilates.Add(estimate.Affiliate);
            }

            var model = EstimateModel.Get(estimate);
            if (estimate.Customer.CustomerType == CustomerType.Retail)
            {
                var matrices = customers.Count > 0 ? customers.First().Matrices.Where(x => x.Status == Statuses.Active || x.Id == estimate.Matrix.Id).ToList() : new List<PriceMatrix>();
                model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, matrices);
                model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates, estimate.Affiliate);
            }
            else
            {
                var customer = customers.SingleOrDefault(x => x.Id == estimate.Customer.Id);
                if (customer == null)
                {
                    customer = this.wholesaleCustomersRepository.Get(estimate.Customer.Id);
                    customers.Add(customer);
                }

                var matrices = customers.Count > 0 ? estimate.Customer.ToPersist<WholesaleCustomer>().Matrices.Where(x => x.Status == Statuses.Active || x.Id == estimate.Matrix.Id).ToList() : new List<PriceMatrix>();
                model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, matrices);
                model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates, estimate.Affiliate);
            }


            this.InitViewBags(Convert.ToInt64(id), model, estimate, "view");
            this.ViewBag.IsAdmin = this.userStorage.Get() is Domain.Model.Users.Admin;
            CommonLogger.View(estimate);


            ViewBag.CurrentUserRole = currentUser.Role;

            return this.View(model);
        }

        [HttpGet]
        public ActionResult OnlyEstimate(long id, string htmlFieldPrefix = null, string viewName = null)
        {
            var estimate = this.estimatesRepository.Get(id);
            var model = EstimateModel.Get(estimate);

            this.ViewBag.Id = id;
            ViewBag.State = "view";
            ViewBag.Order = false;
            var view = this.View(viewName ?? "Partial/OnlyEstimate", model);
            view.ViewData.Add("RenderAction", true);
            if (htmlFieldPrefix != null)
            {
                view.ViewData.TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            return view;
        }

        [HttpPost]
        public JsonResult GetVINDecode(string vincode)
        {
            CheckerVinModel checkerVinModel = new CheckerVinModel();
            var model = checkerVinModel.GetVinModel(vincode);
            model.VinInfo = new VINDecode().Decode(vincode);

            return this.Json(model);
        }

        #region Print PDF Document

        [Transaction]
        [HttpGet]
        public virtual FileContentResult PrintEstimates(string id, bool? isBasic)
        {
            var currentUser = this.userStorage.Get();
            bool isDetailed = !(isBasic ?? currentUser.IsBasic);
            var estimate = this.estimatesRepository.Fetch(x => x.Customer).Fetch(x => x.Matrix).Single(x => x.Id == Convert.ToInt64(id));
            var pdf = this.ConvertToPdf(estimate,isDetailed);
            this.logger.Log(estimate, EstimateLogActions.Print);

            return new FileContentResult(pdf, "application/pdf");
        }

        //[HttpGet]
        //public FileContentResult PrintEstimateBatch(string ids, bool detailed = false)
        //{
        //    var estIds = ids.Split(',');
        //    var estimates =
        //        this.estimatesRepository.Fetch(x => x.Customer).Fetch(x => x.Matrix).Where(
        //            x => estIds.Contains(x.Id.ToString())).ToList();
            
        //    var pdf = this.ConvertToPdf(estimates, detailed);

        //    return new FileContentResult(pdf, "application/pdf");
        //}
        
        #endregion
        
        [Transaction]
        [HttpPost]
        public ActionResult ApproveEstimates(string ids)
        {
            var estimatesIds = ids.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var estimates = this.estimatesRepository.Where(x => estimatesIds.Contains(x.Id)).ToList();
            var flag = false;
            foreach (var estimate in estimates)
            {
                if (estimate.EstimateStatus != EstimateStatus.Completed)
                {
                    flag = true;
                    continue;
                }

                estimate.EstimateStatus = EstimateStatus.Approved;
                this.estimatesRepository.Save(estimate);
                this.logger.Log(estimate, EstimateLogActions.Approve);
            }

            return this.Json(flag);
        }

        #endregion

        #region Ajax Requests For CarInspections

        public JsonResult CheckActiveMatrix()
        {
            var matrix = this.repositoryFactory.CreateForCompany<DefaultMatrix>().SingleOrDefault(x => x.Status == Statuses.Active);

            return Json(matrix == null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDentsCost(string totalDents, string averageSize, string part, long matrixId)
        {
            if (totalDents != null && averageSize != null)
            {
                var data = this.repositoryFactory.CreateNative<MatrixPrice>()
                    .Where(x => x.Matrix.Id == matrixId &&
                                x.PartOfBody == (PartOfBody)int.Parse(part) &&
                                x.AverageSize == (AverageSize)int.Parse(averageSize) &&
                                x.TotalDents == (TotalDents)int.Parse(totalDents)).SingleOrDefault();
                return Json(data == null ? 0 : data.Cost, JsonRequestBehavior.AllowGet);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult GetEffortHours(int year, string model, string make, string id = null)
        {
            var carRepo = ServiceLocator.Current.GetInstance<ICompanyRepository<CarModel>>();
            var cars = ServiceLocator.Current.GetInstance<ICompanyRepository<Car>>();
            Car currentcar = null;
            if(id != null)
            {
                currentcar = cars.SingleOrDefault(x => x.Id == Convert.ToInt64(id));
            }
            var defaultCar = this.repositoryFactory
                                    .CreateForCompany<CarModel>()
                                    .SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            var car = carRepo.SingleOrDefault(x => x.Make.ToLower() == make.ToLower()
                                                && x.Model.ToLower() == model.ToLower()
                                                && x.YearFrom <= year
                                                && x.YearTo >= year)
                                                ?? defaultCar;

            return Json(
                car != null ? new 
                {
                    Data = EffortsDataModel.GetSectionEffortItemViewModels(car, defaultCar),
                    CarMake = car.Make,
                    CarType = currentcar != null ? currentcar.Type.ToString() : car.Type.ToString()
                }
                : null,
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWholesaleCustomerData(long? matrix, long? customer)
        {
            if(matrix != 0 && customer != 0)
            {
                if (customer.HasValue)
                {
                    var wholesaleCustomer = this.wholesaleCustomersRepository.Get(Convert.ToInt64(customer));
                    var matrixprice = this.repositoryFactory.CreateForCompany<Matrix>().Get(Convert.ToInt64(matrix)) ??
                                      this.repositoryFactory.CreateForCompany<DefaultMatrix>().First();

                    return Json(
                        new
                            {
                                wholesaleCustomer.LaborRate,
                                wholesaleCustomer.Discount,
                                MaxCorProtect = matrixprice.MaxCorrosionProtection,
                                wholesaleCustomer.HourlyRate,
                                Aluminium = matrixprice.AluminiumPanel,
                                DoubleMetall = matrixprice.DoubleLayeredPanels,
                                matrixprice.OversizedRoof,
                                matrixprice.OversizedDents,
                                CorProtectPart = matrixprice.CorrosionProtectionPart,
                                matrixprice.Maximum,
                                WorkByThemselve = !wholesaleCustomer.WorkByThemselve,
                                HasEstimateSignature = wholesaleCustomer.EstimateSignature
                            },
                        JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWholesaleMatrices(long? customer)
        {
            var wholesaleCustomer = this.wholesaleCustomersRepository.Get(Convert.ToInt64(customer));
            var list = wholesaleCustomer.Matrices.Where(x => x.Status == Statuses.Active).Select(x => new { Value = x.Id, Text = x.Name });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Transaction]
        public JsonResult GetAffiliateData(long? affiliateId)
        {
            if (affiliateId != 0 && affiliateId.HasValue)
            {
                var affiliate = this.affiliatesRepository.Get(Convert.ToInt64(affiliateId));
                var matrixprice = this.repositoryFactory.CreateForCompany<DefaultMatrix>().First();

                return Json(
                        new
                        {
                            affiliate.LaborRate,
                            affiliate.HourlyRate,
                            Discount = 0,
                            MaxCorProtect = matrixprice.MaxCorrosionProtection,
                            Aluminium = matrixprice.AluminiumPanel,
                            DoubleMetall = matrixprice.DoubleLayeredPanels,
                            matrixprice.OversizedRoof,
                            matrixprice.OversizedDents,
                            CorProtectPart = matrixprice.CorrosionProtectionPart,
                            matrixprice.Maximum,
                            WorkByThemselve = true,
                            HasEstimateSignature = true

                        },
                        JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Auxillary
        
        public byte[] ReadFile(string file)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (var bynaryReader = new BinaryReader(stream))
                    {
                        return bynaryReader.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch
            {
                return new byte[] { 0 };
            }
        }

        private void ConvertToRepairOrder(long? managerId, string estimatesIds)
        {
            var teamEmployee = managerId.HasValue
              ? this.repositoryFactory.CreateForCompany<TeamEmployee>().Get(managerId.Value)
              : this.userStorage.Get() as TeamEmployee;

            var estimateIdsList = estimatesIds.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var allEstimates = this.estimatesRepository.Where(x => estimateIdsList.Contains(x.Id)).ToList();

            foreach (var estimate in allEstimates)
            {
                this.reassignHelper.ConvertToRepairOrder(estimate, teamEmployee, this.userStorage.Get());
            }
        }

        protected string CheckWholesaleCustomerWithInsurance()
        {
            var wholesaleCustomers = this.wholesaleCustomersRepository.Where(x => x.Status == Statuses.Active).ToList();
            var list = wholesaleCustomers.Where(wholesaleCustomer => wholesaleCustomer.Insurance).Select(wholesaleCustomer => wholesaleCustomer.Id.ToString()).ToList();

            return string.Join(",", list);
        }

        private byte[] ConvertToPdf(Estimate estimate, bool isDetailed)
        {
            if (estimate != null)
            {
                var file = this.pdfConverter.Convert(estimate, isDetailed);
                return file;
            }

            return null;
        }

        private byte[] ConvertToPdf(List<Estimate> estimates, bool detailed)
        {
            var file = this.pdfConverter.ConvertEstimates(estimates, detailed);
                return file;
        }

        protected virtual void Save(Estimate estimate, EstimateModel estimateModel)
        {
            var resolver = new SaveEstimateResolver(this.repositoryFactory, this.tempImageManager, this.userStorage);
            resolver.Save(estimate, estimateModel);
            this.estimatesRepository.Save(estimate);
        }
        
        private bool CheckEstimatesForTheSameCustomer(IList<string> ids)
        {
            var first = this.estimatesRepository.Get(Convert.ToInt64(ids[0])).Customer.Id;
            return ids.Any(id => this.estimatesRepository.Get(Convert.ToInt64(id)).Customer.Id != first);
        }

        #endregion

        #region Init ViewBag by Estimate

        protected virtual void InitViewBags(long? id, EstimateModel model, Estimate estimate, string state, long? firstInlistAffiliateId = null)
        {
            var company = this.userStorage.Get().Company;
            var matrix = this.repositoryFactory.CreateForCompany<Matrix>().SingleOrDefault(x => x is DefaultMatrix && x.Status == Statuses.Active && x.Company.Id == company.Id);
            var affiliate = firstInlistAffiliateId.HasValue ? affiliatesRepository.FirstOrDefault(x => x.Id == firstInlistAffiliateId) : null;

            ViewBag.Id = id;
            ViewBag.CarInspectionsModel = model.CarInspectionsModel;
            ViewBag.LimitForBodyPart = company.LimitForBodyPartEstimate;
            ViewBag.DefaultLimit = company.LimitForBodyPartEstimate;
            ViewBag.DefaultHourlyRate = affiliate != null ? affiliate.HourlyRate : 0;
            ViewBag.DefaultCar = true;
            ViewBag.HasSignature = true;
            ViewBag.LaborRate = affiliate != null ? affiliate.LaborRate : 0;
            ViewBag.Discount = 0;
            ViewBag.WCustomersWithInsurance = this.CheckWholesaleCustomerWithInsurance();
            ViewBag.CurrentAffiliate = firstInlistAffiliateId;
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

            if (estimate != null)
            {
                if (estimate.Customer.CustomerType == CustomerType.Wholesale)
                {
                    this.InitWholesaleCustomerData(estimate);
                }
                else
                {
                    this.InitRetailCustomerData(estimate);
                }
            }
            else
            {
                ViewBag.HasInsurance = true;
                ViewBag.WorkByThemselve = true;
            }

            ViewBag.State = state;
            ViewBag.Order = false;
        }

        private void InitRetailCustomerData(Estimate estimate)
        {
            ViewBag.DefaultLimit = estimate.EstLimitForBodyPart;
            ViewBag.DefaultHourlyRate = estimate.EstHourlyRate;
            ViewBag.LaborRate = estimate.EstLaborTax;
            ViewBag.DefaultMaxCorProtect = estimate.EstMaxCorProtect;
            ViewBag.DefaultAluminium = estimate.EstAluminiumPer;
            ViewBag.DefaultDoubleMetall = estimate.EstDoubleMetalPer;
            ViewBag.DefaultOversizedRoof = estimate.EstOversizedRoofPer;
            ViewBag.DefaultOversizedDents = estimate.EstOversizedDents;
            ViewBag.DefaultCorProtectPart = estimate.EstCorProtectPart;
            ViewBag.DefaultMaxPercentPart = estimate.EstMaxPercent;
            ViewBag.HasInsurance = true;
            ViewBag.WorkByThemselve = true;
            ViewBag.HasSignature = true;
        }

        private void InitWholesaleCustomerData(Estimate estimate)
        {
            var customer = this.repositoryFactory.CreateForCompany<WholesaleCustomer>().Get(estimate.Customer.Id);
            ViewBag.WholesaleCustomer = customer.Id;

            if (estimate.Matrix != null)
            {
                ViewBag.Matrix = estimate.Matrix.Id;
            }

            ViewBag.MaxCorProtect = estimate.EstMaxCorProtect;
            ViewBag.LaborRate = estimate.EstLaborTax;
            ViewBag.Discount = estimate.EstDiscount;
            ViewBag.HourlyRate = estimate.EstHourlyRate;
            ViewBag.Aluminium = estimate.EstAluminiumPer;
            ViewBag.DoubleMetall = estimate.EstDoubleMetalPer;
            ViewBag.OversizedRoof = estimate.EstOversizedRoofPer;
            ViewBag.OversizedDents = estimate.EstOversizedDents;
            ViewBag.CorProtectPart = estimate.EstCorProtectPart;
            ViewBag.Maximum = estimate.EstMaxPercent;
            ViewBag.LimitForBodyPart = estimate.EstLimitForBodyPart;
            ViewBag.HasInsurance = customer.Insurance;
            ViewBag.WorkByThemselve = !customer.WorkByThemselve;
            ViewBag.HasSignature = customer.EstimateSignature;
        }

        #endregion
    }
}
