function ReportInfo() {
    var from = $("#from").val() == undefined ? '' : $("#from").val();
    var to = $('#to').val() == undefined ? '' : $("#to").val();
//    var customer = $('#Customers').val() == undefined ? '' : $('#Customers').val();
    var customer = CookieManager.GetCookie('customer') == null ? '' : CookieManager.GetCookie('customer');
    var team = $('#Teams').val() == undefined ? '' : $('#Teams').val();
    
    this.grid = $("#estimateReportInfoGrid");
    this.getdataurl = "../../estimatereports/GetEstimatesForReports?"
        + "startDate=" + from
            + "&endDate=" + to
                + "&customer=" + customer + "&team=" + team;
};

$(function () {
    var reportItem = new ReportInfo();
    ReportGridGetter.GetEstimateGrid(reportItem, false);
});
