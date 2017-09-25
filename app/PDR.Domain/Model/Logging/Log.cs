using System;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    public abstract class Log : CompanyEntity
    {
        protected Log()
        {
        }

        protected Log(Employee currentEmployee)
        {
            this.Date = DateTime.Now;
            this.Employee = currentEmployee;
        }

        public virtual DateTime Date { get; set; }

        public virtual Employee Employee { get; set; }

        public abstract string LogMessage { get; }

    }
}