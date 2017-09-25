using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class EstimateLog : ActionLog<Estimate, EstimateLogActions>
    {
        protected EstimateLog()
        {
        }

        public EstimateLog(Employee currentEmployee, Estimate estimate, EstimateLogActions action, string emails, bool isNewEntity = false)
            : base(currentEmployee, estimate, action, emails)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
