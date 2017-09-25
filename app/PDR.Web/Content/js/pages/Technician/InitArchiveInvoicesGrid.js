function Invoice(param) {
    this.grid = $("#archiveInvoicesGrid");
    this.getdataurl = "getdata?archived=true&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
//    this.getcurrenturl = "employees/getemployee";
//    this.editurl = "employees/editemployee";
//    this.createurl = "employees/createemployee";
//    this.suspendurl = "employees/suspendemployee";
//    this.reactivateurl = "employees/reactivateemployee";
//    this.editform = '#estimateForm';
//    this.editcontainer = $("#estimateInfo");
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
        new Invoice(param),
        'CreationDate', false
    )
        .navButtonAdd('#pager', { caption: "Un-archive", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ToNonArchived(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Discard", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ToDiscard(); }, title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.EmailSending(); }, title: "" })
        .navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ToPrint(); }, title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { InvoiceCommon.History(invoice.grid, 'gethistory'); }, position: "last", title: "" })
        .setGridParam({
                gridComplete: function () {
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
        CookieManager.SetCookie("team", $(this).val());
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $('#Customers').val() + "&team=" +$(this).val()
        }).trigger("reloadGrid");
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#archiveInvoicesGrid").getGridParam("reccount");
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
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true' + '&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
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
///////////////////////////////////////////////////////////////////////////////////////////////
Invoice.ToDiscard = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    var isPaid = false;
    if (ids.length != 0) {
        var rows = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow');
        for (var i = 0; i < rows.length; i++) {
            var item = $('#archiveInvoicesGrid').getRowData(rows[i]);
            if (item.InvoiceStatus == 'Paid' || item.InvoiceStatus == 'Paid In Full') {
                isPaid = true;
                break;
            }
        };

        if (isPaid) {
            jAlert("This action cannot be done for entries with \"Paid\" and \"Paid In Full\" statuses", "Warning!",
                        function () {
                            Helpers.Refresh($('#archiveInvoicesGrid'));
                            return;
                        });
        } else {
            jCustomConfirm('Would you like to discard selected invoice(s)?', null, function (r) {
                if (r) {
                    jCustomConfirm('Would you like to discard corresponded repair order(s)?', null, function (choice) {
                        var d = "ids=" + ids;
                        if (!choice) {
                            d += "&open=" + true;
                        }

                        Helpers.SendAjax("POST", "discard", d, false, function (data) {
                            Helpers.Refresh($('#archiveInvoicesGrid'), function () {
                                jAlert(data, null);
                            });
                        });
                    }, "Yes", "No");

                } else {
                    Helpers.RemoveActiveButtons();
                }
            }, "Yes", "No");
        }
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};
///////////////////////////////////////////////////////////////////////////////////////////////
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