function Invoice() {
    this.grid = $("#invoiceReportsGrid");
    this.getdataurl = "invoicereports/GetInvoicesForReports?archived=false&isNeedFilter=true&customer=" + CookieManager.GetCookie('customer') + '&team=' + $('#Team').val();
//    if ($('#Manager').val() || window.IsAdmin) {
//        this.getdataurl = this.getdataurl + '&team=' + $('#Team').val();
//    }
//    else if (CookieManager.GetCookie('customer')) {
//        var customer = CookieManager.GetCookie('customer');
//        this.getdataurl = this.getdataurl + '&customer=' + customer;
//    }
};

$(function () {
    var invoice = new Invoice();
    ReportGridGetter.GetInvoiceGrid(invoice, true, true);

    $('#com').change(function () {
        $('#com').prop('checked') ? $('#invoiceReportsGrid').jqGrid('showCol', "Commission").trigger("reloadGrid") : $('#invoiceReportsGrid').jqGrid('hideCol', "Commission").trigger("reloadGrid");
    });
});