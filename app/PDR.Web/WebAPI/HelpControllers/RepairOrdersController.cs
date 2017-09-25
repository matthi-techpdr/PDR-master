using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.PushNotification;
using PDR.Domain.Services.Webstorage;
using PDR.Resources.Web.WebApi;
using PDR.Web.Core.Formatters;
using PDR.Web.Core.NLog.FileLoggers;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;
using PDR.Web.WebAPI.WebApiRepoExtensions;
using SmartArch.Data;
using SmartArch.Web.Attributes;
using System.Configuration;
using PDR.Domain.StoredProcedureHelpers;
using PDR.Web.Core.Attributes;


namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize(Roles = "Manager,Technician,Admin, RiTechnician")]
    public class RepairOrdersController : Controller
    {
        private readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        private readonly ICompanyRepository<Invoice> invoiceOrdersRepository;

        private readonly ICompanyRepository<TeamEmployee> teamEmployeeRepository;

        private readonly ICurrentWebStorage<Employee> webStorage;

        private readonly ICompanyRepository<Team> teams;

        private readonly IPushNotification push;

        private readonly ILogger logger;

        private readonly ReassignHelper reassignHelper;

        private const string SortByColumn = "CreationDate";

        protected readonly bool withEmployeeName;

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

        public RepairOrdersController(ICompanyRepository<TeamEmployee> teamEmployeeRepository, ILogger logger, ReassignHelper reassignHelper)
        {
            this.teamEmployeeRepository = teamEmployeeRepository;
            this.repairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            this.webStorage = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>();
            this.invoiceOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
            this.teams = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();
            this.push = ServiceLocator.Current.GetInstance<IPushNotification>();
            this.logger = logger;
            this.reassignHelper = reassignHelper;
            this.withEmployeeName = true;
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetAll()
        {
            var currentUser = this.webStorage.Get();
            var teamSelector = this.Request.Url.ParseQueryString()["team"];
            var archivedSelector = this.Request.Url.ParseQueryString()["archived"];
            var page = this.Request.Url.ParseQueryString()["page"];
            var sortByDateDesc = this.Request.Url.ParseQueryString()["isDesc"] == "true";
            var statusRo = this.Request.Url.ParseQueryString()["statusRO"];

            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            var onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;
            var archived = String.IsNullOrEmpty(archivedSelector) ? (bool?)null : archivedSelector.ToLower() == "true";
            var numPage = Convert.ToInt32(page);
            var status = GeteRepairOrderStatus(statusRo);
            var sort = sortByDateDesc ? "DESC" : "ASC";

            if (currentUser is Estimator)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                message.Content = new StringContent("Current employee can not to have repair orders.");
                throw new HttpResponseException(message);
            }

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentUser.Id, teamId, isOnlyOwn: onlyOwn, rowsPerPage: this.CountRows, pageNumber: numPage + 1,
                                    sortByColumn: SortByColumn, sortType: sort, repairOrdersStatus: status, isForReport: false, isFinalised: archived);

            var repairOrders = GetRepairOrders(repairOrdersSpHelper.RepairOrders);

            return this.Json(new { repairOrders }, JsonRequestBehavior.AllowGet);
        }

        private RepairOrderStatuses? GeteRepairOrderStatus(string status)
        {
            if (status == null)
            {
                return null;
            }
            switch (status.ToLower())
            {
                case "open":
                    return RepairOrderStatuses.Open;
                case "completed":
                    return RepairOrderStatuses.Completed;
                case "approved":
                    return RepairOrderStatuses.Approved;
                case "finalised":
                    return RepairOrderStatuses.Finalised;
                default:            //ALL estimates
                    return null;
            }
        }

        public JsonResult GetAllForSearch()
        {
            var currentUser = this.webStorage.Get();
            var teamSelector = this.Request.Url.ParseQueryString()["team"];
            var vin = this.Request.Url.ParseQueryString()["vin"];
            var stock = this.Request.Url.ParseQueryString()["stock"];
            var custRo = this.Request.Url.ParseQueryString()["custRo"];
            var page = this.Request.Url.ParseQueryString()["page"];

            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            var onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;
            var numPage = Convert.ToInt32(page);

            if (currentUser is Estimator)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                message.Content = new StringContent("Current employee can not to have repair orders.");
                throw new HttpResponseException(message);
            }

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentUser.Id, teamId, isOnlyOwn: onlyOwn, rowsPerPage: this.CountRows, pageNumber: numPage + 1,
                                    sortByColumn: SortByColumn, vinCode: vin, stock: stock, custRo: custRo);

            var repairOrders = GetRepairOrders(repairOrdersSpHelper.RepairOrders);

            return this.Json(new { repairOrders }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable GetRepairOrders(IEnumerable<RepairOrder> repairOrders)
        {
            var result = repairOrders.Select(
                x =>
                {
                    var invoice =
                        this.invoiceOrdersRepository.SingleOrDefault(y => y.RepairOrder.Id == Convert.ToInt64(x.Id));
                    return
                        new
                            {
                                x.Id,
                                CreationDate = x.CreationDate.ToShortDateString(),
                                RepairOrderStatus = x.RepairOrderStatus.ToString(),
                                x.TotalAmount,
                                Customer = new ApiCustomerDocumentModel<RepairOrder>(x),
                                AssignedEmployees = x.TeamEmployeePercents.Select(a => new ApiAssignedEmployee(a)),
                                Estimate = new { x.Estimate.Car },
                                x.New,
                                x.EditedStatus,
                                IsDiscardInvoice = invoice != null && invoice.IsDiscard
                            };
                });
            return result;
        }

        public ActionResult Get(long id)
        {
            var ro = this.repairOrdersRepository.Get(id);

            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "RO with the same ID not found.");
            }

            CommonLogger.View(ro);

            return new JsonNetResult(new ApiRepairOrderModel(ro), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void IsNotNew(long id)
        {
            var ro = this.repairOrdersRepository.GetIfExist(id);
            ro.New = false;
            this.repairOrdersRepository.Save(ro);
        }

        [Transaction]
        [System.Web.Mvc.HttpGet]
        public void RiOperations(long id, bool? isFlatFee, double? paymentFlatFee)
        {
            bool isRIPaymentChanged = false;
            var ro = this.repairOrdersRepository.GetIfExist(id);
            if (ro.IsFlatFee!=isFlatFee || ro.Payment!=paymentFlatFee)
            {
                isRIPaymentChanged = true;
            }
            ro.IsFlatFee = isFlatFee.HasValue ? isFlatFee.Value : (bool?)null;
            ro.Payment = paymentFlatFee.HasValue ? paymentFlatFee.Value : (double?)null;
            this.repairOrdersRepository.Save(ro);
            if (isRIPaymentChanged)
            {
                this.logger.LogAssignToRo(ro);
            }
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void ChangeWorkByThemselve(long id)
        {
            var ro = this.repairOrdersRepository.GetIfExist(id);
            ro.WorkByThemselve = !ro.WorkByThemselve;
            this.repairOrdersRepository.Save(ro);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void AdditionalDiscount(ApiAdditionalDiscountModel model)
        {
            var ro = this.repairOrdersRepository.GetIfExist(model.Id);
            if (ro.AdditionalDiscount != model.Discount)
            {
                logger.LogChangedAdditionalDiscountValue(ro, ro.AdditionalDiscount, model.Discount);
            }
            ro.AdditionalDiscount = model.Discount;
            this.repairOrdersRepository.Save(ro);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void RetailDiscount(ApiRetailDiscountModel model)
        {
            var ro = this.repairOrdersRepository.GetIfExist(model.Id);
            ro.RetailDiscount = model.Discount;
            this.repairOrdersRepository.Save(ro);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void ChangeStatus(long id, string status)
        {
            var repairOrder = this.repairOrdersRepository.GetIfExist(id);

            RepairOrderStatuses enumStatus;
            if (Enum.TryParse(status, true, out enumStatus))
            {
                repairOrder.RepairOrderStatus = enumStatus;
            }

            this.repairOrdersRepository.Save(repairOrder);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public JsonResult ToInvoice(long id)
        {
            var repairOrder = this.repairOrdersRepository.GetIfExist(id);
            var currentUser = this.webStorage.Get();
            if (! (currentUser is TeamEmployee))
            {
                HttpResponseMessage message = new HttpResponseMessage();
                message.Content = new StringContent("Current user is not team employee.");
                throw new HttpResponseException(message);
            }
            var currentTeamEmployee = (TeamEmployee) currentUser;
            var manager = currentTeamEmployee.Teams.SelectMany(x => x.Employees).FirstOrDefault(x => x is Manager);

            var invoice = this.invoiceOrdersRepository.SingleOrDefault(x => x.RepairOrder.Id == repairOrder.Id);
            //string message;
            InvoiceLogActions action;

            if (invoice == null)
            {
                invoice = new Invoice(repairOrder, currentTeamEmployee, true);
                action = InvoiceLogActions.Generate;
                //message = NotificationMessages.GeneratedInvoice;
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
                invoice.TeamEmployee = currentTeamEmployee;
                invoice.Customer = repairOrder.With(x => x.Estimate).With(x => x.Customer);
                invoice.InvoiceSum = Math.Round(repairOrder.TotalAmount, 2);
                action = InvoiceLogActions.ReGenerate;
                //message = NotificationMessages.ReGeneratedInvoice;
            }

            this.invoiceOrdersRepository.Save(invoice);
            this.logger.Log(invoice, action);
            //message = string.Format(message, invoice.Id);

            //if (manager != null && repairOrder.TeamEmployeePercents.Count > 1 && currentUser.Id != manager.Id)
            //{
            //    this.push.Send(manager, message, "RO", new object[] { repairOrder.Id });
            //}

            return this.Json(new { NewInvoiceId = invoice.Id }, JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void AddSupplements(AddSuplementsModel model)
        {
            var repairOrder = this.repairOrdersRepository.GetIfExist(model.RepairOrderId);
            var supplements = repairOrder.Supplements.ToList();
            var supplementsTemp = new HashedSet<Supplement>();
            var supplementRemove = new List<Supplement>();

            var supplementsRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Supplement>>();
            //supplementsRepository.Where(x => x.RepairOrder == null).ToList().ForEach(supplementsRepository.Remove);
            //ServiceLocator.Current.GetInstance<ISession>().Flush();

            //foreach (var sup in repairOrder.Supplements)
            //{
            //    sup.RepairOrder = null;
            //    supplementsRepository.Save(sup);
            //}

            foreach (var supplementModel in model.Supplements)
            {
                var supplement = supplements.Count == 0
                                 ? new Supplement(true)
                                 : supplements.SingleOrDefault(x => x.Id == supplementModel.Id) ?? new Supplement(true);
                supplement.Sum = supplementModel.Cost;
                supplement.Description = supplementModel.Description.Replace("&lt;", "<");
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
                supplementsRepository.Remove(supplement);
            }

            this.repairOrdersRepository.Save(repairOrder);
            CommonLogger.EditRepairOrder(repairOrder);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public void AttachPhotos(long id)
        {
            var repairOrder = this.repairOrdersRepository.GetIfExist(id);

            var files = this.Request.Files;
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file == null)
                {
                    continue;
                }
                var ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                var photo = ms.ToArray();
                repairOrder.AdditionalPhotos.Add(new AdditionalCarPhoto(photo, file.ContentType, true));
            }

            this.repairOrdersRepository.Save(repairOrder);
            CommonLogger.EditRepairOrder(repairOrder);
        }

        public ActionResult GetTeamsMembers()
        {
            var manager = this.webStorage.Get() as Manager;
            var admin = this.webStorage.Get() as Admin;

            IEnumerable<TeamEmployee> employees = null;
            if (manager != null)
            {
                employees = manager.Teams.SelectMany(x => x.Employees).Distinct();
            }

            if (admin != null)
            {
                employees = this.teamEmployeeRepository.Where(x => x.Status == Statuses.Active);
            }

            if (employees != null)
            {
                var model = employees.Select(x => new { x.Id, x.Name });
                return this.Json(model, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(409, "This operation is available only for manager and admin");
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult AddEmployee(ApiAddEmployeeModel model)
        {
            var ro = this.repairOrdersRepository.Get(model.RoId);

            ro.IsFlatFee = model.IsFlatFee.HasValue ? model.IsFlatFee.Value : (bool?)null;
            ro.Payment = model.PaymentFlatFee.HasValue ? model.PaymentFlatFee.Value : (double?)null;

            var empIds = model.AssignedEmployees.Select(x => x.Id).ToList();
            var employees = this.teamEmployeeRepository.Where(x => empIds.Contains(x.Id)).ToList();
            var currentEmp = this.webStorage.Get() as TeamEmployee;

            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "Repair order or employee not found.");
            }

            this.reassignHelper.AddEmployeesToRepairOrder(ro, employees, currentEmp);
            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult RemoveEmployee(ApiAddEmployeeModel model)
        {
            var ro = this.repairOrdersRepository.Get(model.RoId);
            var empIds = model.AssignedEmployees.Select(x => x.Id).ToList();
            var employees = this.teamEmployeeRepository.Where(x => empIds.Contains(x.Id)).ToList();

            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "Repair order or employee not found.");
            }

            this.reassignHelper.RemoveEmployeesFromRepairOrder(ro, employees);
            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult DefinePercents(ApiDefinePercentsModel model)
        {
            RepairOrder ro = this.repairOrdersRepository.Get(model.RoId);
            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "Repair order or employee not found.");
            }

            foreach (var employee in model.Employees)
            {
                ApiAssignedEmployee emp = employee;
                var teamPercent = ro.TeamEmployeePercents.SingleOrDefault(x => x.TeamEmployee.Id == emp.Id);
                if (teamPercent == null)
                {
                    teamPercent = new TeamEmployeePercent(true)
                                  {
                                      RepairOrder = ro,
                                      TeamEmployee = this.teamEmployeeRepository.Get(emp.Id)
                                  };
                    ro.TeamEmployeePercents.Add(teamPercent);
                }

                teamPercent.EmployeePart = employee.Part;
            }

            this.repairOrdersRepository.Save(ro);
            this.logger.LogDefinePercents(ro);

            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        public ActionResult Pdf(long id, bool detailed = false)
        {
            var pdfConverter = ServiceLocator.Current.GetInstance<IPdfConverter>();
            var ro = this.repairOrdersRepository.Get(id);
            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            this.logger.Log(ro, RepairOrderLogActions.Print);
            var pdf = pdfConverter.ConvertRepairOrder(ro, detailed);
            return this.File(pdf, "application/pdf");
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult Approve(long id)
        {
            RepairOrder ro = this.repairOrdersRepository.Get(id);
            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            var files = Request.Files;
            if (files.Count > 0)
            {
                var signature = Request.Files[0];
                if (signature != null)
                {
                    var memoryStream = new MemoryStream();
                    signature.InputStream.CopyTo(memoryStream);
                    ro.RoCustomerSignature = new RoCustomerSignature(memoryStream.ToArray(), signature.ContentType, true);
                }
            }

            ro.RepairOrderStatus = RepairOrderStatuses.Approved;
            this.repairOrdersRepository.Save(ro);
            this.logger.Log(ro, RepairOrderLogActions.Approve);
            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult Complete(long id)
        {
            var ro = this.repairOrdersRepository.Get(id);

            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            ro.RepairOrderStatus = RepairOrderStatuses.Completed;
            this.repairOrdersRepository.Save(ro);
            this.logger.Log(ro, RepairOrderLogActions.Complete);

            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult RequestForEditRO(long id)
        {
            var order = this.repairOrdersRepository.SingleOrDefault(x => x.Id == id);

            if (order == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            order.EditedStatus = EditedStatuses.EditPending;
            this.repairOrdersRepository.Save(order);
            this.logger.LogRequestEdit(order);

            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult RequestForAllowEditRO(long id)
        {
            var order = this.repairOrdersRepository.SingleOrDefault(x => x.Id == id);

            if (order == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            order.EditedStatus = EditedStatuses.Editable;
            this.repairOrdersRepository.Save(order);
            this.logger.Log(order, RepairOrderLogActions.Editable);

            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult RequestForRejectEditRO(long id)
        {
            var order = this.repairOrdersRepository.SingleOrDefault(x => x.Id == id);
            if (order == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            order.EditedStatus = EditedStatuses.EditingReject;
            this.repairOrdersRepository.Save(order);
            this.logger.Log(order, RepairOrderLogActions.EditingReject);

            return new HttpStatusCodeResult(200);
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult Discard(long id)
        {
            var invoice = this.invoiceOrdersRepository.SingleOrDefault(x => x.RepairOrder.Id == Convert.ToInt64(id));
            var ro = this.repairOrdersRepository.Get(id);
            if (invoice != null)
            {
                var employee = this.webStorage.Get();
                if ((employee.Role == UserRoles.Manager || employee.Role == UserRoles.Technician) && (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.PaidInFull))
                {
                    return new HttpStatusCodeResult(417, "This action cannot be done for entries with \"Paid\" status for roles Manager and Technician");
                }

                invoice.Archived = true;
                invoice.IsDiscard = true;
                this.logger.Log(invoice, InvoiceLogActions.Discard);
                this.invoiceOrdersRepository.Save(invoice);
                ro = invoice.RepairOrder;
                //return new HttpStatusCodeResult(404, "Invoice doesn't exist");
            }

            if (ro == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }


            ro.RepairOrderStatus = RepairOrderStatuses.Discard;

            this.logger.Log(ro, RepairOrderLogActions.Discard);
            this.repairOrdersRepository.Save(ro);

            return new HttpStatusCodeResult(200);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetHistory(string id)
        {
            var ro = repairOrdersRepository.Get(Convert.ToInt64(id));
            var historyManager = new DocumentHistoryManager();
            HistoryListModel historyList = new HistoryListModel(historyManager.GenerateHistory(ro, withEmployeeName).ToList());
            var result = this.Json(new { historyList }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [Transaction]
        [System.Web.Mvc.HttpPost]
        public ActionResult HourlyRate(long id, double hourlyRate)
        {
            var order = this.repairOrdersRepository.SingleOrDefault(x => x.Id == id);


            if (order == null)
            {
                return new HttpStatusCodeResult(404, "RO doesn't exist");
            }

            if (!(order.RepairOrderStatus == RepairOrderStatuses.Open || order.EditedStatus == EditedStatuses.Editable))
            {
                return new HttpStatusCodeResult(417, "RO should have Open or Editable status!");
            }

            if (order.Estimate != null)
            {
                var oldLaborRateValue = order.Estimate.NewLaborRate ?? order.Estimate.CurrentHourlyRate;
                if (oldLaborRateValue != hourlyRate)
                {
                    logger.LogChangedLaborRateValue(order, oldLaborRateValue, hourlyRate);
                }
                order.Estimate.NewLaborRate = hourlyRate;
                order.Estimate.TaxSum = order.Estimate.GetTaxSum();
                order.Estimate.LaborSum = order.Estimate.GetLaborSum();
                order.Estimate.TotalAmount = order.Estimate.CalculateTotalAmount();
                this.repairOrdersRepository.Save(order);
            }

            this.logger.Log(order, RepairOrderLogActions.Edit);

            return new HttpStatusCodeResult(200);
        }

        private static string ResolveUrl(string secondUrlPart)
        {
            return string.Format(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, secondUrlPart));
        }
    }
}