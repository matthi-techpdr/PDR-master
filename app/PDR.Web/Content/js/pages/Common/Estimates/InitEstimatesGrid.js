function Estimate(param) {
    this.grid = $("#estimatesGrid");
    this.getdataurl = "estimates/getdata?archived=false&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
    this.editform = '#estimateForm';
    this.editcontainer = $("#estimateInfo");
};

function formatterViewLink(cellvalue) {
    return "<a href='Estimates/View/" + cellvalue + "'>" + cellvalue + "</a>";
}

$(function () {
    var colNames = [
        'Customer type',
        'Insurance',
        'HasEstimateSignature',
        'Creation date',
        'Estimate ID',
        'Customer name',
        'Year/make/model',
        'Total amount',
        'Status',
        'Type',
        'New'
    ];
    var colModels = [
        { name: 'CustomerType', index: 'CustomerType', hidden: true },
        { name: 'HasInsurance', index: 'HasInsurance', hidden: true },
        { name: 'HasEstimateSignature', index: 'HasEstimateSignature', hidden: true },
        { name: 'CreationDate', index: 'CreationDate', width: 140 },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false, width: 120 },
        { name: 'CustomerName', index: 'CustomerName', sortable: false, width: 180 },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 180 },
        { name: 'TotalAmount', index: 'TotalAmount', width: 150 },
        { name: 'EstimateStatus', index: 'EstimateStatus', sortable: false },
        { name: 'Type', index: 'Type', hidden: true },
        { name: 'New', index: 'New', hidden: true }
    ];

    $.event.trigger('addColumnsToGrid', [colNames, colModels]);

    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var estimatesGrid = GridInitializer.InitGrid(
            colNames,
            colModels,
            new Estimate(param),
            'CreationDate',
            false
    );
    var withoutExtraButtons = $('#withoutExtraButtons').length;
    if (withoutExtraButtons) {
        estimatesGrid.navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ToPrint(); }, title: "" })
    } else {
        estimatesGrid.navButtonAdd('#pager', { caption: "Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ToPrint(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Approve", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.Approve(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Archive", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ToArchived(); }, title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { EstimateCommon.History(estimatesGrid, 'estimates/gethistory'); }, position: "last", title: "" });
    }
    estimatesGrid.navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.EmailSending(); }, title: "" });   

    $.event.trigger('estimatesGridCustomize', [estimatesGrid]);

    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());

        $('#estimatesGrid').setGridParam({
            url: 'estimates/getdata?customer=' + $(this).val() + "&archived=false&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        var withoutArchive = $('#withoutArchive').length;
     $('#estimatesGrid').setGridParam({
            url: 'estimates/getdata?customer=' + $('#Customers').val() + "&archived=false&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#estimatesGrid").getGridParam("reccount");
        if (count == 0) {
            if (withoutArchive) {
                jAlert("Search did not find any Estimates", "No results found...");
            } else {
                jCustomConfirm("Search did not find any Estimates. Would you like to search in archived docs?", "No results found...", function (r) {
                    if (r) {
                        var urlRedirect = $("#arhiveEstimateUrl").val() + '?' + param;
                        window.location.assign(urlRedirect);
                    } else {

                    }
                }, "Yes", "No");
            }
        } else {
            $(".h3-wrapper").trigger('click');
        }
    });

    $('.reset-button, .reset-link > a').click(function () {
        resetSearch();
        $('#estimatesGrid').setGridParam({
            url: 'estimates/getdata?customer=' + $('#Customers').val() + "&archived=false&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });
});

Estimate.ToArchived = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        jConfirm('Archive estimate?', null, function (r) {
            if (r) {
                Helpers.SendAjax("POST", "estimates/archiveunarchive", "ids=" + ids + "&toArchived=true", false, function() {
                        Helpers.Refresh($('#estimatesGrid'), function() {
                            jAlert("Operation completed", "", function () {
                        });
                    });
                });
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Estimate.EmailSending = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "estimates/getemaildialog",
            data: "ids=" + ids,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send estimates of different customers in the same message. \tPlease select estimates related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh($('#invoicesGrid'));
                        });
                }
                else {
                    Helpers.GetEmailDialog(data, $('#estimatesGrid'));
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Estimate.ReassignBase = function (ids, title, url, forConvert) {
    if (ids.length != 0) {
        return $.ajax({
            url: "estimates/getmanagers",
            data: "ids=" + ids + "&forConvert=" + forConvert,
            cache: false,
            statusCode: {
                201: function () {
                    jAlert("Operation completed", "", function () {
                        Helpers.Refresh($('#estimatesGrid'), null);
                    });
                },
                204: function () {
                    var msg = "This action cannot be done for multiple wholesale customers.";
                    jAlert(msg, "Warning!", function () { Helpers.RemoveActiveButtons(); });
                },
                200: function (data) {
                    Estimate.GetReassignDialog(data, title, url);
                }
            }
        });
    }
    return Helpers.NoSelectedRowOperation();
};

Estimate.ReassignEstimate = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    Estimate.ReassignBase(ids, "Re-assign estimate", "estimates/reassign", false);
};

Estimate.ConvertToRepairOrder = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow');

    if (ids.length != 0) {
        Estimate.CheckStatusAndCustomerForApproveAndConvertOperation(ids, "Convert", function () {
            var hasSignature = $('#estimatesGrid').jqGrid('getCell', ids[0], 'HasEstimateSignature');
            if (hasSignature == 'true') {
                jCustomConfirm("Signature Recieved ", "Customer Approval", function(r) {
                    if (r) {
                        Helpers.SendAjax("POST", "estimates/approveestimates", "ids=" + ids.join(','), false, function() {
                            Estimate.ConverToRoBase(ids, "Approved");
                        });
                    }
                    else {
                        Helpers.Refresh($('#estimatesGrid'), null);
                    }
                }, "Yes", "No");
            }
            else {
                return Estimate.ConverToRoBase(ids, "Completed");
            }
        });
    }
    else {
        return Helpers.NoSelectedRowOperation();
    }
};

Estimate.CheckStatusAndCustomerForApproveAndConvertOperation = function (ids, operation, functionCompleted) {
    var firstStatus = $('#estimatesGrid').jqGrid('getCell', ids[0], 'EstimateStatus');
    var firstCustomer = $('#estimatesGrid').jqGrid('getCell', ids[0], 'CustomerName');
    for (var i = 0; i < ids.length; i++) {
        var status = $('#estimatesGrid').jqGrid('getCell', ids[i], 'EstimateStatus');
        var customer = $('#estimatesGrid').jqGrid('getCell', ids[i], 'CustomerName');
        if (status != firstStatus) {
            return jAlert("This action cannot be done for entries with different statuses", "Warning!", function () {
                Helpers.Refresh($('#estimatesGrid'), null);
            });
        }
        if (customer != firstCustomer) {
            return jAlert("This action cannot be done for entries with different customers", "Warning!", function () {
                Helpers.Refresh($('#estimatesGrid'), null);
            });
        }
    }

    if (firstStatus == "Open") {
        return jAlert("This action cannot be done for entries with \"Open\" status.", "Warning!", function () {
            Helpers.Refresh($('#estimatesGrid'), null);
        });
    }

    if (firstStatus == "Completed") {
        functionCompleted();
    }
    if (firstStatus == "Approved") {
        if (operation == "Convert") {
            return Estimate.ConverToRoBase(ids, firstStatus);
        }
        else {
            jAlert('Operation can not be completed. \nOne or more estimates already approved', 'Warning!', function () {
                Helpers.Refresh($('#estimatesGrid'), null);
            });
        }
    }
};

Estimate.ConverToRoBase = function (ids, status) {
    jConfirm("Convert this estimate(s) to repair order(s)?", "Convert to repair order",
                function (r) {
                    if (r) {
                        if (status == "Completed") {
                            Helpers.SendAjax("POST", "estimates/approveestimates", "ids=" + ids.join(','), false, function() {
                                Estimate.ReassignBase(ids, "Convert to repair order", "estimates/ConvertToRepairOrders", true);
                            });
                        }
                        else {
                            Estimate.ReassignBase(ids, "Convert to repair order", "estimates/ConvertToRepairOrders", true);
                        }
                    }
                    else {
                        Helpers.Refresh($('#estimatesGrid'), null);
                    }
                });
};

Estimate.GetReassignDialog = function (data, title, url) {
    Helpers.GetDialogBase(400, 220, title, [{
        width: 178,
        text: "Assign",
        click: function () {
            var grid = $('#estimatesGrid');
            var ids = grid.jqGrid('getGridParam', 'selarrrow').join(',');
            var managerId = $('#managers').val();
            var model = { "estimatesIds": ids, "managerId": managerId };
            if (managerId != "-1") {
                Helpers.SendJsonModelBase(model, url, function () {
                    $(container).dialog('close');
                    jAlert("Operation completed", "", function () {
                        Helpers.Refresh($('#estimatesGrid'), null);
                    });
                });
            }
            else {
                jAlert("Choose the employee to whom you want to re-assign chosen estimate(s):", "Warning!", null);
            }
        }
    },
    {
        width: 178, text: "Cancel", click: function () {
            $(this).dialog('close');
            Helpers.Refresh($('#estimatesGrid'), null);
        }
    }], null, data);
};

Estimate.ToPrint = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        Helpers.Refresh($('#estimatesGrid'), function () {
            for (var i = 0; i < ids.length; i++) {
                window.open("estimates/printestimates?id=" + ids[i], "_blank");
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Estimate.Approve = function () {
    var ids = $('#estimatesGrid').jqGrid('getGridParam', 'selarrrow');

    if (ids.length != 0) {
        Estimate.CheckStatusAndCustomerForApproveAndConvertOperation(ids, "Approve", function () {
            var hasSignature = $('#estimatesGrid').jqGrid('getCell', ids[0], 'HasEstimateSignature');
            if (hasSignature == 'true') {
                jCustomConfirm('Signature Recieved', 'Customer Approval', function (r) {
                    Helpers.Refresh($('#estimatesGrid'), null);
                    if (r) {
                        ApproveAjax(ids);
                    }
                }, "Yes", "No");
            }
            else {
                ApproveAjax(ids);
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

function ApproveAjax(ids) {
    Helpers.SendAjax("POST", "estimates/approveestimates", "ids=" + ids, false, function(data) {
        if (data) {
            jAlert('Operation can not be completed. \nOne or more estimates already approved', 'Warning!', function() {
                Helpers.Refresh($('#estimatesGrid'), null);
            });
        }
        else {
            jAlert('Operation completed', '', function() {
                Helpers.Refresh($('#estimatesGrid'), null);
            });
        }
    });
}
