using System.Linq;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class DefinePercentsLog : EntityLog<RepairOrder>
    {
        public DefinePercentsLog()
        {
        }

        public DefinePercentsLog(
            TeamEmployee employee,
            RepairOrder repairOrder,
            bool isNewEntity = false)
            : base(employee, repairOrder)
        {
            this.NewTeamEmployees = string.Join(",", repairOrder.TeamEmployeePercents.Select(x => string.Format("{0}({1}%)", x.TeamEmployee.Name, x.EmployeePart)));
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual string NewTeamEmployees { get; set; }

        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} defined percents for repair order #{2}: {3}",
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
                    "Define percents for repair order #{0}: {1}",
                    this.EntityId,
                    this.NewTeamEmployees);

                return message;
            }
        }
    }
}