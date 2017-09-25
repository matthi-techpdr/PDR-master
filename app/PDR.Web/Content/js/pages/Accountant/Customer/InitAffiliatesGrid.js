function Customer() {
    this.grid = $("#customerGrid");
    this.getdataurl = "location/getdata?team=" + $('#team').val() + "&state=" + $('#UsaStates').val();
    this.getcurrenturl = "location/getaffiliates";
    this.editurl = "location/editaffiliate";
    this.createurl = "location/createaffiliates";
    this.suspendurl = "location/suspendcustomer";
    this.reactivateurl = "location/reactivatecustomer";
    this.name = 'location';
};

$(function () {
    var customer = new Customer;
    var colNames = window.IsAdmin ? [
        'Id',
        'Name',
        'State',
        'City',
        'Phone',
        'E-mail',
//        'Open estimates',
//        'Sum of open estimates',
//        'Sum of open work orders',
//        'Sum of paid invoices',
//        'Sum of unpaid invoices'
    ] : [
        'Id',
        'Adding date',
        'Name',
        'Email',
        'Phone',
        'Status'
    ];

    var colModels = window.IsAdmin ? 
    [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerData(\'', addParam: '\');' }, width: 300 },
            { name: 'State', index: 'State', sortable: false, width: 40 },
            { name: 'City', index: 'City', sortable: false, width: 160 },
            { name: 'Phone', index: 'Phone', sortable: false, width: 160 },
            { name: 'Email', index: 'Email', sortable: false, formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerEmail(\'', addParam: '\');' }, width: 240 },

//            { name: 'AmountOfOpenEstimates', index: 'AmountOfOpenEstimates', sortable: false, width: 120 },
//            { name: 'SumOfOpenEstimates', index: 'SumOfOpenEstimates', sortable: false, width: 120 },
//            { name: 'SumOfOpenWorkOrders', index: 'SumOfOpenWorkOrders', sortable: false, width: 120 },
//            { name: 'SumOfPaidInvoices', index: 'SumOfPaidInvoices', sortable: false, width: 120 },
//            { name: 'SumOfUnpaidInvoices', index: 'SumOfUnpaidInvoices', sortable: false, width: 120 }
        ] :
        [
        { name: 'Id', index: 'Id', hidden: true },
        { name: 'CreatingDate', index: 'CreatingDate', sorttype: 'date', width: 150 },
        { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerData(\'', addParam: '\');' }, width: 250 },
        { name: 'Email', index: 'Email', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Customer.GetCustomerEmail(\'', addParam: '\');' }, width: 300, sortable: false },
        { name: 'Phone', index: 'Phone', width: 200, sortable: false },
        { name: 'Status', index: 'Status', width: 150, sortable: false }
    ];
    
    GridInitializer.InitGrid(
       colNames , colModels , customer, 'Name', null, saveJsonModel, null, null, true)
        .setGridParam({
            gridComplete: function () {
                var ids = $("#customerGrid").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#customerGrid").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'Name',
                        cellValue: rowData.Name,
                        limiter: 45
                    }, {
                        colName: 'Email',
                        cellValue: rowData.Email,
                        limiter: 36
                    }];
                    Helpers.CellFormatter($("#customerGrid"), id, models);
                }
            }
        });

        $.validator.addMethod("uniqueEmail", function (value, element) {
            value = value.toLowerCase();
            var emails = [];
            for (var i = 0; i < $(element).data('emails').length; i++) {
                var item = $(element).data('emails')[i];
                emails.push(item.toLowerCase());
            }
            var result = this.optional(element) || !emails.contains(value);
            return result;
        }, "Email already exist.");

    //$('#team').selectBox("value", 0);
    $('#UsaStates').change(function () {
        CookieManager.SetCookie("state", $(this).val());
        ($("#customersGrid").length == 0 ? $("#customerGrid") : $("#customersGrid")).setGridParam({
            url: "location/getdata?state=" + $("#UsaStates").val() + "&team=" + $(this).val()
        }).trigger("reloadGrid");
    });

    $('#team').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        ($("#customersGrid").length == 0 ? $("#customerGrid") : $("#customersGrid")).setGridParam({
            url: "location/getdata?state=" + $("#UsaStates").val() + "&team=" + $(this).val()
        }).trigger("reloadGrid");
    });

    function saveJsonModel() {
        var model = {
            Id: $("#customerForm #Id").val(),
            Name: $("#customerForm #Name").val(),
            Address1: $("#customerForm #Address1").val(),
            Address2: $("#customerForm #Address2").val(),

            Zip: $("#customerForm #Zip").val(),
            State: $("#customerForm #stateUsa").val(),
            City: $("#customerForm #City").val(),
            Phone: $("#customerForm #Phone").val(),
            Fax: $("#customerForm #Fax").val(),
            ContactName: $("#customerForm #ContactName").val(),
            ContactTitle: $("#customerForm #ContactTitle").val(),

            Email: $("#customerForm #Email").val(),
            LaborRate: $("#customerForm #LaborRate").val(),
            PartRate: $("#customerForm #PartRate").val(),
            HourlyRate: $("#customerForm #HourlyRate").val(),

            TeamsIds: $("#customerForm #TeamsList").val(),
            Comment: $("#customerForm #Comment").val()
        };
        var id = $('#customerForm #Id').val();
        var form = $('#customerForm');
        var teamsList = $('#customerForm #TeamsList');

        if ((form.valid() && teamsList.val() != null)) {
            var url = id == "null" ? customer.createurl : customer.editurl;
            Helpers.SendJsonModel(customer.editform, url, customer.grid, model);
        }
        else if (teamsList.val() == null) {
            var el = teamsList.val() == null ? teamsList.next() : matricesList.next();
            if (teamsList.val() == null) {
                el = $('.ui-multiselect');
            }
            el.addClass('input-validation-error').qtip({
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
    };
});

Customer.GetCustomerData = function (id) {
    return $.ajax({
        type: "GET",
        url: "location/getaffiliates",
        cache: false,
        data: id.replace('?', '') + "&edit=" + false,
        success: function (data) {
            $('select').selectBox('destroy');
            $("#customerInfo").html(data);
            ScrollTo($("#customerInfo"));
        }
    });
};

Customer.GetCustomerEmail = function (id) {
    $.ajax({
        type: "POST",
        url: "location/getemaildialog",
        cache: false,
        data: 'ids=' + id.replace('?id=', '') + '&customer=true',
        success: function (data) {
            Helpers.GetEmailDialog(data, $('#customerGrid'));
        }
    });
};

