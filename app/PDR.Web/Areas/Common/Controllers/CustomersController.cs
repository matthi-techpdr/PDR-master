using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Areas.Estimator.Models.Customers;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Common.Controllers
{
    using System.Collections.Generic;

    using PDR.Domain.Specifications;
    using PDR.Domain.StoredProcedureHelpers;

    [PDRAuthorize]
    public class CustomersController : Controller
    {
        protected readonly IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, WholesaleCustomerViewModel> wholesaleCustomersGridMaster;

        protected readonly Employee currentEmployee;

        protected readonly ICompanyRepository<Customer> customersRepository; 

        protected readonly ICompanyRepository<Estimate> estimatesRepository;

        protected readonly IRepositoryFactory repositoryFactory;

        private IGridMasterForStoredProcedure<Estimate, CustomerEstimateJsonModel, CustomerViewModel> estimatesGridMaster;

        private readonly IGridMasterForStoredProcedure<RepairOrder, CustomersOrderJsonModel, CustomerViewModel> repairOrderGridMaster;

        private readonly IGridMasterForStoredProcedure<Invoice, CustomersInvoiceJsonModel, CustomerViewModel> invoicesGridMaster;

        public CustomersController(
            IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, WholesaleCustomerViewModel> wholesaleCustomersGridMaster)
        {
            this.wholesaleCustomersGridMaster = wholesaleCustomersGridMaster;
        }

        public CustomersController(
            IGridMaster<WholesaleCustomer, WholesaleCustomerJsonModel, WholesaleCustomerViewModel> wholesaleCustomersGridMaster,
            ICurrentWebStorage<Employee> currentEmployee,
            ICompanyRepository<Estimate> estimatesRepository,
            IRepositoryFactory repositoryFactory) : this(wholesaleCustomersGridMaster)
        {
            this.estimatesRepository = estimatesRepository;
            this.currentEmployee = currentEmployee.Get();
            this.repositoryFactory = repositoryFactory;

            this.estimatesGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Estimate, CustomerEstimateJsonModel, CustomerViewModel>>();
            this.repairOrderGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<RepairOrder, CustomersOrderJsonModel, CustomerViewModel>>();
            this.invoicesGridMaster = ServiceLocator.Current.GetInstance<IGridMasterForStoredProcedure<Invoice, CustomersInvoiceJsonModel, CustomerViewModel>>();
            this.customersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>();
        }

        public ActionResult Index()
        {
            var states = ListsHelper.GetStates(null).ToList();
            states.Insert(0, new SelectListItem { Text = @"All states", Selected = true, Value = null });
            if (!(this.currentEmployee is Domain.Model.Users.Technician))
            {
                var teamFilter = new FilterModel(null, false, false);
                ViewBag.Teams = teamFilter.Teams;
            }

            return this.View(states);
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows, int? state, long? team)
        {
            //if (state.HasValue)
            //{
            //    this.wholesaleCustomersGridMaster.ExpressionFilters.Add(x => x.State == (StatesOfUSA)state.Value);
            //}

            //if (team.HasValue && team != 0)
            //{
            //    var currentTeam = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>().Get(team.Value);
            //    this.wholesaleCustomersGridMaster.ExpressionFilters.Add(x => x.Teams.Contains(currentTeam));
            //}

            //if (!(this.currentEmployee is Domain.Model.Users.Estimator))
            //{
            //    var teams = ((TeamEmployee)this.currentEmployee).Teams;
            //    var customers = teams.SelectMany(x => x.Customers).Distinct().ToList();
            //    this.wholesaleCustomersGridMaster.ExpressionFilters.Add(x => customers.Contains(x));
            //}

            //GridModel<WholesaleCustomerJsonModel> data = this.wholesaleCustomersGridMaster.GetData(
            //    page,
            //    rows,
            //    sidx,
            //    sord,
            //    additionalModelParam: team);

            if (currentEmployee == null)
            {
                return null;
            }
            var onlyOwn = team.HasValue && team == 0;
            var manager = new CustomersStoredProcedureHelper(currentEmployee.Id, "wholesalecustomer", team, state, onlyOwn, rows, page, sord);
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

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        protected WholesaleCustomerViewModel GetCustomerViewModel(long? id)
        {
            WholesaleCustomerViewModel model = this.wholesaleCustomersGridMaster.GetEntityViewModel(id);
            if (id != null)
            {
                var helper = new CustomerStoredProcedureHelper(currentEmployee.Id, id.Value, "wholesalecustomer");
                model.AmountOfOpenEstimates = helper.AmountOfOpenEstimates;
                model.SumOfOpenEstimates = helper.SumOfOpenEstimates.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfOpenWorkOrders = helper.SumOfOpenWorkOrders.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfPaidInvoices = helper.SumOfPaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                model.SumOfUnpaidInvoices = helper.SumOfUnpaidInvoices.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }
            return model;
        }


        public virtual ActionResult GetCustomerDetails(long? id)
        {
            WholesaleCustomerViewModel model = this.GetCustomerViewModel(id);
            var manager = this.currentEmployee as Domain.Model.Users.Manager;
            if (manager != null)
            {
                var teams = manager.Teams.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).ToList();
                teams.Insert(0, new SelectListItem { Text = @"All teams" });
                teams.Insert(1, new SelectListItem { Text = @"My activity only", Value = 0.ToString() });

                ViewBag.Teams = teams;
            }
            
            return this.PartialView("CustomerDetails", model);
        }

        [HttpPost]
        public ActionResult GetEmailDialog(string ids, bool customer = false)
        {
            var to = string.Empty;
            if (customer)
            {
                to = this.repositoryFactory.CreateForCompany<Customer>().Get(Convert.ToInt64(ids)).Email ?? string.Empty;
            }
            //else
            //{
            //    var estimatesIds = ids.Split(',');

            //    if (this.CheckEstimatesForTheSameCustomer(estimatesIds) && estimatesIds.Length > 1)
            //    {
            //        return new JsonResult { Data = "Error" };
            //    }

            //    foreach (var id in estimatesIds)
            //    {
            //        var email = this.estimatesRepository.Get(Convert.ToInt64(id)).Customer.Email ?? string.Empty;
            //        if (!string.IsNullOrWhiteSpace(email))
            //        {
            //            to += email + ", ";
            //        }
            //    }
            //}

            return this.PartialView(new SendEmailViewModel { To = to.TrimEnd(',', ' '), IDs = ids });
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendEmail(SendEmailViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var from = this.currentEmployee.Company.Email;
                var mes = new MailService().Send(from, model.To, model.Subject, model.Message, null);
                return this.Json(mes, JsonRequestBehavior.AllowGet);
            }

            return this.Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEstimates(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
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
            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(this.currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord);
            var data = this.repairOrderGridMaster.GetData(repairOrdersSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }

        [HttpGet]
        public JsonResult GetInvoices(string sidx, string sord, int page, int rows, long customer, long? team)
        {
            var onlyOwn = team == 0;
            var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, team, isOnlyOwn: onlyOwn, filterByCustomers: customer, pageNumber: page,
                                                rowsPerPage: rows, sortByColumn: sidx, sortType: sord);
            var data = this.invoicesGridMaster.GetData(invoicesSpHelper, rows, page);
            var json = new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return json;
        }
    }
}
