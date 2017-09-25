using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDR.Web.Areas.Accountant.Models.Invoice
{
    public class PaidDialogViewModel
    {
        public long InvoiceId { get; set; }

        public string InvoiceMessage { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string GetDate
        {
            get
            {
                return ((DateTime)this.PaymentDate).ToString("MM/dd/yyyy");
            }
        }

        public string PaidSum { get; set; }
    }
}