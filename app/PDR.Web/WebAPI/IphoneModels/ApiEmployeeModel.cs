using System.Linq;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using SmartArch.Data.Proxy;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiEmployeeModel : BaseIPhoneModel
    {
        public ApiEmployeeModel()
        {
        }

        public ApiEmployeeModel(Employee employee)
        {
            if (employee != null)
            {
                this.Id = employee.Id;
                this.Name = employee.Name;
                this.Login = employee.Login;
                this.Role = employee.Role.ToString();
                this.Address = employee.Address;
                this.PhoneNumber = employee.PhoneNumber;
                this.Email = employee.Email;
                this.TaxId = employee.TaxId;
                this.Comment = employee.Comment;
                this.CanQuickEstimate = employee.CanQuickEstimate;
                this.CanExtraQuickEstimate = employee.CanExtraQuickEstimate;
                this.Status = employee.Status.ToString();
                this.HiringDate = employee.HiringDate.ToShortDateString();
                if (employee.IsPersist<TeamEmployee>() || employee is TeamEmployee)
                {
                    var teamEmployee = employee.ToPersist<TeamEmployee>();
                    this.Commission = teamEmployee.Commission;
                    this.TeamNames = string.Join(",  ", teamEmployee.Teams.Select(t => t.Id));
                }

                var estimates = employee.Estimates.ToList();
                this.EstimatesAmount = estimates.Count();
                this.OpenEstimatesAmount = estimates.Count(x => x.EstimateStatus == EstimateStatus.Open);
                this.License = new ApiLicenseModel(employee.Licenses.Single(x => x.Status == LicenseStatuses.Active));
                this.SignatureName = employee.SignatureName;
            }
        }

        public string Name { get; set; }

        public string Login { get; set; }

        public string Role { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string TaxId { get; set; }

        public string Comment { get; set; }

        public bool? CanQuickEstimate { get; set; }

        public bool? CanExtraQuickEstimate { get; set; }

        public string Status { get; set; }

        public string HiringDate { get; set; }

        public ApiLicenseModel License { get; set; }

        public int? EstimatesAmount { get; set; }

        public int? OpenEstimatesAmount { get; set; }

        public int? NewEstimatesAmount { get; set; }

        public int? NewRepairOredersAmount { get; set; }

        public int? NewInvoicesAmount { get; set; }

        public string TeamNames { get; set; }

        public int? Commission { get; set; }

        public string SignatureName { get; set; }
    }
}