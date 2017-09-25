function Estimate(param) {
    this.grid = $("#archiveEstimatesGrid");
    this.getdataurl = "getdata?archived=true&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
    this.editform = '#estimateForm';
    this.editcontainer = $("#estimateInfo");
};

function formatterViewLink(cellvalue, options, rowObject) {
    return "<a href=View/" + cellvalue + ">" + cellvalue + "</a>";
}

$(function () {
    var colNames = [
        'Creation date',
        'ID',
        'Customer name',
        'Year/make/model',
        'Total amount',
        'Status'
    ];

    var colModels = [
        { name: 'CreationDate', index: 'CreationDate', width: 160 },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false, width: 160 },
        { name: 'CustomerName', index: 'CustomerName', sortable: false, width: 200 },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
        { name: 'TotalAmount', index: 'TotalAmount', width: 160 },
        { name: 'EstimateStatus', index: 'EstimateStatus', sortable: false, width: 100 }
    ];

    $.event.trigger('addColumnsToArchiveGrid', [colNames, colModels]);

    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var estimatesGrid = GridInitializer.InitGrid(
        colNames, colModels,
        new Estimate(param),
        'CreationDate', false
    )
        .navButtonAdd('#pager', { caption: "Un-archive", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ToNonArchived(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Discard", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ToDiscard(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Print", onClickButton: function () { Estimate.ToPrint(); }, title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.EmailSending(); }, title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { EstimateCommon.History(estimatesGrid, 'gethistory'); }, position: "last", title: "" });

    estimatesGrid.setGridParam({ gridComplete: function () {
        Helpers.HighlightNew(estimatesGrid);
        var ids = estimatesGrid.jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            var rowData = estimatesGrid.jqGrid('getRowData', id);
            var models = [{
                colName: 'CustomerName',
                cellValue: rowData.CustomerName,
                limiter: 19
            }, {
                colName: 'CarInfo',
                cellValue: rowData.CarInfo,
                limiter: 25
            }];
            Helpers.CellFormatter(estimatesGrid, id, models);
        }
    } 
    });

$('#Customers').change(function () {
    CookieManager.SetCookie("customer", $(this).val());
    $('#archiveEstimatesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $(this).val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        $('#archiveEstimatesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#archiveEstimatesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = estimatesGrid.getGridParam("reccount");
        if (count == 0) {
            jCustomConfirm("Search did not find any Estimates. Would you like to search in active docs?", "No results found...", function (r) {
                if (r) {
                    var urlRedirect = $("#EstimateUrl").val() + '?' + param;
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
        $('#archiveEstimatesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });
});

Estimate.GetEstimateData = function (id) {
    return $.ajax({
        type: "GET",
        url: "getestimate",
        data: id.replace('?', '') + "&edit=" + false,
        success: function (data) {
            $("#companyInfo").html(data);
            Helpers.RemoveActiveButtons();
        }
    });
};

Estimate.ToNonArchived = function () {
    var ids = $('#archiveEstimatesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        var incorrectStatus;
        for (var i = 0; i < ids.length; i++) {
            incorrectStatus = $('#archiveEstimatesGrid').jqGrid('getCell', ids[i], 'EstimateStatus') == "Converted";
            if (incorrectStatus) {
                return jAlert("You can not un-archive estimates with converted status.", "Warning!", function () { Helpers.RemoveActiveButtons(); });
            }
        }

        jConfirm('Unarchive estimate(s)?', null, function (r) {
            Helpers.RemoveActiveButtons();
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "archiveunarchive",
                    data: "ids=" + ids + "&toArchived=false",
                    success: function () {
                        Helpers.Refresh($('#archiveEstimatesGrid'), function() {
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

Estimate.EmailSending = function () {
    var ids = $('#archiveEstimatesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "getemaildialog",
            data: "ids=" + ids,
            success: function (data) {
                if (data == "Error") {
                    jAlert("Send estimates of different customers in the same message can not be. \nPlease select the estimates relating to the same customer", "Warning!", function () { Helpers.RemoveActiveButtons(); });
                }
                else {
                    Helpers.GetEmailDialog(data, $('#archiveEstimatesGrid'));
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Estimate.ToPrint = function () {
    var ids = $('#archiveEstimatesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
            Helpers.Refresh($('#archiveEstimatesGrid'), function () {
                for (var i = 0; i < ids.length; i++) {
                    window.open("printestimates?id=" + ids[i], "_blank");
                }
            });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Estimate.ToDiscard = function () {
    var grid = $('#archiveEstimatesGrid');
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        jCustomConfirm('Would you like to discard selected estimate(s)?', null, function (r) {
            if (r) {
                Helpers.SendAjax("POST", "discard", "ids=" + ids, false, function (data) {
                    Helpers.Refresh(grid, function () {
                        jAlert(data, null);
                    });
                });
            } else {
                Helpers.RemoveActiveButtons();
            }
        }, "Yes", "No");
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

