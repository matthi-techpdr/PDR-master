function Company() {
    this.grid = $("#companyGrid");
    this.getdataurl = "companies/getdata";
    this.getcurrenturl = "companies/getcompany";
    this.editurl = "companies/editcompany";
    this.createurl = "companies/createcompany";
    this.suspendurl = "companies/suspendcompany";
    this.reactivateurl = "companies/reactivatecompany";
    this.editform = '#companyForm';
    this.editcontainer = $("#companyInfo");
    this.name = 'company';
};

$(function () {
    GridInitializer.InitGrid(
        [
            'Id',
            dateColumnTitle,
            companyNameColumnTitle,
            phoneColumnTitle,
            emailColumnTitle,
            addressColumnTitle,
            stateColumnTitle,
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'CreationDate', index: 'CreationDate', sorttype: 'date' },
            { name: 'Name', index: 'Name', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Company.GetCompanyData(\'', addParam: '\');' }},
            { name: 'PhoneNumber', index: 'PhoneNumber' },
            { name: 'Email', index: 'Email', formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:void(0);"', showAction: 'onclick="Company.GetCompanyEmail(\'', addParam: '\');' }},
            { name: 'Address1', index: 'Address1' },
            { name: 'Status', index: 'Status' }
        ], new Company(), 'Name')
        .setGridParam({
                gridComplete: function() {
                    var ids = $("#companyGrid").jqGrid('getDataIDs');
                    for (var i = 0; i < ids.length; i++) {
                        var id = ids[i];
                        var rowData = $("#companyGrid").jqGrid('getRowData', id);
                        var models = [{
                            colName: 'PhoneNumber',
                            cellValue: rowData.PhoneNumber,
                            limiter: 18
                        },{
                            colName: 'Email',
                            cellValue: rowData.Email,
                            limiter: 20
                        }];
                        Helpers.CellFormatter($("#companyGrid"), id, models);
                    }
                }
         });


        $('#UsaStates').change(function () {
            $('#companyGrid').setGridParam({
                url: 'companies/getdata?state=' + $(this).val()
            }).trigger("reloadGrid");
        });

        window.sendEmailUrl = "companies/sendemail";
});

Company.GetCompanyData = function (id) {
    return $.ajax({
        type: "GET",
        url: "companies/getcompany",
        data: id.replace('?', '') + "&edit=" + false,
        cache: false,
        success: function (data) {
            $("#companyInfo").html(data);
            ScrollToBottom();
        }
    });
};

Company.GetCompanyEmail = function(id) {
    $.ajax({
        type: "POST",
        url: "companies/getemaildialog",
        data: 'ids=' + id.replace('?id=', '') + '&company=true',
        cache: false,
        success: function (data) {
            Helpers.GetEmailDialog(data);
        }
    });
};