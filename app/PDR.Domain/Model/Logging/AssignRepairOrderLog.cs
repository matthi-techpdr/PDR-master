using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class AssignRepairOrderLog : EntityLog<RepairOrder>
    {
        protected AssignRepairOrderLog()
        {
        }

        public AssignRepairOrderLog(RepairOrder repairOrder, TeamEmployee currentEmployee, bool isNewEntity = false)
            : base(currentEmployee, repairOrder)
        {
            var tmp = string.Empty;
            foreach (var teamEmp in repairOrder.TeamEmployeePercents)
            {
                var isRitechnician = teamEmp.TeamEmployee.Role == UserRoles.RITechnician;
                tmp += string.Format("{0} ({1}),",
                                    teamEmp.TeamEmployee.Name,
                                    isRitechnician
                                    ? teamEmp.RiPart + "$"
                                    : teamEmp.EmployeePart + "%");
            }
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }

            this.NewTeamEmployees = tmp.Trim(new []{','});
        }

        public virtual string NewTeamEmployees { get; set; }

        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} assigned employees to repair order #{2}: {3}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.Employee.Name,
                    this.EntityId,
                    this.NewTeamEmployees);

                return message;
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                var message = string.Format(
                    "Assign employees to repair order #{0}: {1}",
                    this.EntityId,
                    this.NewTeamEmployees);

                return message;
            }
        }
    }
}
