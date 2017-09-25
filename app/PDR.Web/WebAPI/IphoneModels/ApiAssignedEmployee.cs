using PDR.Domain.Model;
using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiAssignedEmployee
    {
        public ApiAssignedEmployee()
        {
        }

        public ApiAssignedEmployee(TeamEmployeePercent percent)
        {
            var employee = percent.TeamEmployee;
            this.Employee = employee.Name;
            this.Part = percent.EmployeePart;
            this.Id = employee.Id;
            this.IsRiTechnician = employee.Role == UserRoles.RITechnician;
        }

        public long Id { get; set; }

        public string Employee { get; set; }

        public double Part { get; set; }

        public bool IsRiTechnician { get; set; }
    }
}