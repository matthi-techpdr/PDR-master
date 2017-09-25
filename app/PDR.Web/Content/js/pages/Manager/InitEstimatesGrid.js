
$(document).bind('addColumnsToGrid', function (event, colNames, colModels) {
    colModels[3].width = 120;
    colModels[4].width = 110;
    colModels[5].width = 140;

    colNames.splice(6, 0, 'Employee');
    colModels.splice(6, 0, { name: 'Employee', index: 'Employee', sortable: false, width: 110 });
});

$(document).bind('estimatesGridCustomize', function (event, grid) {
    grid.navButtonAdd('#pager', { caption: "Re-assign", onClickButton: function () { Estimate.ReassignEstimate(); }, title: "" });
    grid.navButtonAdd('#pager', { caption: "Convert to repair order", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ConvertToRepairOrder(); }, title: "" });
    grid.setGridParam({ gridComplete: function () {
        Helpers.HighlightNew(grid);
        var ids = grid.jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            var rowData = grid.jqGrid('getRowData', id);
            var models = [{
                colName: 'CustomerName',
                cellValue: rowData.CustomerName,
                limiter: 19
            }, {
                colName: 'CarInfo',
                cellValue: rowData.CarInfo,
                limiter: 25
            }, {
                colName: 'Employee',
                cellValue: rowData.Employee,
                limiter: 16
            }];
            Helpers.CellFormatter(grid, id, models);
        }} 
    });

$('#Teams').change(function () {
    CookieManager.SetCookie("team", $(this).val());
    grid.setGridParam({
            url: 'estimates/getdata?&team=' + $(this).val() + "&customer=" + $('#Customers').val() + "&archived=false"
        }).trigger("reloadGrid");
    });
});
