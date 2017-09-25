function GpsReport() {
    this.grid = $("#gpsReportsGrid");
    this.getdataurl = "locations/getdata";
};

$(function () {
    var routesCollections = new Routes();
    var lastLocationCollection = new LastLocations();
    window.routes = routesCollections;
    var gps = new GpsReport();
    var gpsRepostsGrid = GridInitializer.InitGrid(
        [
            'Last report date/time',
            'License ID',
            'Device Owner Name',
            'Teams',
            'Role'
        ], [
            { name: 'LastReportDate', index: 'LastReportDate' },
            { name: 'Id', index: 'Id' },
            { name: 'OwnerName', index: 'OwnerName', sortable: false },
            { name: 'Teams', index: 'Teams', sortable: false },
            { name: 'Role', index: 'Role'}
        ],
        gps,
        'LastReportDate', false
    );

    gpsRepostsGrid.navButtonAdd('#pager', { caption: "View all devices", onClickButton: function () { lastLocationCollection.fetch(); }, title: "" })
        .navButtonAdd('#pager', { caption: "View routes", onClickButton: function () {

            var licenses = $("#gpsReportsGrid").jqGrid('getGridParam', 'selarrrow');
            
            if (licenses.length > 0 && licenses.length <= 10) {
                routesCollections.fetch();
            }
            else {
                jAlert('Choose from 1 to 10 entries', null, function () { Helpers.RemoveActiveButtons(); });
            }
        }, title: ""
        });

    gpsRepostsGrid.setGridParam({
        gridComplete: function () {
            var ids = $("#gpsReportsGrid").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var rowData = $("#gpsReportsGrid").jqGrid('getRowData', id);
                var models = [{
                    colName: 'Teams',
                    cellValue: rowData.Teams,
                    limiter: 27
                }];
                Helpers.CellFormatter($("#gpsReportsGrid"), id, models);
            }
        }
    });

    $('#Team').change(function () {
        $("#gpsReportsGrid").setGridParam({
            url: 'locations/getdata?team=' + $(this).val()
        }).trigger("reloadGrid");
    });
});




