function Customer() {
    this.grid = $("#customersGrid");
    this.getdataurl = "customers/getdata?team=0";
    this.editcontainer = $("#customerInfo");
};

$(function() {
    var custGrid = GridInitializer.InitGrid(
        [
            'Id',
            'Name',
            'State',
            'City',
            'Phone',
            'E-mail'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerData(\'', addParam: '\');' },width:300 },
            { name: 'State', index: 'State', sortable: false, width: 40 },
            { name: 'City', index: 'City', sortable: false, width: 160 },
            { name: 'Phone', index: 'Phone', sortable: false, width: 160 },
            { name: 'Email', index: 'Email', sortable: false, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerEmail(\'', addParam: '\');' }, width: 240 }
        ],
        new Customer(),
        'Name',
        true,
        null,
        null,
        function () { $('.cbox').remove(); },
        "asc"
    );

    custGrid.setGridParam({
        gridComplete: function () {
            var ids = $("#customersGrid").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var rowData = $("#customersGrid").jqGrid('getRowData', id);
                var models = [{
                    colName: 'Name',
                    cellValue: rowData.Name,
                    limiter: 45
                }, {
                    colName: 'Email',
                    cellValue: rowData.Email,
                    limiter: 36
                }];
                Helpers.CellFormatter($("#customersGrid"), id, models);
            }
        }
    });

    $('#team').selectBox("value", 0);

    $('#UsaStates').change(function () {
        ($("#customersGrid").length == 0 ? $("#customerGrid") : $("#customersGrid")).setGridParam({
            url: "customers/getdata?state=" + $(this).val() + "&team=" + $("#team").val()
        }).trigger("reloadGrid");
    });

    $('#team').change(function () {
        ($("#customersGrid").length == 0 ? $("#customerGrid") : $("#customersGrid")).setGridParam({
            url: "customers/getdata?state=" + $("#UsaStates").val() + "&team=" + $(this).val()
        }).trigger("reloadGrid");
    });
});

Customer.GetCustomerData = function (id) {
    return $.ajax({
        type: "GET",
        url: "customers/getcustomerdetails",
        data: id.replace('?', ''),
        cahce: false,
        success: function (data) {
            $("#customerInfo").html(data);
            ScrollTo($(".customer-info"));
            Helpers.RemoveActiveButtons();
        }
    });
};

Customer.GetCustomerEmail = function (id) {
    $.ajax({
        type: "POST",
        url: "customers/getemaildialog",
        cache: false,
        data: 'ids=' + id.replace('?id=', '') + '&customer=true',
        beforeSend: function () {
            $('#loader').show();
        },
        complete: function () {
            $('#loader').hide();
        },    
        success: function (data) {
            Helpers.GetEmailDialog(data, $("#customersGrid"));
        }
    });
};