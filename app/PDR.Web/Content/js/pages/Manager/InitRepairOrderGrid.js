function RepairOrder(param) {
    this.grid = $("#repairOrdersGrid");
    this.getdataurl = "repairorders/getrepairordersdata?finalised=false&team=" + $('#Teams').val() + '&customer=' + CookieManager.GetCookie('customer') 
        + param + '&isNeedFilter=true';
};

function formatterViewLink(cellvalue) {
    return "<a href=RepairOrders/View/" + cellvalue + ">" + cellvalue + "</a>";
}

$(function () {
    var columnNames = [
        'New',
        'Creation date',
        'Order ID',
        'Employee',
        'Customer name',
        'Year/make/model',
        'Total amount',
        'Status',
        'EditableStatus',
        'RoStatus'
    ];
    var columnModel = [
        { name: 'New', index: 'New', hidden: true },
        { name: 'CreationDate', index: 'CreationDate', width: 130 },
        { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false, width: 110 },
        { name: 'Employee', index: 'Employee', sortable: false, width: 130 },
        { name: 'CustomerName', index: 'CustomerName', sortable: false, width: 160 },
        { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 210 },
        { name: 'TotalAmount', index: 'TotalAmount', width: 160 },
        { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false, width: 100 },
        { name: 'EditableStatus', index: 'EditableStatus', hidden: true },
        { name: 'RoStatus', index: 'RoStatus', hidden: true }
    ];

    if (window.IsAdmin != true) {
        columnNames.push('My percent');
        columnModel.push({ name: 'Percent', index: 'Percent', sortable: false });
    }

    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var repairOrder = new RepairOrder(param);

    GridInitializer.InitGrid(
        columnNames, columnModel,
        repairOrder,
        'CreationDate',
        true
    )
        .navButtonAdd('#pager', { caption: "Print", onClickButton: function () { RO.ToPrint(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { RO.Email(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Complete", onClickButton: function () { RO.MarkAsComplete(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Gen. invoice", onClickButton: function () { RO.GenerateInvoice(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Assign", onClickButton: function () { RO.AssignMoreTechnicians(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Participation", onClickButton: function () { RO.DefineParticipation(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "A/R edit", onClickButton: function () { RO.AllowRejectEdit(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { RO.History(repairOrder.grid, 'repairorders/gethistory'); }, position: "last", title: "" })
        .setGridParam({ gridComplete: function () {
            Helpers.HighlightNew(repairOrder.grid);
            var ids = repairOrder.grid.jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var rowData = repairOrder.grid.jqGrid('getRowData', id);
                var models = [{
                    colName: 'CustomerName',
                    cellValue: rowData.CustomerName,
                    limiter: 18
                }, {
                    colName: 'CarInfo',
                    cellValue: rowData.CarInfo,
                    limiter: 23
               }, {
                     colName: 'Employee',
                     cellValue: rowData.Employee.split(',').join(',\n'),
                 }];
                Helpers.CellFormatter(repairOrder.grid, id, models);
            }
        }
        });

    $('#StartSearch').click(function () {
        if (!isValidSearchForm()) {
            return;
        }
        param = GetParamForSearch();
        $('#repairOrdersGrid').setGridParam({
            url: 'repairorders/getrepairordersdata?finalised=false&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val() +
                "&isStartSearch=true" + param
        }).trigger("reloadGrid");
        var count = $("#repairOrdersGrid").getGridParam("reccount");
        if (count == 0) {
            jCustomConfirm("Search did not find any Repair Orders. Would you like to search in finalised docs?", "No results found...", function (r) {
                if (r) {
                    var urlRedirect = $("#finalisedRepairOrderUrl").val() + '?' + param;
                    window.location.assign(urlRedirect);
                }
                else {

                }
            }, "Yes", "No");
        } else {
            $(".h3-wrapper").trigger('click');
        }
    });

    $('.reset-button, .reset-link > a').click(function () {
        resetSearch();
        $('#repairOrdersGrid').setGridParam({
            url: 'repairorders/getrepairordersdata?finalised=false&customer=' + $('#Customers').val() + "&team=" + $('#Teams').val()
        }).trigger("reloadGrid");
    });

    RO.InitFilters(repairOrder.grid, function () { return 'repairorders/getrepairordersdata?&team=' + $('#Teams').val() + "&customer=" + $('#Customers').val() + "&finalised=false"; });

//    repairOrder.AssignMoreTechnicians = function () {
//        var ids = repairOrder.grid.jqGrid('getGridParam', 'selarrrow');
//        if (ids.length != 0) {
//            if (ids.length == 1) {
//                var status = repairOrder.grid.jqGrid('getCell', ids, 'RepairOrderStatus');
//                if (status.toLowerCase() == 'Open'.toLowerCase()) {
//                    $.ajax({
//                        type: "GET",
//                        url: "repairorders/assignmoretechnicians",
//                        cache: false,
//                        data: 'ids=' + ids,
//                        success: function (data) {
//                            Helpers.GetDialogBase('350', 'auto', 'Assign repair order',
//                                    [{ width: 150, text: "OK", click: function () {
//                                        repairOrder.AssignMoreTechniciansSave(this);
//                                    }
//                                    },
//                                        { width: 100, text: "Cancel", click: function () {
//                                            $('#repairOrdersGrid').trigger("reloadGrid");
//                                            $(this).dialog('close');
//                                        }
//                                        }
//                                    ], function () {
//                                        ValidateAjaxForm($('#assignTechniciansForm'));
//                                        $('.ui-dialog-buttonset').css("margin-left", "48px");
//                                    }, data);
//                        }
//                    });
//                }
//                else {
//                    jAlert("You can assign employee for repair orders with 'Open' statuses only", "Alert", function () {
//                        Helpers.Refresh(repairOrder.grid);
//                    });
//                }
//            }
//            else {
//                jAlert("This action cannot be done for multiple repair orders.\nSelect one row, please", "Alert", function () { Helpers.RemoveActiveButtons(); });
//            }
//        }
//        else {
//            jAlert("Select one row, please", "Alert", function () { Helpers.RemoveActiveButtons(); });
//        }
//    };

//    repairOrder.AssignMoreTechniciansSave = function (dialog) {
//        var id = $('#RepairOrderId').val();
//        var val = [];
//        $('input[type="hidden"].technician').each(function () {
//            val.push(arguments[1].value);
//        });
//        $('select.technician').each(function () {
//            val.push(arguments[1].value);
//        });

//        $.ajax({
//            type: "POST",
//            url: "repairorders/assignmoretechnicians",
//            cache: false,
//            data: 'technicianIds=' + val + '&repairOrderId=' + id,
//            success: function () {
//                $(dialog).dialog('close');
//                Helpers.Refresh($('#repairOrdersGrid'));
//            }
//        });
//    };
});




