function Finalised() {
    this.grid = $("#finalisedGrid");
    this.getdataurl = "getrepairordersdata?finalised=true" + '&isNeedFilter=true';
}

$(function () {
    var finalised = new Finalised();
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
        'Status'
    ];
    var columnModel = [
        { name: 'New', index: 'New', hidden: true },
        { name: 'CreationDate', index: 'CreationDate' },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false },
        { name: 'Employee', index: 'Employee', sortable: false },
        { name: 'CustomerName', index: 'CustomerName', sortable: false },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
        { name: 'TotalAmount', index: 'TotalAmount' },
        { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false }
    ];

    var finaliseGrid = GridInitializer.InitGrid(columnNames, columnModel, finalised, 'CreationDate', true)
        .setGridParam({
            gridComplete: function () {
                var boxes = $('.cbox');
                $.each(boxes, function () {
                    $(this).hide();
                });


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
    RO.InitFilters(finalised.grid, function () { return finalised.getdataurl + "&team=" + $('#Teams').val() + "&customer=" + $('#Customers').val(); });
});
