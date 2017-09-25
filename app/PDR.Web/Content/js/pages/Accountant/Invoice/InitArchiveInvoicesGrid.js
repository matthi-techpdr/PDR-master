function Invoice() {
    this.grid = $("#archiveInvoicesGrid");
    this.getdataurl = "getdata?archived=true";
//    this.getcurrenturl = "employees/getemployee";
//    this.editurl = "employees/editemployee";
//    this.createurl = "employees/createemployee";
//    this.suspendurl = "employees/suspendemployee";
//    this.reactivateurl = "employees/reactivateemployee";
//    this.editform = '#estimateForm';
//    this.editcontainer = $("#estimateInfo");
};

$(function () {
    var invoice = new Invoice();
    GridInitializer.InitGrid(
        [
            'Creation date',
            'Invoice ID',
            'Customer name',
            'Year/make/model',
            'Team',
            'Technician(s)',
            'Invoice amount',
            'Paid amount',
            'Status',
            'Paid date',
            'Commission'
        ], [
            { name: 'CreationDate', index: 'CreationDate',  width: 160 },
            { name: 'Id', index: 'Id', hidden: false, sortable: false },
            { name: 'CustomerName', index: 'CustomerName', sortable: false },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
            { name: 'Team', index: 'Team', sortable: false, width: 200 },
            { name: 'Technicians', index: 'Technicians', sortable: false, width: 200 },
            { name: 'InvoiceSum', index: 'InvoiceSum', sorttype: 'currency' },
            { name: 'PaidSum', index: 'PaidSum', sortable: false, editable: true, edittype: 'text' },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: false },
            { name: 'PaidDate', index: 'PaidDate', sortable: false },
            { name: 'Commission', index: 'Commission', sortable: false, hidden: false }
        ],
        invoice,
        'CreationDate',
        true
    ).navButtonAdd('#pager', { caption: "View/Print", onClickButton: function () { Invoice.ToPrint(); }, position: "first", title: "" })
     .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { Invoice.EmailSending(); }, position: "last", title: "" })
     .navButtonAdd('#pager', { caption: "Mark invoice as paid", onClickButton: function () { }, position: "last", title: "" })
     .navButtonAdd('#pager', { caption: "Export to QuickBook", onClickButton: function () { }, position: "last", title: "" })
     .navButtonAdd('#pager', { caption: "Un-archive", onClickButton: function () { Invoice.ToNonArchived(); }, position: "last", title: "" })

    Helpers.InitDateRangeTextboxes('from', 'to', function () { Helpers.UpdateReportGrid($('#archiveInvoicesGrid'), $('#from'), $('#to'), $('#Customers'), 'getdata'); });
    
    $('#Customers').change(function () {
        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?archived=true&customer=' + $(this).val()
        }).trigger("reloadGrid");
    });

    $('#Teams').change(function () {

        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?team=' + $(this).val() + "&archived=true"
        }).trigger("reloadGrid");
    });

    $('#Statuses').change(function () {

        $('#archiveInvoicesGrid').setGridParam({
            url: 'getdata?status=' + $(this).val() + "&archived=true"
        }).trigger("reloadGrid");
    });
});

Invoice.GetEstimateData = function (id) {
    return $.ajax({
        type: "GET",
        url: "getinvoice",
        data: id.replace('?', '') + "&edit=" + false,
        success: function (data) {
            $("#companyInfo").html(data);
        }
    });
};

Invoice.ToNonArchived = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        jConfirm('Unarchive invoice?', null, function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: "archiveunarchive",
                    data: "ids=" + ids + "&toArchived=false",
                    success: function () {
                        Helpers.Refresh($('#archiveInvoicesGrid'), function() {
                            jAlert("Operation completed", "");
                        });
                    }
                });
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Invoice.EmailSending = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow').join(',');
    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: "getemaildialog",
            data: "ids=" + ids,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send invoices of different customers in the same message. \tPlease select invoices related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh($('#archiveInvoicesGrid'));
                        });
                }
                else {
                    Helpers.GetEmailDialog(data, $('#archiveInvoicesGrid'));
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

Invoice.ToPrint = function () {
    var ids = $('#archiveInvoicesGrid').jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
            Helpers.Refresh($('#archiveInvoicesGrid'), function () {
                for (var i = 0; i < ids.length; i++) {
                    window.open("print?ids=" + ids[i], "_blank");
                }
            });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};