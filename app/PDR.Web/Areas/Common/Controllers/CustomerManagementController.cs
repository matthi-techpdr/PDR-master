using System;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Specifications;

using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Common.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;

    using NHibernate;
    using NHibernate.Transform;

    using PDR.Domain.Services.Grid;
    using PDR.Domain.StoredProcedureHelpers;

    using SmartArch.Data.Proxy;

    [PDRAuthorize]
    public class CustomerManagementController : Controller 
    {
        #region private fields

        private readonly IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, CustomerViewModel> wholesaleCustomerGridMaster;

        private readonly ICompanyRepository<Team> teamRepository;

        private readonly ICompanyRepository<PriceMatrix> matricesRepository;

        private readonly IGridMasterForStoredProcedure<Estimate, CustomerEstimateJsonModel, CustomerViewModel> estimatesGridMaster;

        private readonly IGridMasterForStoredProcedure<RepairOrder, CustomersOrderJsonModel, CustomerViewModel> repairOrderGridMaster;

        private readonly IGridMasterForStoredProcedure<Invoice, CustomersInvoiceJsonModel, CustomerViewModel> invoicesGridMaster;

        private readonly ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly ICompanyRepository<WholesaleCustomer> wholesaleCustomers;

        private readonly ICompanyRepository<RetailCustomer> retailCustomers;

        private readonly ICompanyRepository<Customer> Customers;

        private readonly ICurrentWebStorage<Employee> storage;

        private readonly ICompanyRepository<Estimate> estimatesRepositry;

        #endregion

        #region Constructor

        public CustomerManagementController(
            IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, CustomerViewModel> wholesaleCustomerGridMaster,
            ICompanyRepository<Team> teamRepository,
            ICompanyRepository<PriceMatrix> matricesRepository,
            ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository,
            IRepositoryFactory repositoryFactory)
        {
            this.wholesaleCustomerGridMaster = wholesaleCustomerGridMaster;
            this.teamRepository = teamRepository;
            this.matricesRepository = matricesRepository;
            this.wholesalersRepository = wholesalersRepository;
            this.repositoryFactory = repositoryFactory;
            this.wholesaleCustomers = ServiceLocator.Current.GetInstance<ICompanyRepository<WholesaleCustomer>>();
            this.retailCustomers = ServiceLocator.Current.GetInstance<ICompanyRepository<RetailCustomer>>();
            this.Customers = ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>();
            this.storage = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>();
            this.estimatesGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Estimate, CustomerEstimateJsonModel, CustomerViewModel>>();
            this.repairOrderGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<RepairOrder, CustomersOrderJsonModel, CustomerViewModel>>();
            this.invoicesGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Invoice, CustomersInvoiceJsonModel, CustomerViewModel>>();
            this.estimatesRepositry = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
        }

        #endregion

        public ActionResult Index()
        {
            var stateCookie = HttpContext.Request.Cookies.Get("state");
            var teamCookie = HttpContext.Request.Cookies.Get("team");

            var states = ListsHelper.GetStates(null).ToList();
            states.Insert(0, new SelectListItem { Text = @"All states", Selected = true, Value = null });
            
            long stateId;
            var state = stateCookie != null ? stateCookie.Value : null;
            var resultTryParseState = Int64.TryParse(state, out stateId);
            if (resultTryParseState)
            {
                states.Single(x => x.Value == null).Selected = false;
                var item = states.FirstOrDefault(x => x.Value == state);
                if (item != null)
                {
                    states.Remove(item);
                    item.Selected = true;
                    states.Insert(0, item);
                }
            }


            ViewBag.IsAdmin = this.storage.Get() is Domain.Model.Users.Admin;
            if (this.storage.Get() is Domain.Model.Users.Admin)
            {
                var teamFilter = new FilterModel(null, false, false, teamCookie);
                ViewBag.Teams = teamFilter.Teams;
            }

            return this.View(states);
        }

        [HttpPost]
        [Transaction]
        public void CreateCustomer(CustomerViewModel model)
        {
            this.wholesaleCustomerGridMaster.CreateEntity(model, x => this.AddTeamsAndMatrices(model, x));
            CustomerLogger.Create(model);
            this.CreateWholesaler(model);
        }

        [HttpPost]
        [Transaction]
        public void EditCustomer(CustomerViewModel model)
        {
            CustomerLogger.Edit(model);
            this.EditWholesaler(model);
            this.wholesaleCustomerGridMaster.EditEntity(model, x => this.AddTeamsAndMatrices(model, x));
        }

        [HttpGet]
        public virtual JsonResult GetData(string sidx, string sord, int page, int rows, int? state, long? team)
        {
            var currentEmployee = storage.Get();
            if (currentEmployee == null)
            {
                return null;
            }
            var onlyOwn = team.HasValue && team == 0;
            var manager = new CustomersLightStoredProcedureHelper(currentEmployee.Id, "wholesalecustomer", team, state, onlyOwn, rows, page, sord);
            var customers = manager.CustomerModels;
            if (customers != null && !customers.Any())
            {
                customers = manager.CustomerModelsFirstPage;
                page = 1;
            }

            var data = new GridModel<WholesaleCustomerJsonModel>();
            data.rows = new List<WholesaleCustomerJsonModel>();
            for (int i = 0; i < customers.Count; i++)
            {
                var item = new WholesaleCustomerJsonModel(customers[i], team);
                data.rows.Add(item);
            }
            data.page = page;
            data.records = manager.TotalCountRows;
            data.total = customers != null && customers.Count == 0 ? 1 : Math.Ceiling(manager.TotalCountRows / (float)rows);

            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return json;
        }

        [HttpGet]
        public JsonResult GetEstimates(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();
            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, filterByCustomers: customer, pageNumber: page,
                        rowsPerPage: rows, sortByColumn: sidx, sortType: sord);
            var data = this.estimatesGridMaster.GetData(estimatesSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        [HttpGet]
        public JsonResult GetRepairOrders(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();
            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord);
            var data = this.repairOrderGridMaster.GetData(repairOrdersSpHelper, rows, page);


            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        [HttpGet]
        public JsonResult GetInvoices(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, filterByCustomers: customer, pageNumber: page,
                                                rowsPerPage: rows, sortByColumn: sidx, sortType: sord);
            var data = this.invoicesGridMaster.GetData(invoicesSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        #region Suspend/Reactivate Customer

        [HttpPost]
        [Transaction]
        public void SuspendCustomer(string ids)
        {
            var customers = this.wholesaleCustomers.Where(x => ids.Split(',').Contains(x.Id.ToString()));
            foreach (var customer in customers)
            {
                CustomerLogger.Suspend(customer);
                customer.Status = Statuses.Suspended;
                foreach (var team in customer.Teams.ToList())
                {
                    team.Customers.Remove(customer);
                    customer.Teams.Remove(team);
                }

                //this.wholesaleCustomers.Save(customer);

                //var user = this.wholesalersRepository.SingleOrDefault(x => x.Login == customer.Email);

                //if (user == null) continue;

                //user.Status = Statuses.Suspended;
                //this.wholesalersRepository.Save(user);

            }
        }

        [HttpPost]
        [Transaction]
        public void ReactivateCustomer(string ids)
        {
            var customers = this.wholesaleCustomers.Where(x => ids.Split(',').Contains(x.Id.ToString()));
            foreach (var customer in customers)
            {
                CustomerLogger.Reactivate(customer);
                customer.Status = Statuses.Active;
                this.wholesaleCustomers.Save(customer);

                var user = this.wholesalersRepository.SingleOrDefault(x => x.Login == customer.Email);

                if (user == null) continue;

                user.Status = Statuses.Active;
                this.wholesalersRepository.Save(user);
            }
        }

        #endregion

        public ActionResult GetCustomer(long? id, bool edit)
        {
            var model = this.GetCustomerViewModel(id, edit);
            return this.PartialView(edit ? "Edit" : "View", model);
        }

        #region Send Email

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            if (customer)
            {
                to = this.repositoryFactory.CreateForCompany<Customer>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }

            return this.PartialView(new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids });
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var from = this.storage.Get().Company.Email;
                var message = new MailService().Send(from, model.To, model.Subject, model.Message, null);
                CustomerLogger.SendEmail(model);
                return this.Json(message, JsonRequestBehavior.AllowGet);
            }

            return this.Json(false, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        [Transaction]
        private void CreateWholesaler(CustomerViewModel model)
        {
            var employee = new Domain.Model.Users.Wholesaler(true)
            {
                Login = model.Email,
                Password = model.Password,
                Name = model.Name,
                CanCreateEstimates = model.CanCreateEstimates
            };
            this.wholesalersRepository.Save(employee);
        }

        [Transaction]
        private void EditWholesaler(CustomerViewModel model)
        {
            var id = model.Id;
            var customer = this.wholesaleCustomers.Get(Convert.ToInt64(id));

            var wholesaler = this.wholesalersRepository.Single(x => x.Login == customer.Email);
            if (customer.Email != model.Email)
            {
                wholesaler.Login = model.Email;
            }

            if (customer.Password != model.Password)
            {
                wholesaler.Password = model.Password;
            }
            if (this.storage.Get().Role == UserRoles.Admin)
            {
                if (wholesaler.CanCreateEstimates != model.CanCreateEstimates)
                {
                    wholesaler.CanCreateEstimates = model.CanCreateEstimates;
                }
            }

            this.wholesalersRepository.Save(wholesaler);
        }

        private CustomerViewModel GetCustomerViewModel(long? id, bool edit)
        {
            var teams = this.teamRepository.Where(x => x.Status == Statuses.Active).OrderBy(x => x.Title).ToList();
            var matrices = this.matricesRepository.Where(x => x.Status == Statuses.Active).OrderBy(x => x.Name).ToList();

            CustomerViewModel model  = this.wholesaleCustomerGridMaster.GetEntityViewModel(
                id,
                (e, m) =>
                {
                    m.Insurance = e.Insurance;
                    m.WorkByThemselve = e.WorkByThemselve;
                    m.EstimateSignature = e.EstimateSignature;
                    m.OrderSignature = e.OrderSignature;
                    m.LaborRate = string.Format("{0:0.00}", e.LaborRate);
                    m.Status = e.Status.ToString();
                    m.MatricesNames = e.Matrices.Select(x => x.Name).ToList();
                    m.TeamsNames = e.Teams.Select(x => x.Title).ToList();
                    m.CanCreateEstimates = wholesalersRepository.First(x => x.Login == m.Email).CanCreateEstimates;
                    if (edit)
                    {
                        var customerTeams = e.Teams.ToList();
                        var customerMatrices = e.Matrices.ToList();
                        m.States = ListsHelper.GetStates((int?)e.State);

                        m.TeamsList =
                            teams.Select(
                                t =>
                                new SelectListItem
                                {
                                    Text = t.Title,
                                    Value = t.Id.ToString(),
                                    Selected = customerTeams.Contains(t)
                                });

                        m.MatricesList =
                            matrices.Select(
                                mx =>
                                new SelectListItem
                                {
                                    Text = mx.Name,
                                    Value = mx.Id.ToString(),
                                    Selected = customerMatrices.Contains(mx)
                                });
                    }
                });
            this.ViewBag.CurrentUser = this.storage.Get();

            if (model.Id == null)
            {
                model.States = ListsHelper.GetStates(null);
                model.TeamsList = teams.OrderBy(x => x.Title).Select(t => new SelectListItem { Text = t.Title.Length > 31 ? t.Title.Substring(0, 31) + "..." : t.Title, Value = t.Id.ToString() });
                model.MatricesList = matrices.OrderBy(x => x.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() });
            }


            if (edit)
            {
                var allCustomers = this.wholesaleCustomers.ToList();
                var allRetailCustomers = this.retailCustomers.ToList();
                //var allRetailCustomers = this.Customers.ToList();
                var c = this.Customers.ToList();

                if (model.Id != null)
                {
                    allCustomers.Remove(this.wholesaleCustomers.Get(Convert.ToInt64(model.Id)));
                }

                //var ar = allCustomers.Select(x => x.Email).ToList().AddRange(allRetailCustomers.Select(x => x.Email.ToList()));
                //this.ViewData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList()) + "," + (string.Join(",", allRetailCustomers.Select(x => x.Email).ToList()));//.AddRange(allRetailCustomers.Select(x => x.Email.ToList(string))));
                //this.ViewData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList()) + "," + (string.Join(",", c.Select(x => x.Email).ToList()));//.AddRange(allRetailCustomers.Select(x => x.Email.ToList(string))));
                this.ViewData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList());
            }

            var currentUser = storage.Get();
            if (id != null)
            {
                var helper = new CustomerStoredProcedureHelper(currentUser.Id, id.Value, "wholesalecustomer");
                model.AmountOfOpenEstimates = helper.AmountOfOpenEstimates;
                model.SumOfOpenEstimates = helper.SumOfOpenEstimates.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfOpenWorkOrders = helper.SumOfOpenWorkOrders.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfPaidInvoices = helper.SumOfPaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfUnpaidInvoices = helper.SumOfUnpaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }

            return model;
        }

        private void AddTeamsAndMatrices(CustomerViewModel model, WholesaleCustomer customer)
        {
            customer.Matrices.Clear();
            foreach (var team in customer.Teams)
            {
                team.Customers.Remove(customer);
            }

            foreach (var tId in model.TeamsIds)
            {
                var team = this.teamRepository.Get(tId);
                customer.AssignTeam(team);
            }

            foreach (var mId in model.MatricesIds)
            {
                var matrix = this.matricesRepository.Get(mId);
                customer.AddMatrix(matrix);
            }
        }

        #endregion
    }
}