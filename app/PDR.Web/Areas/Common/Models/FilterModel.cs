using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Specifications;
using PDR.Domain.StoredProcedureHelpers;
using SmartArch.Data;

namespace PDR.Web.Areas.Common.Models
{
    using System.Web;

    public class FilterModel
    {
        private readonly ICompanyRepository<Team> teamRepository;

        private readonly ICompanyRepository<Estimate> estimateRepository;

        private readonly ICompanyRepository<RepairOrder> roRepository;

        private readonly ICompanyRepository<Invoice> invoiceRepository;

        private readonly Type activityType;

        private readonly bool archived;

        private readonly bool discarded;

        private readonly bool forReport;

        private long? TeamId { get; set; }

        private long? CustomerId { get; set; }

        public FilterModel(Type activityType, bool archived, bool forReport, HttpCookie teamCookie = null, 
                                HttpCookie customerCookie = null, bool discarded = false)
        {
            this.activityType = activityType;
            this.archived = archived;
            this.forReport = forReport;
            this.discarded = discarded;

            var team = teamCookie != null ? teamCookie.Value : null;
            var customer = customerCookie != null ? customerCookie.Value : null;
            long teamId = 0;
            long customerId = 0;
            var resultTryParseTeam = Int64.TryParse(team, out teamId);
            var resultTryParseCustomer = Int64.TryParse(customer, out customerId);
            TeamId = resultTryParseTeam ? teamId : (long?)null;
            CustomerId = resultTryParseCustomer ? customerId : (long?)null;


            this.CurrentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            this.teamRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>();

            this.estimateRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.roRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            this.invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
            this.Customers = this.GetCustomerSelectedList();

            this.Teams = this.GetTeamsSelectedList();
        }

        public readonly Employee CurrentEmployee;

        public IList<SelectListItem> Customers { get; set; }

        public IList<SelectListItem> Teams { get; set; }

        private IList<SelectListItem>   GetCustomerSelectedList()
        {
            //var selectedListTmp = this.GetCustomers().Select(x => new SelectListItem
            //{
            //    Text = x.GetCustomerName(),
            //    Value = x.Id.ToString(),
            //    Selected = CustomerId.HasValue && x.Id == CustomerId.Value
            //});

            //var selectedListTmp = new List<SelectListItem>();

            //var selectedList = selectedListTmp.OrderBy(x => x.Text).ToList();
            var selectedList = new List<SelectListItem>
                                 {
                                     new SelectListItem
                                         {
                                             Text = @"All customers",
                                             Selected = true,
                                             Value = null
                                         }
                                 };
            //selectedList.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });

            //if (CustomerId == null)
            //{
            //    selectedList.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });
            //}
            //else if (selectedList.Any(x => x.Selected))
            //{
            //    var item = selectedList.Single(x => x.Selected);
            //    selectedList.Remove(item);
            //    selectedList.Insert(0, item);
            //    selectedList.Insert(1, new SelectListItem { Text = @"All customers", Selected = false, Value = null });
            //}
            //else
            //{
            //    selectedList.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });
            //}

            return selectedList;
       }

        private IList<SelectListItem> GetTeamsSelectedList()
        {
            var selectedList = new List<SelectListItem>();

            selectedList.AddRange(this.GetTeams().Select(x => new SelectListItem
                                                               {
                                                                   Text = x.Title,
                                                                   Value = x.Id.ToString(),
                                                                   Selected = TeamId.HasValue && x.Id == TeamId.Value
                                                               }).ToList());


            selectedList = selectedList.OrderBy(x => x.Text).ToList();
            selectedList.Insert(0, new SelectListItem { Text = @"All teams", Selected = (TeamId == null || !selectedList.Any(x => x.Selected)), Value = null });

            if (this.CurrentEmployee.Role == UserRoles.Manager || this.CurrentEmployee.Role == UserRoles.Admin)
            {
                if (TeamId == null)
                {
                    selectedList.Insert(1, new SelectListItem { Text = @"My activity only", Selected = false, Value = 0.ToString() });
                }
                else if (TeamId.Value == 0)
                {
                    selectedList.Single(x => x.Value == null).Selected = false;
                    selectedList.Insert(0, new SelectListItem { Text = @"My activity only", Selected = true, Value = 0.ToString() });
                }
                else
                {
                    selectedList.Insert(1, new SelectListItem { Text = @"My activity only", Selected = false, Value = 0.ToString() });
                }
            }
            var item = selectedList.Single(x => x.Selected);
            selectedList.Remove(item);
            selectedList.Insert(0, item);
            return selectedList;
        }

        private IEnumerable<Team> GetTeams()
        {
            var role = this.CurrentEmployee.Role;

            if (role == UserRoles.Admin || role == UserRoles.Accountant || (role == UserRoles.Manager && this.CurrentEmployee.IsShowAllTeams))
            {
                return this.teamRepository;
            }

            if ((role == UserRoles.Manager && !this.CurrentEmployee.IsShowAllTeams) || role == UserRoles.Technician)
            {
                var teams = ((TeamEmployee)this.CurrentEmployee).Teams;
                return teams;
            }

            return new List<Team>();
        }

        //private IEnumerable<Customer> GetCustomers()
        //{
        //    if (this.activityType == typeof(Estimate))
        //    {
        //        //var customers = 
        //        //this.estimateRepository.Find()
        //        //    .All(new EstimatesByUser(this.currentEmployee, this.forReport, this.archived))
        //        //    .Select(x => x.Customer)
        //        //    .Distinct();

        //        var estimateshelper = new EstimatesStoredProcedureHelper(this.currentEmployee.Id, isForReport: this.forReport, isArchived: this.archived);
        //        var customers = estimateshelper.GetCustomersForFilter();
        //        return customers;
        //    }

        //    if (this.activityType == typeof(RepairOrder))
        //    {
        //        //var customers = this.roRepository.Find()
        //        //    .All(new RepairOrdersByUser(this.currentEmployee, this.forReport, this.archived))
        //        //    .Select(x => x.Customer)
        //        //    .Distinct();
        //        var roHelper = new RepairOrdersStoredProcedureHelper(this.currentEmployee.Id, isForReport:this.forReport, isFinalised: this.archived);
        //        var customers = roHelper.GetCustomersForFilter();
        //        return customers;
        //    }

        //    if (this.activityType == typeof(Invoice))
        //    {
        //        //if (this.currentEmployee is Domain.Model.Users.Manager)
        //        //{
        //        //    var invoicesByUserSql = new InvoicesByUserSql(this.currentEmployee, this.forReport, this.archived);
        //        //    var customers = invoicesByUserSql.Customers;
        //        //    return customers;
        //        //}
        //        //else
        //        //{
        //        //    var customers =
        //        //            this.invoiceRepository.Find()
        //        //                .All(new InvoicesByUser(this.currentEmployee, this.forReport, this.archived))
        //        //                .Select(x => x.Customer)
        //        //                .Distinct();
        //        //    return customers;
        //        //}
        //        var customers = new List<Customer>();
        //        if (this.discarded)
        //        {
        //            customers = this.invoiceRepository.Where(x => x.IsDiscard).Select(x => x.Customer).Distinct().ToList();
        //            return customers;
        //        }

        //        var arhived = !this.forReport ? this.archived : (bool?)null;
        //        var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, isArchive: arhived);
        //        customers = invoicesSpHelper.GetCustomersForFilter().ToList();
        //        return customers;
        //    }
        //    return new List<Customer>();
        //}
    }
}