function Team() {
    this.grid = $("#teamGrid");
    this.getdataurl = "teams/getdata";
    this.getcurrenturl = "teams/getteam";
    this.editurl = "teams/editteam";
    this.createurl = "teams/createteam";
    this.suspendurl = "teams/suspendteam";
    this.reactivateurl = "teams/reactivateteam";
    this.editform = '#teamForm';
    this.editcontainer = $('#teamInfo');
    this.name = 'team';
};

$(function() {
    var team = new Team();
    GridInitializer.InitGrid(
        [
            'Id',
            'Creation date',
            'Title',
            'Technicians',
            'Managers',
            'Status ',
            'Open estimates',
            'Open repair orders',
            'Non-paid invoices'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'CreationDate', index: 'CreationDate' },
            { name: 'Title', index: 'Title', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Team.GetTeamData(\'', addParam: '\');' }},
            { name: 'TechniciansAmount', index: 'TechniciansAmount', sortable: false },
            { name: 'ManagersAmount', index: 'ManagersAmount', sortable: false },
            { name: 'Status', index: 'Status', sortable: false },
            { name: 'OpenEstimatesAmount', index: 'OpenEstimatesAmount', sortable: false },
            { name: 'OpenWorkOrdersAmount', index: 'OpenWorkOrdersAmount', sortable: false },
            { name: 'NonPaidInvoicesAmount', index: 'NonPaidInvoicesAmount', sortable: false }
        ],
        team,
        'Title',
        null,
        saveJsonModel,
        null,
        null,
        true
    ).setGridParam({
            gridComplete: function () {
                var ids = $("#teamGrid").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#teamGrid").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'Title',
                        cellValue: rowData.Title,
                        limiter: 15
                    }];
                    Helpers.CellFormatter($("#teamGrid"), id, models);
                }
            }
        });

    function saveJsonModel() {
        var model = { Id: $("#Id").val(), Title: $("#Title").val(), Comments: $("#Comments").val(), EmployeesList: $("#EmployeesList").val() };
        var id = $('#teamForm #Id').val();
        var form = $('#teamForm');
        if (form.valid()) {
            var url = id == "null" ? team.createurl : team.editurl;
            Helpers.SendJsonModel(team.editform, url, team.grid, model);
        }
    }
});

Team.GetTeamData = function (id) {
    return $.ajax({
        type: "GET",
        url: "teams/getteam",
        cache: false,
        data: id.replace('?', '') + "&edit=" + false,
        success: function (data) {
            $("#teamInfo").html(data);
            ScrollToBottom();
        }
    });
};

window.customSuspend = function () {
    var grid = $('#teamGrid');
    var id = grid.jqGrid('getGridParam', 'selarrrow');
    if (id.length == 1) {
        jConfirm("Suspend?", "Confirm", function (r) {
            if (r) {
                return $.ajax({
                    url: "teams/suspend",
                    data: "id=" + id,
                    type:'post',
                    success: function (data) {
                        jAlert(data, "Alert", function () {
                            grid.trigger('reloadGrid');
                            Helpers.RemoveActiveButtons();
                        });
                    }
                });
            }
        });
    }
    else {
        return jAlert("Select one row, please", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};

