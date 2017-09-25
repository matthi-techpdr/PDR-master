using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Logging
{
    public abstract class ChangeValueLog: EntityLog<RepairOrder>
    {
        protected ChangeValueLog()
        {
        }

        public ChangeValueLog(RepairOrder repairOrder, Employee currentEmployee, double oldValue, double newValue, bool isNewEntity = false)
            : base(currentEmployee, repairOrder)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;

            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual double OldValue { get; set; }

        public virtual double NewValue { get; set; }
    }
}
