using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Technician.Controllers
{
    [PDRAuthorize(Roles = "Technician")]
    public class EstimateReportsController : Common.Controllers.ReportsControllers.EstimateReportsController
    {
    }
}
