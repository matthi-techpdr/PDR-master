using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Model.Users;
using PDR.Web.Areas.Estimator.Models.Reports;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Areas.Technician.Controllers
{
    using System;

    [PDRAuthorize(Roles = "Technician")]
    public class InvoiceReportsController : Common.Controllers.ReportsControllers.InvoiceReportsController
    {
        public override ActionResult Index()
        {
            var customerCookie = HttpContext.Request.Cookies.Get("customer");
            var customer = customerCookie != null ? customerCookie.Value : null;

            var currentUser = this.currentEmployee as TeamEmployee;
            var repairOrders = currentUser.TeamEmployeePercents.Select(x => x.RepairOrder).ToList();
            var customers = this.invoicesRepository.Where(x => repairOrders.Contains(x.RepairOrder) && !x.Archived).Select(x => x.Customer).Distinct().ToList();
            var listCustomers = ListsHelper.GetCustomersSelectedList(customers).ToList();
            if (customer != null)
            {
                listCustomers.Single(x => x.Selected).Selected = false;
                var item = listCustomers.FirstOrDefault(x => x.Value == customer);
                listCustomers.Remove(item);
                item.Selected = true;
                listCustomers.Insert(0, item);
            }

            this.ViewData["customers"] = listCustomers;

            var userReports = this.currentReportTypeRepository.Where(x => x.Employee == this.currentEmployee);

            var model = userReports.ToList().Select(x => new ReportModel { Title = x.Title, Id = x.Id }).ToList();

            ViewBag.ExistReportNames = userReports.Select(x => x.Title).ToList();

            return this.View(model);
        }
    }
}
