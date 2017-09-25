using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Manager.Controllers
{
    [PDRAuthorize(Roles = "Manager")]
    public class EstimateReportsController : Common.Controllers.ReportsControllers.EstimateReportsController
    {
    }
}
