function InvoiceCommon() {
}


//////////////////////////////////////////////////////////////////////////////////
InvoiceCommon.History = function (grid, url) {
        var ids = grid.jqGrid('getGridParam', 'selarrrow');
        if (ids.length != 0) {
            if (ids.length > 1) {
                jAlert('Select a single document to view history', 'Warning!', function () {
                    Helpers.Refresh(grid, null);
                });
            }
            else {
                $.ajax({
                    type: "GET",
                    url: url,
                    cache: false,
                    data: 'id=' + ids[0],
                    success: function (data) {
                        Helpers.GetDialogBase('350', 'auto', 'Invoice history',
                                    [{ width: 100, text: "Close", click: function () {
                                        Helpers.Refresh($(grid), null);
                                        $(this).dialog('close');
                                    }
                                    }], null, data);
                    }
                });
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };

    ////////////////////////////////////////////////////////////////////////////////////
    InvoiceCommon.ReinstateInvoice = function () {
        var ids = $('#discardedInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');

        if (ids.length != 0) {
            jCustomConfirm('Would you like to reinstate selected invoices?', null, function (r) {
                if (r) {
                    $.ajax({
                        type: "POST",
                        url: "reinstateinvoice",
                        data: "ids=" + ids,
                        cache: false,
                        success: function (data) {
                            if (data == "Error") {
                                jAlert("Error",
                            "Warning!",
                            function () {
                                Helpers.Refresh($('#discardedInvoicesGrid'));
                            });
                            }
                        else {
                                jAlert(data, null);
                                Helpers.Refresh($('#discardedInvoicesGrid'));
                            }
                        }
                    });
                }
                else {
                    Helpers.RemoveActiveButtons();
                }
            }, "Yes", "No");
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };
