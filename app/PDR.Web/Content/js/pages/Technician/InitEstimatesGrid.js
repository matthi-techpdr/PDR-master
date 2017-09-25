$(document).bind('estimatesGridCustomize', function (event, grid) {
    // grid.navButtonAdd('#pager', { caption: "Add Estimate", onClickButton: function () { window.location = "Estimates/New"; }, title: "", position: "first" });
    grid.navButtonAdd('#pager', { caption: "Convert to repair order", onClickButton: function () { Estimate.ConvertToRepairOrder(); }, title: "", position: "last" });

    grid.setGridParam({ gridComplete: function () {
        Helpers.HighlightNew(grid);
        var ids = grid.jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            var rowData = grid.jqGrid('getRowData', id);
            var models = [{
                colName: 'CustomerName',
                cellValue: rowData.CustomerName,
                limiter: 25
            }, {
                colName: 'CarInfo',
                cellValue: rowData.CarInfo,
                limiter: 25
            }];
            Helpers.CellFormatter(grid, id, models);
        }
    }});
});