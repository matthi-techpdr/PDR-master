using System;
using System.Web.Mvc;
using PDR.Web.Core.Authorization;
using PDR.Domain.StoredProcedureHelpers;

namespace PDR.Web.Areas.Wholesaler.Controllers
{
    [PDRAuthorize(Roles = "Wholesaler")]
    public class EstimateReportsController //: Common.Controllers.ReportsControllers.EstimateReportsController
    {
        //public override JsonResult GetEstimatesForReports(string sidx, string sord, int page, int rows, int? customer, long? team, string startDate, 
        //    string endDate, bool isNeedFilter = false)
        //{
        //    if (this.currentEmployee == null)
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

        //    var estimatesSpHelper = new EstimatesStoredProcedureHelper(this.currentEmployee.Id, team, customer, onlyOwn, rows, page, sidx, sord, isForReport: true,
        //                        dateFrom: start, dateTo: end);
        //    var data = this.estimatesGridMaster.GetData(estimatesSpHelper, rows, page);
        //    return this.Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}
