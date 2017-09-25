using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Common.Models;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Wholesaler.Controllers
{
    [PDRAuthorize(Roles = "Wholesaler")]
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
            : base(invoicesRepository,
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

        [HttpGet]
        public override ActionResult Print(string ids, bool? isBasic = true)
        {
            return base.Print(ids, isBasic);
        }
    }
}
