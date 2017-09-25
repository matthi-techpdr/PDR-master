using PDR.Domain.Helpers;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class ConvertEstimateLog : EntityLog<Estimate>
    {
        public virtual Employee NewOwner { get; set; }

        public virtual Employee OldOwner { get; set; }

        public virtual Estimate Entity { get; set; }

         protected ConvertEstimateLog()
        {
        }

         public ConvertEstimateLog(
            Employee newEmployee,
            Estimate estimate,
            Employee doer,
            bool isNewEntity = false)
            : base(doer, estimate)
        {
            this.NewOwner = newEmployee;
            this.OldOwner = estimate.Employee;
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
                    "{0} - Convert estimate #{1} {2} -> {3}. Customer: {4}.",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.EntityId,
                    this.OldOwner.Name,
                    this.NewOwner.Name,
                    this.Entity.Customer.GetCustomerName());
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                return string.Format(
                    "Convert estimate #{0} {1} -> {2}. Customer: {3}.",
                    this.EntityId,
                    this.OldOwner.Name,
                    this.NewOwner.Name,
                    this.Entity.Customer.GetCustomerName());
            }
        } 
    }
}