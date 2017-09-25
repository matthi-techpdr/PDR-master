using System;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Reports;
using PDR.Domain.Services.PDFConverters;

using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;

namespace PDR.Web.Areas.Wholesaler.Controllers
{
    using PDR.Domain.StoredProcedureHelpers;

    [PDRAuthorize(Roles = "Wholesaler")]
    public class RepairOrderReportsController //: Common.Controllers.ReportsControllers.RepairOrderReportsController
    {
        //public override JsonResult GetRepairOrdersForReports(string sidx, string sord, int page, int rows, int? customer, long? team, string startDate,
        //    string endDate, bool isNeedFilter = false)
        //{
        //    var currentUser = this.currentEmployee;
        //    if (currentUser == null)
        //    {
        //        return null;
        //    }

        //    var onlyOwn = team == 0;

        //    DateTime s;
        //    var result = DateTime.TryParse(startDate, out s);
        //    var start = result ? s : (DateTime?)null;

        //    DateTime e;
        //    result = DateTime.TryParse(endDate, out e);
        //    var end = result ? e : (DateTime?)null;

        //    var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentUser.Id, team, customer, onlyOwn, rows, page, sidx, sord, isForReport: true,
        //                        dateFrom: start, dateTo: end);
        //    var data = this.roGridMaster.GetData(repairOrdersSpHelper, rows, page);

        //    return this.Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public override ActionResult SaveToPdf(string id, string from, string to, string customer, string team, bool commission, bool report = false)
        //{
        //    var data = new DataForReports
        //    {
        //        Commission = commission,
        //        Customer = GetCustomerForReports(customer),
        //        Employee = this.currentEmployee,
        //        Team = GetTeamForReports(team),
        //        EntityType = "RepairOrder"
        //    };
        //    RepairOrderReport ireport = null;
        //    var currentUser = this.currentEmployee;
        //    IQueryable<RepairOrder> query = this.repairOrdersRepository;

        //    if (currentUser.Role != UserRoles.Wholesaler)
        //    {
        //        query = this.repairOrdersRepository.Where(x => (x.Customer as WholesaleCustomer).Name == GetCustomerForReports(customer));
        //    }

        //    var enddate = new DateTime();
        //    var startdate = new DateTime();

        //    if (report)
        //    {
        //        ireport = this.currentReportTypeRepository.Get(Convert.ToInt64(id));
        //    }

        //    data.DateFrom = string.IsNullOrWhiteSpace(from)
        //                        ? report
        //                              ? startdate.ToString("MM/dd/yyyy")
        //                              : query.Min(x => x.CreationDate).ToString("MM/dd/yyyy")
        //                        : from;

        //    data.DateTo = string.IsNullOrWhiteSpace(to)
        //                      ? report
        //                            ? enddate.ToString("MM/dd/yyyy")
        //                            : query.Max(x => x.CreationDate).ToString("MM/dd/yyyy")
        //                      : to;

        //    if (!string.IsNullOrWhiteSpace(from))
        //    {
        //        startdate = report ? Convert.ToDateTime(ireport.StartDate).Date : Convert.ToDateTime(from).Date;
        //        query = query.Where(x => x.CreationDate >= startdate);
        //    }

        //    if (!string.IsNullOrWhiteSpace(to))
        //    {
        //        enddate = report ? Convert.ToDateTime(ireport.EndDate).Date.AddDays(1).AddSeconds(-1) : Convert.ToDateTime(to).Date.AddDays(1).AddSeconds(-1);
        //        query = query.Where(x => x.CreationDate <= enddate);
        //    }

        //    if (!string.IsNullOrWhiteSpace(customer) && customer != "All customers")
        //    {
        //        long customerId = report ? ireport.CustomerId.Value : Convert.ToInt64(customer);
        //        query = query.Where(x => x.Customer.Id == customerId);
        //    }

        //    if (!string.IsNullOrWhiteSpace(team) && team != "All teams")
        //    {
        //        if (team == "0")
        //        {
        //            query = query.Where(x => x.TeamEmployeePercents.Any(y => y.TeamEmployee == currentUser));
        //        }
        //        else
        //        {
        //            var teamId = report ? ireport.TeamId : Convert.ToInt64(team);
        //            var currentteam = this.teamsRepository.FirstOrDefault(x => x.Id == Convert.ToInt64(teamId));
        //            query = query.Where(x => x.Customer.Teams.Contains(currentteam));
        //        }
        //    }

        //    data.Entities = query.AsEnumerable();

        //    var pdf = this.pdfConverter.ReportConvert(data, currentUser.Company);

        //    if (report || !string.IsNullOrEmpty(id))
        //    {
        //        ireport = this.currentReportTypeRepository.Get(Convert.ToInt64(id));
        //        ReportLogger.Print(ireport);
        //    }
        //    else
        //    {
        //        ReportLogger.PrintNonSaved("repair order", data.DateFrom, data.DateTo, currentUser.Name, team);
        //    }

        //    return new FileContentResult(pdf, "application/pdf");
        //}
    }
}
