using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.ObjectsComparer;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class RepairOrderLog : ActionLog<RepairOrder, RepairOrderLogActions>
    {
        protected RepairOrderLog()
        {
        }

        public RepairOrderLog(Employee currentEmployee, RepairOrder repairOrder, RepairOrderLogActions action,
            string emails, bool isNewEntity = false)
            : base(currentEmployee, repairOrder, action,emails)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}