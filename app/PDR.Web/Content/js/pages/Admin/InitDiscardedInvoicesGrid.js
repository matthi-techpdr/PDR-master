function Invoice(param) {
    this.grid = $("#discardedInvoicesGrid");
    this.getdataurl = "getdata?archived=true&discarded=true&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
};

$(function () {
    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }
    var invoice = new Invoice(param);

    GridInitializer.InitGrid(
        [
            'Creation date',
            'Invoice ID',
            'Employee',
            'Customer name',
            'Year/make/model',
            'Invoice amount',
            'Paid amount',
            'Status'
        ], [
            { name: 'CreationDate', index: 'CreationDate',  width: 130 },
            { name: 'Id', index: 'Id', sortable: false, width: 120 },
            { name: 'Employee', index: 'Employee', sortable: false, width: 130 },
            { name: 'Customer_LastName', index: 'Customer_LastName', sortable: false, width: 200 },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
            { name: 'TotalAmount', index: 'TotalAmount', width: 160 },
            { name: 'PaidSum', index: 'PaidSum', width: 160, sortable: false },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: false, width: 100 }
        ],
        new Invoice(param),
        'CreationDate', false
    )
        .navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ToPrint(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Reinstate invoice", onClickButton: function () { Helpers.RemoveActiveButtons(); InvoiceCommon.ReinstateInvoice(); }, title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { InvoiceCommon.History(invoice.grid, 'gethistory'); }, position: "last", title: "" })
        .setGridParam({
                gridComplete: function () {
                    var ids = $("#discardedInvoicesGrid").jqGrid('getDataIDs');
                    for (var i = 0; i < ids.length; i++) {
                        var id = ids[i];
                        var rowData = $("#discardedInvoicesGrid").jqGrid('getRowData', id);
                        var models = [{
                            colName: 'Customer_LastName',
                            cellValue: rowData.Customer_LastName,
                            limiter: 22
                        }, {
                            colName: 'CarInfo',
                            cellValue: rowData.CarInfo,
                            limiter: 22
                        }];
                        Helpers.CellFormatter($("#discardedInvoicesGrid"), id, models);
                    }
                }
            });
    
    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());
        $('#discardedInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&discarded=true&customer=' + $(this).val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        $('#discardedInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&discarded=true&customer=' + $('#Customers').val() + "&team=" + $(this).val()
        }).trigger("reloadGrid");
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#discardedInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&discarded=true' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#discardedInvoicesGrid").getGridParam("reccount");
        if (count == 0) {
            jCustomConfirm("Search did not find any Invoices. Would you like to search in active docs?", "No results found...", function (r) {
                if (r) {
                    var urlRedirect = $("#InvoiceUrl").val() + '?' + param;
                    window.location.assign(urlRedirect);
                }
                else {

                }
            }, "Yes", "No");
        } else {
            $(".h3-wrapper").trigger('click');
        }
    });

    $('.reset-button, .reset-link > a').click(function () {
        resetSearch();
        $('#discardedInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&discarded=true' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

});

Invoice.ToPrint = function () {
    var ids = $('#discardedInvoicesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        Helpers.Refresh($('#discardedInvoicesGrid'), function () {
            for (var i = 0; i < ids.length; i++) {
                window.open("print?ids=" + ids[i], "_blank");
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};