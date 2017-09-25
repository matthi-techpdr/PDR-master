using MvcContrib.ActionResults;

using PDR.Web.Areas.Default.Controllers;

namespace PDR.Web.Core.Authorization
{
    /// <summary>
    /// Represents redirect to LogOn page action result.
    /// </summary>
    public class UnauthorizeResult : RedirectToRouteResult<AccountController>
    {
        public UnauthorizeResult() : base(x => x.LogOn(null))
        {
        }

        public UnauthorizeResult(string returnUrl) : base(x => x.LogOn(returnUrl))
        {
        }
    }
}