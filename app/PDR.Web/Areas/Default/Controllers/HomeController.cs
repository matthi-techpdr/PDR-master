using System.Web.Mvc;

using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Default.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICurrentWebStorage<User> userStorage;

        public HomeController(ICurrentWebStorage<User> userStorage)
        {
            this.userStorage = userStorage;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [PDRAuthorize]
        public ActionResult Index()
        {
            var user = this.userStorage.Get();
            if (user != null)
            {
                switch (user.Role)
                {
                    case UserRoles.Superadmin:
                        return this.RedirectToAction("Index", "Companies", new { area = "SuperAdmin" });
                    case UserRoles.Estimator:
                        return this.RedirectToAction("Index", "Estimates", new { area = "Estimator" });
                    case UserRoles.Accountant:
                        return this.RedirectToAction("Index", "Invoices", new { area = "Accountant" });
                    case UserRoles.Technician:
                        return this.RedirectToAction("Index", "Estimates", new { area = "Technician" });
                    case UserRoles.RITechnician:
                        return this.RedirectToAction("Index", "RepairOrders", new {area = "RITechnician"});
                    case UserRoles.Admin:
                        return this.RedirectToAction("Index", "Estimates", new { area = "Admin" });
                    case UserRoles.Manager:
                        return this.RedirectToAction("Index", "Estimates", new { area = "Manager" });
                    case UserRoles.Wholesaler:
                        return this.RedirectToAction("Index", "Estimates", new { area = "Wholesaler" });
                    case UserRoles.Worker:
                        return this.RedirectToAction("Index", "Download", new { area = "Worker"});
                }
            }
            
            return this.RedirectToAction("LogOn", "Account");
        }
    }
}
