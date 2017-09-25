using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PDR.Domain.Helpers;
using PDR.Domain.Model;

using PDR.Web.Areas.Common.Models;

namespace PDR.Web.Areas.Technician.Models
{
    public class InvoiceJsonModel : InvoiceJsonModelBase
    {
        public InvoiceJsonModel(Invoice invoice) : base(invoice)
        {
        }
    }
}