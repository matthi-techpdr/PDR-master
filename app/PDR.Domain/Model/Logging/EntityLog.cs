using System;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    public abstract class EntityLog<TEntity> : Log
        where TEntity : Entity
    {
        protected EntityLog()
        {
        }

        protected EntityLog(Employee currentEmployee, TEntity entity) : base(currentEmployee)
        {
            this.EntityId = entity.Id;
        }

        private string GetEmployeeName()
        {
            return Employee!=null? string.Format(" - {0}", this.Employee.Name): String.Empty;
        }

        public virtual long EntityId { get; set; }

        public virtual string GetLogMessage(bool withEmployeeName = false)
        {
            return string.Format(
                "{0}{1}",
               this.LogMessage,
               withEmployeeName ? GetEmployeeName() : String.Empty);
        }

    }
}