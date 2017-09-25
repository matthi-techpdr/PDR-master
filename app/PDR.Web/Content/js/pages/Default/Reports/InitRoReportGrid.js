function RoReport() {
    this.grid = $("#roReportsGrid");
    this.getdataurl = "repairorderreports/GetRepairOrdersForReports?isNeedFilter=true&customer=" + CookieManager.GetCookie('customer') + '&team=' + $('#Team').val();
//    if ($('#Manager').val() || window.IsAdmin) {
//        this.getdataurl = this.getdataurl + '&team=' + $('#Team').val();
//    }
//    else if (CookieManager.GetCookie('customer')) {
//        var customer = CookieManager.GetCookie('customer');
//        this.getdataurl = this.getdataurl + '&customer=' + customer;
//    }
};

$(function () {
    var report = new RoReport();
    ReportGridGetter.GetRoGrid(report,true);
});
