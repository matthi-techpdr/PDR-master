////@reference ~/Content/js/knockout.js
////@reference ~/Content/js/knockout.settings.js
////@reference ~/Content/js/plugins/jqueryui/jquery.ui.js
////@reference ~/Content/js/plugins/jqueryui/jquery.ui.widget.js
////@reference ~/Content/js/plugins/jquery.multi-accordion.js
////@reference ~/Content/js/plugins/jquery.textchange.min.js
////@reference ~/Content/js/plugins/jquery.selectBox.js

///*global window.globals.customerType = "@Model.Customer.CustomerType";*/

//var viewModel;

//var elem;
//var url = '';
//var sections;
//var inspection;
//var retailCustomer;
//var flagAlert = true;
//var $year;
//var $make;
//var $model;
//var $loader;
//var $wholesaleId;
//var $wholesalaMatrixId;
//var $selectBox;
//var $insurance;
//var $fieldset;
//var $insuranceInputs;
//var $insuranceCompanyNames;
//var $retailFirstName;
//var $retailLastName;

//$().ready(function () {

//    $("#accordion").multiAccordion({ active: 'none' });
//    window.globals.partsTemp = new PartsTemp();
//    $year = $('.estimate-car-info-year');
//    $make = $('.estimate-car-info-make');
//    $model = $('.estimate-car-info-model');
//    $loader = $('#loader');
//    $wholesaleId = $('#Customer_Wholesale_CustomerId');
//    $wholesalaMatrixId = $('#Customer_Wholesale_MatrixId');
//    $selectBox = $('select.select-wholesale');
//    $insurance = $('#insurance');
//    $fieldset = $('.fieldset11');
//    $insuranceInputs = $insurance.find('input[type="text"]');
//    $insuranceCompanyNames = $('#Insurance_CompanyNames');
//    $retailFirstName = window.globals.order == "True" ? $('#EstimateModel_Customer_Retail_FirstName') : $('#Customer_Retail_FirstName');
//    $retailLastName = window.globals.order == "True" ? $('#EstimateModel_Customer_Retail_LastName') : $('#Customer_Retail_LastName');


//    inspection = new CarInspectionsViewModel();

//    retailCustomer = new CustomerRetailInfo();
//    var effortsModel = new EffortsModel();

//    retailCustomer.firstName($retailFirstName.val());
//    retailCustomer.lastName($retailLastName.val());

//    viewModel = new ViewModel();

//    viewModel.carInspection(inspection);
//    viewModel.customerRetailInfo(retailCustomer);

//    var type = $(this).attr('value');
//    window.globals.type = type;

//    ko.applyBindings(viewModel);

//    var model = window.globals.inspection;

//    inspection.priorDamages(model['PriorDamages']);
//    inspection.readonly(window.globals.isReadOnlyMode);


//    for (var i = 0, len = model.AverageSize.length; i < len; i++) {
//        inspection.averageSize.push(new AverageSize(model.AverageSize[i]));
//    }
//    for (var j = 0, lenDents = model.TotalDents.length; j < lenDents; j++) {
//        inspection.totalDents.push(new TotalDents(model.TotalDents[j]));
//    }
//    //    $.each(model['AverageSize'], function () {
//    //            var item = arguments[1];
//    //            inspection.averageSize.push(new AverageSize(item));
//    //        });

//    //        $.each(model['TotalDents'], function () {
//    //            var item = arguments[1];
//    //            inspection.totalDents.push(new TotalDents(item));
//    //        });

//    inspection.initEstimateCustomLines(model['EstimateCustomLines']);

//    // console.log("start carInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());

//    inspection.initParts(model['PartsNames'], model['FullPartsNames'], effortsModel.effortsArray);

//    //console.log("start carInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());



//    //    $.each(model["CarInspections"], function () {
//    //            var index = arguments[0];
//    //        var dataName = model['PartsNames'][index];
//    //        var fullName = model['FullPartsNames'][index];
//    //        var effortLine = line[index];
//    //        var part = new Part(dataName, fullName, index, effortLine);

//    //        inspection.parts.push(part);
//    //        });

//    setupVin();

//    setupEffort();

//    setupCustomerTypeChange();

//    inspection.discount(window.globals.discount);
//    inspection.laborRate(window.globals.laborRate);
//    inspection.maxCorProtect(window.globals.defaultMaxCorProtect);

//    if (window.globals.customerType == 'Retail') {
//        InitDefaultData();
//        UpdateCarInspectionsData();
//        viewModel.customerInfo(retailCustomer.fullName());
//    }
//    else {
//        InitWholesaleCurrentData();
//        UpdateCarInspectionsData();
//        WholesaleCustomerDataBindChange();
//        $wholesaleId.selectBox('value', window.globals.currentWholesaleCustomer);
//        $wholesalaMatrixId.selectBox('value', window.globals.currentMatrix);
//        viewModel.customerInfo(' - ' + $('#Customer_Wholesale_CustomerId :selected').text());
//        $('#Customer_Retail_State').selectBox('disable');
//        $('#retail-customer input[type="text"]').attr('disabled', 'disabled');
//        if (window.globals.hasInsurance == "False") {
//            InsuranceHide();
//        }
//    }

//    //console.log("start CarInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    for (var f = 0, lenInspect = model.CarInspections.length; f < lenInspect; f++) {
//        inspection.parts()[f].selectedAverageSize(model.CarInspections[f].AverageSize);
//        inspection.parts()[f].selectedTotalDents(model.CarInspections[f].TotalDents);

//        inspection.parts()[f].dentsCost(model.CarInspections[f].DentsCost);
//    }
//    //    $.each(model["CarInspections"], function () {
//    //            var index = arguments[0];
//    //            var inspect = inspection.parts()[index];

//    //            inspect.selectedAverageSize(this.AverageSize);
//    //            inspect.selectedTotalDents(this.TotalDents);

//    //            inspect.dentsCost(this.DentsCost);
//    //        });
//    //console.log("end CarInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());

//    if (window.globals.order == 'True') {
//        WholesaleDisable();
//    }
//    else {
//        if (window.globals.state == "view") {

//            WholesaleDisable();
//        }
//        else {
//            WholesaleEnable();
//        }
//    }

//    setupEmail();
//    setupPrint();
//    // console.log("start validation - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    $.validator.unobtrusive.parse(document);
//    $("form").validate();
//    // console.log("end validation - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//});

//function InitDefaultData() {
//    //console.log("start initdefault - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    $('#matrixId').val(window.globals.defaultMatrix);
//    window.globals.partsTemp.remove();
//    var len = 15;
//    while (len >= 0) {
//        var part = inspection.parts()[len];
//        part.hourlyRate(window.globals.defaultHourlyRate);
//        part.aluminiumPercent(window.globals.defaultAluminium);
//        part.maxCorProtect(window.globals.defaultMaxCorProtect);
//        part.doubleMetallPercent(window.globals.defaultDoubleMetall);
//        part.oversizedRoofPercent(window.globals.defaultOversizedRoof);
//        part.oversizedDents(window.globals.defaultOversizedDents);
//        part.corProtectPart(window.globals.defaultCorProtect);
//        part.maxPercentPart(window.globals.defaultMaxPercent);
//        Temp(part);
//        len--;
//    }

//    //    for (var i = 0, len = 16; i < len; i++) {
//    //        var part = inspection.parts()[i];
//    //        part.hourlyRate(window.globals.defaultHourlyRate);
//    //        part.aluminiumPercent(window.globals.defaultAluminium);
//    //        part.maxCorProtect(window.globals.defaultMaxCorProtect);
//    //        part.doubleMetallPercent(window.globals.defaultDoubleMetall);
//    //        part.oversizedRoofPercent(window.globals.defaultOversizedRoof);
//    //        part.oversizedDents(window.globals.defaultOversizedDents);
//    //        part.corProtectPart(window.globals.defaultCorProtect);
//    //        part.maxPercentPart(window.globals.defaultMaxPercent);
//    //        Temp(part);
//    //    }
//    //    $.each(inspection.parts(), function () {
//    //            var part = this;
//    //            part.hourlyRate(window.globals.defaultHourlyRate);
//    //            part.aluminiumPercent(window.globals.defaultAluminium);
//    //            part.maxCorProtect(window.globals.defaultMaxCorProtect);
//    //            part.doubleMetallPercent(window.globals.defaultDoubleMetall);
//    //            part.oversizedRoofPercent(window.globals.defaultOversizedRoof);
//    //            part.oversizedDents(window.globals.defaultOversizedDents);
//    //            part.corProtectPart(window.globals.defaultCorProtect);
//    //            part.maxPercentPart(window.globals.defaultMaxPercent);
//    //            Temp(part);
//    //        });
//    //console.log("end initdefault - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//}

//function InitWholesaleCurrentData() {
//    //console.log("start initwholesale - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    $('#matrixId').val(window.globals.currentMatrix);
//    window.globals.partsTemp.remove();
//    var len = 15;
//    while (len >= 0) {
//        var part = inspection.parts()[len];
//        part.hourlyRate(parseFloat(window.globals.hourlyRate));
//        part.aluminiumPercent(parseFloat(window.globals.currentAluminium));
//        part.maxCorProtect(parseFloat(window.globals.maxCorProtect));
//        part.doubleMetallPercent(parseFloat(window.globals.currentDoubleMetall));
//        part.oversizedRoofPercent(parseFloat(window.globals.currentOversizedRoof));
//        part.oversizedDents(parseFloat(window.globals.currentOversizedDents));
//        part.corProtectPart(parseFloat(window.globals.currentCorProtect));
//        part.maxPercentPart(parseFloat(window.globals.currentMaxPercent));
//        Temp(part);
//        len--;
//    }
//    //    $.each(inspection.parts(), function () {
//    //            var part = this;
//    //            part.hourlyRate(parseFloat(window.globals.hourlyRate));
//    //            part.aluminiumPercent(parseFloat(window.globals.currentAluminium));
//    //            part.maxCorProtect(parseFloat(window.globals.maxCorProtect));
//    //            part.doubleMetallPercent(parseFloat(window.globals.currentDoubleMetall));
//    //            part.oversizedRoofPercent(parseFloat(window.globals.currentOversizedRoof));
//    //            part.oversizedDents(parseFloat(window.globals.currentOversizedDents));
//    //            part.corProtectPart(parseFloat(window.globals.currentCorProtect));
//    //            part.maxPercentPart(parseFloat(window.globals.currentMaxPercent));
//    //            Temp(part);

//    //        });
//    //console.log("end initwholesale - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//}

//function SetChosenEffortItems() {

//    var model = window.globals.inspection;
//    for (var i = 0, len = 15; i < len; i++) {
//        var inspect = inspection.parts()[i];
//        var itemInspect = model.CarInspections[i];
//        inspect.idSection(itemInspect.Id);
//        if (itemInspect.EffortItems.length > 0) {
//            var effortItems1 = itemInspect.EffortItems;
//            var effortItems = inspect.efforts().effortItems();
//            //console.log("start setchosenefforts - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//            for (var x = 0, len1 = effortItems1.length; x < len1; x++) {
//                effortItems[x].operations(effortItems1[x].Operations);
//                effortItems[x].selectedEffortItem(effortItems1[x].Choosed);
//                effortItems[x].hoursInput(effortItems1[x].Hours);
//            }
//            //console.log("end setchosenefforts - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//        }
//        inspect.oversizedDentsId(itemInspect.OversizedDentsId);
//        inspect.amountOversizedDents(itemInspect.AmountOversizedDents);
//        inspect.valuePriorDamage(itemInspect.PriorDamage);

//        inspect.Aluminium(itemInspect.Aluminium);
//        inspect.DoubleMetal(itemInspect.DoubleMetal);
//        inspect.OversizedRoof(itemInspect.OversizedRoof);
//        inspect.CorrosionProtection(itemInspect.CorrosionProtection);

//        if (itemInspect.CustomCarInspectionLines.length > 0) {
//            inspect.customLines.removeAll();
//            //console.log("start setCarcustomlines - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//            $.each(itemInspect.CustomCarInspectionLines, function () {
//                inspect.customLines.push(new CustomCarInspectionLine(this, inspect.nameCustomLines, arguments[0]));
//            });
//            //console.log("start setCarcustomlines - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//        }
//        else {
//            inspect.customLines.removeAll();
//            inspect.customLines.push(new CustomCarInspectionLine({ Cost: 0, Name: "" }, inspect.nameCustomLines, 0));
//        }

//        inspect.OptionsPersent(itemInspect.OptionsPersent);
//        inspect.CorrosionProtectionCost(itemInspect.CorrosionProtectionCost);
//        inspect.oversizedDentsCost(itemInspect.OversizedDents);

//        inspect.effortsCost(itemInspect.EffortLineCost);
//    }


//    //    $.each(model["CarInspections"], function () {
//    //        var index = arguments[0];
//    //        var inspect = inspection.parts()[index];
//    //        inspect.idSection(this.Id);

//    //        if (this.EffortItems.length > 0) {
//    //            var effortItems1 = this.EffortItems;
//    //            var effortItems = inspect.efforts().effortItems();

//    //            $.each(effortItems, function () {
//    //                var item = arguments[1];
//    //                $.each(effortItems1, function () {
//    //                    var item2 = arguments[1];
//    //                    if (item.text == item2.Name) {
//    //                        item.operations(item2.Operations);
//    //                        return false;
//    //                    }
//    //                    return true;
//    //                });
//    //            });

//    //            $.each(effortItems, function () {
//    //                var item = arguments[1];
//    //                $.each(effortItems1, function () {
//    //                    var item2 = arguments[1];
//    //                    if (item.text == item2.Name) {
//    //                        item.selectedEffortItem(item2.Choosed);
//    //                        return false;
//    //                    }
//    //                    return true;
//    //                });
//    //            });

//    //            $.each(effortItems, function () {
//    //                var item = arguments[1];
//    //                $.each(effortItems1, function () {
//    //                    var item2 = arguments[1];
//    //                    if (item.text == item2.Name) {
//    //                        item.hoursInput(item2.Hours);
//    //                        return false;
//    //                    }
//    //                    return true;
//    //                });
//    //            });
//    //        }
//    //        
//    //        inspect.oversizedDentsId(this.OversizedDentsId);
//    //        inspect.amountOversizedDents(this.AmountOversizedDents);
//    //        inspect.valuePriorDamage(this.PriorDamage);
//    //        
//    //        inspect.Aluminium(this.Aluminium);
//    //        inspect.DoubleMetal(this.DoubleMetal);
//    //        inspect.OversizedRoof(this.OversizedRoof);
//    //        inspect.CorrosionProtection(this.CorrosionProtection);

//    //        if (this.CustomCarInspectionLines.length > 0) {
//    //            inspect.customLines.removeAll();
//    //            $.each(this.CustomCarInspectionLines, function () {
//    //                inspect.customLines.push(new CustomCarInspectionLine(this, inspect.nameCustomLines, arguments[0]));
//    //            });
//    //        }
//    //        else {
//    //            inspect.customLines.removeAll();
//    //            inspect.customLines.push(new CustomCarInspectionLine({ Cost: 0, Name: "" }, inspect.nameCustomLines, 0));
//    //        }

//    //        inspect.OptionsPersent(this.OptionsPersent);
//    //        inspect.CorrosionProtectionCost(this.CorrosionProtectionCost);
//    //        inspect.oversizedDentsCost(this.OversizedDents);

//    //        inspect.effortsCost(this.EffortLineCost);
//    //    });

//}

//function setupCustomerTypeChange() {
//    $selectBox.selectBox();

//    $wholesaleId.change(function () {
//        CheckWCustomersForInsurance(this);
//        WholesaleCustomerDataBindChange();
//        UpdateData();
//    });

//    $('#estimate-customer label input[type="radio"]').change(function () {

//        var type = $(this).attr('value');
//        var $retailState = $('#Customer_Retail_State');
//        var $retailInputs = $('#retail-customer input[type="text"]');

//        window.globals.type = type;

//        if (type == 'Retail') {
//            viewModel.customerInfo(' - ' + $retailFirstName.val() + ' ' + $retailLastName.val());
//            InsuranceShow();
//            inspection.discount(0);
//            inspection.laborRate(0);
//            inspection.currentHourlyRate(window.globals.defaultHourlyRate);
//            inspection.maxCorProtect(window.globals.defaultMaxCorProtect);

//            InitDefaultData();
//            UpdateCarInspectionsData();
//            $retailInputs.removeAttr('disabled');
//            $retailState.selectBox('enable');
//            WholesaleDisable();
//            WholesaleCustomerDataUnBindChange();
//        }
//        else if (type == 'Wholesale') {
//            if (window.globals.order == 'True') {
//                WholesaleDisable();
//            }
//            else {
//                WholesaleEnable();
//            }
//            viewModel.customerInfo(' - ' + $('#Customer_Wholesale_CustomerId :selected').text());
//            $retailInputs.attr('disabled', 'disabled');
//            $retailState.attr('disabled', 'disabled');
//            CheckWCustomersForInsurance('#Customer_Wholesale_CustomerId');
//            WholesaleCustomerDataUnBindChange();
//            WholesaleCustomerDataBindChange();
//            UpdateData();

//        }
//    });
//}

//function WholesaleDisable() {
//    $wholesaleId.attr('disabled', 'disabled');
//    $wholesalaMatrixId.attr('disabled', 'disabled');
//    $selectBox.selectBox('disable');
//}

//function WholesaleEnable() {
//    $wholesaleId.removeAttr('disabled');
//    $wholesalaMatrixId.removeAttr('disabled');
//    $selectBox.selectBox('enable');
//}

//function CheckWCustomersForInsurance(select) {
//    var customers = $(select);

//    if (customers.data("checkInsurance").contains(customers.val())) {
//        InsuranceShow();
//    }
//    else {
//        InsuranceHide();
//    }
//}

//function InsuranceShow() {
//    $insurance.show().removeAttr('disabled');
//    $fieldset.show();
//    $insuranceInputs.removeAttr('disabled');
//    $insuranceCompanyNames.removeAttr('disabled');
//    $('input[name="Insurance.CompanyName"]').removeAttr('disabled');
//    window.globals.hasInsurance = "True";
//}

//function InsuranceHide() {
//    $insurance.hide().attr('disabled', 'disabled');
//    $fieldset.hide();
//    $insuranceInputs.attr('disabled', 'disabled');
//    $insuranceCompanyNames.attr('disabled', 'disabled');
//    $('input[name="Insurance.CompanyName"]').attr('disabled', 'disabled');
//    window.globals.hasInsurance = "False";
//}

//function WholesaleCustomerDataBindChange() {
//    $wholesaleId.bind('change', function () { UpdateMatrixData(); });
//    $wholesalaMatrixId.bind('change', function () { UpdateData(); });
//}

//function WholesaleCustomerDataUnBindChange() {
//    $wholesaleId.unbind('change');
//    $wholesalaMatrixId.unbind('change');
//}

//function UpdateMatrixData() {
//    var customerId = $wholesaleId.val();
//    viewModel.customerInfo(' - ' + $('#Customer_Wholesale_CustomerId :selected').text());

//    $.ajax({
//        type: 'POST',
//        url: window.globals.UrlGetWholesaleMatrices,
//        data: { customer: customerId },
//        beforeSend: function () {
//            $loader.show();
//        },
//        complete: function () {
//            $loader.hide();
//        },
//        success: function (data) {
//            var text = '';
//            $.each(data, function () {
//                var d = this;
//                text += '<option value="' + d.Value + '">' + d.Text + '</option>';
//            });
//            $wholesalaMatrixId.find('option').remove();
//            $wholesalaMatrixId.append(text);
//            $wholesalaMatrixId.selectBox('refresh');
//            UpdateData();
//        }
//    });
//}

//function UpdateCarInspectionsData() {
//    // console.log("start updateCarInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    var temp = window.globals.partsTemp.parts;
//    var len = 15;
//    while (len >= 0) {
//        var part = inspection.parts()[len];
//        part.Aluminium(temp[len].aluminium);
//        part.DoubleMetal(temp[len].doublemetal);
//        part.OversizedRoof(temp[len].oversizedroof);
//        part.selectedAverageSize(temp[len].averagesize);
//        part.selectedTotalDents(temp[len].totaldents);
//        part.amountOversizedDents(temp[len].amountoversizeddents);
//        part.CorrosionProtection(temp[len].corrosionprotection);
//        part.change();
//        len--;
//    }
//    //    for (var i = 0, len = 15; i < len; i++ )
//    //        $.each(inspection.parts(), function () {
//    //            var part = this;
//    //            var i = arguments[0];
//    //            part.Aluminium(temp[i].aluminium);
//    //            part.DoubleMetal(temp[i].doublemetal);
//    //            part.OversizedRoof(temp[i].oversizedroof);
//    //            part.selectedAverageSize(temp[i].averagesize);
//    //            part.selectedTotalDents(temp[i].totaldents);
//    //            part.amountOversizedDents(temp[i].amountoversizeddents);
//    //            part.CorrosionProtection(temp[i].corrosionprotection);
//    //            part.change();
//    //        });

//    window.globals.partsTemp.remove();
//    //console.log("end updateCarInspections - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//}

//function UpdateData() {
//    // console.log("start updateData - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    var customerId = $wholesaleId.val();
//    var matrixId = $wholesalaMatrixId.val();
//    window.globals.currentMatrix = matrixId;
//    CheckWCustomersForInsurance('#Customer_Wholesale_CustomerId');

//    $.ajax({
//        type: 'POST',
//        url: window.globals.UrlGetWholesaleCustomerData,
//        data: { customer: customerId, matrix: matrixId },
//        beforeSend: function () {
//            $loader.show();
//        },
//        complete: function () {
//            $loader.hide();
//        },
//        success: function (data) {
//            window.globals.maxCorProtect = data['MaxCorProtect'];
//            window.globals.laborRate = data['LaborRate'];
//            window.globals.discount = data['Discount'];
//            window.globals.hourlyRate = data['HourlyRate'];
//            window.globals.currentAluminium = data['Aluminium'];
//            window.globals.currentMaxCorProtect = data['MaxCorProtect'];
//            window.globals.currentDoubleMetall = data['DoubleMetall'];
//            window.globals.currentOversizedRoof = data['OversizedRoof'];
//            window.globals.currentOversizedDents = data['OversizedDents'];
//            window.globals.currentCorProtect = data['CorProtectPart'];
//            window.globals.currentMaxPercent = data['Maximum'];

//            inspection.discount(data['Discount']);
//            inspection.laborRate(data['LaborRate']);
//            inspection.maxCorProtect(data['MaxCorProtect']);

//            InitWholesaleCurrentData();

//            UpdateCarInspectionsData();
//        }
//    });
//    // console.log("end updateData - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//}

//function setupEffort() {
//    if (window.globals.state == 'view' ||
//        window.globals.state == 'edit') {
//        CarModel();
//    }
//    $year.bind('change', function () { CarModel(); });
//    $make.bind('change', function () { CarModel(); });
//    $model.bind('change', function () { CarModel(); });

//}

//function InitEffortItems(data) {
//    //  console.log("start initEffortItems - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    sections = data['Data'];
//    var parts = inspection.parts();
//    $.each(parts, function (i) {
//        var part = this;
//        var efforts = sections[i].EffortItems;
//        if (sections[i].Cost != null) {
//            part.newsectioncost(sections[i].Cost);
//        }
//        var effortLine = part.efforts();
//        var effortItems = effortLine.effortItems();
//        if (effortItems != null) {
//            $.each(effortItems, function () {
//                var item = this;
//                $.each(efforts, function () {
//                    var effort = this;
//                    if (item.text == effort.Name) {

//                        item.hoursR_I(effort.HoursR_I);
//                        item.hoursR_R(effort.HoursR_R);
//                        item.id(effort.Id);

//                        if (effort.HoursR_I != null && effort.HoursR_R == null) {
//                            item.hoursInput(effort.HoursR_I);
//                        }
//                        else if (effort.HoursR_I == null && effort.HoursR_R != null) {
//                            item.hoursInput(effort.HoursR_R);
//                        }
//                        else if (effort.HoursR_I != null && effort.HoursR_R != null) {
//                            item.hoursInput(effort.HoursR_I);
//                        }
//                        item.operations(effort.HoursR_I == null ? 1 : effort.HoursR_R == null ? 0 : 0);

//                        return false;
//                    }
//                });
//            });
//        }
//    });
//    // console.log("end initEffortItems - %d:%d", new Date().getSeconds(), new Date().getMilliseconds());
//    SetChosenEffortItems();
//}

//function CarModel() {
//    var year = $year.val();
//    var make = $make.val();
//    var model = $model.val();
//    var url = window.globals.UrlGetEffortHours;
//    var regYear = /^\d{4}$/gi;

//    if (regYear.test(year) && make.length > 0 && model.length > 0) {
//        $.ajax({
//            type: "POST",
//            data: { year: year, make: make, model: model },
//            url: url,
//            beforeSend: function () {
//                $loader.show();
//            },
//            complete: function () {
//                $loader.hide();
//            },
//            success: function (data) {
//                if (data['CarId'] == '1') {
//                    window.globals.defaultCar = "True";
//                }
//                else {
//                    window.globals.defaultCar = "False";
//                }

//                if (data['Data'] != null) {
//                    InitEffortItems(data);
//                }
//            }
//        });
//    }
//}