
$(document).bind('addColumnsToGrid', function (event, colNames, colModels) {

    colModels[2].width = 160;
    colModels[3].width = 140;
    colModels[4].width = 140;
    colModels[6].width = 250;
    colNames.splice(5, 1);
    colModels.splice(5, 1);
    colNames.splice(5, 0, 'Employee');
    colModels.splice(5, 0, { name: 'Employee', index: 'Employee', sortable: false, width: 110 });
});

$(document).bind('estimatesGridCustomize', function (event, grid) {
    grid.setGridParam({ gridComplete: function () {
        Helpers.HighlightNew(grid);
        var ids = grid.jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            var rowData = grid.jqGrid('getRowData', id);
            var models = [
            {
                colName: 'CarInfo',
                cellValue: rowData.CarInfo,
                limiter: 25
            }, {
                colName: 'Employee',
                cellValue: rowData.Employee,
                limiter: 16
            }];
            Helpers.CellFormatter(grid, id, models);
        } 
    }
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        grid.setGridParam({
            url: 'estimates/getdata?&team=' + $(this).val() + "&customer=" + $('#Customers').val() + "&archived=false"
        }).trigger("reloadGrid");
    });
});
