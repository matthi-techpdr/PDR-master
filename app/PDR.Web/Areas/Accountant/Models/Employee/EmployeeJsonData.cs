using System.Linq;

using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Employee
{
    public class EmployeeJsonData : IJsonModel
    {
        public EmployeeJsonData()
        {
        }

        public EmployeeJsonData(Domain.Model.Users.Employee employee)
        {
            this.Id = employee.Id.ToString();
            this.Name = employee.Name;
            this.Email = employee.Email ?? string.Empty;
            this.HiringDate = employee.HiringDate.ToString("MM/dd/yyyy");
            this.PhoneNumber = employee.PhoneNumber;
            var teamEmployee = employee as TeamEmployee;
            this.Team = teamEmployee != null ? string.Join(", ", teamEmployee.Teams.Select(t => t.Title)) : string.Empty;
            this.Role = employee.Role.ToString();
            this.Status = employee.Status.ToString();
            this.OpenEstimatesAmount = employee.Estimates.Where(e => e.EstimateStatus == EstimateStatus.Open).Count();
            this.OpenWorkOrdersAmount = employee is TeamEmployee ? 
                ((TeamEmployee)employee)
                .RepairOrders
                .Where(x => x.RepairOrderStatus == RepairOrderStatuses.Open).Count()
                                            : 0;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string HiringDate { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Team { get; set; }

        public string Role { get; set; }

        public int OpenEstimatesAmount { get; set; }

        public int OpenWorkOrdersAmount { get; set; }

        public string Status { get; set; }
    }
}