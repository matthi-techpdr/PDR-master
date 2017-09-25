function iPhoneApp() {
    this.grid = $("#iPhoneAppGrid");
    this.getdataurl = "manageDownload/getdata";
    this.getcurrenturl = "manageDownload/getBuild";
    this.editurl = "manageDownload/editIphoneBuild";
    this.editform = '#editBuildForm';
    this.editcontainer = $("#iPhoneAppInfo");
    this.name = 'build status';
};

$(function () {
    GridInitializer.InitGrid(
        [
            'Id',
            'Version',
            'Date',
            'Status',
            'Available for users'
            
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'Version', index: 'Version' },
            {name: 'DateUpload', index: 'DateUpload' },
            { name: 'IsWorkingBild', index: 'IsWorkingBild' },
            { name: 'IsDownLoaded', index: 'IsDownLoaded' }
        ], new iPhoneApp(), 'Version')
        .setGridParam({
            renderCheckboxes: true,
            gridComplete: function () {
                var ids = $("#iPhoneAppGrid").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#iPhoneAppGrid").jqGrid('getRowData', id);
                   var models = [{
                       colName: 'DateUpload',
                       cellValue: rowData.DateUpload,
                        
                        limiter: 18
                    }, {
                        colName: 'IsWorkingBild',
                        cellValue: rowData.IsWorkingBild,
                        limiter: 20,
                        dataType: "bool"
                    }];
                    Helpers.CellFormatter($("#iPhoneAppGrid"), id, models);
                }
            }
        });

//    $('#UsaStates').change(function () {
//        $('#companyGrid').setGridParam({
//            url: 'companies/getdata?state=' + $(this).val()
//        }).trigger("reloadGrid");
//    });

//    window.sendEmailUrl = "companies/sendemail";
});

    iPhoneApp.GetBuildData = function (id) {
    return $.ajax({
        type: "GET",
        url: "manageDownload/getBuild",
        data: id,
        cache: false,
        success: function (data) {
            $("#iPhoneAppInfo").html(data);
            ScrollToBottom();
        }
    });
};