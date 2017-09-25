using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    public class AddAdditionalDiscountLog : ChangeValueLog
    {
        protected AddAdditionalDiscountLog()
        {
        }

        public AddAdditionalDiscountLog(RepairOrder repairOrder, Employee currentEmployee, double oldValue, double newValue, bool isNewEntity = false)
            : base(repairOrder, currentEmployee, oldValue, newValue, isNewEntity)
        {
        }

        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - {1} changed additional discount for repair order #{2}: from {3} to {4}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.Employee.Name,
                    this.EntityId,
                    this.OldValue,
                    this.NewValue);

                return message;
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                var message = string.Format(
                    "Changed additional discount for repair order #{0}: from {1} to {2}",
                    this.EntityId,
                    this.OldValue,
                    this.NewValue);

                return message;
            }
        }
    }
}
