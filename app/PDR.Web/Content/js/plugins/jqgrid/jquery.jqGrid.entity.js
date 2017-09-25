function Helpers() { }
Helpers.SendAjax = function (type, url, data, cache, callback) {
    $.ajax({
        type: type,
        url: url,
        data: data,
        cache: cache,
        beforeSend: function () {
            $('#loader').show();
        },
        complete: function () {
            $('#loader').hide();
        },
        success: function (dat) {
            if (callback) {
                callback(dat != undefined ? dat : null);
            }
        }
    });
};

Helpers.NoSelectedRowOperation = function () {
    jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
};

Helpers.CellFormatter = function (grid, rowid, models) {
    for (var i = 0; i < models.length; i++) {
        grid.jqGrid(
            'setCell',
            rowid,
            models[i].colName,
            models[i].cellValue == undefined ? null : models[i].cellValue.length > models[i].limiter
                ? models[i].cellValue.substring(0, models[i].limiter) + '...'
                : models[i].cellValue, '',
            { 'title': models[i].cellValue }
                );
    }   
};

Helpers.Refresh = function (grid, callback) {
    Helpers.RemoveActiveButtons();
    if (grid) {
        grid.trigger("reloadGrid");
    }
    if (callback)
        callback();
};

Helpers.HighlightNew = function (grid) {
    var rows = grid.jqGrid('getRowData');
    for (var i = 0; i < rows.length; i++) {
        var newRow = rows[i].New;
        var editStatus = rows[i].EditableStatus;
        if (newRow == "true" || newRow == true || editStatus == 'Editable' || editStatus == 'EditPending') {
            var id = $(rows[i].Id).html() || rows[i].Id;
            grid.jqGrid('setRowData', id, true, 'important-row');
        }
    }
};

Helpers.SendAjaxForm = function (editFormSelector, url, grid) {
    $(editFormSelector).ajaxSubmit({
        url: url,
        success: function (data) {
            if (data == null || data == "" || data == "success") {
                $(window.container).dialog('close');
                grid.trigger("reloadGrid");
            }
            else if (data == 'not found') {
                $(window.container).dialog('close');
                jAlert("Operation can not be done. Build's file not found on server-side", "Warning!", function () {
                    grid.trigger("reloadGrid");
                });
            }
            else {
                $(editFormSelector).replaceWith(data);
                $('select').selectBox();
                ValidateAjaxForm($(editFormSelector));
                QtipInitializer.Init();

                if (navigator.userAgent.toLowerCase().match('chrome') != null) {
                    $('#companyForm select').next().css('width', '50px');
                }
            }
        }
    });
};

Helpers.ShowSearchData = function(grid, searchField, searchValue, filters) {
    if (searchValue.length == 0) {
        grid.setGridParam({ search: false });
        return grid.trigger("reloadGrid", [{ page: 1 }]);
    }
    var postdata = grid.jqGrid('getGridParam', 'postData');
    $.extend(postdata,
               { 
                   filters: filters,
            searchField: searchField,
            searchOper: 'eq',
            searchString: searchValue
        });
    grid.jqGrid('setGridParam', { search: true, postData: postdata });
    grid.trigger("reloadGrid", [{ page: 1 }]);
};

Helpers.RemoveActiveButtons = function () {
    $('td[class*="button"]').each(function () {
        $(this).children(':first').removeAttr('id');
        $(this).children(':first').css('color', '#444447');
    });
};

$(function () {
    $(document).bind('onAfterSetup', function () {
        $('td[class*="button"]').bind('click', function () {
            if ($(this).children().html().match('Print') == null) {
                $(this).children(':first').attr('id', 'active-grid-button');
            }
            else {
                $('table[id*="Grid"]').trigger('reloadGrid');
            }
        });
    });
});

Helpers.SendJsonModelBase = function (model, url, successFunction) {
    $.ajax({
        url: url,
        cache: false,
        type: "POST",
        data: JSON.stringify(model),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (successFunction) {
                successFunction(data);
            }
        }
    });
};

Helpers.SendJsonModel = function (editFormSelector, url, grid, jsonModel) {
    Helpers.SendJsonModelBase(jsonModel, url, function (data) {
            $(window.container).dialog('close');
            grid.trigger("reloadGrid");
            $('form[id*="Form"] select').selectBox('destroy');
    });
};

Helpers.ChangeStatus = function (grid, confirmMessage, url, customFunction) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');

    if (ids.length != 0) {
        jConfirm(confirmMessage, null, function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "ids=" + ids.join(','),
                    success: function (mes) {
                        Helpers.Refresh(grid);
                        if (customFunction != null) {
                            customFunction(mes);
                        }
                    }
                });
            }
            else {
                Helpers.Refresh(grid);
            }
        });
    }
    else {
        jAlert("Please select row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};

Helpers.GetDialogBase = function (width, height, title, buttons, openFunction, data) {
    var container = document.createElement('div');
    $(container).html(data).dialog({
        width: width,
        height: height,
        modal: true,
        open: function () {
            if (openFunction != null) {
                openFunction();
            }
            window.container = container;

            Helpers.SetDialogFocus();
            Helpers.SetHover();

            $(container).find('select').not('.multiselect').not('.combobox').selectBox();
            $('select.multiselect').length > 0 ? $('select.multiselect').multiselect({ dividerLocation: 0.5 }) : void (0);

            if (window.navigator.userAgent.toLowerCase().match('chrome') != null) {
                $('#companyForm select').next().css('width', '48px');
            }
            if (window.navigator.userAgent.toLowerCase().match('firefox') != null) {
                $('#companyForm select').next().css('width', '50px');
            }
            var isiPad = navigator.userAgent.match(/iPad/i) != null;

            if (isiPad) {
                var selects = $(container).find('select').not('.multiselect').not('.combobox');
                $.each(selects, function () {
                    $(this).wrap('<div class="wrapperBlock"></div>');
                });
                selects.after('<span class="selectBox-arrow"></span>');

            } else {
                $(container).find('select').not('.multiselect').not('.combobox').selectBox();
            }
            $('select.multiselect').length > 0 ? $('select.multiselect').multiselect({ dividerLocation: 0.5 }) : void (0);
            window.afterDialogSetup != null ? window.afterDialogSetup() : void (0);
        },
        close: function () {
            $(container).remove();
            Helpers.RemoveActiveButtons();
        },
        beforeClose: function () {
            $(container).find('select').selectBox('destroy');
        },
        resizable: false,
        title: title,
        buttons: buttons,
        position: { my: "center", at: "center", of: window }
    });
};

Helpers.GetDialog = function (entityName, create, data, saveFunction) {
    Helpers.GetDialogBase('auto', 'auto', create ? 'Add new ' + entityName : 'Edit ' + entityName, [
        { width: 229, text: "Save", click: function () {
            saveFunction.call();
        }
        },
        { width: 82, text: "Cancel", click: function () {
            $(this).dialog('close');
        }
        }
    ], function () {
        ValidateAjaxForm($(self.EditFormSelector));
       }, data);
};

Helpers.SetDialogFocus = function () {
    $('button').bind('focus', function () {
        $(this).attr('id', 'active-form-button');
    });
    $('button').bind('focusout', function () {
        $(this).removeAttr('id');
    });
    $('button:first').focus();
};

Helpers.SetHover = function () {
    $('.ui-dialog-buttonset button span.ui-button-text').bind('mouseover', function () {
        $(this).addClass('ui-dialog-button');
    });
    $('.ui-dialog-buttonset button span.ui-button-text').bind('mouseout', function () {
        $(this).removeClass('ui-dialog-button');
    });
};

Helpers.GetEmailDialog = function (data, grid) {
    Helpers.GetDialogBase('auto', 'auto', 'Send e-mail',
     [{ width: 229, text: "Send", click: function () {
         Helpers.SendEmail(container, grid);
        }
     },
     { width: 82, text: "Cancel", click: function () {
         $(this).dialog('close');
         Helpers.Refresh(grid);
        }
     }], function () {
         ValidateAjaxForm($('#sendEmailForm'));
         $('.ui-dialog-buttonset').css("margin-left", "85px");
     }, data);

};

Helpers.GetpPushNotificationDialog = function (data, grid) {
    Helpers.GetDialogBase('auto', 'auto', 'Send push notification',
     [{ width: 229, text: "Send", click: function () {
         Helpers.SendPushNotification(container, grid);
     }
     },
     { width: 82, text: "Cancel", click: function () {
         $(this).dialog('close');
         Helpers.Refresh(grid);
     }
     }], function () {
         ValidateAjaxForm($('#sendPushNotificationForm'));
         $('.ui-dialog-buttonset').css("margin-left", "85px");
     }, data);

};

Helpers.UpdateReportGrid = function (gridEl, startDateEl, endDateEl, customersEl, updateUrl, teamEl) {
    updateUrl = updateUrl.split('?')[0];
    var start = startDateEl.val();
    var end = endDateEl.val();
    var customer = customersEl.val();
    var team = null;
    if (teamEl != null) {
        team = teamEl.val();
    }

    var url = updateUrl + '?' + 'customer=' + customer + "&startDate=" + start + "&endDate=" + end + "&team=" + team;

    gridEl.setGridParam({
        url: url
    }).trigger("reloadGrid");
};

Helpers.UpdateInvoiceGrid = function (gridEl, startDateEl, endDateEl, customersEl, teamEl, statusEl, updateUrl, param) {
    var start = "&startDate=" + startDateEl.val();
    var end = "&endDate=" + endDateEl.val();
    var customer = '&customer=' + customersEl.val();
    var team = teamEl != null ? "&team=" + teamEl.val() : '';
    var status = statusEl != null ? "&status=" + statusEl.val() : '';
    if (param == undefined) {
        param = "";
    }
    var url = updateUrl + param + customer + start + end + team + status;

    gridEl.setGridParam({
        url: url
    }).trigger("reloadGrid");
};

Helpers.InitDateRangeTextboxes = function (startDateTextboxSelector, endDateTextboxSelector, afterSelectFunction) {
    var dates = $("#" + startDateTextboxSelector + ",#" + endDateTextboxSelector).datepicker({
            numberOfMonths: 1,
            onSelect: function(selectedDate) {
                var option = this.id == startDateTextboxSelector ? "minDate" : "maxDate",
                    instance = $(this).data("datepicker"),
                    date = $.datepicker.parseDate(
                        instance.settings.dateFormat ||
                            $.datepicker._defaults.dateFormat,
                        selectedDate, instance.settings);
                dates.not(this).datepicker("option", option, date);
                    afterSelectFunction.call();
                
            }
        }).attr('readonly', 'readonly');

};

Helpers.SendEmail = function (container, grid) {
    var to = $('#To').val();
    var to2 = $('#To2').val() == undefined ? '' : $('#To2').val();
    var to3 = $('#To3').val() == undefined ? '' : $('#To3').val();
    var to4 = $('#To4').val() == undefined ? '' : $('#To4').val();
    var subject = $('#Subject').val();
    var message = $('#Message').val();
    var ids = $('#ids').val();
    var form = $('#sendEmailForm');
    var isbasic = $('#IsBasic_true').is(':checked') || ($("#WholesalerRole").length != 0);
    if (form.valid()) {
        $.ajax({
            type: "POST",
            cache: false,
            url: window.sendEmailUrl != null ? window.sendEmailUrl : $('#urlEmail').val(),
            data: "to=" + to + "&to2=" + to2 + "&to3=" + to3 + "&to4=" + to4 + "&subject=" + subject + "&message=" + message + "&ids=" + ids + "&isbasic=" + isbasic,
            beforeSend: function () {
                $('#loader').show();
            },
            complete: function () {
                $('#loader').hide();
            },
            success: function (data) {
                if (data.length == 0 || data == true) {
                    $(container).dialog('close');
                    jAlert("Operation completed", "", function () {
                        Helpers.Refresh(grid);
                    });
                }
                else if (data.length > 0 || data == false) {
                    jAlert(data, "Warning!", null);
                }
            }
        });
    }
};

Helpers.SendPushNotification = function (container, grid) {
    var message = $('#sendPushNotificationArea').val();
    var form = $('#sendEmailForm');

    if (form.valid()) {
        $.ajax({
            type: "POST",
            cache: false,
            url: "manageDownload/sendPushNotification",
            data: "message=" + message,
            beforeSend: function () {
                $('#loader').show();
            },
            complete: function () {
                $('#loader').hide();
            },
            success: function (data) {
                if (data.length == 0 || data == true) {
                    $(container).dialog('close');
                    jAlert("Push notification has been sucessfully sent.", "Alert", function () {
                        Helpers.Refresh(grid);
                    });
                }
                else if (data.length > 0 || data == false) {
                    jAlert(data, "Warning!", null);
                }
            }
        });
    }
};

function Entity(entity) {
    this.Grid = entity.grid;
    this.EditContainer = entity.editcontainer;
    this.GetCurrentUrl = entity.getcurrenturl;
    this.GetDataUrl = entity.getdataurl;
    this.EditUrl = entity.editurl;
    this.CreateUrl = entity.createurl;
    this.SuspendUrl = entity.suspendurl;
    this.ReactivateUrl = entity.reactivateurl;
    this.EditFormSelector = entity.editform;
    this.Name = entity.name;
    this.ResetPassword = entity.resetpassword;
}

Entity.prototype.GetCurrent = function (create, customSaveFunction, msg204) {
    var self = this;
    var id = !create ? self.Grid.jqGrid('getGridParam', 'selarrrow') : null;

    if (id != null) {
        if (id.length == 0 && create == false) {
            return Helpers.NoSelectedRowOperation();
        }

        else if (id.length > 1 && create == false) {
            return jAlert("Select one row, please", "Warning!", function () { Helpers.RemoveActiveButtons(); });
        }

        var isAdmin = self.Grid.jqGrid('getCell', id, 'Role') == "Admin";
        if (window.IsAccountant && isAdmin) {
            return jAlert("Only admins can edit admin.", "Warning!", function () { Helpers.RemoveActiveButtons(); });
        }
    }
    return $.ajax({
        type: "GET",
        url: this.GetCurrentUrl,
        cache: false,
        data: "id=" + id + "&edit=" + true,
        statusCode: {
            200: function (data) {
                if (customSaveFunction == undefined) {
                    Helpers.GetDialog(self.Name, create, data, function () {
                        var form = $(self.EditFormSelector);
                        if (form.valid()) {
                            if (create) {
                                self.Create();
                            }
                            else {
                                self.Edit();
                            }
                        }
                    });
                } else {
                    window.Helpers.GetDialog(self.Name, create, data, customSaveFunction);
                }
            },
            204: function () {
                jAlert(msg204, "Alert", function () { Helpers.RemoveActiveButtons(); });
            }
        }
    });
};

Entity.prototype.Edit = function () {
    return Helpers.SendAjaxForm(this.EditFormSelector, this.EditUrl, this.Grid);
};

Entity.prototype.Create = function () {
    return Helpers.SendAjaxForm(this.EditFormSelector, this.CreateUrl, this.Grid);
};

Entity.prototype.Suspend = function () {
    Helpers.ChangeStatus(this.Grid, 'Suspend?', this.SuspendUrl,function () {
        jAlert("Operation completed", "", function () { Helpers.RemoveActiveButtons(); });
    });
};

Entity.prototype.Reactivate = function () {
    Helpers.ChangeStatus(this.Grid, 'Reactivate?', this.ReactivateUrl, function (data) {
        if (data != "") {
            jAlert(data, "Warning!", function () { Helpers.Refresh(this.Grid); });
        }
        else {
            jAlert("Operation completed", "", function () { Helpers.RemoveActiveButtons(); });
        }
    });
};

Entity.prototype.ResetPassword = function () {
    Helpers.ResetPassword();
};

