function Invoice(param) {
    this.grid = $("#invoicesGrid");
    this.getdataurl = "invoices/getdata?archived=false&team=" + $('#Teams').val() + '&customer=' 
                        + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
};

$(function () {
    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }
    var invoice = new Invoice(param);
    var counter = false;
    GridInitializer.InitGrid(
        [
            'Creation date',
            'Invoice ID',
            'New',
            'Employee',
            'Customer name',
            'Year/make/model',
            'Invoice amount',
            'Paid amount',
            'Status',
            window.currentUserRole === 'Admin'? 'Commission':'My commission'
        ], [
            { name: 'CreationDate', index: 'CreationDate', width: 160, resizable: false },
            { name: 'Id', index: 'Id', hidden: false, sortable: false, width: 110 },
            { name: 'New', index: 'New', hidden: true },
            { name: 'Employee', index: 'Employee', sortable: false, width: 140 },
            { name: 'Customer_LastName', index: 'Customer_LastName', sortable: false, width: 180 },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 210 },
            { name: 'TotalAmount', index: 'TotalAmount' },
            { name: 'PaidSum', index: 'PaidSum', sortable: false },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: true, width: 80 },
            { name: 'Commission', index: 'Commission', sortable: false, hidden: true }
        ],
        invoice,
        'CreationDate',
        true
    ).navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Invoice.ToPrint(); }, position: "first", title: "" })
     .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Invoice.EmailSending(); }, position: "last", title: "" })
     .navButtonAdd('#pager', { caption: "Archive invoice", onClickButton: function () { Invoice.ToArchived(); }, position: "last", title: "" })
     .navButtonAdd('#pager', { caption: "History", onClickButton: function () { InvoiceCommon.History(invoice.grid, 'invoices/gethistory'); }, position: "last", title: "" })
     .setGridParam({
         gridComplete: function () {
             if (document.all && document.querySelector && !document.addEventListener) {
                 var col = invoice.grid.getColProp("Commission");
                 if (col.hidden) {
                     if (counter) {
                         var elems = invoice.grid.find('tr');
                         for (var e = 0, len = elems.length; e < len; e++) {
                             $(elems[e]).find('td:eq(9)').css('width', '105px');
                         }
                     }
                 } else {
                     counter = true;
                 }
             }
             Helpers.HighlightNew(invoice.grid);
             var ids = invoice.grid.jqGrid('getDataIDs');
             for (var i = 0; i < ids.length; i++) {
                 var id = ids[i];
                 var rowData = invoice.grid.jqGrid('getRowData', id);
                 var models = [{
                     colName: 'Customer_LastName',
                     cellValue: rowData.Customer_LastName,
                     limiter: $('#com').prop('checked') ? 16 : 18
                 }, {
                     colName: 'CarInfo',
                     cellValue: rowData.CarInfo,
                     limiter: 23
                 }, {
                     colName: 'Employee',
                     cellValue: rowData.Employee.split(',').join(',\n'),
                 }];
                 Helpers.CellFormatter(invoice.grid, id, models);
             }
         }
     });

     $('#Teams').change(function () {
         CookieManager.SetCookie("team", $(this).val());
        $('#invoicesGrid').setGridParam({
            url: 'invoices/getdata?team=' + $(this).val() + "&archived=false" + "&customer=" + $('#Customers').val()
        }).trigger("reloadGrid");
    });

    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());
        $('#invoicesGrid').setGridParam({
            url: 'invoices/getdata?team=' + $('#Teams').val() + "&archived=false" + "&customer=" + $(this).val()
        }).trigger("reloadGrid");
    });

    $('#com').change(function () {
        if ($('#com').prop('checked')) {
            $('#invoicesGrid').jqGrid('showCol', "Commission").trigger("reloadGrid");
        } else {
            $('#invoicesGrid').jqGrid('hideCol', "Commission").trigger("reloadGrid");
        }
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#invoicesGrid').setGridParam({
            url: 'invoices/getdata?archived=false' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#invoicesGrid").getGridParam("reccount");
        if (count == 0) {
            jCustomConfirm("Search did not find any Invoices. Would you like to search in archived docs?", "No results found...", function (r) {
                if (r) {
                    var urlRedirect = $("#ArchiveInvoiceUrl").val() + '?' + param;
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
        $('#invoicesGrid').setGridParam({
            url: 'invoices/getdata?archived=false' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

});

Invoice.ToArchived = function () {
    var ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        jConfirm('Archive invoice?', null, function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "invoices/archiveunarchive",
                    data: "ids=" + ids + "&toArchived=true",
                    success: function () {
                        Helpers.Refresh($('#invoicesGrid'), function() {
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
    var ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "invoices/getemaildialog",
            data: "ids=" + ids,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send invoices of different customers in the same message. \tPlease select invoices related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh($('#invoicesGrid'));
                        });
                }
                else {
                    Helpers.GetEmailDialog(data, $('#invoicesGrid'));
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Invoice.ToPrint = function () {
    var ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
            Helpers.Refresh($('#invoicesGrid'), function () {
                for (var i = 0; i < ids.length; i++) {
                    window.open("invoices/print?ids=" + ids[i], "_blank");
                }
            });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
 };

