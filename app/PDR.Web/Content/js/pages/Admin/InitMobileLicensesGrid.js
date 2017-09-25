function MobileLicense() {
    this.grid = $("#licensesGrid");
    this.getdataurl = "mobilelicense/getdata";
    this.editform = '#licenseForm';
    this.suspendurl = 'mobilelicense/suspendlicense';
    this.reactivateurl = "mobilelicense/reactivatelicense";
};

$(function () {
    var license = new MobileLicense();
    var licenseGrid = GridInitializer.InitGrid(
        [
            'Id',
            'Creation date',
            'License ID',
            'Device ID',
            'Device name',
            'Owner',
            'Device type',
            'Phone number',
            'Status'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'CreationDate', index: 'CreationDate', width: 120 },
            { name: 'Id', index: 'Id', sortable: true, width: 100, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="MobileLicense.GetLicenseData(\'', addParam: '\');'} },
            { name: 'DeviceId', index: 'DeviceId', sortable: false, width: 100 },
            { name: 'DeviceName', index: 'DeviceName', sortable: false, width: 130 },
            { name: 'Owner', index: 'Owner', sortable: true, width: 130 },
            { name: 'DeviceType', index: 'DeviceType', sortable: false, width: 120 },
            { name: 'PhoneNumber', index: 'PhoneNumber', sortable: false, width: 160 },
            { name: 'Status', index: 'Status', sortable: false, width: 100 }
        ],
        license,
        'LicenseNumber', false
    );
    licenseGrid.navButtonAdd('#pager', { caption: "Edit", onClickButton: function () { MobileLicense.EditLicense(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Suspend", onClickButton: function () { license.Suspend(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Re-activate", onClickButton: function () { license.Reactivate(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Add new", onClickButton: function () { MobileLicense.AddNewLicense(); }, title: "" })
        .navButtonAdd('#pager', { caption: "Clear device ID&Token", onClickButton: function () { MobileLicense.ClearDeviceIdAndToken(); }, title: "" });
    licenseGrid.setGridParam({
        gridComplete: function () {
            var ids = $("#licensesGrid").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var rowData = $("#licensesGrid").jqGrid('getRowData', id);
                var models = [{
                    colName: 'DeviceId',
                    cellValue: rowData.DeviceId,
                    limiter: 12
                }, {
                    colName: 'DeviceName',
                    cellValue: rowData.DeviceName,
                    limiter: 17
                }, {
                    colName: 'Owner',
                    cellValue: rowData.Owner,
                    limiter: 18
                }];
                Helpers.CellFormatter($("#licensesGrid"), id, models);
            }
        }
    });
    licenseGrid.sortorder = 'asc';

    $('#Status').change(function () {
        $('#licensesGrid').setGridParam({
            url: 'mobilelicense/getdata?status=' + $(this).val()
        }).trigger("reloadGrid");
    });

    license.Suspend = function () {
        Helpers.ChangeStatus(license.grid, 'Suspend?', license.suspendurl, function () {
            MobileLicense.GetCountLicense();
            jAlert("Success", "", null);
        });
    };

    license.Reactivate = function () {
        var amount = parseInt($('.activeLicense').text());
        var allowedAmount = parseInt($('.allowedLicense').text());
        var ids = license.grid.jqGrid('getGridParam', 'selarrrow');
        var idsTmp = [];

        $.each(ids, function (index) {
            var i = ids[index];
            var status = license.grid.jqGrid('getCell', i, 'Status');
            if (status != 'Active') {
                idsTmp.push(i);
            }
        });

        if (idsTmp.length != 0) {
            if (amount < allowedAmount) {
                Helpers.ChangeStatus(license.grid, 'Reactivate?', license.reactivateurl, function (data) {
                    jAlert(data, "Alert", null);
                    MobileLicense.GetCountLicense();
                });
            }
            else {
                jAlert("Limit mobile licenses completed", 'Warning!', function () { Helpers.Refresh(license.grid); });
            }
        }
        else {
            jAlert("Chosen licenses already have been activated", "Warning!", function () { Helpers.Refresh(license.grid); });
        }
    };
});

MobileLicense.AddNewLicense = function () {
    var amount = parseInt($('.activeLicense').text());
    var allowedAmount = parseInt($('.allowedLicense').text());
    if (amount < allowedAmount) {
        $.ajax({
            type: "POST",
            url: "mobilelicense/mobilelicensedialog",
            data: "edit=false",
            success: function (data) {
                Helpers.GetDialogBase('360', '250', 'Add new license',
                        [{ width: 229, text: "Save", click: function () { MobileLicense.SaveLicense(container); } },
                            { width: 82, text: "Cancel", click: function () { $(this).dialog('close'); } }],
                        function () {
                            $('#DeviceType').change(function () {
                                var val = $(this).val();
                                if (val != 'iPhone') {
                                    $('#PhoneNumber').attr("disabled", 'disabled');
                                    $('#PhoneNumber').val('');
                                }
                                else {
                                    $('#PhoneNumber').removeAttr('disabled');
                                }
                            });

                            ValidateAjaxForm($('#licenseForm'));

                        }, data);
            }
        });
    }
    else {
        jAlert("Limit mobile licenses completed", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};

MobileLicense.EditLicense = function () {
    var ids = $('#licensesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        if (ids.length > 1) {
            jAlert("You can select only one license to edit", "Warning!");
        }
        else {
            $.ajax({
                type: "POST",
                url: "mobilelicense/mobilelicensedialog",
                data: "ids=" + ids + "&edit=true",
                success: function (data) {
                    Helpers.GetDialogBase('360', '360', 'Edit license',
                        [{ width: 229, text: "Save", click: function () { MobileLicense.SaveLicense(container); } },
                         { width: 82, text: "Cancel", click: function () { $(this).dialog('close'); } }],
                        function () {
                            if ($('#DeviceType').val() == 'iPad') {
                                $('#PhoneNumber').attr("disabled", 'disabled');
                            }
                            $('#DeviceType').change(function () {
                                var val = $(this).val();
                                if (val != 'iPhone') {
                                    $('#PhoneNumber').attr("disabled", 'disabled');
                                }
                                else {
                                    $('#PhoneNumber').removeAttr('disabled');
                                }
                            });

                            ValidateAjaxForm($('#licenseForm'));

                        }, data);

                }
            });
        }
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

MobileLicense.SaveLicense = function () {
    var emp = $('#Owners').val();
    var emp1 = $('#Employee').val();
    var model = {
        Id: $('#Id').val(),
        LicenseNumber: $('#LicenseNumber').val(),
        DeviceId: $('#DeviceId').val(),
        DeviceName: $('#DeviceName').val(),
        DeviceType: $('#DeviceType').val(),
        Employee: emp == undefined ? emp1 : emp,
        GpsFrequency: $('#GpsFrequencies').val(),
        PhoneNumber: $('#PhoneNumber').val(),
        Edit: $('#Edit').val()
    };
    var form = $('#licenseForm');

    var value = $('.isChoose').val();
    var isChoose = $('select').is('.isChoose ');
    if (form.valid() && (!isChoose || value.toLowerCase().indexOf("choose") == -1)) {
        Helpers.SendJsonModelBase(model, 'mobilelicense/save', function (data) {
            $(window.container).dialog('close');
            $('form[id*="Form"] select').selectBox('destroy');
            $('#licensesGrid').trigger("reloadGrid");
            MobileLicense.GetCountLicense();
        });
    }
    
    if (value.toLowerCase().indexOf("choose") != -1) {
        $('.isChoose').css("border-color", "red");
    }
};

MobileLicense.GetLicenseData = function (id) {
    return $.ajax({
        type: "GET",
        url: "mobilelicense/viewlicense",
        data: id.replace('?', ''),
        success: function (data) {
            $("#licensesInfo").html(data);
            ScrollToBottom();
            Helpers.RemoveActiveButtons();
        }
    });
};

MobileLicense.GetCountLicense = function () {
    $.ajax({
        type: "GET",
        url: 'mobilelicense/getcountlicense',
        success: function (data) {
            $('.activeLicense').text(data.active);
            $('.allowedLicense').text(data.allowed);
        }
    });
};

MobileLicense.ClearDeviceIdAndToken = function () {
    var ids = $('#licensesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length > 0) {
        jCustomConfirm("Clear device ID and Token?", "Warning!", function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "mobilelicense/cleardeviceid",
                    data: "ids=" + ids.join(','),
                    cache: false,
                    success: function (data) {
                        Helpers.Refresh($('#licensesGrid'), function () {
                            jAlert(data, "", null);
                        });
                    }
                });
            }else {
                Helpers.Refresh($('#licensesGrid'));
            }
        }, "Yes", "No");
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};