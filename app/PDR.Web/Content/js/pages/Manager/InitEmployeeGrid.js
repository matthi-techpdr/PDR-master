function Employee() {
    this.grid = $("#employeeGrid");
    this.getdataurl = "teamMembers/getdata";
};

$(function () {
    var employee = new Employee();
    GridInitializer.InitGrid(
        [
            'Id',
            'Date of hiring',
            'Name',
            'E-mail',
            'Phone',
            'Role',
            'Open estimates',
            'Open repair orders'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'HiringDate', index: 'HiringDate', width: 120 },
            { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Employee.GetEmployeeData(\'', addParam: '\');'} },
            { name: 'Email', index: 'Email', width: 200, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Employee.GetEmployeeEmail(\'', addParam: '\');'}, sortable: false },
            { name: 'PhoneNumber', index: 'PhoneNumber', sortable: false, width: 120 },

            { name: 'Role', index: 'Role', width: 120 },
            { name: 'OpenEstimatesAmount', index: 'OpenEstimatesAmount', sortable: false, width: 80 },
            { name: 'OpenWorkOrdersAmount', index: 'OpenWorkOrdersAmount', sortable: false, width: 80 }
        ],
        employee,
        'Name',
        false,
        null,
        null,
        null,
        true)
        .navButtonAdd('#pager', { caption: "Reset password", onClickButton: function () { Employee.ResetPassword(); }, position: "last", title: "" });

    $('#Roles').change(function () {
        $('#employeeGrid').setGridParam({
            url: 'teamMembers/getdata?roleId=' + $(this).val() + "&teamid=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());

        $('#employeeGrid').setGridParam({
            url: 'teamMembers/getdata?teamId=' + $(this).val() + "&roleid=" + $('#Roles').val()
        }).trigger("reloadGrid");
    });
    window.sendEmailUrl = "teamMembers/sendemail";
});

Employee.GetEmployeeEmail = function (id) {
    return $.ajax({
        type: "GET",
        cache: false,
        url: "teamMembers/getemaildialog",
        data: id.replace('?', ''),
        success: function (data) {
            Helpers.GetEmailDialog(data);
        }
    });
};

Employee.GetEmployeeData = function (id) {
    return $.ajax({
        type: "GET",
        url: "teamMembers/view",
        data: id.replace('?', ''),
        success: function (data) {
            $("#employeeDetails").html(data);
            ScrollToBottom();
        }
    });
};

Employee.ResetPassword = function() {
    var ids = $("#employeeGrid").jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        return jConfirm("Are you sure?", "Confirm", function(r) {
            if (r) {
                $.ajax({
                    url: 'teamMembers/resetpassword',
                    data: 'ids=' + ids,
                    success: function(data) {
                        jAlert(data, "Success", function() {
                            Helpers.Refresh($("#employeeGrid"));
                        });
                    }
                });
            } else {
                Helpers.RemoveActiveButtons();
            }
        });
    }
    
    return jAlert("Please select row for this operation", "Warning!", function() { Helpers.RemoveActiveButtons(); });
};