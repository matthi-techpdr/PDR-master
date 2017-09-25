function VehiclesModel() {
    this.grid = $("#vehiclesGrid");
    this.getdataurl = "Vehicles/GetData";
    this.editurl = "Vehicles/Edit";
    this.createurl = "Vehicles/New";
    this.duplicateurl = "Vehicles/Duplicate";
    //this.editform = '#carsForm';
    //this.suspendurl = 'mobilelicense/suspendlicense';
    //this.reactivateurl = "mobilelicense/reactivatelicense";
};

function formatterViewLink(cellvalue) {
    return "<a href='Vehicles/View/" + cellvalue + "'>" + cellvalue + "</a>";
}

$(function () {
    var vehicleModel = new VehiclesModel();
    var vehiclesGrid = GridInitializer.InitGrid(
        [
            'Id',
            'Vehicle ID',
            'Year From',
            'To',
            'Make',
            'Model',
            'Type'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'Id', index: 'Id', sortable: true, width: 150, formatter: formatterViewLink },
            { name: 'YearFrom', index: 'YearFrom', sortable: true, width: 140 },
            { name: 'YearTo', index: 'YearTo', sortable: false, width: 140 },
            { name: 'Make', index: 'Make', sortable: true, width: 185 },
            { name: 'Model', index: 'Model', sortable: true, width: 185 },
            { name: 'VehicleType', index: 'VehicleType', sortable: false, width: 100 }
        ],
        vehicleModel,
        'Id', false
    );
    vehiclesGrid.navButtonAdd('#pager', { caption: "Edit", onClickButton: function () { VehiclesModel.EditVehicle(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Add new", onClickButton: function () { VehiclesModel.AddNewVehicle(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Duplicate", onClickButton: function () { VehiclesModel.Duplicate(); }, title: "" });

    $('#CarMake').change(function () {
        $('#vehiclesGrid').setGridParam({
            url: 'vehicles/getdata?make=' + $(this).val()
        }).trigger("reloadGrid");
    });

    //    carsGrid.setGridParam({
    //            gridComplete: function () {
    //                var ids = $("#licensesGrid").jqGrid('getDataIDs');
    //                for (var i = 0; i < ids.length; i++) {
    //                    var id = ids[i];
    //                    var rowData = $("#licensesGrid").jqGrid('getRowData', id);
    //                    var models = [{
    //                        colName: 'DeviceId',
    //                        cellValue: rowData.DeviceId,
    //                        limiter: 12
    //                    },{
    //                        colName: 'DeviceName',
    //                        cellValue: rowData.DeviceName,
    //                        limiter: 17
    //                    },{
    //                        colName: 'Owner',
    //                        cellValue: rowData.Owner,
    //                        limiter: 18
    //                    }];
    //                    Helpers.CellFormatter($("#licensesGrid"), id, models);
    //                }
    //            }
    //        });
    //    carsGrid.sortorder = 'asc';
    VehiclesModel.EditVehicle = function () {
        var ids = $('#vehiclesGrid').jqGrid('getGridParam', 'selarrrow');
        if (ids.length != 0) {
            if (ids.length == 1) {
                document.location.href = vehicleModel.editurl + '/' + ids;
            }
            else {
                jAlert("This action cannot be done for different vehicles. \nPlease select one row for this operation", "Warning!", function () { Helpers.Refresh($('#vehiclesGrid')); });
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }   
    };

    VehiclesModel.AddNewVehicle = function () {
        document.location.href = vehicleModel.createurl;
    };

    VehiclesModel.Duplicate = function () {
        var ids = $('#vehiclesGrid').jqGrid('getGridParam', 'selarrrow');
        if (ids.length != 0) {
            if (ids.length == 1) {
                document.location.href = vehicleModel.duplicateurl + '/' + ids;
            }
            else {
                jAlert("This action cannot be done for different vehicles. \nPlease select one row for this operation", "Warning!", function () { Helpers.Refresh($('#vehiclesGrid')); });
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };
});


