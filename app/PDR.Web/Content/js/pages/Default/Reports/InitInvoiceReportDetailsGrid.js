function ReportInfo() {
    var from = $("#from").val() == undefined ? '' : $("#from").val();
    var to = $('#to').val() == undefined ? '' : $("#to").val();
    //var customer = $('#Customers').val() == undefined ? '' : $('#Customers').val();
    var customer = CookieManager.GetCookie('customer') == null ? '' : CookieManager.GetCookie('customer');
    var team = $('#Teams').val() == undefined ? '' : $('#Teams').val();
    var commission = $('#com').is(':checked');
    
    this.grid = $("#invoiceReportInfoGrid");
    this.getdataurl = "../../invoicereports/GetInvoicesForReports"
        + "?startDate=" + from
            + "&endDate=" + to
                + "&customer=" + customer
                    + "&team=" + team
                        + "&commission=" + commission;
};

$(function () {
    var reportItem = new ReportInfo();
    ReportGridGetter.GetInvoiceGrid(reportItem, false, $('#Commission').val()!='True');
});