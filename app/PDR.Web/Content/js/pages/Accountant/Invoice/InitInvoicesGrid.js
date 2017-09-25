function Invoice() {
    this.grid = $("#invoicesGrid");
    this.getdataurl = "invoices/getdataforaccountant?archived=null&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer');
};

function InvoiceMarker() {
    this.ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow');
    this.idItem = 0;

    this.Dowork = function () {
        if (this.ids.length != 0) {
            for (var i = 0; i < this.ids.length; i++) {
                this.hand = new DialogHandler(this.ids[i]);
                this.hand.dosomething();
            }
        }
        else {
            jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
        }
    };
};

function DialogHandler(id) {
    this.invoiceId = id;

    this.dosomething = function () {
        var ident = this.invoiceId;
        $.ajax({
            type: "POST",
            url: "invoices/getdialog",
            data: "id=" + ident,
            cache: false,
            success: function (data) {
                var item = new CreatePaidDialog(data, $("#invoicesGrid"), ident);
                item.GetDialog(item.invoiceId);
            }
        });
    };
};

function CreatePaidDialog(data, grid, id) {
    this.invoiceId = id;
    this.container = document.createElement('div');
    this.dlg = $(this.container).addClass('paidDialog_' + this.invoiceId);
    this.dlg.html(data);
    this.remove = function () {
        this.dlg.remove();
        Helpers.RemoveActiveButtons();
    };

    this.GetDialog = function (inerInvoiceId) {
        this.dlg.dialog({
            width: 440,
            height: 'auto',
            modal: false,
            open: function () {
                $('#PaymentFlatFeeDate_' + inerInvoiceId).datepicker({
                    maxDate: "+0D",
                    autoOpen: false,
                    fixFocusIE: true,

                    onSelect: function (dateText, inst) {
                        this.fixFocusIE = true;
                        $(this).blur().change().focus();
                    },
                    onClose: function (dateText, inst) {
                        this.fixFocusIE = true;
                        this.focus();
                    },
                    beforeShow: function (input, inst) {
                        var result = !this.fixFocusIE;
                        this.fixFocusIE = false;
                        return result;
                    },
                });
                $('#PaymentFlatFeeDate_' + inerInvoiceId)[0].disabled = true;
                setTimeout(function () {
                    $('#PaymentFlatFeeDate_' + inerInvoiceId)[0].disabled = false;
                }, 100);
                $('[id^="PaidSum"]').focus();
                ValidateAjaxForm($('.paidInvoiceForm'));
            },
            close: function () {
                $('#PaymentFlatFeeDate_' + inerInvoiceId).remove();
                Helpers.RemoveActiveButtons();
            },
            dialogClass: "dlg_" + inerInvoiceId,
            beforeClose: function () {
            },
            resizable: false,
            title: 'Paid invoice',
            buttons:
                        [{ width: 150, text: "Invoice is paid in full", click: function () {
                            Paid(true, grid, inerInvoiceId);
                        }
                        },
                         { width: 150, text: "Invoice is paid partially", click: function () {
                             Paid(false, grid, inerInvoiceId);
                         }
                         },
                         { width: 82, text: "Cancel", click: function () {
                             $(this).dialog('close');
                         }
                         }]
        });

    };
};

function Paid(isFullSum, grid, id) {
    var reg = /^\d*\.?\d{1,}?$/g;
    var paiddate = $('#PaymentFlatFeeDate_' + id).val();
    var sum = $('#PaidSum_' + id).val() == '' ? " " : $('#PaidSum_' + id).val();
    var invoiceStatus = isFullSum ? 'PaidInFull' : 'Paid';
    if (!(isFullSum)) {
        if (!reg.test(sum)) {
            sum = null;
        }
    }

    if (paiddate && sum) {

        $.ajax({
            type: "POST",
            url: 'invoices/paidinvoice',
            data: "paiddate=" + paiddate + "&sum=" + sum + "&id=" + id,
            cache: false,
            success: function (data) {
                if (data.length > 1) {
                    jAlert(data);
                }
                else {
                    $.ajax({
                        type: "GET",
                        url: 'invoices/paidsave',
                        cache: false,
                        data: "paiddate=" + paiddate + "&sum=" + sum + "&id=" + id + "&invoiceStatus=" + invoiceStatus,
                        success: function () {
                            Helpers.Refresh($("#invoicesGrid"));//                            
                        }
                    });
                    $('.paidDialog_' + id).remove();
                }
            }
        });
    }
};

$(function () {
    $(".repair-orders-content").addClass("invoicesAccountant");
    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var invoice = new Invoice();
    GridInitializer.InitGrid(
        [
            'Creation date',
            'Invoice ID',
            'Customer name',
            'Year/make/model',
            'Team',
            'Employee(s)',
            'Invoice amount',
            'Paid amount',
            'Status',
            'Paid date',
            'Commission'
            ], [
            { name: 'CreationDate', index: 'CreationDate', width: 170 },
            { name: 'Id', index: 'Id', hidden: false, sortable: false, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Invoice.GetLogData(\'', addParam: '\');' }, width: 130 },
            { name: 'Customer_LastName', index: 'Customer_LastName', sortable: false, width: 200 },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 240 },
            { name: 'Team', index: 'Team', sortable: false, width: 140 },
            { name: 'Technicians', index: 'Technicians', sortable: false, width: 300 },
            { name: 'TotalAmount', index: 'TotalAmount', width: 110 },
            { name: 'PaidSum', index: 'PaidSum', sortable: false, editable: true, edittype: 'text', width: 110 },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: false, width: 130 },
            { name: 'PaidDate', index: 'PaidDate', sortable: false, width: 130 },
            { name: 'Commission', index: 'Commission', sortable: false, hidden: true }
        ],
        invoice,
        'CreationDate',
        true
    ).navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ToPrint(); }, position: "first", title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.EmailSending(); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Mark invoice as paid", onClickButton: function () { Helpers.RemoveActiveButtons(); new InvoiceMarker().Dowork(); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Export to QuickBook", onClickButton: function () { Helpers.RemoveActiveButtons(); Invoice.ExportToXLS(); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { InvoiceCommon.History(invoice.grid, 'invoices/gethistory'); }, position: "last", title: "" })
        .setGridParam({
            afterSubmitCell: function (serverresponse, rowid, cellname, value, iRow, iCol) {
                var data = jQuery.parseJSON(serverresponse.responseText);
                if (data != null) {
                    jAlert(data['Message'], "Warning!", function () { $("#invoicesGrid").trigger("reloadGrid"); });
                }
                return [true, ""];
            },
            serializeCellData: function (postdata) {
                var reg = /^\d*\.?\d{1,}?$/g;
                if (!reg.test(postdata.PaidSum)) {
                    postdata.PaidSum = 0;
                }
                return postdata;
            },
            afterSaveCell: function (rowid, celname, value, iRow, iCol) {
                $("#invoicesGrid").trigger("reloadGrid");
            },
            cellEdit: true,
            cellsubmit: "remote",
            cellurl: 'invoices/setdata',
            onSelectCell: function (rowid, celname, value, iRow, iCol) {
                $(".edit-cell").removeClass("ui-state-highlight");
            },
            beforeEditCell: function (rowid, cellname, value, iRow, iCol) {
                $(".edit-cell").removeClass("ui-state-highlight");
            },
            afterEditCell: function (rowid, cellname, value, iRow, iCol) {
                var sum = value.replace("$", "");
                $(".edit-cell input").val(sum);
            },
            gridComplete: function () {
                var ids = invoice.grid.jqGrid('getDataIDs');
                var teamVal = $('#Teams').val();
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = invoice.grid.jqGrid('getRowData', id);
                    var models = [{
                        colName: 'Customer_LastName',
                        cellValue: rowData.Customer_LastName,
                        limiter: 16
                    }, {
                        colName: 'CarInfo',
                        cellValue: rowData.CarInfo,
                        limiter: 18
                    }];
                    if (teamVal.toLowerCase() != 'all teams') {
                        models.push({
                            colName: 'Team',
                            cellValue: rowData.Team,
                            limiter: 7
                        });
                    }
                    Helpers.CellFormatter(invoice.grid, id, models);
                }
            }
        });

    $.event.trigger('invoicesGridCustomize', [invoice]);

    Helpers.InitDateRangeTextboxes('from', 'to', function () { Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $('#Customers'), $('#Teams'), $('#Statuses'), 'invoices/getdataforaccountant?archived=false'); });

    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());
        Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $(this), $('#Teams'), $('#Statuses'), 'invoices/getdataforaccountant?', '&archived=false');
        //hardcode, fixed_bug
        $(this).next().removeClass('selectBox-menuShowing');
    });

     $('#com').change(function () {
        if ($('#com').prop('checked')) {
            $('#invoicesGrid').jqGrid('showCol', "Commission").trigger("reloadGrid");
        } else {
            $('#invoicesGrid').jqGrid('hideCol', "Commission").trigger("reloadGrid");
        }
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $('#Customers'), $(this), $('#Statuses'), 'invoices/getdataforaccountant?', '&archived=false');
    });

    $('#Statuses').change(function () {

        Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $('#Customers'), $('#Teams'), $(this), 'invoices/getdataforaccountant?', '&archived=false');
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = '&archived=false&isStartSearch=true' + GetParamForSearch();
        Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $('#Customers'), $('#Teams'), $('#Statuses'), 'invoices/getdataforaccountant?', param);
        var count = $("#invoicesGrid").getGridParam("reccount");
        if (count == 0) {
            jAlert("Search did not find any Invoices.", "No results found...", function () {
                Helpers.RemoveActiveButtons();
            });
        } else {
            $(".h3-wrapper").trigger('click');
        }
    });

    $('.reset-button, .reset-link > a').click(function () {
        resetSearch();
        Helpers.UpdateInvoiceGrid($('#invoicesGrid'), $('#from'), $('#to'), $('#Customers'), $('#Teams'), $('#Statuses'), 'invoices/getdataforaccountant?', '&archived=false');
    });

});

$(document).bind('invoicesGridCustomize', function (event, grid) {
    $('#jqgh_InvoiceStatus').css('position', 'relative').css('top', '-8px');
    $('#invoicesGrid_Id').css('width', '67px');
    var r = $('#invoicesGrid').find('td[aria-describedby="invoicesGrid_Id"]');
    $(r).css('width', '67px');
});


Invoice.EmailSending = function () {
    var ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "invoices/getemaildialog",
            data: "ids=" + ids,
            cache:false,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send invoices of different customers in the same message. \tPlease select invoices related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh($('#invoicesGrid'));
                        });
                }
                else {
                    Helpers.GetEmailDialog(data);
                }
            }
        });
    }
    else {
        jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });

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

Invoice.GetLogData = function (id) {
    return $.ajax({
        type: "GET",
        cache: false,
        url: "invoices/getlogdata",
        data: id.replace('?', ''),
        success: function (data) {
            $("#logInfo").html(data);
            ScrollToBottom();
        }
    });
};

Invoice.ExportToXLS = function () {
    var ids = $('#invoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    var $loader = $('#loader');
    if (ids.length != 0) {
        $.ajax({
            type: "GET",
            url: "invoices/generateexcel",
            data: "ids=" + ids,
            cache: false,
            beforeSend: function () {
                $loader.show();
            },
            complete: function () {
                $loader.hide();
            },
            success: function (data) {
                if (!data) {
                    jAlert("Selected invoice(s) was imported.", "Warning!", function () { $("#invoicesGrid").trigger("reloadGrid"); Helpers.RemoveActiveButtons(); });
                }
                else {
                    window.location.href = data;
                    jCustomConfirm("Was Export to Quickbook successful?", null, function (r) {
                        if (r) {
                            $.ajax({
                                type: "GET",
                                url: "invoices/setimportedstatus",
                                data: "ids=" + ids,
                                cache: false,
                                beforeSend: function () {
                                    $loader.show();
                                },
                                complete: function () {
                                    $loader.hide();
                                },
                                success: function () {
                                    Helpers.Refresh()
                                    $("#invoicesGrid").trigger("reloadGrid");
                                    Helpers.RemoveActiveButtons();
                                }
                            });
                        }
                        else {
                            $("#invoicesGrid").trigger("reloadGrid");
                            Helpers.RemoveActiveButtons();
                        }
                    },
                    "Yes",
                    "No");
                    $.alerts._overlay('hide');
                }
            }
        });
    }
    else {
        jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};
