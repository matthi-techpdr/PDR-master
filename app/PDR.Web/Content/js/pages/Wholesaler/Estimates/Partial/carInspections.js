//@reference ~/Content/js/knockout.js
//@reference ~/Content/js/knockout.settings.js
//@reference ~/Content/js/knockout.validation.debug.js

function CustomCarInspectionLine(data, name, index) {
    var self = this;
    self.index = ko.observable(index);
    self.id = ko.observable(data.Id);
    self.nameCustomLinesId = ko.computed(function () { return name + '[' + self.index() + '].Id'; });
    self.nameCustomLinesText = ko.computed(function () { return name + '[' + self.index() + '].Name'; });
    self.nameCustomLinesPrice = ko.computed(function () { return name + '[' + self.index() + '].Cost'; });
    self.text = ko.observable(data.Name).extend({ maxLength: 101 });
    self.price = ko.observable(data.Cost).extend({ numeric: 2 });

    self.price.subscribe(function () {
        var customLineText = $('.customLineText');
        var linePrice = $('.priceLine');
        viewModel.validationCustomLines(customLineText, linePrice);
    });

    self.text.subscribe(function() {
        var customLineText = $('.customLineText');
        var linePrice = $('.priceLine');
        viewModel.validationCustomLines(customLineText, linePrice);
    });
}

function EstimateCustomLine(data, name, index) {
    var self = this;
    self.id = ko.observable(data.Id);
    self.index = ko.observable(index);
    self.nameEstimateCustomLinesId = ko.computed(function () { return name + '[' + self.index() + '].Id'; });
    self.nameEstimateCustomLinesText = ko.computed(function () { return name + '[' + self.index() + '].Name'; });
    self.nameEstimateCustomLinesPrice = ko.computed(function () { return name + '[' + self.index() + '].Cost'; });
    self.text = ko.observable(data.Name).extend({ maxLength: 101 });
    self.price = ko.observable(data.Cost).extend({ numeric: 2 });

    self.price.subscribe(function () {
        var customLineText = $('.mainLineText');
        var linePrice = $('.small');
        viewModel.validationCustomLines(customLineText, linePrice);
    });

    self.text.subscribe(function () {
        var customLineText = $('.mainLineText');
        var linePrice = $('.small');
        viewModel.validationCustomLines(customLineText, linePrice);
    });
}

function EffortLine(section, price, data) {

    var self = this;
    self.section = ko.observable(section);
    self.price = ko.observable(price).extend({ numeric: 2 });
    self.effortItems = ko.observableArray(data);
    self.chosenEffort = ko.observableArray([]);
    self.visibleCost = ko.observable(true);

    self.effortsHours = ko.computed(function () {
        var hours = 0;
        if (self.effortItems() == null) {
            self.visibleCost(false);
            return 0;
        }
        
        for (var b = 0; b < self.effortItems().length; b++) {
            if (self.effortItems()[b].selectedEffortItem()) {
                hours += parseFloat(self.effortItems()[b].hoursInput() == '' ? 0 : self.effortItems()[b].hoursInput());
            }
        }

        return hours;
    });
}

function EffortItem(id, text, hoursRI, hoursRR, parentName, index, type) {

    var self = this;
    self.id = ko.observable(id);
    self.text = text;
    self.name = parentName + index;
    self.hoursR_I = ko.observable(hoursRI);
    self.index = index;
    self.hoursR_R = ko.observable(hoursRR);
    self.selectedEffortItem = ko.observable(false);
    self.parentName = ko.observable(parentName);
    self.className = parentName + index;
    self.effortType = ko.observable();
    //self.currentEffortType = ko.observable(type);
    self.baseEffortType = ko.observable();

    self.radioIdRI = parentName + self.index;
    self.radioIdRR = parentName + (self.index) + 1;
    self.hoursInput = ko.observable().extend({ numeric: 2 });
    self.operations = ko.observable();
    
    self.selectedOperationRI = function (newValue) {
        if (self.hoursR_I() != null) {
            self.operations(0);
            self.hoursInput(self.hoursR_I());
        }
        else { self.operations(1); }
        return true;
    };
    
    self.selectedOperationRR = function (newValue) {
        if (self.hoursR_R() != null) {
            self.operations(1);
            self.hoursInput(self.hoursR_R());
        }
        else { self.operations(0); }
        return true;
    };


    self.hoursInput.subscribe(function (newValue) {
        var val = isNaN(parseFloat(newValue)) ? 0 : parseFloat(newValue);
        if (self.operations() == 0) {
            var ri = parseFloat(self.hoursR_I());
            self.effortType(ri != val ? 2 : self.baseEffortType());
        }
        else {
            var rr = parseFloat(self.hoursR_R());
            self.effortType(rr != val ? 2 : self.baseEffortType());
        }
    });
    
    self.visibleRI = ko.computed(function () {
        return self.hoursR_I() != null;
    });
    
    self.visibleRR = ko.computed(function () {
        return self.hoursR_R() != null;
    });

    self.selectedEffortItem.subscribe(function (newValue) {
        self.selectedEffortItem(newValue);
        var operation = self.operations();
        if (!newValue) {
            self.hoursInput(operation == 0 ? self.hoursR_I() : self.hoursR_R());
        }
        self.effortType(self.baseEffortType());
    });

    self.nameEffortItemId = parentName + '[' + self.index + '].Id';
    self.nameEffortItemText = parentName + '[' + self.index + '].Name';
    self.nameEffortItemHourManual = parentName + '[' + self.index + '].Hours';
    self.nameEffortItemHourR_I = parentName + '[' + self.index + '].HoursR_I';
    self.nameEffortItemHourR_R = parentName + '[' + self.index + '].HoursR_R';
    self.nameEffortItemOperations = parentName + '[' + self.index + '].Operations';
    self.nameEffortItemSelected = parentName + '[' + self.index + '].Choosed';
    self.nameEffortItemType = parentName + '[' + self.index + '].EffortType';
}

function AverageSize(data) {
    var self = this;
    self.text = ko.observable(data.Text);
    self.value = ko.observable(data.Value);
    self.selected = ko.observable(data.Selected);
}

function TotalDents(data) {
    var self = this;
    self.text = ko.observable(data.Text);
    self.value = ko.observable(data.Value);
    self.selected = ko.observable(data.Selected);
}

function PartNames(parent, data, fullName, inspection, index, sectionCost) {
    var self = this;
    self.parent = parent;
    self.initializeCompleted = false;
    self.id = data.Value;
    self.name = ko.observable(fullName);
    self.visibleSection = ko.observable(true);
    self.flagAlert = ko.observable(inspection.HasAlert == null ? '' : inspection.HasAlert);
    self.count = ko.observable(0);
    
//    self.typeTruck = ko.observable(false);
//    self.name.subscribe(function () {
//        var name = self.name();
//        if (name == "Left cab corner" || name == "Right cab corner") {
//            self.typeTruck(true);
//        }
//    });
    self.value = data.Value;
    self.idSection = ko.observable();

    self.newsectioncost = ko.observable(sectionCost);
    self.isChanges = ko.observable(0);
    self.IsSelected = ko.observable(data.Selected);
    self.dentsCost = ko.observable(inspection.DentsCost == null ? '': inspection.DentsCost);
    self.optionsPercent = ko.observable(inspection.OptionsPersent == null ? '' : inspection.OptionsPersent);
    self.oversizedDentsCost = ko.observable(inspection.OversizedDents == null ? '' : inspection.OversizedDents);
    self.corrosionProtectionCost = ko.observable(inspection.CorrosionProtectionCost == null ? '' : inspection.CorrosionProtectionCost).extend({ maxLength: 3, pattern: '^(d*\.?\d{2}?){1}$' });
    self.customCarInspectLines = ko.observableArray(inspection.CustomCarInspectionLines);

    self.already = ko.observable(false);
    self.now = ko.observable(false);

    self.atleastOnePart = ko.observable(false);

    self.effortsCost     = ko.computed(function () {
        var cost = 0;
        var efforts = inspection.EffortItems;
        for (var i = 0, len = efforts.length; i < len; i++) {
            cost += parseFloat(efforts[i].Hours);
        }

        return cost * parseFloat(self.parent.hourlyRate());;
    });

    self.totalEfforts = ko.observable(self.effortsCost());

    self.effortsCost.subscribe(function() {
        self.totalEfforts(self.effortsCost());
    });
    
    self.totalCustomLines = ko.computed(function () {
        var total = 0;
        var customLines = self.customCarInspectLines();
        for (var s = 0, len = customLines.length; s < len; s++) {
            var cost = parseFloat(customLines[s].Cost);
            total += isNaN(cost) ? 0 : cost;
        }
        return isNaN(total) ? '' : total;
    });
    
    self.totalCustLines = ko.observable(self.totalCustomLines());

    self.nameTotalOneParts = 'CarInspectionsModel.CarInspections[' + index + '].PartsTotal';
    self.nameIsChanges = 'CarInspectionsModel.CarInspections[' + index + '].IsChanges';
    self.nameHasAlert = 'CarInspectionsModel.CarInspections[' + index + '].HasAlert';

    //sum one section
    self.totalParts = ko.computed(function () {
        var protect = parseFloat(self.parent.maxCorProtect());
        var sumprotect = parseFloat(self.parent.sumAllPartsCorProtect());
        var total = 0;
        var $pic1 = $('.sp' + index + '-1');
        var $pic2 = $('.sp' + index + '-2');
        var $pic3 = $('.sp' + index + '-3');

        if (viewModel.vehicleType() == "Truck") {
            $pic1 = $('.tr' + index + '-1');
            $pic2 = $('.tr' + index + '-3');
            $pic3 = $('.tr' + index + '-2');
        }

        total += parseFloat(self.dentsCost() == '' ? 0 : self.dentsCost());
        total += total * parseFloat(self.optionsPercent() == '' ? 0 : self.optionsPercent()) * 0.01;

        total += parseFloat(self.oversizedDentsCost() == '' ? 0 : self.oversizedDentsCost());

        total += parseFloat(self.totalEfforts() == '' ? 0 : self.totalEfforts());

        total += protect >= sumprotect ? parseFloat(self.corrosionProtectionCost() == '' ? 0 : self.corrosionProtectionCost()) : 0;

        total += parseFloat(self.totalCustLines());
        //window.globals.defaultCar == "False" &&
        //if (viewModel.unknownCar() != 'true') {

        if (parseFloat(self.newsectioncost()) > 0 && total > 0) {
            if (total >= (parseFloat(self.newsectioncost()) * parseInt(window.globals.limit) * 0.01) && total != 0) {

                $pic1.hide();
                $pic3.hide();
                $pic2.show();

             //   if (window.globals.state != 'view' && self.initializeCompleted) {// &&
                if (self.initializeCompleted) {// &&
                    //window.globals.state != 'edit'
                    
                    if (!self.flagAlert() && parseFloat(self.newsectioncost()) > 0) { //window.globals.flagAlert
                        if (self.count() == 0)
                            jAlert("Estimated value of the car body repair is almost equal to the sum of its replacement. \nPlease, redefine the estimate.", "Warning!", function () {
                                self.flagAlert(true); // window.globals.flagAlert = false;
                                var c = self.count();
                                self.count(c++);
                            });
                    }
                }
                self.now(true);
                self.already(false);
            }
            else if (total > 0 && total < (parseFloat(self.newsectioncost()) * parseInt(window.globals.limit) * 0.01)) {
                $pic1.hide();
                $pic2.hide();
                $pic3.show();
                self.flagAlert(false);
                self.already(true);
                self.now(false);
            }
        }
        else {
            if (total > 0) {
                $pic1.hide();
                $pic2.hide();
                $pic3.show();
                self.already(true);
                self.now(false);
            }
            else {
                $pic3.hide();
                $pic2.hide();
                $pic1.show();
                self.already(false);
                self.now(false);
            }
        }
        self.atleastOnePart(total > 0 ? true : false);
      
        return isNaN(total) ? 0 : parseFloat(total.toFixed(2));
    });

    self.totalQuick = ko.observable();
    
    self.formattedTotalParts = ko.computed(function () {
        var price = parseFloat(self.totalParts());
        var quick = parseFloat(self.totalQuick());

        if (self.parent.typeEstimate() == "Quick") {
            if (price > 0) {
                return isNaN(price) ? '$0.00' : "$" + price.toFixed(2);
                
            } //price == 0 && quick  != 0 &&
            return isNaN(quick)? '$0.00' : "$" + quick.toFixed(2);
        }
        return isNaN(price) ? '$0.00' : "$" + price.toFixed(2);
    });
}

function Part(parent, data, fullName, index, effortLine) {
    var self = this;
    self.initializeCompleted = false;
    self.parent = ko.observable(parent);
    self.idSection = ko.observable();
    self.id = data.Value;
    self.index = index;
    self.name = fullName;
    self.valueName = data.Value;
    self.value = data.Value;
    self.countEdit = 0;
    self.efforts = ko.observable(effortLine);
    self.amountOversizedDents = ko.observable('').extend({ numeric: 0 }); ;
    self.dentsCost = ko.observable('').extend({ numeric: 2 });
    self.oversizedDentsId = ko.observable();
    self.oversizedDentsCost = ko.observable('').extend({ numeric: 2 });
    self.valuePriorDamage = ko.observable();

    self.blockPercent = ko.observable();
    self.blockDentsCost = ko.observable();
    self.blockOversizedDents = ko.observable();

    self.oversizedDentsCost.subscribe(function (newValue) {
            self.parent().partsNames()[index].oversizedDentsCost(isNaN(newValue) ? '' : newValue);
    });
    
    self.sumAllPartsCorProtect = ko.observable(0);
    self.atleastOnePart = ko.observable(false);

    self.already = ko.observable(false);
    self.now = ko.observable(false);
    
    self.selectedAverageSize = ko.observable();
    self.selectedTotalDents = ko.observable();

    self.effortsCost = ko.observable('').extend({ numeric: 2 });

    self.nameIdSection = 'CarInspectionsModel.CarInspections[' + self.index + '].Id';
    self.nameName = 'CarInspectionsModel.CarInspections[' + self.index + '].Name';
    self.nameTotalDents = 'CarInspectionsModel.CarInspections[' + self.index + '].TotalDents';
    self.nameAverageSize = 'CarInspectionsModel.CarInspections[' + self.index + '].AverageSize';
    self.nameAluminium = 'CarInspectionsModel.CarInspections[' + self.index + '].Aluminium';
    self.nameCorrosionProtection = 'CarInspectionsModel.CarInspections[' + self.index + '].CorrosionProtection';
    self.nameDoubleMetal = 'CarInspectionsModel.CarInspections[' + self.index + '].DoubleMetal';
    self.nameOversizedDentsId = 'CarInspectionsModel.CarInspections[' + self.index + '].OversizedDentsId';
    self.nameOversizedDents = 'CarInspectionsModel.CarInspections[' + self.index + '].OversizedDents';
    self.nameAmountOversizedDents = 'CarInspectionsModel.CarInspections[' + self.index + '].AmountOversizedDents';
    self.nameOversizedRoof = 'CarinspectionsModel.CarInspections[' + self.index + '].OversizedRoof';
    self.nameEffortItems = 'CarInspectionsModel.CarInspections[' + self.index + '].EffortItems';
    self.nameCustomLines = 'CarInspectionsModel.CarInspections[' + self.index + '].CustomCarInspectionLines';
    
    self.nameOptionsPercent = 'CarInspectionsModel.CarInspections[' + self.index + '].OptionsPersent';
    self.nameCorrosionProtectSum = 'CarInspectionsModel.CarInspections[' + self.index + '].CorrosionProtectionCost';
    self.nameEffortLineCost = 'CarInspectionsModel.CarInspections[' + self.index + '].EffortLineCost';
    self.nameDentsCost = 'CarInspectionsModel.CarInspections[' + self.index + '].DentsCost';
    self.namePriorDamage = 'CarInspectionsModel.CarInspections[' + self.index + '].PriorDamage';

    self.IsSelected = ko.observable(data.Selected);

    //================= Start CheckBox Options =============================
    self.Aluminium = ko.observable(false);
    self.DoubleMetal = ko.observable(false);
    self.OversizedRoof = ko.observable(false);
    self.CorrosionProtection = ko.observable(false);
    self.OptionsPersent = ko.observable();
    self.CorrosionProtectionCost = ko.observable().extend({numeric: 2 });

    //================= End CheckBox Options =============================

    self.sectionTextPriorDamages = ko.computed(function () {
        return self.name + " prior damages:";
    });

    self.sectionTextCustomLines = ko.computed(function () {
        return self.name + " custom fields:";
    });
    
    //================= Start Effort Item ================================

    self.effortsHours = ko.computed(function () {
     
        var effortline = self.efforts();
        var hours = effortline.effortsHours().toFixed(2);

        if (hours > 0) {
            var rate = parseFloat(self.parent().hourlyRate());
            var cost = parseFloat(hours) * rate;
            self.effortsCost(cost.toFixed(2));
            self.parent().partsNames()[index].totalEfforts(cost.toFixed(2));
        }
        else {
            self.effortsCost('');
            self.parent().partsNames()[index].totalEfforts('');
        }
        return isNaN(hours) ? '0.00' : hours;
    });

    //===============End effort item=======================================

   
    self.formattedEffortHours = ko.computed(function () {
        var hours = self.effortsHours();
        return hours + " hours";
    }, this);

    self.customLines = ko.observableArray([new CustomCarInspectionLine({ Cost: 0, Name: "" }, self.nameCustomLines, 0)]);

    self.currentLengthCustomLines = ko.computed(function () {
        var length = self.customLines().length;
        return length - 1;
    });

    self.addCustomLine = function () {
        self.customLines.push(new CustomCarInspectionLine({ Cost: 0, Name: "" }, self.nameCustomLines, self.customLines().length));
        //viewModel.validationCustomLines();
    };

    self.removeCustomLine = function (customLine) {
        var customLines = self.customLines();
        var len = customLines.length;
        if (len > 1) {
            self.customLines.remove(customLine);
        }
        len = customLines.length;
        for (var f = 0; f < len; f++) {
            customLines[f].index(f);
        }
    };

    self.Aluminium.subscribe(function (newValue) {
        self.changeOption();
    });

    self.OversizedRoof.subscribe(function (newValue) {
        self.changeOption();
    });

    self.DoubleMetal.subscribe(function (newValue) {
        self.changeOption();
    });

    self.CorrosionProtection.subscribe(function (newValue) {
        var corp = parseInt(self.parent().corProtectPart());
        var copTemp = self.CorrosionProtectionCost();
        if (newValue) {
            $('.CorProtectClass').attr('readonly', false);
            self.CorrosionProtectionCost(copTemp != '' || copTemp != 0 ? copTemp : corp);
            self.parent().partsNames()[index].corrosionProtectionCost(copTemp != '' || copTemp != 0 ? copTemp : corp);
        }
        else {
            $('.CorProtectClass').attr('readonly', true);
            self.CorrosionProtectionCost('');
            self.parent().partsNames()[index].corrosionProtectionCost('');
        }
    });

    self.dentsCost.subscribe(function (newValue) {
        self.parent().partsNames()[index].dentsCost(isNaN(newValue) ? '' : newValue);
        if (newValue > 0) {
            self.blockPercent(false);
        }
        else {
            self.Aluminium(false);
            self.OversizedRoof(false);
            self.DoubleMetal(false);
            self.blockPercent(true);
        }
    });
    
    self.CorrosionProtectionCost.subscribe(function (newValue) {
        self.CorrosionProtectionCost(newValue);
        self.parent().partsNames()[index].corrosionProtectionCost(newValue);
    });

    self.changeOption = function () {
        var sumPercent = 0;
        var or = self.OversizedRoof();
        var dm = self.DoubleMetal();
        var al = self.Aluminium();
        var maxPercent = parseInt(self.parent().maxPercentPart());
        if (al || dm || or) {
            if (al) {
                sumPercent += parseInt(self.parent().aluminiumPercent());
            }
            if (or) {
                sumPercent += parseInt(self.parent().oversizedRoofPercent());
            }
            if (dm) {
                sumPercent += parseInt(self.parent().doubleMetallPercent());
            }
            self.OptionsPersent(sumPercent > maxPercent ? maxPercent : sumPercent);
            self.parent().partsNames()[index].optionsPercent(sumPercent > maxPercent ? maxPercent : sumPercent);
        }
        else {
            self.OptionsPersent('');
            self.parent().partsNames()[index].optionsPercent('');
        }
    };

    self.amountOversizedDents.subscribe(function (newValue) {
        self.amountOversizedDents(newValue);
        var amount = parseInt(self.amountOversizedDents());
        var overDents = self.parent().oversizedDents();
        if (amount >= 0) {
            var cost = amount * parseFloat(overDents);
            self.oversizedDentsCost(cost);
            self.parent().partsNames()[index].oversizedDentsCost(cost);
        }
        else {
            self.oversizedDentsCost('');
            self.parent().partsNames()[index].oversizedDentsCost('');
        }
    });

    self.totalCustomLines = ko.computed(function () {
        var total = 0;
        var customLines = self.customLines();
        var len = customLines.length;
        for (var s = 0; s < len; s++) {
            var cost = parseFloat(customLines[s].price());
            total += isNaN(cost) ? 0 : cost;
        }
        
        self.parent().partsNames()[index].totalCustLines(isNaN(total) ? 0 : total);
    });

    self.changeTotalDents = function () {
        if (self.parent().isDentsAmountAllowsToEditDentsCost(self.selectedTotalDents())) {
            self.parent().disableDentsCost(true, self.index);
            self.blockOversizedDents(true);
            self.amountOversizedDents('');
            self.oversizedDentsCost('');
        } else {
            self.parent().disableDentsCost(false, self.index);
            self.blockOversizedDents(false);
            self.change();
        }
    };

    self.change = function () {
        var totalDents = self.selectedTotalDents();
        var averageSize = self.selectedAverageSize();
        var part = self.valueName;
        var $loader = $('#loader');
        var matrixId = $('#matrixId').val();
        var url = window.globals.UrlGetDentsCost;
       // if (window.globals.state != 'view') {
            if (totalDents >= 0 && averageSize >= 0) {
//                if (window.globals.state == 'edit' ? self.countEdit >= 1 : true) {
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: { totalDents: totalDents, averageSize: averageSize, part: part, matrixId: matrixId },
                        beforeSend: function () {
                            $loader.show();
                        },
                        complete: function () {
                            $loader.hide();
                        },
                        success: function (dentscost) {
                            self.dentsCost(dentscost);
                            self.parent().partsNames()[index].dentsCost(dentscost);
                            self.blockDentsCost(false);
                        }
                    });
//                }
//                else {
//                    self.countEdit++;
//                }
            }
            else {

                self.dentsCost('');
                self.parent().partsNames()[index].dentsCost('');
                self.Aluminium(false);
                self.OversizedRoof(false);
                self.DoubleMetal(false);
                self.blockPercent(true);
                self.blockDentsCost(true);
            }
       // }
    };
}

//======================================== QUICK MODEL=======================================

function Quick(parent, name, cost) {
    var self = this;
    self.parent = ko.observable(parent);
    self.name = ko.observable(name);
    self.cost = ko.observable(cost);

    self.initCost = function () {
        var parts = self.parent().partsNames();
        var price = self.cost();

        for (var i = 0, len = parts.length; i < len; i++) {
            if (parts[i].name() == name) {
                parts[i].totalQuick(price);
            }
        }
    };

    self.costView = ko.computed(function () {
        var price = parseFloat(self.cost());
        return isNaN(price) ? '$0.00' : '$' + price.toFixed(2);
    });
}

//======================================== CAR INSPECTION VIEW MODEL=======================================

function CarInspectionsViewModel(parent) {
    var self = this;
    self.DENTS_SIZE_ID_BASE = "#dents-size";
    self.parent = parent;
    self.readonly = ko.observable();
    self.priorDamages = ko.observable();
    self.averageSize = ko.observableArray([]);
    self.totalDents = ko.observableArray([]);
    self.partsIndex = [];
    self.newSectionCosts = [];
    self.parts = ko.observableArray([]);
    self.partsNames = ko.observableArray([]);

    self.efforts = ko.observableArray([]);
    self.typeEstimate = ko.observable();
    self.quickSections = ko.observableArray([]);
    self.totalExtraQuick = ko.observable();
    self.totalWithoutDiscountAndLabor = ko.observable();
    self.totalExtraQuickView = ko.computed(function () {
        var total = parseFloat(self.totalExtraQuick());
        return isNaN(total) ? '' : '$' + total.toFixed(2);
    });
    
    self.maxCorProtect = ko.observable(0);
    self.laborRate = ko.observable(0);
    self.discount = ko.observable(0);

    //=========================================
    self.hourlyRate = ko.observable(0);
    self.aluminiumPercent = ko.observable(0);
    self.doubleMetallPercent = ko.observable(0);
    self.oversizedRoofPercent = ko.observable(0);
    self.oversizedDents = ko.observable(0);
    self.corProtectPart = ko.observable(0);
    self.maxPercentPart = ko.observable(0);

//    self.workByThemselve = ko.observable(true);

//    self.workByThemselve.subscribe(function (newValue) {
//        debugger;
//        var parts = self.parts();
//        if (!newValue) {
//            for (var i = 0, len = parts.length; i < len; i++) {
//                var efforts = parts[i].efforts().effortItems();
//                for (var x = 0, l = efforts.length; x < l; x++) {
//                    efforts[x].selectedEffortItem(false);
//                }
//            }
//        }
//    });

    self.sumAllPartsCorProtect = ko.computed(function () {
        var sum = 0;
        var cost = 0;
        var parts = self.partsNames();
        //if (parts.length == 16) {
        for (var q = 0, len = 19; q < len; q++) {
            if (parts[q] != undefined && parts[q] != null) {
                cost = parseFloat(parts[q].corrosionProtectionCost());
                sum += isNaN(cost) || cost == '' ? 0 : cost;
            }
            //parts[q].sumAllPartsCorProtect(sum);
        }
        //}

        return sum;
    });
 
    self.nameEstimateCustomLines = 'CarInspectionsModel.EstimateCustomLines';
    self.namePriorDamages = 'CarInspectionsModel.PriorDamages';
    self.nameFullTotal = 'CarInspectionsModel.Total';
    self.nameTypeEstimate = 'Type';
    //self.nameCalculatedSum = 'CarInspectionsModel.CalculatedSum';

    self.estimateCustomLines = ko.observableArray([]);

    self.currentLengthCustomEstimateLines = ko.computed(function () {
        var length = self.estimateCustomLines().length;
        return length - 1;
    });

    self.initPartNames = function (names, fullnames, carInspections) {
        var sectionCost = self.parent.sections;

        for (var e = 0, len = 19; e < len; e++) {
            self.partsNames.push(new PartNames(self, names[e], fullnames[e], carInspections[e], e, sectionCost[e] == undefined ? 0 : sectionCost[e].Cost));
            self.partsNames()[e].isChanges(false);
            var name = self.partsNames()[e].name();
            if (name == "Left cab corner" || name == "Right cab corner") {
                self.partsNames()[e].visibleSection(viewModel.vehicleType() == "Truck");
            }
        }
    };

    self.initQuickParts = function (partsName, inspections) {
        var quick;
        for (var i = 0, len = 19; i < len; i++) {
            if (inspections[i].QuickCost != null) {
                quick = new Quick(self, partsName[i], inspections[i].QuickCost);
            }
            else {
                quick = new Quick(self, partsName[i], 0);
            }
            quick.initCost();
            self.quickSections.push(quick);
        }
        //        var quick = new Quick(self, 'Deck lid', 456);
        //        quick.initCost();
        //        self.quickSections.push(quick);
        //        quick = new Quick(self, 'Roof', 26);
        //        quick.initCost();
        //        self.quickSections.push(quick);
    };

    self.initCarInspections = function (model, effortsModel) {
        viewModel.inspectionsData(model.CarInspections);
        effortsModel.initEffortsArray();
        self.priorDamages(model.PriorDamages);
        self.readonly(window.globals.isReadOnlyMode);

        for (var i = 0, len = model.AverageSize.length; i < len; i++) {
            self.averageSize.push(new AverageSize(model.AverageSize[i]));
        }
        for (var j = 0, lenDents = model.TotalDents.length; j < lenDents; j++) {
            self.totalDents.push(new TotalDents(model.TotalDents[j]));
        }

        self.initEstimateCustomLines(model.EstimateCustomLines);

//        self.discount(window.globals.discount);
//        self.laborRate(window.globals.laborRate);
        self.maxCorProtect(window.globals.defaultMaxCorProtect);

        //self.workByThemselve(window.globals.workByThemselve == "False" ? false : true);

//        self.initData();

//        if (window.globals.state != 'new') {
//            self.parent.car.carModel(function () {
//                self.initPartNames(model.PartsNames, model.FullPartsNames, model.CarInspections);
//                self.parent.setNewSectionCost();
//                self.initParts(model.PartsNames[0], model.FullPartsNames[0], effortsModel.effortsArray[0], model.CarInspections[0], 0);
//                if (self.typeEstimate() == "Quick") {
//                    self.initQuickParts(model.FullPartsNames, model.CarInspections);
//                }
//                else if (self.typeEstimate() == "ExtraQuick") {
//                    self.totalExtraQuick(model.ExtraQuickCost);
//                }
//                else {
//                    self.initQuickParts(model.FullPartsNames, model.CarInspections);
//                    self.totalExtraQuick(model.ExtraQuickCost);
//                }

//                if (self.quickSections().length > 0) {
//                    self.currentQuickName(self.quickSections()[0].name());
//                    self.currentQuickCost(self.quickSections()[0].cost());
//                }
//                $('#0').addClass('active');
//            });

//        }
//        else {
            self.parent.initSectionCost();
            self.initPartNames(model.PartsNames, model.FullPartsNames, model.CarInspections);
            self.parent.setNewSectionCost();
            self.initParts(model.PartsNames[0], model.FullPartsNames[0], effortsModel.effortsArray[0], model.CarInspections[0], 0);
//        }

        $('#0').addClass('active');
    };

    self.isDentsAmountAllowsToEditDentsCost = function (selectedTotalDentsValue) {
        var selectedTotalDentsIndex = -1;
        for (var i = 0; i < self.totalDents().length; ++i) {
            if (self.totalDents()[i].value() == selectedTotalDentsValue) {
                selectedTotalDentsIndex = i;
                break;
            }
        }

        if (selectedTotalDentsIndex != -1) {
            var selectedTotalDentsText = self.totalDents()[i].text();

            if (selectedTotalDentsText == 'Conventional' || selectedTotalDentsText == 'No Damage') {
                return true;
            }
        }
        return false;
    };

    self.disableDentsCost = function (disable, index) {
        var ENABLE = 'enable';
        var DISABLE = 'disable';
        var dentsSizeId = self.DENTS_SIZE_ID_BASE + index;
        if (disable) {
            $(dentsSizeId).selectBox(DISABLE);
            self.parts()[self.partsIndex[index]].selectedAverageSize(-1);
            $(dentsSizeId).next().children('.selectBox-label').text($(dentsSizeId + " option:selected").text());
        } else {
            $(dentsSizeId).selectBox(ENABLE);
        }
    };

    self.initParts = function (name, fullname, effort, inspection, index) {
        var isiPad = navigator.userAgent.match(/iPad/i) != null;
        self.parts.push(new Part(self, name, fullname, index, effort));

        var partsIndex = self.parts().length - 1;
        self.partsIndex[index] = partsIndex;
        var part = self.parts()[partsIndex];
        part.idSection(inspection.Id);

        self.partsNames()[index].idSection(inspection.Id);
        self.partsNames()[index].isChanges(1);
        self.partsNames()[index].IsSelected(true);
        part.IsSelected(true);

        part.selectedAverageSize(inspection.AverageSize);
        part.selectedTotalDents(inspection.TotalDents);
        if (isiPad) {
            var selects = $('#parts').find('select');
            $.each(selects, function () {
                $(this).wrap('<div class="wrapperBlock"></div>');
            });
            selects.after('<span class="selectBox-arrow" style="top:-2px;right:26px;"></span>');
        }
        else {
            $('#parts').find('select').selectBox();
        }

        part.dentsCost(inspection.DentsCost);

        if (inspection.AverageSize != '-1' && inspection.TotalDents != '-1') {
            part.blockDentsCost(false);
        }

        if (self.isDentsAmountAllowsToEditDentsCost(inspection.TotalDents)) {
            self.disableDentsCost(true, part.index);
        }

        if (inspection.DentsCost == null || inspection.DentsCost == 0) {
            part.countEdit = 1;
        }
        part.amountOversizedDents(inspection.AmountOversizedDents);
        part.oversizedDentsCost(inspection.OversizedDents);
        part.oversizedDentsId(inspection.OversizedDentsId);

        part.valuePriorDamage(inspection.PriorDamage);

        part.Aluminium(inspection.Aluminium);

        part.DoubleMetal(inspection.DoubleMetal);
        part.OversizedRoof(inspection.OversizedRoof);
        part.CorrosionProtection(inspection.CorrosionProtection);
        part.CorrosionProtectionCost(inspection.CorrosionProtectionCost);
        //part.OptionsPersent(0);
        //

        if (inspection.EffortItems.length > 0) {
            var effortItems1 = inspection.EffortItems;
            var effortItems = part.efforts().effortItems();
            for (var x = 0, len1 = effortItems.length; x < len1; x++) {
                for (var y = 0, len2 = effortItems1.length; y < len2; y++) {
                    if (effortItems[x].text == effortItems1[y].Name) {
                        effortItems[x].operations(effortItems1[y].Operations);
                        effortItems[x].effortType(effortItems1[y].EffortType);
                        effortItems[x].selectedEffortItem(effortItems1[y].Choosed);
                        effortItems[x].hoursInput(effortItems1[y].Hours);
                        break;
                    }
                }
            }
        }

        if (inspection.CustomCarInspectionLines.length > 0) {
            part.customLines.removeAll();
            $.each(inspection.CustomCarInspectionLines, function () {
                part.customLines.push(new CustomCarInspectionLine(this, part.nameCustomLines, arguments[0]));
            });
        }

        if (viewModel.isiPad()) {
            $('.totalDentsSel').show();
            $('.averageSel').show();
        }
        self.partsNames()[index].initializeCompleted = true;
    };

//    self.initData = function() {
//        if (window.globals.customerType == 'Retail') {
//            self.parent.initDefaultData();
//            //self.parent.updateCarInspectionsData();
//        }
//        else {
//            self.parent.initWholesaleCurrentData();
//            //self.parent.updateCarInspectionsData();
//        }
//        //self.parent.setupEffort();

//       // $loader.hide(); 
//    };

    self.initEstimateCustomLines = function (data) {
        if (data.length > 0) {
            self.estimateCustomLines.removeAll();
            for (var e = 0, len = data.length; e < len; e++) {
                self.estimateCustomLines.push(new EstimateCustomLine(data[e], self.nameEstimateCustomLines, e));
            }
        }
        else {
            self.estimateCustomLines.push(new EstimateCustomLine({ Name: "", Cost: 0 }, self.nameEstimateCustomLines, 0));
        }
    };

    self.addEstimateCustomLine = function () {
        self.estimateCustomLines.push(new EstimateCustomLine({ Name: "", Cost: 0 }, self.nameEstimateCustomLines, self.estimateCustomLines().length));
    };

    self.removeEstimateCustomLine = function (estimateCustomLine) {
        if (self.estimateCustomLines().length > 1) {
            self.estimateCustomLines.remove(estimateCustomLine);
        }
        var customlines = self.estimateCustomLines();
        $.each(customlines, function (i) {
            this.index(i);
        });
    };

    self.totalCarInspectionLines = ko.computed(function () {
        var total = 0;
        var customLines = self.estimateCustomLines();
        var len = customLines.length;
        for (var s = 0; s < len; s++) {
            var cost = parseFloat(customLines[s].price());
            total += isNaN(cost) ? 0 : cost;
        }
        return isNaN(total) ? '' : total;
    });

    self.selectedPartsId = ko.observable(0);
    self.currentQuickName = ko.observable();
    self.currentQuickCost = ko.observable();
    self.currentQuickEstimate = ko.computed(function () {
        var cost = parseFloat(self.currentQuickCost());
        var name = self.currentQuickName();

        return isNaN(cost) || cost == 0 ? '' : ' - ' + name + ' $' + cost.toFixed(2);
    });
    self.goToParts = function (parts) {
        $loader.show();
        var part = self.parts();
        var quickSections = self.quickSections();
        var len = part.length;
        $('.none').removeClass('active');
        $('.already').removeClass('active');
        for (var i = 0; i < 19; i++) {
            if (part[i] != undefined && part[i] != null) {
                part[i].IsSelected(false);
            }
        }

        parts.IsSelected(true);
        self.selectedPartsId(parts.id);
        $('#' + parts.id).addClass('active');
        var index = +parts.id;
        if (quickSections.length > 0) {
            self.currentQuickName(quickSections[index].name());
            self.currentQuickCost(quickSections[index].cost());
        }
        if (self.partsIndex[index] == null || self.partsIndex[index] == undefined) {
            self.initParts(window.globals.inspection.PartsNames[index], window.globals.inspection.FullPartsNames[index], self.parent.effortsModel.effortsArray[index], window.globals.inspection.CarInspections[index], index);
        }

        $loader.hide();
        part = self.parts();
        part[self.partsIndex[index]].IsSelected(true);
    };
   
    //sum all section + estimate custom lines + (only wholesalecustomer settings)
    self.totalFull = ko.computed(function () {
        var totalFull = 0;
        var sum = parseFloat(self.sumAllPartsCorProtect());
        var max = parseFloat(self.maxCorProtect());

        var parts = self.partsNames();
        if (parts.length == 19) {
            totalFull += parseFloat(self.totalCarInspectionLines() == '' ? 0 : self.totalCarInspectionLines());
            $.each(parts, function (i) {
                totalFull += parseFloat(parts[i].totalParts() == '' ? 0 : parts[i].totalParts());
            });
        }

        if (sum > max) {
            totalFull += max;
        }

        viewModel.totalWithoutDiscountAndLabor(totalFull);
        
//        if (window.globals.type == 'Wholesale' ||
//            window.globals.customerType == 'Wholesale') {
            //var discount = viewModel.discount();
            var labor = self.laborRate();
            //remove discount for estimate
            //totalFull -= (totalFull * discount / 100);
            totalFull += Math.round(totalFull * labor * 0.01 * 100) /100;
//        }

        return isNaN(totalFull) ? 0.00 : parseFloat(totalFull.toFixed(2));
    });

    self.totalQuickFull = ko.computed(function () {

        var parts = self.partsNames();
        var total = 0;
        if (parts.length == 19) {
            for (var i = 0, len = 19; i < len; i++) {
                var t = parseFloat(parts[i].totalQuick());
                total += isNaN(t) ? 0 : t;
            }
        }
        return total;
    });

    self.formattedTotalFull = ko.computed(function () {
        
        var total = parseFloat(self.totalFull());
        var totalEst = parseFloat(viewModel.totalEstimate());
        if (self.typeEstimate() == 'Quick') {
            var quick = parseFloat(self.totalQuickFull());
            viewModel.total(quick);

            if (total == 0 && quick != 0) {
                return "$" + quick.toFixed(2);
            }
        }
        else if (self.typeEstimate() == 'ExtraQuick') {
            var extraquick = parseFloat(self.totalExtraQuick());
            viewModel.total(extraquick);
            if (total == 0 && extraquick != 0) {
                return "$" + extraquick.toFixed(2);
            }
        }

        if (total == totalEst) {
            viewModel.totalEstimate(0);
        }
        viewModel.total(total);
        return '$' + total.toFixed(2);
    });
}