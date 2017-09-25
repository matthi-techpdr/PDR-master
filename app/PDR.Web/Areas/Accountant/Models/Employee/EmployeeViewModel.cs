using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Attributes;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Accountant.Validators;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Areas.Accountant.Models.Employee
{
    public class EmployeeViewModel : IViewModel
    {
        public EmployeeViewModel()
        {
        }

        public EmployeeViewModel(bool forAdmin)
        {
            this.Roles = ListsHelper.GetAllRoles(0, addAdmin: forAdmin);
            this.Role = (int)UserRoles.Accountant;
            this.States = ListsHelper.GetStates(null);
        }

        public EmployeeViewModel(Domain.Model.Users.Employee employee, bool forAdmin)
        {
            this.Id = employee.Id.ToString();
            this.Name = employee.Name;
            this.SignatureName = employee.SignatureName;
            this.Address = employee.Address;
            this.PhoneNumber = employee.PhoneNumber;
            this.Email = employee.Email;
            this.TaxId = employee.TaxId;
            this.Login = employee.Login;
            this.Comment = employee.Comment;
            this.Role = (int)employee.Role;
            this.CanQuickEstimate = employee.CanQuickEstimate;
            this.CanExtraQuickEstimate = employee.CanExtraQuickEstimate;
            this.IsShowAllTeams = employee.IsShowAllTeams;
            this.CanEditTeamMembers = employee.CanEditTeamMembers;
            this.Roles = ListsHelper.GetAllRoles((int)employee.Role, addAdmin: forAdmin);
            if (employee.Role != UserRoles.Accountant)
            {
                this.LogMessages = GenerateHistory(employee);
            }

            this.Active = employee.Status == Statuses.Active;
            this.States = ListsHelper.GetStates((int)employee.State);
            this.Zip = employee.Zip;
            this.City = employee.City;
            this.State = employee.State.ToString();
            this.Password = employee.Password;
            //if (this.CanEditTeamMembers.HasValue)
            //{
            //    this.OldCanEditTeamMembers = this.CanEditTeamMembers.Value;
            //}
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string SignatureName { get; set; }

        public string Address { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public IList<SelectListItem> States { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string TaxId { get; set; }

        public string Comment { get; set; }

        public int? Commission { get; set; }

        public int Role { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool Active { get; set; }

        public bool CanQuickEstimate { get; set; }

        public bool CanExtraQuickEstimate { get; set; }

        public bool IsShowAllTeams { get; set; }

        public bool? CanEditTeamMembers { get; set; }

        public string FullLocation
        {
            get
            {
                var msg = string.Empty;
                if (this.City != null)
                {
                    msg += string.Format("{0}, ", this.City);
                }

                msg += string.Format("{0} {1}", this.State, this.Zip);
                return msg;
            }
        }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public IEnumerable<SelectListItem> EmployeeTeams { get; set; }

        public IEnumerable<long> TeamsList { get; set; }

        public IEnumerable<string> LogMessages { get; set; }

        private static IEnumerable<string> GenerateHistory(Domain.Model.Users.Employee employee)
        {
            var msgs = employee.Logs.Where(x => x.Date.Date >= DateTime.Now.AddDays(-30).Date).OrderByDescending(x => x.Date).Select(x =>
                {
                    try
                    {
                        return x.LogMessage;
                    }
                    catch
                    {
                        return null;
                    }
                }).ToList();
            return msgs;
        }
    }
}