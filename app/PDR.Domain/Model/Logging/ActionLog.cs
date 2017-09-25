using PDR.Domain.Model.Base;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.ObjectsComparer;

namespace PDR.Domain.Model.Logging
{
    using PDR.Domain.Model.Enums.LogActions;

    public abstract class ActionLog<TEntity, TAction> : EntityLog<TEntity>
        where TEntity : Entity
    {
        protected ActionLog()
        {
        }

        protected ActionLog(Employee currentEmployee, TEntity entity, TAction action, string emails = null)
            : base(currentEmployee, entity)
        {
            this.Action = action;
            this.Emails = emails;
        }

        public virtual string Emails { get; set; }

        public virtual TAction Action { get; protected set; }

        public override string LogMessage
        {
            get
            {
                var action = this.Action.ToString() == EstimateLogActions.RepeatVehicleEntryConfirmed.ToString() 
                    ? "Repeat vehicle entry confirmed by " + this.Employee.Name + ";"
                    : this.Action.ToString();
                return string.Format(
                    "{0} {1} {2} - #{3}.{4}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    action,
                    LogHelper.SplitPropertyName(typeof(TEntity).Name),
                    this.EntityId, 
                    string.IsNullOrWhiteSpace(this.Emails) ? string.Empty : " Emails: " + this.Emails);
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                return string.Format(
                    "{0} {1} - #{2}.{3}",
                    this.Action,
                    LogHelper.SplitPropertyName(typeof(TEntity).Name),
                    this.EntityId,
                    string.IsNullOrWhiteSpace(this.Emails) ? string.Empty : " Emails: " + this.Emails);
            }
        }
    }
}