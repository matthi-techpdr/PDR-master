using System;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Specifications;

using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;
using PDR.Domain.StoredProcedureHelpers;
using PDR.Web.Areas.Estimator.Models.Affiliates;


using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;

    using PDR.Domain.Services.Grid;
    using PDR.Web.Areas.Estimator.Models.Customers;

    [PDRAuthorize(Roles = "Admin")]
    public class LocationController : Controller
    {
        #region private fields

        private readonly IGridMaster<Affiliate, AffiliatesCustomerJsonModel, AffiliatesViewModel> affiliatesCustomerGridMaster;

        private readonly ICompanyRepository<Team> teamRepository;

        private readonly IGridMasterForStoredProcedure<Estimate, CustomerEstimateJsonModel, CustomerViewModel> estimatesGridMaster;

        private readonly IGridMasterForStoredProcedure<RepairOrder, CustomersOrderJsonModel, CustomerViewModel> repairOrderGridMaster;

        private readonly IGridMasterForStoredProcedure<Invoice, CustomersInvoiceJsonModel, CustomerViewModel> invoicesGridMaster;

        private readonly ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly ICompanyRepository<Affiliate> affiliate;

        private readonly ICurrentWebStorage<Employee> storage;

        private readonly ICompanyRepository<Estimate> estimatesRepositry;

        #endregion

        #region Constructor

        public LocationController(IGridMaster<Affiliate, AffiliatesCustomerJsonModel, AffiliatesViewModel> affiliatesCustomerGridMaster, 
            ICompanyRepository<Team> teamRepository, ICompanyRepository<Domain.Model.Users.Wholesaler> wholesalersRepository, 
            IRepositoryFactory repositoryFactory)
            
        {
            this.affiliatesCustomerGridMaster = affiliatesCustomerGridMaster;
            this.teamRepository = teamRepository;
            this.wholesalersRepository = wholesalersRepository;
            this.repositoryFactory = repositoryFactory;
            this.affiliate = ServiceLocator.Current.GetInstance<ICompanyRepository<Affiliate>>();
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


            if (this.storage.Get() is Domain.Model.Users.Admin)
            {
                var teamFilter = new FilterModel(null, false, false, teamCookie);
                ViewBag.Teams = teamFilter.Teams;
            }

            return this.View(states);
        }

        [HttpGet]
        public virtual JsonResult GetData(string sidx, string sord, int page, int rows, int? state, long? team)
        {
            //if (team.HasValue && team != 0)
            //{
            //    var currentTeam = this.teamRepository.Get(team.Value);
            //    this.affiliatesCustomerGridMaster.ExpressionFilters.Add(x => x.Teams.Contains(currentTeam));
            //}

            //if (state.HasValue)
            //{
            //    this.affiliatesCustomerGridMaster.ExpressionFilters.Add(x => x.State == (StatesOfUSA)state.Value);
            //}

            //var data = this.affiliatesCustomerGridMaster.GetData(page, rows, sidx, sord, additionalModelParam: team);
            //var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            //return json;
            var currentEmployee = this.storage.Get();
            if (currentEmployee == null)
            {
                return null;
            }
            var onlyOwn = team.HasValue && team == 0;
            var manager = new CustomersLightStoredProcedureHelper(currentEmployee.Id, "affiliate", team, state, onlyOwn, rows, page, sord);
            var customers = manager.CustomerModels;
            if (customers != null && !customers.Any())
            {
                customers = manager.CustomerModelsFirstPage;
                page = 1;
            }

            var data = new GridModel<AffiliatesCustomerJsonModel>();
            data.rows = new List<AffiliatesCustomerJsonModel>();
            for (int i = 0; i < customers.Count; i++)
            {
                var item = new AffiliatesCustomerJsonModel(customers[i], team);
                data.rows.Add(item);
            }
            data.page = page;
            data.records = manager.TotalCountRows;
            data.total = customers != null && customers.Count == 0 ? 1 : Math.Ceiling(manager.TotalCountRows / (float)rows);

            return this.Json(data, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        [Transaction]
        public void CreateAffiliates(AffiliatesViewModel model)
        {
            this.affiliatesCustomerGridMaster.CreateEntity(model, x => this.AddTeams(model, x));
            CustomerLogger.Create(model);
            this.CreateWholesaler(model);
        }

        [HttpPost]
        [Transaction]
        public void EditAffiliate(AffiliatesViewModel model)
        {
            CustomerLogger.Edit(model);
            this.EditWholesaler(model);
            this.affiliatesCustomerGridMaster.EditEntity(model, x => this.AddTeams(model, x));
        }


        public ActionResult GetAffiliates(long? id, bool edit)
        {
            var model = this.GetAffiliatesViewModel(id, edit);
            return this.PartialView(edit ? "EditAffiliates" : "AffiliateView", model);
        }

        [HttpPost]
        [Transaction]
        public void SuspendCustomer(string ids)
        {
            var customers = this.affiliate.Where(x => ids.Split(',').Contains(x.Id.ToString()));
            foreach (var customer in customers)
            {
                CustomerLogger.Suspend(customer);
                customer.Status = Statuses.Suspended;
                foreach (var team in customer.Teams.ToList())
                {
                    team.Customers.Remove(customer);
                    customer.Teams.Remove(team);
                }
            }
        }

        [HttpPost]
        [Transaction]
        public void ReactivateCustomer(string ids)
        {
            var customers = this.affiliate.Where(x => ids.Split(',').Contains(x.Id.ToString()));
            foreach (var customer in customers)
            {
                CustomerLogger.Reactivate(customer);
                customer.Status = Statuses.Active;
                this.affiliate.Save(customer);

                var user = this.wholesalersRepository.SingleOrDefault(x => x.Login == customer.Email);

                if (user == null) continue;

                user.Status = Statuses.Active;
                this.wholesalersRepository.Save(user);
            }
        }

        public ActionResult GetCustomer(long? id, bool edit)
        {
            var model = this.GetAffiliatesViewModel(id, edit);
            return this.PartialView(edit ? "Edit" : "View", model);
        }

        [HttpGet]
        public JsonResult GetEstimates(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();
            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, rowsPerPage: rows, pageNumber: page, 
                                    sortByColumn: sidx, sortType: sord, affiliateId: customer);
            var data = this.estimatesGridMaster.GetData(estimatesSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        [HttpGet]
        public JsonResult GetRepairOrders(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, rowsPerPage: rows, pageNumber: page,
                                    sortByColumn: sidx, sortType: sord, affiliateId: customer);
            var data = this.repairOrderGridMaster.GetData(repairOrdersSpHelper, rows, page);

            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        [HttpGet]
        public JsonResult GetInvoices(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var currentEmployee = this.storage.Get();
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, affiliateId: customer, pageNumber:page, 
                                                rowsPerPage: rows, sortByColumn: sidx, sortType: sord);
            var data = this.invoicesGridMaster.GetData(invoicesSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        #region Send Email

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            if (customer)
            {
                to = this.repositoryFactory.CreateForCompany<Affiliate>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
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

        private void AddTeams(AffiliatesViewModel model, Affiliate affiliates)
        {
            foreach (var team in affiliates.Teams)
            {
                team.Customers.Remove(affiliates);
            }

            foreach (var tId in model.TeamsIds)
            {
                var team = this.teamRepository.Get(tId);
                affiliates.AssignTeam(team);
            }
        }

        [Transaction]
        private void CreateWholesaler(AffiliatesViewModel model)
        {
            var employee = new Domain.Model.Users.Wholesaler(true)
            {
                Login = model.Email,
                Name = model.Name
            };
            this.wholesalersRepository.Save(employee);
        }

        [Transaction]
        private void EditWholesaler(AffiliatesViewModel model)
        {
            var id = model.Id;
            var customer = this.affiliate.Get(Convert.ToInt64(id));

            var wholesaler = this.wholesalersRepository.Single(x => x.Login == customer.Email);
            if (customer.Email != model.Email)
            {
                wholesaler.Login = model.Email;
            }

            this.wholesalersRepository.Save(wholesaler);
        }


        private AffiliatesViewModel GetAffiliatesViewModel(long? id, bool edit)
        {
            var teams = this.teamRepository.Where(x => x.Status == Statuses.Active).OrderBy(x => x.Title).ToList();

            AffiliatesViewModel model = this.affiliatesCustomerGridMaster.GetEntityViewModel(
                id,
                (e, m) =>
                {
                    m.LaborRate = string.Format("{0:0.00}", e.LaborRate);
                    m.Status = e.Status.ToString();
                    m.TeamsNames = e.Teams.Select(x => x.Title).ToList();

                    if (edit)
                    {
                        var customerTeams = e.Teams.ToList();
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

                    }
                });

            if (model.Id == null)
            {
                model.States = ListsHelper.GetStates(null);
                model.TeamsList = teams.OrderBy(x => x.Title).Select(t => new SelectListItem { Text = t.Title.Length > 31 ? t.Title.Substring(0, 31) + "..." : t.Title, Value = t.Id.ToString() });
            }


            if (edit)
            {
                var allCustomers = this.affiliate.ToList();
                if (model.Id != null)
                {
                    allCustomers.Remove(this.affiliate.Get(Convert.ToInt64(model.Id)));
                }

                this.ViewData["customersEmails"] = string.Join(",", allCustomers.Select(x => x.Email).ToList());
            }

            var currentUser = storage.Get();
            if (id != null)
            {
                var helper = new CustomerStoredProcedureHelper(currentUser.Id, id.Value, "affiliate");
                model.AmountOfOpenEstimates = helper.AmountOfOpenEstimates;
                model.SumOfOpenEstimates = helper.SumOfOpenEstimates.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfOpenWorkOrders = helper.SumOfOpenWorkOrders.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfPaidInvoices = helper.SumOfPaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfUnpaidInvoices = helper.SumOfUnpaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }

            return model;
        }

        #endregion
    }
}
