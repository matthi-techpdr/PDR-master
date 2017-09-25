using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Logging
{
    public class PaidInvoiceLog : EntityLog<Invoice>
    {
         protected PaidInvoiceLog()
        {
        }

         public PaidInvoiceLog(Invoice invoice, Employee currentEmployee, bool isNewEntity = false)
             : base(currentEmployee, invoice)
         {
             this.Status = invoice.Status;
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual InvoiceStatus Status { get; set; }


        public override string LogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - Invoice #{1} was {2}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.EntityId,
                    this.Status == InvoiceStatus.PaidInFull? "Paid in Full": Status.ToString());

                return message;
            }
        }

        public virtual string FileLogMessage
        {
            get
            {
                var message = string.Format(
                    "{0} - Invoice #{1} was {2}",
                    string.Format("{0:MM/dd/yyyy HH:mm:ss}", this.Date),
                    this.EntityId,
                    this.Status == InvoiceStatus.PaidInFull ? "Paid in Full" : Status.ToString());

                return message;
            }
        }
    }
}
