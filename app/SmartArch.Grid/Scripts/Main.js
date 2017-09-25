// global settings

var lastsel;

window.cust = function (cellvalue, options, rowObject) {
    return "<button>NewButton</button>";
};

window.GenerateEditFormOnPage = function () {
    var element = $("#editForm");
    var ef = new EditForm(element, 4, "edit");
    ef.GenerateHtml();
    $('#editCustomFormCancelButton').button();
    $('#editCustomFormSaveButton').button();

};

window.CustomEditButtonForMultiplySelectFuction = function (cellvalue, options, rowObject) {
    var grid = $("#" + settings.TableId);
    var selRow = grid.jqGrid('getGridParam', 'selarrrow');
    if (selRow.length != 0) {
        if (selRow.length == 1) {
            grid.jqGrid('editGridRow', selRow, {});
        }

        else if (selRow.length > 1) {
            var popup = document.createElement("div");
            $(popup).css("text-align", "center");
            $(popup).html("You can choose one row for edit");
            $(popup).dialog({ height: 60, title: "Warning", resizable: false, position: ['center', 100], modal: true, zIndex: 950 });
        }
    }
    else {
        var popup = document.createElement("div");
        $(popup).css("text-align", "center");
        $(popup).html("Please select row");
        $(popup).dialog({ height: 60, title: "Warning", resizable: false, position: ['center', 100], modal: true });
    }
};

// modify formatters
$.each(settings.DataSourceSettings.ColumnModels, function (i) {
    var columnModel = settings.DataSourceSettings.ColumnModels[i];
    columnModel.formatter = columnModel.formatter == "customFormatter" ? window[columnModel.CustomFormatterFunctionName] : columnModel.formatter;
});
var table = $("#" + settings.TableId);
$(document).ready(function () {
    
    table.jqGrid({
        datatype: settings.DataType,
        url: settings.ActionsUrl.GetDataAction,
        createurl: settings.ActionsUrl.AddDataAction,
        deleteurl: settings.ActionsUrl.DeleteDataAction,
        editurl: settings.ActionsUrl.EditDataAction,
        mtype: "POST",
        jsonReader: {
            page: "page",
            total: "total",
            records: "records",
            root: "rows",
            repeatitems: false,
            id: 'Id'
        },

        multiselect: settings.MultiSelect,
        colNames: settings.DataSourceSettings.ColumnNames,
        colModel: settings.DataSourceSettings.ColumnModels,
        onSelectRow: window[settings.InLineEditingFunctionName],

        pager: settings.PagerId,
        width: settings.Width,
        viewrecords: true,

        rowNum: settings.RowInfo.RowNumber,
        rowList: settings.RowInfo.RowList,

        loadonce: settings.Loadonce,
        onPaging: onPagingFunction,
        sortorder: settings.Sortorder,
        sortName: settings.Sortname,
        //postData: window[settings.PostDataFunctionName].call(),
        gridComplete: window[settings.onGridCompleteFunction],

        footerrow: settings.FooterRow,
        userDataOnFooter: settings.FooterRow,
        altRows: settings.FooterRow,
        viewsortcols: [settings.ViewSortableColumns, 'vertical', settings.ViewSortableColumns],
        caption: '',
        height: "auto"
    });
    
    if (settings.Buttons != null) {
        table.jqGrid('navGrid', "#" + settings.PagerId, {
            edit: settings.Buttons.ShowEditButton,
            add: settings.Buttons.ShowAddButton,
            del: settings.Buttons.ShowDeleteButton,
            refresh: settings.Buttons.ShowRefreshButton,
            search: settings.Buttons.ShowSearchButton
            
        },
            { url: settings.ActionsUrl.EditDataAction },
            { url: settings.ActionsUrl.AddDataAction },
            { url: settings.ActionsUrl.DeleteDataAction },
            { multipleSearch: settings.Multiple.Search, multipleGroup: settings.Multiple.Group }
        );

        for (var i = 0; i < settings.Buttons.CustomButtons.length; i++) {
            var customButton = settings.Buttons.CustomButtons[i];
            customButton.onClickButton = customButton.onClickButton != null ? window[customButton.onClickButton] : null;
            table.jqGrid('navButtonAdd', "#" + settings.PagerId, customButton);
        }
    }

    //table.jqGrid(settings.HasFilterToolbarFunction, { autosearch: true, searchOnEnter: true });
    //AppendFromToFilter();


    // customization for from-to filter
    $(".ui-jqgrid-hbox input").unbind("keypress");
    $(".ui-jqgrid-hbox input").live("keypress", function (e) {
        if (e.keyCode == '13') {
            table.trigger("reloadGrid");
        }
    });
});