using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    public class UnassignRepairOrderLog : EntityLog<RepairOrder>
    {
        public UnassignRepairOrderLog()
        {
        }

        public UnassignRepairOrderLog(RepairOrder repairOrder, TeamEmployee teamEmployee, IEnumerable<TeamEmployee> removedEmployees) 
            : base(teamEmployee, repairOrder)
        {
            this.NewTeamEmployees = string.Join(", ", removedEmployees.Select(x => x.Name));
        }

        public virtual string NewTeamEmployees { get; set; }

        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} removed {2} from RepairOrder #{3}: ",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.Employee.Name,
                    this.NewTeamEmployees,
                    this.Entity.Id);

                foreach (var percent in this.Entity.TeamEmployeePercents.ToList())
                {
                    message += string.Format("{0}({1}%), ", percent.TeamEmployee.Name, percent.EmployeePart);
                }

                return message;
            }
        }
    }
}