function GridInitializer() { }
window.pagersCount = 0;
GridInitializer.InitGrid = function (columnNames, columnModel, currentEntity, sortName, customButtons, customSaveFuntion, msg204, loadComplete, ascOrder, allowNavButton) {
    allowNavButton = typeof allowNavButton !== 'undefined' ? allowNavButton : true;
    var entity = new Entity(currentEntity);
    var pager = window.pagersCount == 0 ? '#pager' : '#pager' + window.pagersCount;
    var grid = entity.Grid.jqGrid({
        url: entity.GetDataUrl,
        datatype: "json",
        colNames: columnNames,
        colModel: columnModel,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: pager,
        sortname: sortName,
        sortorder: ascOrder ? "asc" : "desc",
        mtype: 'GET',
        autowidth: true,
        height: "auto",
        multiselect: true,
        pagerpos: "right",
        loadComplete: function(data) {
            if (data.customersFilter != null) {
                initCastomerFiltr(data.customersFilter);
            }
            if (loadComplete != null) {
                loadComplete(data);
            }
        },
        jsonReader: {
            repeatitems: false,
            root: function(obj) { return obj.rows; },
            page: function(obj) { return obj.page; },
            total: function(obj) { return obj.total; },
            records: function(obj) { return obj.records; },
            id: "Id"
        }

    });
    if (allowNavButton) {
        grid.navGrid('#pager', { edit: false, add: false, search: false, refresh: false, del: false });
    }
    if (typeof (customButtons) == "undefined" || customButtons == null) {
        grid.navButtonAdd('#pager', { caption: "Edit", onClickButton: function () { entity.GetCurrent(false, customSaveFuntion); }, position: "first", title: "", id: "editGridButton" })
            .navButtonAdd('#pager', { caption: "Suspend", onClickButton: function () { window.customSuspend == null ? entity.Suspend() : window.customSuspend(); }, title: "", id: "suspendGridButton" })
            .navButtonAdd('#pager', { caption: "Re-activate", onClickButton: function () { entity.Reactivate(); }, title: "", id: "reactivateGridButton" })
            .navButtonAdd('#pager', { caption: "Add new", onClickButton: function () { entity.GetCurrent(true, customSaveFuntion, msg204); }, title: "", id: "addGridButton" });
    }
    window.pagersCount++;

    var cm = grid[0].p.colModel;
    $.each(grid[0].grid.headers, function (index, value) {
        var cmi = cm[index], colName = cmi.name;
        if (!cmi.sortable && colName !== 'rn' && colName !== 'cb' && colName !== 'subgrid') {
            $('div.ui-jqgrid-sortable', value.el).css({ cursor: "default" });
        }
    });
    return grid;
};

function initCastomerFiltr(customers) {
    $('#Customers').empty();
    var selectedValue = null;
    for (var i = 0; i < customers.length; i++) {
        if (customers[i].Selected) {
            selectedValue = customers[i].Value;
        }
        $('#Customers')
         .append($("<option></option>")
         .attr("value", customers[i].Value)
         .text(customers[i].Text));
    }
    $('#Customers option:odd').addClass('greyFilter');
    $('#Customers').selectBox('value', selectedValue);
    $('#Customers').selectBox('refresh');
}

