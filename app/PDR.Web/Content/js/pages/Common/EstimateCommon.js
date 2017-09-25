
function EstimateCommon() {
}


//////////////////////////////////////////////////////////////////////////////////
EstimateCommon.History = function (grid, url) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        if (ids.length > 1) {
            jAlert('Select a single document to view history', 'Warning!', function () {
                Helpers.Refresh(grid, null);
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                data: 'id=' + ids[0],
                success: function (data) {
                    Helpers.GetDialogBase('350', 'auto', 'Estimate history',
                                    [{ width: 100, text: "Close", click: function () {
                                        Helpers.Refresh($(grid), null);
                                        $(this).dialog('close');
                                    }
                                    }], null, data);
                }
            });
        }
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};


///////////////////////////////////////////////////////////////////////////
EstimateCommon.FilterValueToParam = function(teams, customers) {
    EstimateCommon.Changelinks(teams, customers);
    $(teams, customers).change(function() {
        EstimateCommon.Changelinks(teams, customers);
    });

};

EstimateCommon.Changelinks = function(teams, customers) {
    var links = $('.linkWithFilter');
    var param = '?cust=' + $(customers).val() + '&team=' + $(teams).val();
    for (var i = 0; i < links.length; i++) {
        links[i].href = links[i].href.split('?')[0];
        links[i].href += param;
    }
};