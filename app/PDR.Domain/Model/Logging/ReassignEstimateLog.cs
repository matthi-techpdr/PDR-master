using PDR.Domain.Helpers;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class ReassignEstimateLog : EntityLog<Estimate>
    {
        public virtual Employee NewOwner { get; set; }

        public virtual Employee OldOwner { get; set; }

        public virtual Estimate Entity { get; set; }

        protected ReassignEstimateLog()
        {
        }

        public ReassignEstimateLog(
            Employee oldEmployee,
            Estimate estimate,
            Employee doer,
            bool isNewEntity = false)
            : base(doer, estimate)
        {
            this.NewOwner = estimate.Employee;
            this.OldOwner = oldEmployee;
            this.Entity = estimate;
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
                return string.Format(
                    "{0} - Reassign estimate #{1} {2} -> {3}.",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.EntityId,
                    this.OldOwner.Name,
                    this.NewOwner.Name);
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                return string.Format(
                    "Reassign estimate #{0} {1} -> {2}. Customer: {3}.",
                    this.EntityId,
                    this.OldOwner.Name,
                    this.NewOwner.Name,
                    this.Entity.Customer.GetCustomerName());
            }
        }
    }
}