function Finalised(param) {
    this.grid = $("#finalisedGrid");
    this.getdataurl = "getrepairordersdata?finalised=true&team=" + $('#Teams').val() + "&customer=" + CookieManager.GetCookie('customer') + param + '&isNeedFilter=true';
}

$(function () {
    function formatterViewLink(cellvalue) {
        return "<a href=View/" + cellvalue + ">" + cellvalue + "</a>";
    }
    var columnNames = [
        'New',
        'Creation date',
        'Order ID',
        'Employee',
        'Customer name',
        'Year/make/model',
        'Total amount',
        'Status',
        'IsPaidInvoice'
    ];
    var columnModel = [
        { name: 'New', index: 'New', hidden: true },
        { name: 'CreationDate', index: 'CreationDate' },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false },
        { name: 'Employee', index: 'Employee', sortable: false },
        { name: 'CustomerName', index: 'CustomerName', sortable: false },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
        { name: 'TotalAmount', index: 'TotalAmount' },
        { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false },
        { name: 'IsPaidInvoice', index: 'IsPaidInvoice', hidden: true }
    ];

    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var finalised = new Finalised(param);
    var finaliseGrid = GridInitializer.InitGrid(columnNames, columnModel, finalised, 'CreationDate', true)
        .navButtonAdd('#pager', { caption: "Discard", onClickButton: function () { RO.ToDiscard(finalised.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Print", onClickButton: function () { RO.ToPrint(finalised.grid, true); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { RO.Email(finalised.grid, true); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { RO.History(finalised.grid, 'gethistory'); }, position: "last", title: "" })
        .setGridParam({
            gridComplete: function () {
                if (!window.IsTechnician) {
                    Helpers.HighlightNew($("#finalisedGrid"));
                }
                var ids = $("#finalisedGrid").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#finalisedGrid").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CustomerName',
                        cellValue: rowData.CustomerName,
                        limiter: 19
                    }, {
                        colName: 'CarInfo',
                        cellValue: rowData.CarInfo,
                        limiter: 27
                    }];
                    Helpers.CellFormatter($("#finalisedGrid"), id, models);
                }
            }
        });

    $.event.trigger('finaliseGridCustomize', [finaliseGrid]);
    RO.InitFilters(finalised.grid, function () {
        var url = 'getrepairordersdata?finalised=true';
        if ($('#Teams').val() != 'All teams') {
            url += "&team=" + $('#Teams').val();
        }

        url += "&customer=" + $('#Customers').val();

        return url;
    });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#finalisedGrid').setGridParam({
            url: 'getrepairordersdata?finalised=true&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#finalisedGrid").getGridParam("reccount");
        if (count == 0) {
            jCustomConfirm("Search did not find any Repair Orders. Would you like to search in active docs?", "No results found...", function (r) {
                if (r) {
                    var urlRedirect = $("#RepairOrderUrl").val() + '?' + param;
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
        $('#finalisedGrid').setGridParam({
            url: 'getrepairordersdata?finalised=true&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });


});