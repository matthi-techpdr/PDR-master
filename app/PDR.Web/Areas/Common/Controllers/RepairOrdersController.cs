using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Automapper;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.PushNotification;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.StoredProcedureHelpers;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Areas.Technician.Models;
using PDR.Web.Areas.Technician.Models.RepairOrders;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;
using SmartArch.Core.Helpers;
using SmartArch.Data;
using SmartArch.Data.Fetching;
using SmartArch.Data.Proxy;
using SmartArch.Web.Attributes;
using SmartArch.Web.Helpers;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Common.Controllers
{
    [PDRAuthorize]
    public class RepairOrdersController : Controller
    {
        #region Private and protected fields

        protected readonly IGridMasterForStoredProcedure<RepairOrder, RepairOrderJsonModel, RepairOrderViewModel> repairOrderGridMaster;

        protected readonly IRepositoryFactory repositoryFactory;

        protected readonly ICompanyRepository<Domain.Model.Users.Manager> managers;

        protected readonly ICompanyRepository<Customer> customersRepository;

        protected readonly ICompanyRepository<Car> carRepository;

        protected readonly ICompanyRepository<Estimate> estimates;

        protected readonly ICompanyRepository<Insurance> insuranceRepository;

        protected readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        protected readonly ICompanyRepository<Invoice> invoicesRepository;

        protected readonly ITempImageManager tempImageManager;

        protected readonly ICurrentWebStorage<Employee> userStorage;

        protected readonly Employee currentTeamEmployee;

        protected readonly ICompanyRepository<Team> teamsRepository; 

        protected readonly ILogger logger;

        protected readonly IPdfConverter pdfConverter;

        private readonly IPushNotification push;

        private readonly ReassignHelper reassignHelper;

        protected readonly bool withEmployeeName;

        #endregion

        #region Constructor

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
        {
            this.currentTeamEmployee = userStorage.Get();
            this.repositoryFactory = repositoryFactory;
            this.estimates = estimates;
            this.repairOrderGridMaster = repairOrderGridMaster;
            this.userStorage = userStorage;
            this.customersRepository = customersRepository;
            this.insuranceRepository = insuranceRepository;
            this.carRepository = carRepository;
            this.tempImageManager = tempImageManager;
            this.managers = managers;
            this.repairOrdersRepository = repairOrdersRepository;
            this.invoicesRepository = invoicesRepository;
            this.teamsRepository = teamsRepository;
            this.logger = logger;
            this.pdfConverter = pdfConverter;
            this.push = ServiceLocator.Current.GetInstance<IPushNotification>();
            this.reassignHelper = ServiceLocator.Current.GetInstance<ReassignHelper>();
            this.withEmployeeName = true;
        }

        #endregion

        [ValidateInput(false)]
        public virtual ActionResult Index(string vin = null, string stock = null, string custRo = null, bool withFilters = true)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            FilterModel filters = null;
            if (withFilters)
            {
                var teamCookie = HttpContext.Request.Cookies.Get("team");

                filters = new FilterModel(typeof(RepairOrder), false, false, teamCookie);
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
        public virtual ActionResult Finalised(string vin = null, string stock = null, string custRo = null)
        {
            //var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            this.ViewBag.currenUserRole = userStorage.Get().Role.ToString();

            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }

            ViewBag.IsRITechnician = this.userStorage.Get() is Domain.Model.Users.RITechnician;

            return this.View(new FilterModel(typeof(RepairOrder), true, false, teamCookie));
        }

        [ValidateInput(false)]
        public virtual ActionResult GetRepairOrdersData(string sidx, string sord, int page, int rows, long? customer, long? team, bool finalised,
            bool? isStartSearch = null, string vin = null, string stock = null, string custRo = null, bool isNeedFilter = false)
        {
            var onlyOwn = team == 0;

            var currentUser = this.userStorage.Get();
            var customerCookie = HttpContext.Request.Cookies.Get("customer");

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentUser.Id, team, customer, onlyOwn, rows, page, sidx, sord, finalised,
                                    null, vin, stock, custRo, false, isForCustomerFilter: isNeedFilter);
            var data = this.repairOrderGridMaster.GetData(repairOrdersSpHelper, rows, page, isCustomerFilter: isNeedFilter, customerCookie: customerCookie);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHistory(string id)
        {
            var ro = repairOrdersRepository.Get(Convert.ToInt64(id));
            var model = new List<string>();
            var historyManager = new DocumentHistoryManager();
            model = historyManager.GenerateHistory(ro, withEmployeeName).ToList();
            
            return this.PartialView(model);
        }


        [Transaction]
        public ActionResult MarkAsCompleted(long[] ids)
        {
            var repairOrders = this.repairOrdersRepository.Where(x => ids.Contains(x.Id)).ToList();
            var errorIdsList = new List<string>(); 
            foreach (var repairOrder in repairOrders)
            {
                if (repairOrder.RepairOrderStatus != RepairOrderStatuses.Open)
                {
                    errorIdsList.Add(repairOrder.Id.ToString());
                }
                else
                {
                    this.logger.Log(repairOrder, RepairOrderLogActions.Complete);
                    repairOrder.RepairOrderStatus = RepairOrderStatuses.Completed;
                    repairOrder.IsConfirmed = true;
                    if (repairOrder.TeamEmployeePercents.Count == 0)
                    {
                        repairOrder.TeamEmployeePercents.Add(new TeamEmployeePercent(true)
                                                                 {
                                                                     TeamEmployee = repairOrder.TeamEmployee,
                                                                     EmployeePart = 100,
                                                                     RepairOrder = repairOrder
                                                                 });
                    }

                    this.repairOrdersRepository.Save(repairOrder);
                }
            }

            return this.Content(!errorIdsList.Any()
                ? "Success"
                : string.Format("Repair order(s) {0} is not open.", string.Join(",", errorIdsList)));
        }

        [Transaction]
        public ActionResult View(long id)
        {
            var repairOrder = this.repairOrdersRepository.Get(id);
            this.MarkAsOld(repairOrder);
            var customerResolver = new CustomersResolver(this.repositoryFactory, this.userStorage);
            var model = new RepairOrderModel(repairOrder)
                            {
                                EstimateModel = customerResolver.CustomerResolve(repairOrder.Estimate)
                            };
            
            CustomAutomapper.Map(repairOrder, model);
            
            model.GrandTotal = "$" + Math.Round(Convert.ToDouble(model.GrandTotal), 2, MidpointRounding.AwayFromZero).ToString("#.00");
            model.AdditionalDiscount = Math.Round(repairOrder.AdditionalDiscount, 2, MidpointRounding.AwayFromZero).ToString();
            Model = model;

            this.InitViewBags(id, model, repairOrder, "view");
            ViewBag.IsAdmin = this.userStorage.Get() is Domain.Model.Users.Admin;
            ViewBag.IsRITechnician = this.userStorage.Get() is Domain.Model.Users.RITechnician;
            CommonLogger.View(repairOrder);

            ViewBag.CurrentUserRole = this.userStorage.Get().Role;
            
            return this.View(model);
        }

        #region Edit

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var repairOrder = this.repairOrdersRepository.Get(id);
            var customerResolver = new CustomersResolver(this.repositoryFactory, this.userStorage);
            var model = new RepairOrderModel(repairOrder)
                            {
                                EstimateModel = customerResolver.CustomerResolve(repairOrder.Estimate)
                            };
            CustomAutomapper.Map(repairOrder, model);
            model.GrandTotal = "$" + Math.Round(Convert.ToDouble(model.GrandTotal), 2, MidpointRounding.AwayFromZero).ToString("#.00");
            model.AdditionalDiscount = Math.Round(repairOrder.AdditionalDiscount, 2, MidpointRounding.AwayFromZero).ToString();
            Model = model;
            IsEdit = true;
            ViewBag.IsRITechnician = this.userStorage.Get() is Domain.Model.Users.RITechnician;
            
            this.InitViewBags(id, model, repairOrder, "view");
            
            return this.View(model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(RepairOrderModel model)
        {
            var repairOrder = this.repairOrdersRepository.Get(model.Id);
            foreach (var item in ModelState.GetInvalidItems())
            {
                var tmp = Reflector.Property<RepairOrderModel>(x => x.EstimateModel).Name;
                if (item.Key.Contains(tmp))
                {
                    item.Value.Errors.Clear();
                }
            }

            if (!ModelState.IsValid)
            {
                model.EstimateModel = EstimateModel.Get(model: model.EstimateModel);
                Model = model;
                IsEdit = true;
                ViewBag.IsRITechnician = this.userStorage.Get() is Domain.Model.Users.RITechnician;

                this.InitViewBags(null, model, repairOrder, "view");

                return this.View(model);
            }

            double newAdditionalDiscount;
            if (Double.TryParse(model.AdditionalDiscount, out newAdditionalDiscount) && repairOrder.AdditionalDiscount != newAdditionalDiscount)
            {
                logger.LogChangedAdditionalDiscountValue(repairOrder, repairOrder.AdditionalDiscount, Double.Parse(model.AdditionalDiscount));
            }

            CustomAutomapper.Map(model, repairOrder);
            repairOrder.RepairOrderStatus = model.Statuses;
                
            if (model.SupplementsApproved)
            {
                repairOrder.IsConfirmed = true;
            }

            // save suplements
            var supplements = repairOrder.Supplements.ToList();
            var supplementsTemp = new HashedSet<Supplement>();
            var supplementRemove = new List<Supplement>();
                
            if (!string.IsNullOrWhiteSpace(model.BaseDescription))
            {
                model.SupplementModels.Add(new SupplementModel { Description = model.BaseDescription, Sum = model.BaseSum });
            }

            foreach (var supplementModel in model.SupplementModels)
            {
                if (string.IsNullOrWhiteSpace(supplementModel.Description))
                {
                    continue;
                }

                var supplement = supplements.Count == 0 ? new Supplement(true) : supplements.SingleOrDefault(x => x.Id == supplementModel.Id) ?? new Supplement(true);
                CustomAutomapper.Map(supplementModel, supplement);
                supplement.Description = supplement.Description.Replace("&lt;", "<");
                supplement.RepairOrder = repairOrder;
                repairOrder.Supplements.Add(supplement);
                supplementsTemp.Add(supplement);
            }

            supplements.AddRange(supplementsTemp);

            if (supplementsTemp.Count != 0)
            {
                supplementRemove.AddRange(supplements.Where(x => !supplementsTemp.Contains(x)));
            }
            else
            {
                supplementRemove = supplements;
            }

            // remove old supplements
            foreach (var supplement in supplementRemove)
            {
                repairOrder.Supplements.Remove(supplement);
                this.repositoryFactory.CreateForCompany<Supplement>().Remove(supplement);
            }

            var storedPhotosIds = model.StoredPhotos.Select(x => long.Parse(x.Id));
            var removedPhotos = repairOrder.AdditionalPhotos.Where(x => !storedPhotosIds.Contains(x.Id)).ToList();
            foreach (var removedPhoto in removedPhotos)
            {
                removedPhoto.RepairOrder = null;
                repairOrder.AdditionalPhotos.Remove(removedPhoto);
                this.repositoryFactory.CreateForCompany<Photo>().Remove(removedPhoto);
            }

            // save upload photos
            foreach (var imageInfo in model.UploadPhotos)
            {
                var imgInfo = this.tempImageManager.Get(imageInfo.Id);
                var photo = new AdditionalCarPhoto(this.Server.MapPath(imgInfo.FullSizeUrl), this.Server.MapPath(imgInfo.ThumbnailUrl), imgInfo.ContentType, true) { RepairOrder = repairOrder };
                repairOrder.AdditionalPhotos.Add(photo);
                this.tempImageManager.Remove(imageInfo.Id);
            }
            if (repairOrder.Estimate != null)
            {
                var oldLaborRateValue = repairOrder.Estimate.NewLaborRate ?? repairOrder.Estimate.CurrentHourlyRate;
                var newLaborRateValue = model.NewHourlyRate ?? repairOrder.Estimate.CurrentHourlyRate;
                if (oldLaborRateValue != newLaborRateValue)
                {
                    logger.LogChangedLaborRateValue(repairOrder, oldLaborRateValue, newLaborRateValue);
                }
                repairOrder.Estimate.NewLaborRate = model.NewHourlyRate;
                repairOrder.Estimate.TaxSum = repairOrder.Estimate.GetTaxSum();
                repairOrder.Estimate.LaborSum = repairOrder.Estimate.GetLaborSum();
                repairOrder.Estimate.TotalAmount = repairOrder.Estimate.CalculateTotalAmount();
            }

            this.repairOrdersRepository.Save(repairOrder);

            return this.RedirectToAction("Index");
        }

        #endregion

        [HttpGet]
        [Transaction]
        public JsonResult SetApprovedStatus(long id)
        {
            if (this.userStorage.Get() is Domain.Model.Users.Wholesaler)
            {
                return this.Json(JsonRequestBehavior.DenyGet);
            }
            var repairOrder = this.repairOrdersRepository.Get(id);
            repairOrder.RepairOrderStatus = RepairOrderStatuses.Approved;
            repairOrder.SupplementsApproved = true;
            repairOrder.IsConfirmed = true;
            this.logger.Log(repairOrder, RepairOrderLogActions.Approve);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Transaction]
        public ContentResult Approve(long id)
        {
            if (this.userStorage.Get() is Domain.Model.Users.Wholesaler)
            {
                return this.Content("Failed");
            }
            var ro = this.repairOrdersRepository.Get(id);
            
            if (ro == null)
            {
                return this.Content("Repair order can not be found.");
            }

            if (ro.RepairOrderStatus != RepairOrderStatuses.Completed)
            {
                return this.Content("Repair order is not completed.");
            }

            ro.RepairOrderStatus = RepairOrderStatuses.Approved;
            ro.SupplementsApproved = true;
            this.logger.Log(ro, RepairOrderLogActions.Approve);
            this.repairOrdersRepository.Save(ro);
            
            return this.Content("Success");
        }
        
        [HttpGet]
        [Transaction]
        public virtual ActionResult Print(string id, bool? isBasic)
        {
            var currentUser = this.userStorage.Get();
            bool isDetailed = !(isBasic ?? currentUser.IsBasic);
            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(id));
            var pdf = this.ConvertToPdf(repairOrder, isDetailed);
            this.logger.Log(repairOrder, RepairOrderLogActions.Print);
            return new FileContentResult(pdf, "application/pdf");
        }

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            var subject = this.userStorage.Get().Company.RepairOrdersEmailSubject;

            if (customer)
            {
                to = this.repositoryFactory.CreateForCompany<Customer>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }
            else
            {
                var repairOrderIds = ids.Split(',');

                if (this.CheckRepairOrderForTheSameCustomer(repairOrderIds) && repairOrderIds.Length > 1)
                {
                    return new JsonResult { Data = "Error" };
                }

                foreach (var id in repairOrderIds)
                {
                    var estimate = this.repairOrdersRepository.Get(Convert.ToInt64(id)).Estimate;
                    var email = estimate.Customer.Email ??
                                string.Empty;
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        to += email + ", ";
                    }
                    if (!String.IsNullOrEmpty(estimate.Car.Stock))
                    {
                        subject += " " + estimate.Car.Stock + ",";
                    }
                        else if(!String.IsNullOrEmpty(estimate.Car.CustRO))
                        {
                            subject += " " + estimate.Car.CustRO + ",";
                        }
                            else if(!String.IsNullOrEmpty(estimate.Car.Make) && !String.IsNullOrEmpty(estimate.Car.Model))
                            {
                                subject += " " + estimate.Car.Year + "/" + estimate.Car.Make + "/" + estimate.Car.Model + ",";
                            }
                }

                to = string.Join(", ", to.Split(new[] { ',', ' ' }).Distinct());
            }

            var idWh = ids.Split(',')[0];
            var wholesale = this.repairOrdersRepository.Get(Convert.ToInt64(idWh)).Customer;
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

            return this.PartialView("GetEmailDialog", result);
        }

        [HttpPost]
        [Transaction]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }

            var ordersIds = model.IDs.Split(',');
            IList<Attachment> attachments = new List<Attachment>();
            var from = this.currentTeamEmployee.Company.Email;
            
            foreach (var id in ordersIds)
            {
                var order = this.repairOrdersRepository.Get(Convert.ToInt64(id));
                
                if (order == null)
                {
                    continue;
                }

                var pdf = this.ConvertToPdf(order, !model.IsBasic);
                var fileName = string.Format("Repair Order #{0}_{1}.pdf", order.Id, order.CreationDate.ToString("MM-dd-yyyy"));
                attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                this.logger.Log(order, RepairOrderLogActions.Email, model.To);
            }

            var to = model.To +
                (String.IsNullOrEmpty(model.To2) ? "" : "," + model.To2) +
                (String.IsNullOrEmpty(model.To3) ? "" : "," + model.To3) +
                (String.IsNullOrEmpty(model.To4) ? "" : "," + model.To4);

            var mes = new MailService().Send(@from, to, model.Subject, model.Message, attachments.Count > 0 ? attachments : null);
                
            return this.Json(mes, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult GenerateInvoice(long[] ids)
        {
            var repairOrders = this.repairOrdersRepository.Where(x => ids.Contains(x.Id)).Fetch(x => x.Customer).ToList();
            var correctCustomers = false;
            var firstCustomer = repairOrders.First().Customer;
            
            if (firstCustomer is WholesaleCustomer)
            {
              correctCustomers = repairOrders.All(x => x.Customer == firstCustomer);
            }
            else if (firstCustomer is RetailCustomer)
            {
                correctCustomers = repairOrders.All(x => x.Customer is RetailCustomer);
            }

            if (!correctCustomers)
            {
                return this.Content("Error");
            }

            var employee = (TeamEmployee)this.currentTeamEmployee;
            var teams = employee.Teams.SelectMany(x => x.Employees).ToList();
            var manager = teams.FirstOrDefault(x => x.Role == UserRoles.Manager);

            foreach (var repairOrder in repairOrders.Where(x => !x.IsInvoice))
            {
                //string notificationMsg;
                InvoiceLogActions action;
                
                var invoice = this.invoicesRepository.SingleOrDefault(x => x.RepairOrder.Id == repairOrder.Id);
                
                if (invoice == null)
                {
                    invoice = new Invoice(repairOrder, employee, true);
                    action = InvoiceLogActions.Generate;
                    //notificationMsg = NotificationMessages.GeneratedInvoice;
                }
                else
                {
                    repairOrder.IsInvoice = true;
                    repairOrder.IsConfirmed = true;
                    repairOrder.RepairOrderStatus = RepairOrderStatuses.Finalised;
                    repairOrder.EditedStatus = EditedStatuses.EditingReject;

                    repairOrder.New = repairOrder.TeamEmployeePercents.Count > 1;

                    invoice.New = true;
                    invoice.IsDiscard = false;
                    invoice.IsImported = false;
                    invoice.Archived = false;
                    invoice.CreationDate = DateTime.Now;
                    invoice.Status = InvoiceStatus.Unpaid;
                    invoice.PaidDate = new DateTime(1753, 1, 1);
                    invoice.TeamEmployee = employee;
                    invoice.Customer = repairOrder.With(x => x.Estimate).With(x => x.Customer);
                    invoice.InvoiceSum = Math.Round(repairOrder.TotalAmount, 2);
                    action = InvoiceLogActions.ReGenerate;
                    //notificationMsg = NotificationMessages.ReGeneratedInvoice;
                }

                this.invoicesRepository.Save(invoice);
                this.logger.Log(invoice, action);
                //var message = string.Format(notificationMsg, invoice.Id);
                    
                if (manager == null)
                {
                    continue;
                }
                    
                //if ((this.currentTeamEmployee.Id != manager.Id && repairOrder.TeamEmployeePercents.Count > 1))
                //{
                //    this.push.Send(manager, message, "RO", new object[] { repairOrder.Id });
                //}
            }

            return new HttpStatusCodeResult(200);
        }

        public bool CheckCustomersType(long[] ids)
        {
            var repairOrders = this.repairOrdersRepository.Where(x => ids.Contains(x.Id) && !x.IsInvoice).Fetch(x => x.Customer).ToList();
            var correctCustomers = true;
            var firstCustomer = repairOrders.First().Customer;

            if (firstCustomer is WholesaleCustomer)
            {
                correctCustomers = repairOrders.All(x => x.Customer == firstCustomer);
            }
            else if (firstCustomer is RetailCustomer)
            {
                correctCustomers = repairOrders.All(x => x.Customer is RetailCustomer);
            }

            return correctCustomers;
        }

        [Transaction]
        [HttpPost]
        public bool Complete(long[] ids)
        {
            return this.ChangeStatus(ids, RepairOrderStatuses.Completed);
        }

        [HttpPost]
        [Transaction]
        public bool ApproveMany(long[] ids)
        {
            return this.ChangeStatus(ids, RepairOrderStatuses.Approved);
        }

        [HttpPost]
        [Transaction]
        public JsonResult AssignMoreTechnicians(string technicianIds, long repairOrderId, string riOperations)
        {
            bool isRIPaymentChanged = false;
            var teamEmployees = this.repositoryFactory.CreateForCompany<TeamEmployee>();
            var longIds = technicianIds.Split(',').Select(x => x.Split(':')[0]).Select(x => Convert.ToInt64(x)).ToList();
            var technicians = teamEmployees.Where(x => longIds.Contains(x.Id));
            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(repairOrderId));
            var teamemployees = repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee).ToList();

            var addemp = technicians.Where(technician => !teamemployees.Contains(technician));

            if (!string.IsNullOrEmpty(riOperations) && riOperations.Split(':')[0] == "RiOperations")
            {
                if (repairOrder.IsFlatFee == true)
                {
                    isRIPaymentChanged = true;
                }
                repairOrder.IsFlatFee = false;
                repairOrder.Payment = 0;
            }
            else if (!string.IsNullOrEmpty(riOperations) && riOperations.Split(':')[0] == "FlatFee")
            {
                try
                {
                    var sum = Convert.ToDouble(riOperations.Split(':')[1]);
                    if (repairOrder.IsFlatFee == false || repairOrder.Payment!=sum)
                    {
                        isRIPaymentChanged = true;
                    }
                    repairOrder.Payment = sum;
                }
                catch (Exception)
                {
                    repairOrder.Payment = 0;
                }
                repairOrder.IsFlatFee = true;
            }

            if (addemp.Any())
            {
                this.reassignHelper.AddEmployeesToRepairOrder(repairOrder, technicians, this.userStorage.Get());
            }
            else if (isRIPaymentChanged)
            {
                this.logger.LogAssignToRo(repairOrder);
            }

            var roEmployees = repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee);
            var removeEmployees = roEmployees.Except(technicians).ToList();
            this.reassignHelper.RemoveEmployeesFromRepairOrder(repairOrder, removeEmployees);

            return this.Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DefineParticipation(long? ids)
        {
            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(ids));

            var model = new DefineParticipationViewModel
            {
                RepairOrderId = repairOrder.Id,
                TeamEmployeePercents = repairOrder.TeamEmployeePercents.Where(x => x.TeamEmployee.Role != UserRoles.RITechnician ).ToList()
            };

            return this.PartialView(model);
        }

        [Transaction]
        [HttpPost]
        public JsonResult DefineParticipation(string technicianIds, long repairOrderId, string technicianPercents)
        {
            var technicians = technicianIds.Split(',').Select(x => Convert.ToInt64(x)).ToList();

            var repairOrder = this.repairOrdersRepository.Get(Convert.ToInt64(repairOrderId));

            var percents = technicianPercents.Split(',').Select(Convert.ToDouble).ToList();

            foreach (var teamEmp in repairOrder.TeamEmployeePercents)
            {
                for (var i = 0; i < technicians.Count; i++)
                {
                    if (teamEmp.Id != technicians[i])
                    {
                        continue;
                    }

                    teamEmp.EmployeePart = percents[i];
                    this.repositoryFactory.CreateForCompany<TeamEmployeePercent>().Save(teamEmp);
                }
            }

            repairOrder.New = false;
            this.repositoryFactory.CreateForCompany<RepairOrder>().Save(repairOrder);
            this.logger.LogDefinePercents(repairOrder);

            return this.Json("Success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpGet]
        public JsonResult AllowRejectEdit(string repairOrderIds, bool isAllow)
        {
            if (string.IsNullOrEmpty(repairOrderIds))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var orderIds = repairOrderIds.Split(',').Select(x => Convert.ToInt64(x)).ToArray();
            var orders = this.repairOrdersRepository.Where(x => orderIds.Contains(x.Id)).ToList();

            foreach (var repairOrder in orders)
            {
                repairOrder.EditedStatus = isAllow ? EditedStatuses.Editable : EditedStatuses.EditingReject;
                this.repairOrdersRepository.Save(repairOrder);
                if (repairOrder.EditedStatus == EditedStatuses.Editable)
                {
                    logger.LogEditApproval(repairOrder);
                }
            }

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public JsonResult Discard(string ids)
        {
            if(string.IsNullOrEmpty(ids))
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            try
            {
                foreach (var invoice in ids.Split(',')
                                           .Select(id => this.invoicesRepository.SingleOrDefault(x => x.RepairOrder.Id == Convert.ToInt64(id)))
                                           .Where(invoice => invoice != null))
                {
                    invoice.Archived = true;
                    invoice.IsDiscard = true;
                    var ro = invoice.RepairOrder;
                    ro.RepairOrderStatus = RepairOrderStatuses.Discard;
                    this.logger.Log(invoice, InvoiceLogActions.Discard);
                    this.logger.Log(ro, RepairOrderLogActions.Discard);
                    this.repairOrdersRepository.Save(ro);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        #region Request Storage
        private const string IS_EDIT_KEY = "Technician-RepairOrdersController-IsEdit";

        private const string MODEL_KEY = "Technician-RepairOrdersController-RepairOrdersModel";

        public static bool IsEdit
        {
            get
            {
                var value = System.Web.HttpContext.Current.Items[IS_EDIT_KEY];
                if (value is bool)
                {
                    return (bool)value;
                }

                return false;
            }

            set
            {
                System.Web.HttpContext.Current.Items[IS_EDIT_KEY] = value;
            }
        }

        public static RepairOrderModel Model
        {
            get
            {
                return System.Web.HttpContext.Current.Items[MODEL_KEY] as RepairOrderModel;
            }

            set
            {
                System.Web.HttpContext.Current.Items[MODEL_KEY] = value;
            }
        }
        #endregion        

        private void MarkAsOld(RepairOrder repairOrder)
        {
            repairOrder.New = false;
            this.repairOrdersRepository.Save(repairOrder);
        }

        private byte[] ConvertToPdf(RepairOrder ro, bool isDetailed)
        {
            return ro != null ? this.pdfConverter.ConvertRepairOrder(ro, isDetailed) : null;
        }

        private string CheckWholesaleCustomerWithInsurance()
        {
            var list = new List<string>();
            var wholesaleCustomers = this.repositoryFactory.CreateForCompany<WholesaleCustomer>().ToList();
            
            foreach (var wholesaleCustomer in wholesaleCustomers)
            {
                if (wholesaleCustomer.Insurance)
                {
                    list.Add(wholesaleCustomer.Id.ToString());
                }
            }

            return string.Join(",", list);
        }

        private bool ChangeStatus(IEnumerable<long> ids, RepairOrderStatuses status)
        {
            var ro = this.repairOrdersRepository.Where(x => ids.Contains(x.Id)).ToList();
            foreach (var r in ro)
            {
                if (status == RepairOrderStatuses.Approved)
                {
                    this.logger.Log(r, RepairOrderLogActions.Approve);
                    r.SupplementsApproved = true;
                }
                else
                {
                    this.logger.Log(r, RepairOrderLogActions.Complete);
                }

                r.RepairOrderStatus = status;
                this.repairOrdersRepository.Save(r);
                return true;
            }

            return false;
        }

        private bool CheckRepairOrderForTheSameCustomer(IList<string> ids)
        {
            var first = this.repairOrdersRepository.Get(Convert.ToInt64(ids[0])).Customer.Id;
            return ids.Any(id => this.repairOrdersRepository.Get(Convert.ToInt64(id)).Customer.Id != first);
        }

        protected AssignTechniciansViewModel GetRiOperations(RepairOrder repairOrder)
        {
            var model = new AssignTechniciansViewModel { RepairOrderId = repairOrder.Id };

            foreach (var emp in repairOrder.TeamEmployeePercents)
            {
                model.TeamEmployees.Add(emp.TeamEmployee);
                if (emp.TeamEmployee is Domain.Model.Users.RITechnician)
                {
                    model.RiTechnicianModel.IsVisible = true;
                    model.RiTechnicianModel.IsFlatFee = repairOrder.IsFlatFee;
                    model.RiTechnicianModel.PaymentFlatFee = repairOrder.Payment;
                }
                model.RiTechnicianModel.PaymentRiOperations =
                    repairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum()) <= 0
                        ? 0
                        : Math.Round(repairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum()) * (1 -repairOrder.Estimate.DocumentDiscount) , 2);
            }
            return model;
        }

        #region Init ViewBag

        private void InitViewBags(long? id, RepairOrderModel model, RepairOrder repairOrder, string state)
        {
            this.ViewBag.Id = id;
            var company = this.userStorage.Get().Company;
            ViewBag.CarInspectionsModel = model.EstimateModel.CarInspectionsModel;
            ViewBag.LimitForBodyPart = company.LimitForBodyPartEstimate;
            ViewBag.WCustomersWithInsurance = this.CheckWholesaleCustomerWithInsurance();
            ViewBag.DefaultLimit = company.LimitForBodyPartEstimate;
            ViewBag.DefaultHourlyRate = company.DefaultHourlyRate;
            ViewBag.DefaultCar = true;
            ViewBag.HasOrderSignature = true;
            ViewBag.LaborRate = 0;
            ViewBag.Discount = 0;
            ViewBag.NewHourlyRate = model.NewHourlyRate;
            ViewBag.CurrentHourlyRate = model.CurrentHourlyRate;
            ViewBag.IsNewHourlyRate = model.IsNewHourlyRate;
            ViewBag.EffortHours = repairOrder.Estimate.TotalLaborHours;

            var matrix = this.repositoryFactory.CreateForCompany<DefaultMatrix>().SingleOrDefault();
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

            if (repairOrder.Estimate != null)
            {
                if (repairOrder.Estimate.Customer.CustomerType == CustomerType.Wholesale)
                {
                    this.InitWholesaleCustomerData(repairOrder);
                }
                else
                {
                    this.InitRetailCustomerData(repairOrder);
                }
            }
            else
            {
                ViewBag.HasInsurance = true;
                ViewBag.WorkByThemselve = false;
            }


            ViewBag.State = state;
            ViewBag.Order = true;
            ViewBag.StateOrder = IsEdit;
        }

        private void InitRetailCustomerData(RepairOrder repairOrder)
        {
            ViewBag.DefaultLimit = repairOrder.Estimate.EstLimitForBodyPart;
            ViewBag.DefaultHourlyRate = repairOrder.Estimate.NewLaborRate.HasValue ? repairOrder.Estimate.NewLaborRate.Value : repairOrder.Estimate.EstHourlyRate;
            ViewBag.DefaultMaxCorProtect = repairOrder.Estimate.EstMaxCorProtect;
            ViewBag.DefaultAluminium = repairOrder.Estimate.EstAluminiumPer;
            ViewBag.DefaultDoubleMetall = repairOrder.Estimate.EstDoubleMetalPer;
            ViewBag.DefaultOversizedRoof = repairOrder.Estimate.EstOversizedRoofPer;
            ViewBag.DefaultOversizedDents = repairOrder.Estimate.EstOversizedDents;
            ViewBag.DefaultCorProtectPart = repairOrder.Estimate.EstCorProtectPart;
            ViewBag.DefaultMaxPercentPart = repairOrder.Estimate.EstMaxPercent;
            ViewBag.HasInsurance = true;
            ViewBag.WorkByThemselve = false;
        }

        private void InitWholesaleCustomerData(RepairOrder repairOrder)
        {
            var customer = this.repositoryFactory.CreateForCompany<WholesaleCustomer>().Get(repairOrder.Estimate.Customer.Id);
            ViewBag.WholesaleCustomer = customer.Id;
            ViewBag.Matrix = repairOrder.Estimate.Matrix.Id;
            ViewBag.MaxCorProtect = repairOrder.Estimate.EstMaxCorProtect;
            ViewBag.LaborRate = repairOrder.Estimate.EstLaborTax;
            ViewBag.Discount = repairOrder.Estimate.EstDiscount;
            ViewBag.HourlyRate = repairOrder.Estimate.NewLaborRate.HasValue ? repairOrder.Estimate.NewLaborRate.Value : repairOrder.Estimate.EstHourlyRate;
            ViewBag.Aluminium = repairOrder.Estimate.EstAluminiumPer;
            ViewBag.DoubleMetall = repairOrder.Estimate.EstDoubleMetalPer;
            ViewBag.OversizedRoof = repairOrder.Estimate.EstOversizedRoofPer;
            ViewBag.OversizedDents = repairOrder.Estimate.EstOversizedDents;
            ViewBag.CorProtectPart = repairOrder.Estimate.EstCorProtectPart;
            ViewBag.Maximum = repairOrder.Estimate.EstMaxPercent;
            ViewBag.LimitForBodyPart = repairOrder.Estimate.EstLimitForBodyPart;
            ViewBag.HasInsurance = customer.Insurance;
            ViewBag.WorkByThemselve = repairOrder.WorkByThemselve;
            ViewBag.HasOrderSignature = customer.OrderSignature;
        }

        #endregion
    }
}
