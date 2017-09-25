function Invoice() {
    this.grid = $("#archiveInvoicesGrid");
    this.getdataurl = "getdata?archived=true&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') + '&isNeedFilter=true';
};

$(function () {
    GridInitializer.InitGrid(
        [
            'Creation date',
            'Invoice ID',
            'Customer name',
            'Year/make/model',
            'Invoice amount',
            'Paid amount',
            'Status'
        ], [
            { name: 'CreationDate', index: 'CreationDate',  width: 160 },
            { name: 'Id', index: 'Id', sortable: false, width: 160 },
            { name: 'Customer_LastName', index: 'Customer_LastName', sortable: false, width: 200 },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
            { name: 'TotalAmount', index: 'TotalAmount', width: 160 },
            { name: 'PaidSum', index: 'PaidSum', width: 160, sortable: false },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: false, width: 100 }
        ],
        new Invoice(),
        'CreationDate', false
    )
        
        .setGridParam({
            gridComplete: function () {
                var boxes = $('.cbox');
                $.each(boxes, function () {
                    $(this).hide();
                });

                    var ids = $("#archiveInvoicesGrid").jqGrid('getDataIDs');
                    for (var i = 0; i < ids.length; i++) {
                        var id = ids[i];
                        var rowData = $("#archiveInvoicesGrid").jqGrid('getRowData', id);
                        var models = [{
                            colName: 'Customer_LastName',
                            cellValue: rowData.Customer_LastName,
                            limiter: 23
                        }, {
                            colName: 'CarInfo',
                            cellValue: rowData.CarInfo,
                            limiter: 22
                        }];
                        Helpers.CellFormatter($("#archiveInvoicesGrid"), id, models);
                    }
                }
            });
    
    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $(this).val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#Teams').change(function () {
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $('#Customers').val() + "&team=" +$(this).val()
        }).trigger("reloadGrid");
    });
});

Invoice.GetEstimateData = function (id) {
    return $.ajax({
        type: "GET",
        url: "getinvoice",
        data: id.replace('?', '') + "&edit=" + false,
        cache: false,
        success: function (data) {
            $("#companyInfo").html(data);
        }
    });
};

Invoice.ToNonArchived = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        jConfirm('Unarchive invoice?', null, function (r) {
            Helpers.RemoveActiveButtons();
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "archiveunarchive",
                    data: "ids=" + ids + "&toArchived=false",
                    cache: false,
                    success: function () {
                        Helpers.Refresh($('#archiveInvoicesGrid'), function () {
                            jAlert("Operation completed", "");
                        });
                    }
                });
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Invoice.EmailSending = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "getemaildialog",
            data: "ids=" + ids,
            cache: false,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send invoices of different customers in the same message. \tPlease select invoices related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh($('#archiveInvoicesGrid'));
                        });
                }
                else {
                    Helpers.GetEmailDialog(data, $('#archiveInvoicesGrid'));
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Invoice.ToPrint = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
            Helpers.Refresh($('#archiveInvoicesGrid'), function () {
                for (var i = 0; i < ids.length; i++) {
                    window.open("print?ids=" + ids[i], "_blank");
                }
            });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};