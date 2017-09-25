function ReportGridGetter() {
}

ReportGridGetter.IsManager = $('#Manager').val();

ReportGridGetter.addButtons = function (grid, dataUrl, notForDetails, gridType) {
    grid
        .navButtonAdd('#pager', { caption: "Save/Print", onClickButton: function () { Helpers.RemoveActiveButtons(); Report.SaveToPdf(grid); }, title: "" })
        .setGridParam({
            gridComplete: function () {
                var boxes = $('.cbox');
                $.each(boxes, function () {
                    $(this).hide();
                });
                var models = null;
                var ids = grid.jqGrid('getDataIDs');
                for (var i = 0; i < ids.length; i++) {
                    var id = ids[i];
                    var rowData = grid.jqGrid('getRowData', id);
                    if (gridType == "Estimate") {
                        models = [{
                            colName: 'Customer_LastName',
                            cellValue: rowData.Customer_LastName,
                            limiter: 22
                        }, {
                            colName: 'Employee',
                            cellValue: rowData.Employee,
                            limiter: 20
                        }]; 
                    }
                    else if (gridType == "RO") {
                        models = [{
                            colName: 'CustomerName',
                            cellValue: rowData.CustomerName,
                            limiter: 23
                        }];
                    }
                    else if (gridType == "Invoice") {
                        models = [{
                            colName: 'CustomerName',
                            cellValue: rowData.CustomerName,
                            limiter: 20
                        }//, {
//                            colName: 'Employee',
//                            cellValue: rowData.Employee,
//                            limiter: 20
//                        }
                        ];
                    }
                    Helpers.CellFormatter(grid, id, models);
                }
            }
        }); ;

    if (notForDetails) {
        grid.navButtonAdd('#pager', { caption: "Save report", onClickButton: function () { Helpers.RemoveActiveButtons(); Report.Save(); }, position: "first", title: "" });
        Report.InitControls(grid, dataUrl.replace('?team=' + $('#Team').val(), ''));
    }
};

ReportGridGetter.GetEstimateGrid = function (report, notForDetails) {
    var grid = GridInitializer.InitGrid(
            [
                'Id',
                'Creation date',
                'Estimate ID',
                'Customer name',
                'Employee',
                'Total amount',
                'Status'
            ], [
                { name: 'Id', index: 'Id', hidden: true },
                { name: 'CreationDate', index: 'CreationDate' },
                { name: 'Id', index: 'Id', sortable: false },
                { name: 'Customer_LastName', index: 'Customer_LastName', sortable: false },
                { name: 'Employee', index: 'Employee', sortable: false, hidden: $('#Manager').val() != null || window.IsAdmin ? false : true },
                { name: 'TotalAmount', index: 'TotalAmount', sortable: false },
                { name: 'EstimateStatus', index: 'EstimateStatus', sortable: false }
            ],
            report,
            'CreationDate',
            true
    );

    grid.setGridParam({
            
        });

    ReportGridGetter.addButtons(grid, report.getdataurl, notForDetails, "Estimate");
};

ReportGridGetter.GetRoGrid = function (report, notForDetails) {
       var grid = GridInitializer.InitGrid(
           [
               'Id',
               'Creation date',
               'Repair order ID',
               'Customer name',
               'Employee',
               'Total amount',
               'Status'
           ], [
               { name: 'Id', index: 'Id', hidden: true },
               { name: 'CreationDate', index: 'CreationDate', sortable: true },
               { name: 'Id', index: 'Id', hidden: false, sortable: false },
               { name: 'CustomerName', index: 'CustomerName', sortable: false },
               { name: 'Employee', index: 'Employee', sortable: false, hidden: $('#Manager').val() != null ? false : true },
               { name: 'TotalAmount', index: 'TotalAmount', sortable: false },
               { name: 'RepairOrderStatus', index: 'RepairOrderStatus', sortable: false }
           ],
           report,
           'CreationDate',
           true
    );
       ReportGridGetter.addButtons(grid, report.getdataurl, notForDetails, "RO");
   };

ReportGridGetter.GetInvoiceGrid = function(report, notForDetails, hideCommission) {
    var grid = GridInitializer.InitGrid(
        [
            'Id',
            'Creation date',
            'Invoice ID',
            'Customer name',
            'Employee',
            'Invoice amount',
            'Paid amount',
            'Status',
            'My commission'
            
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'CreationDate', index: 'CreationDate', width: 160 },
            { name: 'Id', index: 'Id', sortable: false, width: 120 },
            { name: 'CustomerName', index: 'CustomerName', sortable: false, width: 160 },
            { name: 'Employee', index: 'Employee', sortable: false, hidden: $('#Manager').val() != null ? false : true },
            { name: 'InvoiceAmount', index: 'InvoiceAmount' },
            { name: 'PaidAmount', index: 'PaidAmount', sortable: false },
            { name: 'InvoiceStatus', index: 'InvoiceStatus', sortable: false },
            { name: 'Commission', index: 'Commission', sortable: false, hidden: hideCommission, width: 150 }
        ],
        report,
        'CreationDate',
        true
    );
    ReportGridGetter.addButtons(grid, report.getdataurl, notForDetails, "Invoice");
};
   
