function ClearMyDeviceId () {
        jCustomConfirm("Clear device ID and Token?", "Warning!", function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "settings/cleardeviceid",
                    cache: false,
                    success: function (data) {
                            jAlert(data, "", null);
                    }
                });
            }
        }, "Yes", "No");
        };

function SaveJsonModel() {
    var model = {
        Id: $("#employeeForm #Id").val(),
        Name: $("#employeeForm #Name").val(),
        SignatureName: $("#employeeForm #SignatureName").val(),
        Address: $("#employeeForm #Address").val(),
        PhoneNumber: $("#employeeForm #PhoneNumber").val(),
        Email: $("#employeeForm #Email").val(),
        Login: $("#employeeForm #Login").val(),
        Password: $("#employeeForm #Password").val(),
        TaxId: $("#employeeForm #TaxId").val(),
        City: $("#employeeForm #City").val(),
        Zip: $("#employeeForm #Zip").val(),
        State: $("#employeeForm #stateUsa").val(),
        IsBasic: $('#IsBasic_true').is(':checked')
    };
    var form = $('#employeeForm');
    if (form.valid()) {
        var url = 'settings/index';
        Helpers.SendJsonModelBase(model, url, function (data) { jAlert(data, "", null); });
    }
}

