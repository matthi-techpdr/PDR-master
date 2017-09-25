using System;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Reports
{
    public class InvoiceReportItem : CompanyEntity
    {
        protected InvoiceReportItem()
        {
        }

        public InvoiceReportItem(Invoice invoice)
        {
            this.InvoiceID = invoice.Id;
            this.CreationDate = invoice.CreationDate;
            this.CustomerName = invoice.RepairOrder.Estimate.Customer.GetCustomerName();
            this.InvoiceSum = invoice.InvoiceSum;
            this.PaidSum = invoice.PaidSum;
            this.Status = invoice.Status;
            this.Commission = invoice.GetCommission();
        }

        public virtual long InvoiceID { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual double InvoiceSum { get; set; }

        public virtual double PaidSum { get; set; }

        public virtual InvoiceStatus Status { get; set; }

        public virtual InvoiceReport InvoiceReport { get; set; }

        public virtual double Commission { get; set; }
    }
}
