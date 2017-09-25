function RepairOrder(param) {
    this.grid = $("#repairOrdersGrid");
    this.getdataurl = "repairorders/getrepairordersdata?finalised=false&" + param + '&isNeedFilter=true';
};

function formatterViewLink(cellvalue) {
    return "<a href=RepairOrders/View/" + cellvalue + ">" + cellvalue + "</a>";
}

$(function () {
    var param = "";
    if ($("#IsStartSearch").val()) {
        param = "&isStartSearch=true" + GetParamForSearch();
    }

    var repairOrder = new RepairOrder(param);
    GridInitializer.InitGrid(
        [
            'New',
            'Creation date',
            'Order ID',
            'Customer name',
            'Year/make/model',
            'Total amount',
            'Status',
            'My percent',
            'HasOrderSignature',
            'EditableStatus',
            'RoStatus'
        ], [
            { name: 'New', index: 'New', hidden: true },
            { name: 'CreationDate', index: 'CreationDate' },
            { name: 'Id', index: 'Id', formatter: formatterViewLink, sortable: false },
            { name: 'CustomerName', index: 'CustomerName', sortable: false },
            { name: 'CarInfo', index: 'CarInfo', sortable: true, width: 200 },
            { name: 'TotalAmount', index: 'TotalAmount' },
            { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false },
            { name: 'Percent', index: 'Percent', sortable: false },
            { name: 'HasOrderSignature  ', index: 'HasOrdersignature', hidden: true },
            { name: 'EditableStatus', index: 'EditableStatus', hidden: true },
            { name: 'RoStatus', index: 'RoStatus', hidden: true }
        ],
        repairOrder,
        'CreationDate',
        true
    )
        .navButtonAdd('#pager', { caption: "Print", onClickButton: function () { RO.ToPrint(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "E-mail", onClickButton: function () { RO.Email(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Complete", onClickButton: function () { RO.MarkAsComplete(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Approve", onClickButton: function () { RO.Approve(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Generate invoice", onClickButton: function () { RO.GenerateInvoice(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "Assign", onClickButton: function () { RO.AssignMoreTechnicians(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "To Edit", onClickButton: function () { RO.RequestForEdit(repairOrder.grid); }, position: "last", title: "" })
        .navButtonAdd('#pager', { caption: "History", onClickButton: function () { RO.History(repairOrder.grid, 'repairorders/gethistory'); }, position: "last", title: "" })

        .setGridParam({
            gridComplete: function () {
                Helpers.HighlightNew(repairOrder.grid);
                var ids = repairOrder.grid.jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = repairOrder.grid.jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CustomerName',
                        cellValue: rowData.CustomerName,
                        limiter: 19
                    }, {
                        colName: 'CarInfo',
                        cellValue: rowData.CarInfo,
                        limiter: 23
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
});