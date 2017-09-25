using System.Web.Mvc;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Default.Controllers.Base
{
    [Transaction]
    public abstract class PDRController : Controller
    {
    }
}
