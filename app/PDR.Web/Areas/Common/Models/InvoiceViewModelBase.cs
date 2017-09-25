using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Common.Models
{
    public class InvoiceViewModelBase : IViewModel
    {
        public string Id { get; set; }
    }
}