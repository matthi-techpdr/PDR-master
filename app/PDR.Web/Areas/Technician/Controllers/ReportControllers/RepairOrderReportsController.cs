using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Technician.Controllers
{
    [PDRAuthorize(Roles = "Technician")]
    public class RepairOrderReportsController : Common.Controllers.ReportsControllers.RepairOrderReportsController
    {
    }
}
