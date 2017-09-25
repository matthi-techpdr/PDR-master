function EstimateReport() {
    this.grid = $("#estimateReportsGrid");
    this.getdataurl = "estimatereports/getestimatesforreports?isNeedFilter=true&team=+" + $('#Team').val() + '&customer=' + CookieManager.GetCookie('customer');
//    if ($('#Manager').val() || window.IsAdmin) {

//        this.getdataurl = this.getdataurl + '&team=' + $('#Team').val() + '&customer=' + customer;
//    }
//    else if (CookieManager.GetCookie('customer')) {
//        var customer = CookieManager.GetCookie('customer');
//        this.getdataurl = this.getdataurl + '&customer=' + customer;
//    }
};

$(function () {
    var report = new EstimateReport();
    ReportGridGetter.GetEstimateGrid(report, true);
});

