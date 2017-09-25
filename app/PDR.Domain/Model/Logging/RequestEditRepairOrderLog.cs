using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Logging
{
    public class RequestEditRepairOrderLog : EntityLog<RepairOrder>
    {
        protected RequestEditRepairOrderLog()
        {
        }

        public RequestEditRepairOrderLog(RepairOrder repairOrder, Employee currentEmployee, bool isNewEntity = false)
            : base(currentEmployee, repairOrder)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} Request to Edit repair order #{2}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.Employee.Name,
                    this.EntityId);

                return message;
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} Request to Edit repair order #{2}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.Employee.Name,
                    this.EntityId);

                return message;
            }
        }
    }
}
