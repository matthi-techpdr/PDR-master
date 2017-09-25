$(document).bind('estimatesGridCustomize', function (event, grid) {
    grid.navButtonAdd('#pager', { caption: "Re-assign", onClickButton: function () { Helpers.RemoveActiveButtons(); Estimate.ReassignEstimate(); },  title: "" });
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
                limiter: 26
            }, {
                colName: 'CarInfo',
                cellValue: rowData.CarInfo,
                limiter: 25
            }];
            Helpers.CellFormatter(grid, id, models);
        }
    }});
});