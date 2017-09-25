function RO() {
}

///////////////////////////////////////////////////////////////////////////////////////////////////
RO.ToPrint = function (grid, forFinalise) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    var url = "repairorders/print";
    if (forFinalise) {
        url = "../" + url;
    }
    if (ids.length != 0) {
            Helpers.Refresh($('#estimatesGrid'), function () {
                for (var i = 0; i < ids.length; i++) {
                    window.open(url + "?id=" + ids[i], "_blank");
                }
            });
    }
    else {
        Helpers.NoSelectedRowOperation();
    } 
};

///////////////////////////////////////////////////////////////////////////////////////////////////
RO.Email = function (grid, forFinalise) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow').join(',');
    var url = "repairorders/getemaildialog";
    if (forFinalise) {
        url = "../" + url;
    }

    if (ids.length != 0) {
        $.ajax({
            type: "POST",
            url: url,
            data: 'ids=' + ids,
            success: function (data) {
                if (data == "Error") {
                    jAlert("You can not send repair orders of different customers in the same message. \tPlease select repair orders related to the same customer",
                        "Warning!",
                        function () {
                            Helpers.Refresh(grid);
                        });
                }
                else {
                    Helpers.GetEmailDialog(data, grid);
                }
            }
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

///////////////////////////////////////////////////////////////////////////////////////////////////
RO.InitFilters = function (grid, url) {
    $('#Teams').change(function () {
        CookieManager.SetCookie("team", $(this).val());
        grid.setGridParam({
            url: url.call()
        }).trigger("reloadGrid");
    });

    $('#Customers').change(function () {
        CookieManager.SetCookie("customer", $(this).val());
        grid.setGridParam({
            url: url.call()
        }).trigger("reloadGrid");
    });
};

///////////////////////////////////////////////////////////////////////////////////////////////////
RO.DefineParticipation = function (grid, forFinalise) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');

    var url = $('.defineParticipation').val();// "repairorders/defineparticipation";
//    if (forFinalise) {
//        url = "../" + url;
//    }
    if (ids.length != 0) {
        if (ids.length == 1) {
            var status = grid.jqGrid('getCell', ids, 'RepairOrderStatus');
            if (status.toLowerCase() != 'Open'.toLowerCase()) {
                $.ajax({
                    type: "GET",
                    url: url,
                    cache: false,
                    data: 'ids=' + ids,
                    success: function (data) {
                        Helpers.GetDialogBase('350', 'auto', 'Define participation percents',
                                    [{ width: 150, text: "OK", click: function () {
                                        RO.DefineParticipationSave(grid, this, forFinalise);
                                    }
                                    },
                                        { width: 100, text: "Cancel", click: function () {
                                            grid.trigger("reloadGrid");
                                            $(this).dialog('close');
                                        }
                                        }
                                    ], function () {
                                        ValidateAjaxForm($('#assignTechniciansForm'));
                                        $('.ui-dialog-buttonset').css("margin-left", "45px");
                                    }, data);
                    }
                });
            }
            else {
                jAlert("You can define the percent of each employee's participation for repair orders with 'Completed', 'Approved' or 'Finalised' statuses only",
                    "Warning!",
                    function () {
                        Helpers.Refresh(grid);
                });
            }
        }
        else {
            jAlert("This action cannot be done for multiple repair orders", "Warning!", function () { Helpers.Refresh(grid); });
        }
    }
    else {
        jAlert("Please select one row for this operation", "Warning!", function () { Helpers.RemoveActiveButtons(); });
    }
};


RO.DefineParticipationSave = function (grid, dialog) {
    var id = $('#RepairOrderId').val();
    var val = [];
    var percents = [];
    var count = 0;
    $('.technicians input[type="hidden"]').each(function () {
        val.push(arguments[1].value);
    });
    $('input.percentInput').each(function () {
        count += 1;
        percents.push(arguments[1].value);
    });

    var url = $('.defineParticipation').val(); // "repairorders/defineparticipation";
    //    if (forFinalise) {
    //        url = "../" + url;
    //    }

    if (RO.CheckSumPercents(percents, count)) {
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: 'technicianIds=' + val + '&repairOrderId=' + id + '&technicianPercents=' + percents,
            success: function (data) {
                $(dialog).dialog('close');
                jAlert(data, "Alert", function () {
                    Helpers.Refresh(grid);
                });
            }
        });
    }
    else {
        jAlert("The percent sum must be equal to 100%", "Warning!");
    }
};

RO.CheckSumPercents = function (percents, count) {
    var sum = 0;
    var flag = 100 % count == 0;
    var manualFlag = false;

    var oneVal = (100 / count).toFixed(2);
    var min = oneVal * count;
    $(percents).each(function () {
        if (oneVal != parseFloat(this)) {
            manualFlag = true;
        }
        sum += parseFloat(this);
    });
    return flag ? sum == 100 : manualFlag ? sum == 100 : sum >= min && sum <= 100 ? true : false;
};


///////////////////////////////////////////////////////////////////////////////////////////////////
RO.GenerateInvoice = function (grid) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        Helpers.SendJsonModelBase(ids, 'repairorders/CheckCustomersType', function (data) {
            if (data == "True" || data == true) {
                var firstStatus = grid.jqGrid('getCell', ids[0], 'RoStatus');

                for (var i = 0; i < ids.length; i++) {
                    var status = grid.jqGrid('getCell', ids[i], 'RoStatus');
                    if (status != firstStatus) {
                        return jAlert("This action cannot be done for entries with different statuses", "Warning!", function () { Helpers.RemoveActiveButtons(); });
                    }
                }
                
                switch (firstStatus) {
                    case "Open":
                        jConfirm("Repair order(s) completed by employee?", "Complete repair order", function (c) {
                            if (c) {
                                Helpers.SendJsonModelBase(ids, 'repairorders/complete');
                                jConfirm("Repair order approved by customer?", "Approve repair order", function (a) {
                                    if (a) {
                                        Helpers.SendJsonModelBase(ids, 'repairorders/approvemany');
                                        RO.ToInvoiceBase(ids, grid);
                                    }
                                    else {
                                        Helpers.Refresh(grid);
                                    }
                                });
                            }
                            else {
                                Helpers.Refresh(grid);
                            }
                        });
                        break;
                    case "Completed":
                        jConfirm("Repair order approved by customer?", "Approve repair order", function (a) {
                            if (a) {
                                Helpers.SendJsonModelBase(ids, 'repairorders/approvemany');
                                RO.ToInvoiceBase(ids, grid);
                            }
                            else {
                                Helpers.Refresh(grid);
                            }
                        });
                        break;
                    case "Approved":
                        RO.ToInvoiceBase(ids, grid);
                        break;
                    }
            }
            else {
                var msg = "This action cannot be done for multiple wholesale customers.";
                return jAlert(msg, "Warning!", function () { Helpers.Refresh(grid); });
            }
        });
    }
    else {
        return jAlert('Please select row for this operation', 'Warning!', function () { Helpers.Refresh(grid); });
    }
};

RO.ToInvoiceBase = function (ids, grid) {
    var refresh = function () { Helpers.Refresh(grid); };
    jConfirm("Would you like to generate Invoice?", "Generate invoice", function (inv) {
        if (inv) {
            Helpers.SendJsonModelBase(ids, 'repairorders/generateinvoice', function (data) {
                if (data == "Error") {
                    var msg = "This action cannot be done for multiple wholesale customers.";
                    return  jAlert(msg, "Warning!", refresh);
                }
                else {
                   return jAlert("Operation completed", "", refresh);
                }
            });
        }
        else {
           return  refresh();
        }
    });
};

RO.CheckCustomers = function (ids) {
    var result = false;
    Helpers.SendJsonModelBase(ids, 'repairorders/CheckCustomersType', function (data) {
        result = data;
    });
    return result;
};

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
RO.MarkAsComplete = function (grid) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length > 0) {
        var url = "repairOrders/MarkAsCompleted";
        Helpers.SendJsonModelBase(ids, url, function (data) {
            
            jAlert(data, data != "Success" ? "Warning!":"", function () {
                Helpers.Refresh(grid);
            });
        });
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
RO.Approve = function (grid) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        var firstStatus = grid.jqGrid('getCell', ids[0], 'RepairOrderStatus');

        for (var i = 0; i < ids.length; i++) {
            var status = grid.jqGrid('getCell', ids[i], 'RepairOrderStatus');
            if (status != firstStatus) {
                return jAlert("This action cannot be done for entries with different statuses", "Warning!", function () { Helpers.RemoveActiveButtons(); });
            }
        }

        if (firstStatus == "Open") {
            jConfirm("Repair order(s) completed by employee?", "Complete repair order", function (c) {
                if (c) {
                    Helpers.SendJsonModelBase(ids, 'repairorders/complete');
                    jConfirm("Repair order approved by customer?", "Approve repair order", function (a) {
                        if (a) {
                            Helpers.SendJsonModelBase(ids, 'repairorders/approvemany');
                            $('#repairOrdersGrid').trigger('reloadGrid');
                        }
                        else {
                            Helpers.Refresh(grid);
                        }
                    });
                }
                else {
                    Helpers.Refresh(grid);
                }
            });
        }
        else if (firstStatus == "Completed") {
            jConfirm("Repair order approved by customer?", "Approve repair order", function (a) {
                if (a) {
                    Helpers.SendJsonModelBase(ids, 'repairorders/approvemany');
                }

                Helpers.Refresh(grid);
            });
        }
        else {
            return jAlert('The repair order(s) is already approved.', 'Warning!', function() { Helpers.RemoveActiveButtons(); });
        }
    }
    else {
        return jAlert('Please select row for this operation', 'Warning!', function () { Helpers.Refresh(grid); });
    }
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
RO.AssignMoreTechnicians = function (grid) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
        if (ids.length == 1) {
            var status = grid.jqGrid('getCell', ids, 'RepairOrderStatus');
            if (status.toLowerCase() == 'Open'.toLowerCase()) {
                $.ajax({
                    type: "GET",
                    url: "repairorders/assignmoretechnicians",
                    cache: false,
                    data: 'ids=' + ids,
                    success: function (data) {
                        Helpers.GetDialogBase('350', 'auto', 'Assign repair order',
                                    [{ width: 150, text: "OK", click: function () {
                                        var isOnlyRi = RO.IsOnlyRI();
                                        if (isOnlyRi) {
                                            jAlert("Can not be assigned to only RiTechnicians", "Warning!");
                                        } else {
                                            RO.AssignMoreTechniciansSave(this);
                                        }
                                    }
                                    },
                                        { width: 100, text: "Cancel", click: function () {
                                            $('#repairOrdersGrid').trigger("reloadGrid");
                                            $(this).dialog('close');
                                        }
                                        }
                                    ], function () {
                                        ValidateAjaxForm($('#assignTechniciansForm'));
                                        $('.ui-dialog-buttonset').css("margin-left", "48px");
                                    }, data);
                    }
                });
            }
            else {
                jAlert("You can assign employee for repair orders with 'Open' statuses only", "Warning!", function () {
                    Helpers.Refresh(grid);
                });
            }
        }
        else {
            jAlert("This action cannot be done for multiple repair orders.", "Warning!", function () { Helpers.RemoveActiveButtons(); });
        }
    }
    else {
        Helpers.NoSelectedRowOperation();
    }
};

var isNoSelection = function() {
    var riDiv = $('#RiPayment');
    if (riDiv.css('visibility') == 'visible') {
        var riOperation = $('.RiOperations');
        var flatFee = $('.FlatFee');
        if (!riOperation.prop("checked") && !flatFee.prop("checked")) {
            return true;
        }
    }
    return false;
};

RO.IsOnlyRI = function() {
    var result = false;
    var val = [];
    $('input[type="hidden"].technician').each(function () {
        val.push(arguments[1].value);
    });

    $('select.technician').each(function () {
        val.push(arguments[1].value);
    });

    var allCount = val.length;
    var riCount = 0;
    for (var i = 0; i < allCount; i++) {
        var isRi = val[i].split(':')[1] == 'RITechnician';
        if (isRi) {
            riCount++;
        }
    }
    result = (riCount == allCount);
    return result;
};

RO.AssignMoreTechniciansSave = function (dialog) {
    var id = $('#RepairOrderId').val();
    var val = [];
    var riVal = [];
    if (isNoSelection()) {
        jAlert("Please select R&I Payment Type", "Warning!");
        return;
    }

    var riDiv = $('#RiPayment');
    if (riDiv.css('visibility') == 'visible') {
        var riOperations = $('.RiOperations');
        var flatFee = $('.FlatFee');
        var tmp;
        if (riOperations.prop("checked")) {
            tmp = "RiOperations";
            riVal.push(tmp);
        }
        if (flatFee.prop("checked")) {
            var sum = $('#FieldFlatFee').val();
            tmp = "FlatFee:" + sum;
            riVal.push(tmp);
        }
    }


    $('input[type="hidden"].technician').each(function () {
        val.push(arguments[1].value);
    });
    
    $('select.technician').each(function () {
        val.push(arguments[1].value);
    });

    $.ajax({
        type: "POST",
        url: "repairorders/assignmoretechnicians",
        cache: false,
        data: 'technicianIds=' + val + '&repairOrderId=' + id + '&riOperations=' + riVal,
        success: function () {
            $(dialog).dialog('close');
            Helpers.Refresh($('#repairOrdersGrid'), function () {
                jAlert("Operation completed", "");
            });
        }
    });
};

/////////////////////////////////////////////////////////////////////////////
RO.RequestForEdit = function (grid) {
    var ids = grid.jqGrid('getGridParam', 'selarrrow');
    if (ids.length != 0) {
            var isAllowed = true;
            for(var i=0; i < ids.length; i++)
            {
                var tmp = grid.jqGrid('getCell', ids[i], 'RepairOrderStatus');
                if (!((tmp.toLowerCase() == 'Open'.toLowerCase()) || (tmp.toLowerCase() == 'Completed'.toLowerCase()))) {
                    isAllowed = false;
                    break;
                }
            }
            if (isAllowed) {
                jConfirm("You really want to get access to edit the documents?", "Access to edit repair order", function (a) {
                    if (a) {
                        $.ajax({
                            type: "GET",
                            url: "repairorders/requestforeditrepairorder",
                            cache: false,
                            data: 'ids=' + ids,
                            success: function (data) {
                                Helpers.Refresh(grid);
                            }
                        });
                        
                    } else {
                        Helpers.Refresh(grid);
                    }
                });
            }
            else {
                jAlert("You can to get access to edit for repair orders with 'Open' and 'Completed' statuses only", "Warning!", function () {
                    Helpers.Refresh(grid);
                });
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };


//////////////////////////////////////////////////

    RO.AllowRejectEdit = function (grid) {
        var ids = grid.jqGrid('getGridParam', 'selarrrow');
        var isAllowedEditing;
        if (ids.length != 0) {
            var isAllowed = true;
            for (var i = 0; i < ids.length; i++) {
                var tmp = grid.jqGrid('getCell', ids[i], 'RepairOrderStatus');
                if (!((tmp.toLowerCase() == 'Open'.toLowerCase()) || (tmp.toLowerCase() == 'Completed'.toLowerCase())
                    || (tmp.toLowerCase() == 'Editable'.toLowerCase()) || (tmp.toLowerCase() == 'EditPending'.toLowerCase()))) {
                    isAllowed = false;
                    break;
                }
            }
            if (isAllowed) {
                jCustomConfirm("You really want to set access to edit the documents?", "Access to edit repair order", function (a) {
                    if (a) {
                        isAllowedEditing = true;
                        $.ajax({
                            type: "GET",
                            url: "repairorders/allowrejectedit",
                            cache: false,
                            data: 'repairOrderIds=' + ids + '&isAllow=' + isAllowedEditing,
                            success: function (data) {
                                Helpers.Refresh(grid, function() {
                                    jAlert("Operation completed", "");
                                });
                            }
                        });
                    } else {
                        isAllowedEditing = false;
                        $.ajax({
                            type: "GET",
                            url: "repairorders/allowrejectedit",
                            cache: false,
                            data: 'repairOrderIds=' + ids + '&isAllow=' + isAllowedEditing,
                            success: function (data) {
                                Helpers.Refresh(grid, function () {
                                    jAlert("Operation completed", "");
                                });
                            }
                        });
    
                    }
                }, "Allow", "Reject");
            }
            else {
                jAlert("You can to set access to edit for repair orders with 'Open' and 'Completed' statuses only", "Warning!", function () {
                    Helpers.Refresh(grid);
                });
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };


    //////////////////////////////////////////////////////////
    RO.ToDiscard = function (grid) {
        var ids = grid.jqGrid('getGridParam', 'selarrrow');
        if (ids.length != 0) {
            var isManagerOrTechnician = $('#curentUserRole').val() == 'Manager' || $('#curentUserRole').val() == 'Technician';
            var isPaid = false;
            for (var i = 0; i < ids.length; i++) {
                var item = grid.jqGrid('getCell', ids[i], 'IsPaidInvoice');
                if (item == 'true') {
                    isPaid = true;
                    break;
                }
            };
            if (isPaid && isManagerOrTechnician) {
                jAlert("This action cannot be done for entries with Invoice \"Paid\" status ", "Warning!",
                function () {
                    Helpers.Refresh($('#finalisedGrid'));
                    return;
                });
            } else {
                jCustomConfirm('Would you like to discard selected repair order(s)?\t\nCorresponded invoice(s) will be also discarded.', null, function (r) {
                    if (r) {
                        Helpers.SendAjax("POST", "discard", "ids=" + ids, false, function (data) {
                            Helpers.Refresh($(grid), function () {
                                jAlert(data, null);
                            });
                        });
                    } else {
                        Helpers.RemoveActiveButtons();
                    }
                }, "Yes", "No");
            }
        }
        else {
            Helpers.NoSelectedRowOperation();
        }
    };

//////////////////////////////////////////////////////////////////////////////////
    RO.History = function (grid, url) {
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
                        Helpers.GetDialogBase('350', 'auto', 'Repair Order history',
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
