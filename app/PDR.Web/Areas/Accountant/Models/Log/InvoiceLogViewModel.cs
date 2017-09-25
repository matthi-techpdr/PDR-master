using System.Collections.Generic;
using PDR.Domain.Model.Logging;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Log
{
    public class InvoiceLogViewModel : IViewModel
    {
        public string Id { get; set; }

        public IDictionary<InvoiceLog, string> InvoiceLogs { get; set; }
    }
}