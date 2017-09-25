function RepairOrder() {
    this.grid = $("#repairOrdersGrid");
    this.getdataurl = "repairorders/getrepairordersdata?finalised=false&isNeedFilter=true&";
};

function formatterViewLink(cellvalue) {
    return "<a href=RepairOrders/View/" + cellvalue + ">" + cellvalue + "</a>";
}

$(function () {
    var repairOrder = new RepairOrder();
    GridInitializer.InitGrid(
        [
            'New',
            'Creation date',
            'Order ID',
            'Customer name',
            'Year/make/model',
            'Total amount',
            'Status',
            'My share',
            'HasOrderSignature',
            'EditableStatus',
            'RoStatus'
        ], [
            { name: 'New', index: 'New', hidden: true },
            { name: 'CreationDate', index: 'CreationDate' },
            { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false },
            { name: 'CustomerName', index: 'CustomerName', sortable: false },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
            { name: 'TotalAmount', index: 'TotalAmount' },
            { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false },
            { name: 'Share', index: 'Share', sortable: false },
            { name: 'HasOrderSignature  ', index: 'HasOrdersignature', hidden: true },
            { name: 'EditableStatus', index: 'EditableStatus', hidden: true },
            { name: 'RoStatus', index: 'RoStatus', hidden: true }
        ],
        repairOrder,
        'CreationDate',
        true
    )
        .navButtonAdd('#pager', { caption: "Print", onClickButton: function () { RO.ToPrint(repairOrder.grid); }, position: "last", title: "" })
        .setGridParam({
            gridComplete: function () {
                Helpers.HighlightNew(repairOrder.grid);
                var ids = repairOrder.grid.jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = repairOrder.grid.jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CustomerName',
                        cellValue: rowData.CustomerName,
                        limiter: 19
                    }, {
                        colName: 'CarInfo',
                        cellValue: rowData.CarInfo,
                        limiter: 23
                    }];
                    Helpers.CellFormatter(repairOrder.grid, id, models);
                }
            }
        });
    RO.InitFilters(repairOrder.grid, function () { return 'repairorders/getrepairordersdata?&team=' + $('#Teams').val() + "&customer=" + $('#Customers').val() + "&finalised=false"; });
});