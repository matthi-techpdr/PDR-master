using System.Collections.Generic;
using System.Web.Mvc;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Areas.Common.Models
{
    using PDR.Domain.Services.Grid.Interfaces;

    public class SettingsViewModel : IViewModel
    {
        public SettingsViewModel(){}

        public SettingsViewModel(Domain.Model.Users.Employee employee)
        {
            this.Id = employee.Id.ToString();
            this.Name = employee.Name;
            this.SignatureName = employee.SignatureName;
            this.Address = employee.Address;
            this.PhoneNumber = employee.PhoneNumber;
            this.Email = employee.Email;
            this.TaxId = employee.TaxId;
            this.Login = employee.Login;
            this.Role = employee.Role.ToString();
            this.CanQuickEstimate = employee.CanQuickEstimate;
            this.CanExtraQuickEstimate = employee.CanExtraQuickEstimate;
            this.States = ListsHelper.GetStates((int)employee.State);
            this.Zip = employee.Zip;
            this.City = employee.City;
            this.State = employee.State.ToString();
            this.Password = employee.Password;
            this.IsBasic = employee.IsBasic;
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

        public int? Commission { get; set; }

        public string Role { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool CanQuickEstimate { get; set; }

        public bool CanExtraQuickEstimate { get; set; }

        public bool IsBasic { get; set; }

        public IList<string> EmployeeTeams { get; set; }
    }
}