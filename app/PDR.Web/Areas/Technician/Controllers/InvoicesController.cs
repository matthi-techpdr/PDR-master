using System;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Common.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Technician.Controllers
{
    [PDRAuthorize(Roles = "Technician")]
    public class InvoicesController : Common.Controllers.InvoicesController
    {
        public InvoicesController(
            ICompanyRepository<Invoice> invoicesRepository,
            IGridMasterForStoredProcedure<Invoice, InvoiceJsonModelBase, InvoiceViewModelBase> invoiceGridMaster,
            ICurrentWebStorage<Employee> userStorage,
            ICompanyRepository<Customer> customersRepository,
            ICompanyRepository<Estimate> estimates,
            ICompanyRepository<Team> teamsRepository,
            ILogger logger,
            IPdfConverter pdfConverter,
            ICompanyRepository<RepairOrder> roRepository)
            : base(
                invoicesRepository,
                invoiceGridMaster,
                userStorage,
                customersRepository,
                estimates,
                teamsRepository,
                logger,
                pdfConverter,
                roRepository)
        {
        }

        public override ActionResult Index(string vin = null, string stock = null, string custRo = null, bool withFilters=true)
        {
            

            this.ViewBag.Vin = vin;
            this.ViewBag.Stock = stock;
            this.ViewBag.CustRo = custRo;
            this.ViewBag.isStartSearch = false;
            if (!String.IsNullOrEmpty(vin) || !String.IsNullOrEmpty(stock) || !String.IsNullOrEmpty(custRo))
            {
                this.ViewBag.isStartSearch = true;
            }
            FilterModel filters = null;
            if (withFilters)
            {
                var customerCookie = HttpContext.Request.Cookies.Get("customer");
                filters = new FilterModel(typeof (Invoice), false, false, null, customerCookie);
            }
            this.ViewData["statuses"] = ListsHelper.GetStatuses(null, Enum.GetValues(typeof(InvoiceStatus)).Cast<object>()); 
            return this.View(filters);
        }

        [Transaction]
        [HttpPost]
        public void MarkAsOld(long[] ids)
        {
            var invoices = this.invoicesRepository.Where(x => ids.Contains(x.Id)).ToList();
            foreach (var invoice in invoices)
            {
                invoice.New = false;
                this.invoicesRepository.Save(invoice);
            }
        }
    }
}
