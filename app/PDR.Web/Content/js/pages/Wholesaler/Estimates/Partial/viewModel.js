//@reference ~/Content/js/knockout.js
//@reference ~/Content/js/knockout.settings.js

var ViewModel = function () {
    var self = this;
    self.isiPad = ko.observable(navigator.userAgent.match(/iPad/i) != null);
//    self.customerType = ko.observable(window.globals.customerType);
    self.carInspection = ko.observable();
//    self.customerInfo = ko.observable('');
//    self.customerRetailInfo = ko.observable();
//    self.order = ko.observable();
    self.discount = ko.observable(window.globals.currentCustomer.Discount);
    self.laborRate = ko.observable(window.globals.currentCustomer.LaborRate);
//    self.workByThemselve = ko.observable();
//    self.workByThemselve = ko.observable(window.globals.workByThemselve);
    self.workByThemselve = ko.observable(window.globals.currentCustomer.WorkByThemselve);

    // self.effortHours = ko.observable(window.globals.effortHours);
    self.effortHours = ko.observable();

    self.SupplementsSum = ko.observable();
    self.EstSubtotal = ko.observable();

//    self.workByThemselve.subscribe(function (newValue) {
//        $(".WorkByThemselve").val(newValue);
//    });

//    self.newHourlyRate = ko.observable(window.globals.newHourlyRate);
//    self.currentHourlyRate = ko.observable(window.globals.currentHourlyRate);
//    self.isNewHourlyRate = ko.observable(window.globals.isNewHourlyRate);

//    self.isNewHourlyRate.subscribe(function (newValue) {
//        var isLoaded = $('#pageIsAlreadyLoaded').val();
//        var notUndefined = (typeof isLoaded !== 'undefined');
//        if (notUndefined && isLoaded == 'true') {
//            if (newValue) {
//                $('.laborRates').show();
//            } else {
//                $('.laborRates').hide();
//                $('#NewHourlyRate').val("");
//                var hourlyRate = self.currentHourlyRate();
//                self.carInspection().hourlyRate(hourlyRate);
//                var sum = self.effortHours() * hourlyRate;
//                self.effortSum(sum);
//                self.ReCalculateEstimate();
//            }
//        }
//        $(".IsNewHourlyRate").val(newValue);
//    });

//    self.newHourlyRate.subscribe(function (value) {
//        if (value == "") value = self.currentHourlyRate();
//        self.carInspection().hourlyRate(value);
//        var sum = self.effortHours() * value;
//        self.effortSum(sum);
//        self.ReCalculateEstimate();
//    });

 //   self.currentHourlyRate = ko.observable(window.globals.currentCustomer.HourlyRate);


    self.ReCalculateEstimate = function () {
        //        var estTaxSum = (self.EstSubtotal() + self.effortSum()) * self.EstCurrentLaborTax();
        //        var estTotalAmount = self.EstSubtotal() + self.effortSum() + estTaxSum;
        //        var estCleanTotalAmount = estTotalAmount - estTaxSum;
        var repairOrderSumWithoutDiscountAndTax = self.EstSubtotal() + self.effortSum() + self.SupplementsSum();
        self.orderSum(repairOrderSumWithoutDiscountAndTax);
    };

    self.discountRetail = ko.observable().extend({ numeric: 0 }); ;
    self.additionalDiscount = ko.observable().extend({ numeric: 2 }); ;

    self.totalWithoutDiscountAndLabor = ko.observable();
    self.effortSum = ko.observable();

    self.inspectionsData = ko.observable();

//    self.customerName = ko.computed(function () {
//        var name = self.customerInfo();
//       // var type = self.customerType();

////        if (type == 'Retail') {
////            var retail = self.customerRetailInfo();
////            name = retail != undefined ? retail.fullName() : '';
////        }

//        return name;
//    });
    self.carInfo = ko.observable();
    self.carName = ko.computed(function () {
        return self.carInfo() != undefined ? self.carInfo().fullName() : '';
    });

//    self.hasEstimateSignature = ko.observable();
    self.hasEstimateSignature = ko.observable(window.globals.currentCustomer.EstimateSignature);

  //  self.hasOrderSignature = ko.observable();

    self.total = ko.observable();

    self.orderSum = ko.observable();

    self.getValue = function (value) {
        var newvalue = parseFloat(value);
        return isNaN(newvalue) ? 0 : newvalue;
    };

    self.totalOrder = ko.computed(function () {
        var effortSum = self.getValue(self.effortSum());
        var totalOrder = self.getValue(self.orderSum());
        var retailDiscount = self.getValue(self.discountRetail());
        var addDiscount = self.getValue(self.additionalDiscount());
        var discount = self.getValue(self.discount());
        var labor = self.getValue(self.laborRate());
        var workByThemselve = self.workByThemselve();
        if (!(discount > 0)) {
            discount = retailDiscount;
        }

        if (workByThemselve) {
            totalOrder -= effortSum;
        }

        totalOrder -= addDiscount;

        var disc = totalOrder * discount * 0.01;
        totalOrder -= disc;
        var labr = totalOrder * labor * 0.01;
        totalOrder += labr;
        totalOrder = Math.round(totalOrder * 100) / 100;

        return totalOrder;
    });

    self.totalEstimate = ko.observable();

    self.totalView = ko.computed(function () {
        var total = parseFloat(self.total());
        var totalEstimate = parseFloat(self.totalEstimate());
        var totalOrder = parseFloat(self.totalOrder());
//        var order = self.order();

//        return order
//            ? (isNaN(totalOrder) || totalOrder == 0 ? '' : ' - $' + totalOrder.toFixed(2))
//            : (isNaN(total) || total == 0 ? isNaN(totalEstimate) || totalEstimate == 0
//                                                ? ''
//                                                : ' - $' + totalEstimate.toFixed(2)
        //                                          : ' - $' + total.toFixed(2));

       return (isNaN(total) || total == 0 ? isNaN(totalEstimate) || totalEstimate == 0
                                                ? ''
                                                : ' - $' + totalEstimate.toFixed(2)
                                                  : ' - $' + total.toFixed(2));
    });

    self.grandTotalView = ko.computed(function () {
        var total = self.totalView();
        if (total == '') {
            total = '$0.00';
        }
        else {
            total = total.substring(3);
        }
        return total;
    });

    self.amountStoredPhoto = ko.observable();

    self.amountEstimatePhoto = ko.observable();

    self.amountPhotoView = ko.computed(function () {
        var total = 0;
//        var order = self.order();
        var stored = self.amountStoredPhoto();
        total += parseInt(stored);
        var photo = self.amountEstimatePhoto();
//        if (order) {
//            if (isNaN(total)) {
//                total = 0;
//            }

//            total += parseInt(photo);
//        }

        return isNaN(total) || total == 0 ? '' : ' - ' + total;
    });

    self.validationCustomLines = function (lineText, linePrice) {
        $(linePrice).each(function () {
            var i = arguments[0];

            if ($(lineText[i]).valid()) {
                $.validator.unobtrusive.parse(document);
                $("form").validate();
            }
        });
    };
    self.unknownCar = ko.observable(true);
    self.vehicleType = ko.observable();

    self.vehicleType.subscribe(function () {
        var type = self.vehicleType();
        var partsNames   = self.carInspection().partsNames();
        if (partsNames.length == 19) {
            if (type == "Truck") {
                partsNames[7].visibleSection(true);
                partsNames[10].visibleSection(true);
            } else {
                partsNames[7].visibleSection(false); //false
                partsNames[10].visibleSection(false);
                self.nullingTruckSection();
                self.carInspection().goToParts(self.carInspection().parts()[0]);
            }
        }
    });

    self.focusOut = function (dat, event) {
        var curval = $(event.currentTarget).val();
        var val = isNaN(parseFloat(curval)) || curval == 0 ? '' : parseFloat(curval) + '';
        $(event.currentTarget).val(val);
    };

    self.nullingTruckSection = function () {
        var partsNames = self.carInspection().partsNames();
        var inspections = self.inspectionsData();
        var a = [7, 10];
        for (var h = 0, l = a.length; h < l; h++) {
            inspections[a[h]].DentsCost = null;
            inspections[a[h]].OversizedDents = null;
            inspections[a[h]].OptionsPercent = null;
            inspections[a[h]].TotalDents = -1;
            inspections[a[h]].AverageSize = -1;
            inspections[a[h]].DoubleMetal = false;
            inspections[a[h]].Aluminium = false;
            inspections[a[h]].AmountOversizedDents = null;
            inspections[a[h]].CorrosionProtection = false;
            inspections[a[h]].CorrosionProtectionCost = null;
            inspections[a[h]].CustomCarInspectionLines = [];
            partsNames[a[h]].isChanges(1);
            partsNames[a[h]].corrosionProtectionCost('');
            partsNames[a[h]].dentsCost('');
            partsNames[a[h]].optionsPercent('');
            partsNames[a[h]].oversizedDentsCost('');
            partsNames[a[h]].customCarInspectLines([]);
            partsNames[a[h]].totalCustLines(0);

        }

        var parts = self.carInspection().parts();

        for (var i = 0, len = parts.length; i < len; i++) {
            if (parts[i].name == "Left cab corner" || parts[i].name == "Right cab corner") {
                parts[i].selectedAverageSize(-1);
                parts[i].selectedTotalDents(-1);
                parts[i].amountOversizedDents('');
                parts[i].Aluminium(false);
                parts[i].DoubleMetal(false);
                parts[i].CorrosionProtection(false);
                var name = parts[i].name == "Left cab corner" ? 10 : 7;
                parts[i].customLines([new CustomCarInspectionLine({ Cost: 0, Name: "" }, 'CarInspectionsModel.CarInspections[' + name + '].CustomCarInspectionLines', 0)]);
            }
        }

        $("select[name='CarInspectionsModel.CarInspections\[10\].AverageSize']").selectBox('value', -1);
        $("select[name='CarInspectionsModel.CarInspections\[7\].AverageSize']").selectBox('value', -1);
        $("select[name='CarInspectionsModel.CarInspections\[10\].TotalDents']").selectBox('value', -1);
        $("select[name='CarInspectionsModel.CarInspections\[7\].TotalDents']").selectBox('value', -1);
    };
};