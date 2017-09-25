function onPagingFunction() {
    table.setGridParam({ datatype: 'json' }).trigger('reloadGrid');
};

var onSelectRowFunction = function(id) {
    if (id && id !== lastsel) {
        $("#" + settings.TableId).jqGrid('restoreRow', lastsel);
        $("#" + settings.TableId).jqGrid('editRow', id, true);
        lastsel = id;
    }
};

// создание from to фильтра для указаных колонок

// генерация post данных для отправки на сервер при фильтрации from to
var PostDataFunction = function () {
    var post = {};
    $.each(settings.DataSourceSettings.Columns, function (i) {
        if (settings.DataSourceSettings.Columns[i].FromToFilter) {

            post["" + settings.DataSourceSettings.Columns[i].Name + "From"] = function () {
                return $("#gs_" + settings.DataSourceSettings.Columns[i].Name + "2").val();
            };
            post["" + settings.DataSourceSettings.Columns[i].Name + "To"] = function () {
                return $("#gs_" + settings.DataSourceSettings.Columns[i].Name).val();
            };
        } else {
            post["" + settings.DataSourceSettings.Columns[i].Name] = function () {
                return $("#gs_" + settings.DataSourceSettings.Columns[i].Name).val();
            };
        }
    });
    return post;
};


// Stub
var onGridCompleteFunction = function () {
   
};

// присоединение дополнительного текстового поля к фильтру from to
function AppendFromToFilter() {
    $.each(settings.DataSourceSettings.Columns, function (i) {

        if (settings.DataSourceSettings.Columns[i].FromToFilter) {

            $("#gs_" + settings.DataSourceSettings.Columns[i].Name).css("width", "45%");
            $("#gs_" + settings.DataSourceSettings.Columns[i].Name)
                .parent()
                .prepend('<input type="text" value="" class = "additionalField" id="gs_' + settings.DataSourceSettings.Columns[i].Name +
                    '2" name = "' + settings.DataSourceSettings.Columns[i].Name + '" style="width: 45%; padding: 0px;">');
        }
    });
};