function Employee() {
    this.grid = $("#employeeGrid");
    this.getdataurl = "employees/getdata";
    this.getcurrenturl = "employees/getemployee";
    this.editurl = "employees/editemployee";
    this.createurl = "employees/createemployee";
    this.suspendurl = "employees/suspendemployee";
    this.reactivateurl = "employees/reactivateemployee";
    this.editform = $('#employeeForm');
    this.editcontainer = $("#employeeInfo");
    this.name = 'employee';
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
            'Team',
            'Role',
            'Open estimates',
            'Open repair orders',
            'Account status'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'HiringDate', index: 'HiringDate', width: 120 },
            { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Employee.GetEmployeeData(\'', addParam: '\');'} },
            { name: 'Email', index: 'Email', width: 200, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Employee.GetEmployeeEmail(\'', addParam: '\');'}, sortable: false  },
            { name: 'PhoneNumber', index: 'PhoneNumber', sortable: false, width: 120 },
            { name: 'Team', index: 'Team', sortable: false, width: 160 },
            { name: 'Role', index: 'Role', width: 120 },
            { name: 'OpenEstimatesAmount', index: 'OpenEstimatesAmount', sortable: false, width: 100 },
            { name: 'OpenWorkOrdersAmount', index: 'OpenWorkOrdersAmount', sortable: false, width: 100 },
            { name: 'Status', index: 'Status', sortable: false, width: 100 }
        ],
        employee,
        'Name',
        null,
        saveJsonModel,
        "You can not create more employees.",
        updateActiveEmployees,
        true
    )
        //.navButtonAdd('#pager', { caption: "Reset password", onClickButton: function () { Employee.ResetPassword(); }, position: "last", title: "" })
        .setGridParam({
            gridComplete: function () {
                var ids = $("#employeeGrid").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#employeeGrid").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'Name',
                        cellValue: rowData.Name,
                        limiter: 15
                    }, {
                        colName: 'Email',
                        cellValue: rowData.Email,
                        limiter: 22
                    }, {
                        colName: 'Team',
                        cellValue: rowData.Team,
                        limiter: 16
                    }];
                    Helpers.CellFormatter($("#employeeGrid"), id, models);
                }
            }
        });

    function updateActiveEmployees(data) {
        $('#activeEmp').html(data.active);
    }

    function saveJsonModel() {
        var newRole = $("#employeeForm #Role").val();
        var canEditTeamMembers = newRole == '1' ? $("#employeeForm #CanEditTeamMembers").is(':checked') : null;
        var model = {
            Id: $("#employeeForm #Id").val(),
            Name: $("#employeeForm #Name").val(),
            SignatureName: $("#employeeForm #SignatureName").val(),
            Address: $("#employeeForm #Address").val(),
            PhoneNumber: $("#employeeForm #PhoneNumber").val(),
            Email: $("#employeeForm #Email").val(),
            Comment: $("#employeeForm #Comment").val(),
            TeamsList: $("#employeeForm #TeamsList").val(),
            Login: $("#employeeForm #Login").val(),
            Password: $("#employeeForm #Password").val(),
            Commission: $("#employeeForm #Commission").val(),
            CanQuickEstimate: $("#employeeForm #CanQuickEstimate").is(':checked'),
            CanExtraQuickEstimate: $("#employeeForm #CanExtraQuickEstimate").is(':checked'),
            IsShowAllTeams: $("#employeeForm #IsShowAllTeams").is(':checked'),
            CanEditTeamMembers: canEditTeamMembers,
            Role:  newRole,
            TaxId: $("#employeeForm #TaxId").val(),
            City: $("#employeeForm #City").val(),
            Zip: $("#employeeForm #Zip").val(),
            State: $("#employeeForm #stateUsa").val(),
        };
        var id = $('#employeeForm #Id').val();
        var form = $('#employeeForm');
        var teamsList = $('#TeamsList');
        if ((form.valid() && teamsList.is(':disabled') || (form.valid() && teamsList.val() != null && !teamsList.is(':disabled')))) {
            var url = id == "null" ? employee.createurl : employee.editurl;
            Helpers.SendJsonModel(employee.editform, url, employee.grid, model);
        }
        else if (teamsList.val() == null && !teamsList.is(':disabled')) {
            $('.ui-multiselect').addClass('input-validation-error').qtip({
                content: 'You must choose one item at least',
                position: {
                    corner: {
                        target: 'topRight',
                        tooltip: 'bottomLeft'
                    }
                },
                show: 'mouseover',
                hide: 'mouseout',
                style: 'pdrstyle'
            });
        }
    }

    $('#Roles').change(function () {
        $('#employeeGrid').setGridParam({
            url: 'employees/getdata?role=' + $(this).val()
        }).trigger("reloadGrid");
    });

    $.validator.addMethod("uniqueLogin", function (value, element) {
        return this.optional(element) || !$(element).data('logins').contains(value);
    }, "Login already exist.");

    window.sendEmailUrl = "employees/sendemail";
});

Employee.GetEmployeeData = function (id) {
    return $.ajax({
        type: "GET",
        url: "employees/getemployee",
        data: id.replace('?', '') + "&edit=" + false,
        cache: false,
        success: function (data) {
            $("#employeeInfo").html(data);
            ScrollToBottom();
        }
    });
};

Employee.GetEmployeeEmail = function (id) {
    return $.ajax({
        type: "GET",
        cache: false,
        url: "employees/getemaildialog",
        data: id.replace('?', ''),
        success: function (data) {
            Helpers.GetEmailDialog(data);
        }
    });
};

Employee.DisableInputs = function () {
    if ($('#Active').val() == "True" && $('#Id').val() != "null") {
        var currentRole = $('#currentRole').val();
        $('#Role').selectBox('value', currentRole);
        return jAlert("The role can be changed only for suspended employee.");
    }
    var isTeamEmployee = $("#Role").val() == 0 || $("#Role").val() == 1;

    if (isTeamEmployee) {
        $('#Commission').removeAttr('disabled');
        $('#TeamsList').removeAttr('disabled');
        $('#teamLabel').show();
        $('.ui-multiselect').show();
    }
    else {
        $('#Commission').attr('disabled', 'disabled').removeClass("input-validation-error");
        $('#TeamsList').attr('disabled', 'disabled');
        $('#teamLabel').attr('disabled', 'disabled');
        $('.ui-multiselect').hide();
        $('#teamLabel').hide();
        if ($('#Role').val() == 2 || $('#Role').val() == 128) {
            $('.checkboxesLabel').hide();
        }
    }
    if ($('#Role').val() != 2 && $('#Role').val() != 128) {
        $('.checkboxesLabel').show();
    }

    if ($('#Role').val() == 1) {
        $('.checkboxesForManager').show();
    } else {
        $('.checkboxesForManager').hide();
    }

    if ($('#Role').val() == 128) {
        var parent = $('#Commission').parent();
        $(parent).children().hide();
        
        $('#TeamsList').removeAttr('disabled');
        $('#teamLabel').show();
        $('.ui-multiselect').show();
        
    } else {
        var parent = $('#Commission').parent();
        $(parent).children().show();
    }

};

Employee.ResetPassword = function () {
    var ids = $("#employeeGrid").jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        return jConfirm("Are you sure?", "Confirm", function (r) {
            if (r) {
                $.ajax({
                    url: 'employees/resetpassword',
                    data: 'ids=' + ids,
                    success: function (data) {
                        jAlert(data, "Success", function () {
                            Helpers.RemoveActiveButtons();
                        });
                    }
                });
            }
            else {
                Helpers.RemoveActiveButtons();
            }
        });
    }
    else {
        jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};

window.customSuspend = function () {
    var grid = $('#employeeGrid');
    var id = grid.jqGrid('getGridParam', 'selarrrow');
    if (id.length == 1) {
        jConfirm("Suspend?", "Confirm", function (r) {
            if (r) {
                return $.ajax({
                    url: "employees/getmanagers",
                    data: "id=" + id,
                    statusCode: {
                        204: function () {
                            var msg = "Success";
                            jAlert(msg, "Alert", function () {
                                $('#employeeGrid').trigger("reloadGrid");
                                Helpers.RemoveActiveButtons();
                            });
                        },
                        200: function (dt) {
                            Helpers.GetDialogBase(400, 200, "Suspend employee", [{
                                width: 178,
                                text: "Re-assign & suspend",
                                click: function () {
                                    var managerId = $('#managers').val();
                                    var empId = $('#empId').val();
                                    var model = { "empId": empId, "managerId": managerId };
                                    Helpers.SendJsonModelBase(model, "employees/suspendemployee", function () {
                                        
                                                    $(container).dialog('close');
                                                    $('#employeeGrid').trigger("reloadGrid");
                                                });
                                            }
                                        },
                                {
                                    width: 178,
                                    text: "Cancel",
                                    click: function () {
                                        $(this).dialog('close');
                                    }
                                }], null, dt);
                        }
                    }
                });
            }
        });
            }
            else {
                return jAlert("Please select one row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
            }
};
