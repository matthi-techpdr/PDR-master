using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Web.Core.Helpers
{
    using PDR.Domain.Model.Enums.Extensions;

    public static class ListsHelper
    {
        public static IList<SelectListItem> GetStates(int? currentState)
        {
            var states = Enum.GetValues(typeof(StatesOfUSA)).Cast<int>().Except(new[] { -1 })
                .Select(
                    s => new SelectListItem
                        {
                        Value = s.ToString(),
                        Text = ((StatesOfUSA)s).ToString(),
                        Selected = currentState != null && s == currentState
                    }).ToList();

            return states;
        }

        public static IEnumerable<SelectListItem> GetStatuses(int? currentStatus, IEnumerable<object> enumerable)
        {
            var statuses = enumerable
                    .Select(s => new SelectListItem
                                     {
                                         Value = Convert.ToInt32(s).ToString(),
                                         Text = s is InvoiceStatus ? ((InvoiceStatus)s).GetDescription() : ((LicenseStatuses)s).ToString(),
                                         Selected = currentStatus != null && Convert.ToInt32(s) == currentStatus
                                     }).ToList();
                statuses.Insert(0, new SelectListItem { Text = @"All statuses", Value = null });
            return statuses;
        }

        public static IEnumerable<SelectListItem> GetCustomersSelectListForEstimates(IQueryable<Estimate> estimates, User currentUser, bool withoutConverted = false)
        {
            var customerQuery = estimates;
            if (!(currentUser is Admin))
            {
                customerQuery = customerQuery.Where(e => e.Employee == currentUser);
            }

            if (withoutConverted)
            {
                customerQuery = customerQuery.Where(x => x.EstimateStatus != EstimateStatus.Converted);
            }

            var customers = customerQuery.Where(x => x.Customer != null).Select(c => c.Customer).Distinct().ToList();

            return GetCustomersSelectedList(customers);
        }

        public static IEnumerable<SelectListItem> GetCustomersSelectListForRepairOrders(IQueryable<RepairOrder> repairOrders, User currentUser)
        {
            var customerQuery = repairOrders;
            if (!(currentUser is Admin))
            {
                customerQuery = customerQuery.Where(e => e.TeamEmployee == currentUser);
            }

            var customers = customerQuery.Where(x => x.Customer != null).Select(x => x.Customer).Distinct().ToList();
            return GetCustomersSelectedList(customers);
        }

        public static IEnumerable<SelectListItem> GetCustomersSelectedList(IEnumerable<Customer> customers)
        {
            var customerItems = customers.Count() != 0 ?
                customers.Select(s => new SelectListItem
                                          {
                                              Text = s.GetCustomerName().Length > 25 ? s.GetCustomerName().Substring(0, 25) + "..." : s.GetCustomerName(),
                                              Value = s.Id.ToString()
                                          }).OrderBy(x => x.Text)
                .ToList()
                : new List<SelectListItem>();
            customerItems.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });
            return customerItems;
        }

        private static IEnumerable<SelectListItem> GetTeamsSelectedList(IEnumerable<Team> teams, Employee employee = null)
        {
            var teamItems = teams != null ?
                teams.Select(s => new SelectListItem
                                      {
                                          Text = s.Title.Length > 21 ? s.Title.Substring(0, 21) + "..." : s.Title,
                                          Value = s.Id.ToString()
                                      }).OrderBy(x => x.Text)
                .ToList()
                : new List<SelectListItem>();
            teamItems.Insert(0, new SelectListItem { Text = @"All teams", Selected = true, Value = null });
            if (employee is Manager)
            {
                teamItems.Insert(1, new SelectListItem { Text = @"My activity only", Selected = true, Value = 0.ToString() });
            }

            return teamItems;
        }

        public static IList<SelectListItem> GetAllRoles(int? currentRole, bool insertAll = false, bool addAdmin = false)
        {
            var superadmin = new List<int> { 16, 8, 32, 64 };
            if (addAdmin)
            {
                superadmin.Remove(8);
            }

            var roles = Enum.GetValues(typeof(UserRoles)).Cast<int>().Except(superadmin);
            var rolesSelectItems = 
                roles.Select(
                    r =>
                    new SelectListItem
                        {
                            Value = r.ToString(),
                            Text = ((UserRoles)r).ToString() == "RITechnician" ? "R&I Technician" : ((UserRoles)r).ToString(),
                            Selected = currentRole != null && r == currentRole
                        }).OrderBy(r => r.Text).ToList();
            if (insertAll)
            {
                rolesSelectItems.Insert(0, new SelectListItem { Text = @"All roles", Value = null });
            }

            return rolesSelectItems;
        }

        public static IEnumerable<SelectListItem> GetCustomersSelectListForInvoices(IQueryable<Invoice> invoices, User currentUser)
        {
            List<Customer> customers = null;

            if (currentUser == null || currentUser is Admin)
            {
                customers = invoices.Select(c => c.RepairOrder.Estimate.Customer).Distinct().ToList();
            }
            else
            {
                customers = invoices
                        .Where(e => e.TeamEmployee.Login == currentUser.Login)
                        .Select(c => c.RepairOrder.Estimate.Customer)
                        .Distinct()
                        .ToList();
            }

            return GetCustomersSelectedList(customers);
        }

        public static IEnumerable<SelectListItem> GetTeamsSelectListForInvoices(IQueryable<Invoice> invoices, IQueryable<Team> teams)
        {
            var inv = invoices.ToList();
            var teamEmploeeys = inv
                .SelectMany(c => c.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee))
                        .Distinct();
            var teamsEmp = teamEmploeeys.SelectMany(x => x.Teams).Distinct();

            //var teamsCustomers = inv.SelectMany(x => x.Customer.Teams).Distinct();
            
            //var intersectteams = teamsCustomers.Intersect(teamsEmp);

            //foreach (var teamEmployee in teamEmployees)
            //{
            //    TeamEmployee employee = teamEmployee;
            //    team = teams.Where(x => x.Employees.Contains(employee)).Select(t => t).Distinct().ToList();
            //}

            return GetTeamsSelectedList(teamsEmp);
        }

        public static IEnumerable<SelectListItem> GetTeamsSelectListCurrentManager(IQueryable<Team> teams, Employee employee)
        {
            var teamQuery = teams;
            if (!(employee is Admin))
            {
                teamQuery = teamQuery.Where(x => x.Employees.Contains(employee as TeamEmployee));
            }

            var team = teamQuery.Distinct().ToList();

            return GetTeamsSelectedList(team, employee);
        }

        public static IList<SelectListItem> GetAffiliates(IEnumerable<Affiliate> affiliates, Affiliate curentAffiliate = null)
        {
            var result = affiliates.OrderBy(m => m.Name).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name.Length > 37
                        ? x.Name.Substring(0, 37) + "..."
                        : x.Name,
                Selected = curentAffiliate != null && Equals(x, curentAffiliate) 
            }).ToList();


            result.Insert(0, new SelectListItem { Value = "0", Text = string.Empty });

            return result;
        }
    }
}