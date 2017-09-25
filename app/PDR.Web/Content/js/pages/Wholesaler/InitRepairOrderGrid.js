function RepairOrder(param) {
    this.grid = $("#repairOrdersGrid");
    this.getdataurl = "repairorders/getrepairordersdata?finalised=false&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer')
        + param + '&isNeedFilter=true';
};

function formatterViewLink(cellvalue) {
    return "<a href=RepairOrders/View/" + cellvalue + ">" + cellvalue + "</a>";
}

$(function () {
    var MAX_EMPLOYEE_LENGTH = 26;
    var columnNames = [
        'New',
        'Creation date',
        'Order ID',
        'Employee',
        'Year/make/model',
        'Total amount',
        'Status',
        'EditableStatus',
        'RoStatus'
    ];
    var columnModel = [
        { name: 'New', index: 'New', hidden: true },
        { name: 'CreationDate', index: 'CreationDate', width: 130 },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false, width: 120 },
        { name: 'Employee', index: 'Employee', sortable: false, width: 220 },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 250 },
        { name: 'TotalAmount', index: 'TotalAmount', width: 160 },
        { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false, width: 120 },
        { name: 'EditableStatus', index: 'EditableStatus', hidden: true },
        { name: 'RoStatus', index: 'RoStatus', hidden: true }
    ];

    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var repairOrder = new RepairOrder(param);

    GridInitializer.InitGrid(
        columnNames, columnModel,
        repairOrder,
        'CreationDate',
        true
    )
        .navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { RO.ToPrint(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { RO.Email(repairOrder.grid); }, position: "last", title: "" })
        .setGridParam({ gridComplete: function () {
            Helpers.HighlightNew(repairOrder.grid);
            var ids = repairOrder.grid.jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var rowData = repairOrder.grid.jqGrid('getRowData', id);
                var models = [{
                    colName: 'CustomerName',
                    cellValue: rowData.CustomerName,
                    limiter: 18
                }, {
                    colName: 'CarInfo',
                    cellValue: rowData.CarInfo,
                    limiter: 26
                 }, {
                     colName: 'Employee',
                     cellValue: formEmployee(rowData.Employee, MAX_EMPLOYEE_LENGTH),
                 }];
                Helpers.CellFormatter(repairOrder.grid, id, models);
            }
        }
        });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#repairOrdersGrid').setGridParam({
            url: 'repairorders/getrepairordersdata?finalised=false&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#repairOrdersGrid").getGridParam("reccount");
        if (count == 0) {
            jAlert("Search did not find any Repair Orders", "No results found...");
        } else {
            $(".h3-wrapper").trigger('click');
        }
    });

    $('.reset-button, .reset-link > a').click(function () {
        resetSearch();
        $('#repairOrdersGrid').setGridParam({
            url: 'repairorders/getrepairordersdata?finalised=false&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    RO.InitFilters(repairOrder.grid, function () { return 'repairorders/getrepairordersdata?&team=' + $('#Teams').val() + "&customer=" + $('#Customers').val() + "&finalised=false"; });
});




