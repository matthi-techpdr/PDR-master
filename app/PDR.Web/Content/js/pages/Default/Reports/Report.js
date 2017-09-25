//@reference ~/Content/js/plugins/jqueryui/jquery.ui.js
//@reference ~/Content/js/plugins/jqueryui/jquery.ui.widget.js
//@reference ~/Content/js/plugins/jquery.multi-accordion.js

function Report() {
}

$(document).bind('onAfterSetup', function () {
    Report.BaseUrl = window.location.pathname.split("/").last() + "/";
    $('#accordione').multiAccordion({ active: '0', autoHeight: false });
});

Report.Save = function () {
    var title = $('#ReportTitle').val().trim();
    var start = $('#from').val();
    var end = $('#to').val();
    var customer = $('#Customers').val();
    var team = $('#Teams').val();
    var commission = $('#com').is(':checked');

    if (title.length > 0) {
        var existNames = $('#ReportTitle').data('existNames');
        if (existNames.contains(title)) {
            var errorMsg = 'You have report with the same name. Please, specify another name.';
            return jAlert(errorMsg, 'Warning!', function () { Helpers.RemoveActiveButtons(); });
        }

        var model = {
            "Title": title,
            "StartDate": start,
            "EndDate": end,
            "CustomerId": customer,
            "TeamId": team,
            "Commission": commission
        };

        var url = Report.BaseUrl + "save";
        return Helpers.SendJsonModelBase(model, url, function () {
            var loc = window.location;
            loc.reload();
        });

    }
    else {
        return jAlert('Specify report title', 'Warning!', function () { Helpers.RemoveActiveButtons(); });
    }
};

Report.Delete = function(reportItem) {
    var id = reportItem.attr('id');
    jConfirm("Are you sure?", "Delete report", function(r) {
        if (r) {
            $.ajax({
                    url: Report.BaseUrl + "delete",
                    data: 'id=' + id,
                    cache: false,
                    success: function() {
                        window.location.reload();
                    }
                });
        }
    });
};

Report.SaveToPdf = function (grid) {
    var from = $("#from").val() == undefined ? '' : $("#from").val();
    var to = $('#to').val() == undefined ? '' : $('#to').val();
    var customer = $('#Customers').val() == undefined ? '' : $('#Customers').val();
    var team = $('#Teams').val() == undefined ? $('#CurrentTeams').val() == undefined ? '' : $('#CurrentTeams').val() : $('#Teams').val();
    var commission = $('#com').is(':checked');
    var id = $('#Id').val() || "";

    var url = $('.urlReport').val();

    Helpers.RemoveActiveButtons();
    grid.trigger('reloadGrid');
    if (url != null) {
        window.open(url + '?from=' + from + "&to=" + to + "&customer=" + customer + "&team=" + team + "&commission=" + commission + "&id=" + id, "_blank");
    }
    else {
        window.open(Report.BaseUrl + "savetopdf?from=" + from + "&to=" + to + "&customer=" + customer + "&team=" + team + "&commission=" + commission + "&id=" + id, "_blank");
    }
};

Report.Print = function (grid) {
    var ids = grid.jqGrid('getDataIDs');
    
    if (ids.length != 0) {
        for (var i = 0; i < ids.length; i++) {
            $.ajax({
                type: "GET",
                url:  Report.BaseUrl + "print",
                data: "ids=" + ids[i],
                success: function (data) {
                    window.open(data, '_blank');
                }
            });
        }
    }
    else {
        jAlert("No rows", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};


Report.InitControls = function (grid, getDataUrl) {

    $('#savedReports p').mouseover(function () {
        $(this).find('img').css('display', 'inline');
    });

    $('#savedReports p').mouseout(function () {
        $(this).find('img').css('display', 'none');
    });

    $('.deleteReport').click(function () {
        var reportItem = $(this);
        Report.Delete(reportItem);
    });

    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());

        Helpers.UpdateReportGrid(grid, $('#from'), $('#to'), $('#Customers'), getDataUrl, $('#Teams'));
    });

    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());

        Helpers.UpdateReportGrid(grid, $('#from'), $('#to'), $('#Customers'), getDataUrl, $('#Teams'));
    });

    Helpers.InitDateRangeTextboxes('from', 'to', function () { Helpers.UpdateReportGrid(grid, $('#from'), $('#to'), $('#Customers'), getDataUrl); });
}