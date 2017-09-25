using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class InvoiceLog : ActionLog<Invoice, InvoiceLogActions>
    {
        protected InvoiceLog()
        {
        }

        public InvoiceLog(
            Employee currentEmployee,
            Invoice invoice,
            InvoiceLogActions action,
            string emails,
            bool isNewEntity = false)
            : base(currentEmployee, invoice, action, emails)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}