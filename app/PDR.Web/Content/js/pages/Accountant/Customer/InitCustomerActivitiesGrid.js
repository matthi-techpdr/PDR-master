
var TablesRenderer = function (role) {
    var estimateColumnNames = ['Creation date', 'Estimate ID', 'Year/Make/Model', 'Total amount', 'Status'];
    var roColumnNames = ['Creation date', 'Repair order ID', 'Year/Make/Model', 'Total amount', 'Status'];
    var invoiceColumnNames = ['Creation date', 'Invoice ID', 'Year/Make/Model', 'Invoice amount', 'Paid amount', 'Status'];

    var estId = { name: 'Id', index: 'Id', sortable: false };
    var roId = { name: 'Id', index: 'Id', sortable: false };

    if (role == "technician" || role == "manager" || role == "admin") {
        estId.formatoptions = { baseLinkUrl: 'Estimates/View' };
        estId.formatter = 'showlink';

        roId.formatoptions = { baseLinkUrl: "RepairOrders/View" };
        roId.formatter = 'showlink';
    }
    var estimateColumModel = [
        { name: 'CreationDate', index: 'CreationDate', sorttype: 'date' },
        estId,
        { name: 'CarsMakeModelYear', index: 'CarsMakeModelYear', sortable: true },
        { name: 'TotalAmount', index: 'TotalAmount' },
        { name: 'Status', index: 'Status', sortable: false}];

    var roColumnModel = [
        { name: 'CreationDate', index: 'CreationDate', sorttype: 'date' },
        roId,
        { name: 'CarsMakeModelYear', index: 'CarsMakeModelYear', sortable: true },
        { name: 'TotalAmount', index: 'TotalAmount' },
        { name: 'Status', index: 'Status', sortable: false}];

    var invoiceColumnModel = [
            { name: 'CreationDate', index: 'CreationDate', sorttype: 'date' },
            { name: 'Id', index: 'Id', sortable: false },
            { name: 'CarsMakeModelYear', index: 'CarsMakeModelYear', sortable: true },
            { name: 'InvoiceSum', index: 'InvoiceSum' },
            { name: 'PaidSum', index: 'PaidSum', sortable: false },
            { name: 'InvoiceStatus', index: 'InvoiceStatus'}];

    if (role != "technician") {
        estimateColumnNames.splice(3, 0, 'Who created');
        roColumnNames.splice(3, 0, 'Technician');
        invoiceColumnNames.splice(3, 0, 'Technician');
        estimateColumModel.splice(3, 0, { name: 'EmployeeName', index: 'EmployeeName', sortable: false });
        roColumnModel.splice(3, 0, { name: 'TechnicianName', index: 'TechnicianName', sortable: false });
        invoiceColumnModel.splice(3, 0, { name: 'TechnicianName', index: 'TechnicianName', sortable: false });
    }


    var renderEstimateTable = function () {
        var estimates = { grid: $("#estimates"), getdataurl: "customers/getestimates?team=" + $('#team').val() + "&customer=" + $("#Id").val() };
        GridInitializer.InitGrid(estimateColumnNames, estimateColumModel, estimates, 'CreationDate', true, null, null, function () { $('#customerInfo .cbox').remove(); },false,false)
        .setGridParam({
            gridComplete: function () {
                var ids = $("#estimates").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#estimates").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CarsMakeModelYear',
                        cellValue: rowData.CarsMakeModelYear,
                        limiter: role != "technician" ? 22 : 30
                    }, {
                        colName: 'EmployeeName',
                        cellValue: rowData.EmployeeName,
                        limiter: role != "technician" ? 20 : 30
                    }];
                    Helpers.CellFormatter($("#estimates"), id, models);
                }
            }
        });
    };


    var renderRepairOrderTable = function () {
        var repairOrders = { grid: $("#repairOrders"), getdataurl: "customers/getrepairorders?team=" + $('#team').val() + "&customer=" + $("#Id").val() };
        GridInitializer.InitGrid(roColumnNames, roColumnModel, repairOrders, 'CreationDate', true, null, null, function () { $('#customerInfo .cbox').remove(); }, false, false)
        .setGridParam({
            gridComplete: function () {
                var ids = $("#repairOrders").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#repairOrders").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CarsMakeModelYear',
                        cellValue: rowData.CarsMakeModelYear,
                        limiter: role != "technician" ? 22 : 30
                    }, {
                        colName: 'TechnicianName',
                        cellValue: rowData.TechnicianName,
                        limiter: role != "technician" ? 20 : 30
                    }];
                    Helpers.CellFormatter($("#repairOrders"), id, models);
                }
            }
        });
    };

    var renderInvoiceTable = function () {
        var invoices = { grid: $("#invoices"), getdataurl: "customers/getinvoices?team=" + $('#team').val() + "&customer=" + $("#Id").val() };
        GridInitializer.InitGrid(invoiceColumnNames, invoiceColumnModel, invoices, 'CreationDate', true, null, null, function () { $('#customerInfo .cbox').remove(); }, false, false)
        .setGridParam({
            gridComplete: function () {
                var ids = $("#invoices").jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = $("#invoices").jqGrid('getRowData', id);
                    var models = [{
                        colName: 'CarsMakeModelYear',
                        cellValue: rowData.CarsMakeModelYear,
                        limiter: role != "technician" ? 18 : 21
                    }, {
                        colName: 'TechnicianName',
                        cellValue: rowData.TechnicianName,
                        limiter: role != "technician" ? 18 : 30
                    }];
                    Helpers.CellFormatter($("#invoices"), id, models);
                }
            }
        });
    };

    this.Render = function () {
        renderEstimateTable();
        renderRepairOrderTable();
        renderInvoiceTable();
        var isiPad = navigator.userAgent.match(/iPad/i) != null;
        if (isiPad) {
            iPadStyles(4);
        }
        else {
            $('select').selectBox();
        }

        $('#pager1_right table tbody tr td[dir="ltr"]').last().prepend('<span class="showLabel">Show:</span>').css('text-align', 'right');
        $('#pager2_right table tbody tr td[dir="ltr"]').last().prepend('<span class="showLabel">Show:</span>').css('text-align', 'right');
        $('#pager3_right table tbody tr td[dir="ltr"]').last().prepend('<span class="showLabel">Show:</span>').css('text-align', 'right');
        window.pagersCount = 1;

        $('#team').change(function () {
            $("#estimates").setGridParam({
                url: "customers/getestimates?customer=" + $("#Id").val() + "&team=" + $(this).val()
            }).trigger("reloadGrid");

            $("#repairOrders").setGridParam({
                url: "customers/getrepairorders?customer=" + $("#Id").val() + "&team=" + $(this).val()
            }).trigger("reloadGrid");

            $("#invoices").setGridParam({
                url: "customers/getinvoices?customer=" + $("#Id").val() + "&team=" + $(this).val()
            }).trigger("reloadGrid");


            ($("#customersGrid").length == 0 ? $("#customerGrid") : $("#customersGrid")).setGridParam({
                url: "customers/getdata?state=" + $("#UsaStates").val() + "&team=" + $(this).val()
            }).trigger("reloadGrid");
        });
    };

    this.RenderOnlyEstimates = function () {
        renderEstimateTable();
        var isiPad = navigator.userAgent.match(/iPad/i) != null;
        if (isiPad) {
            iPadStyles(2);
        } else {
            $('select').selectBox();
        }

        $('#pager1_right table tbody tr td[dir="ltr"]').last().prepend('<span class="showLabel">Show:</span>').css('text-align', 'right');
        window.pagersCount = 1;
    };

    function iPadStyles(length) {
        var selects = $('select').not('.multiselect').not('.combobox');
        $.each(selects, function () {
            var elem = $(this).parent().hasClass('wrapperBlock');
            if (!elem) {
                $(this).wrap('<div class="wrapperBlock" style="left:-5px;top:-5px;"></div>');
                $(this).after('<span class="selectBox-arrow"></span>');
            }
        });
        for (var i = 1; i < length; i++) {
            var td = $('#pager' + i + '_right table tbody').find('tr:first-child').find('td:last-child');
            $('#pager' + i + '_right table tbody tr').remove('td:last-child');
            //$(td).css('border', '1px solid black');
            $('#pager' + i + '_right table tbody').find('tr:first-child').find('td:first-child').before(td);
            //$(td).find('wrapperBlock').css('border', '1px solid black');
            $(td).find('select').css('left', '-3px');
            $(td).find('.selectBox-arrow').css('position', 'absolute').css('top', '6px');
        }
    };
};