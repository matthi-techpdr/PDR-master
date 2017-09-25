using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Manager.Controllers
{
    [PDRAuthorize(Roles = "Manager")]
    public class InvoiceReportsController : Common.Controllers.ReportsControllers.InvoiceReportsController
    {
    }
}
