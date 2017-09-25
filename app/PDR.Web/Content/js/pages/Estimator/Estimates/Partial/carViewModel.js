function CarViewModel(parent) {
    var self = this;
    self.parent = parent;
    self.make = ko.observable($('.estimate-car-info-make').val());
    self.model = ko.observable($('.estimate-car-info-model').val());
    self.year = ko.observable($('.estimate-car-info-year').val());
    self.vin = ko.observable($('.estimate-car-info-vin').val());

    self.fullName = ko.computed(function () {

        var year = self.year();
        var make = self.make();
        var model = self.model();
        var str = '';
        if ((year != undefined && year != '') && (model != undefined && model != '') && (make != undefined && make != '')) {
            str = ' - ' + year + '/' + make + '/' + model;
        }
        else if ((year != undefined && year != '') && (model != undefined && model != '') && (make == undefined || make == '')) {
            str = ' - ' + year + '/' + model;
        }
        else if ((year != undefined && year != '') && (model == undefined || model == '') && (make != undefined && make != '')) {
            str = ' - ' + year + '/' + make;
        }
        else if ((year == undefined && year == '') && (model != undefined && model != '') && (make != undefined && make != '')) {
            str = ' - ' + make + '/' + model;
        }
        else if ((year != undefined && year != '') && (model == undefined || model == '') && (make == undefined || make == '')) {
            str = ' - ' + year;
        }
        else if ((year == undefined || year == '') && (model != undefined && model != '') && (make == undefined || make == '')) {
            str = ' - ' + model;
        }
        else if ((year == undefined || year == '') && (model == undefined || model == '') && (make != undefined && make != '')) {
            str = ' - ' + make;
        }

        return str;
    });

    self.year.subscribe(function () {
        self.carModel(null);
    });
    
    self.make.subscribe(function () {
        self.carModel(null);
    });
    
    self.model.subscribe(function () {
        self.carModel(null);
    });


    self.carModel = function (callback) {
        var year = self.year();
        var make = self.make();
        var model = self.model();
        var id = $('.carId').val();
        var url = window.globals.UrlGetEffortHours;
        var regYear = /^\d{4}$/gi;
        if (regYear.test(year) && make.length > 0 && model.length > 0) {
            $.ajax({
                type: "POST",
                data: { year: year, make: make, model: model, id: id },
                url: url,
                beforeSend: function () {
                    $loader.show();
                },
                complete: function () {
                    $loader.hide();
                },
                success: function (data) {
                    if (data == null) {
                        jAlert('Not data about car', 'Warning!', function () {
                            window.globals.defaultCar = "True";
                            callback();
                        });
                    }
                    else {
                        if (data.CarMake.toLowerCase() == 'DefaultCar'.toLowerCase()) {
                            window.globals.defaultCar = "True";
                        }
                        else {
                            window.globals.defaultCar = "False";
                        }
                        viewModel.unknownCar(false);
                        $("#CarInfo_Type").selectBox('value', data.CarType);
                        viewModel.vehicleType(data.CarType);

                        if (data.Data != null) {
                            self.parent.sections = data.Data;

                            self.parent.updateEfforts();
                            if (callback != null) {
                                callback();
                            }
                            else {
                                self.parent.setNewSectionCost();
                            }
                        }
                    }
                }
            });
        }
    };

    self.vin.subscribe(function (newValue) {
        var $vin = $('.estimate-car-info-vin');
        var $qtip = $('.qtip');
        var $alert = $('.alert');
        var $check = $('.check');
        var $make = $('.estimate-car-info-make');
        var $year = $('.estimate-car-info-year');
        var $model = $('.estimate-car-info-model');

        if (window.globals.state != 'view') {

            //$vin.bind('textchange', function (event, previousText) {
            var vin = newValue.toUpperCase();
            self.vin(vin);

            var reg = /^[^OoIiQq\W_]*$/gi;
            $alert.css('border-color', '#dddddd');
            $check.css('border-color', '#dddddd');
            $qtip.remove();
            self.year('');
            self.make('');
            self.model('');

            if (vin.length == 0) {
                $vin.removeClass("alert").css('border-color', '#dddddd');
                $vin.removeClass("input-validation-error");
                $qtip.remove();
            }
            else {
                $vin.removeClass('check').addClass('alert').css('border-color', '#cd0a0a');

                if (vin.length == 17 && reg.test(vin)) {

                    $.ajax({
                        url: $("#url1").val(),
                        type: 'POST',
                        data: { vincode: vin },
                        beforeSend: function () {
                            $loader.show();
                        },
                        complete: function () {
                            $loader.hide();
                        },
                        success: function (result) {
                            if (result.IsExistDocument == true) {
                                jCustomConfirm(result.Message, 'Warning!', function (r) {
                                    if (r) {
                                        $('#IsExistVin').val(true);
                                    } else {
                                        var urlRedirect = window.globals.UrlIndexEstimate;
                                        window.location.assign(urlRedirect);
                                    }
                                },"Yes", "No");
                            }
                            var response = result.VinInfo;
                            if (response != null) {
                                $vin.removeClass('alert').addClass('check').css('border-color', '#15cd0a');
                                self.year(response.Year);
                                self.make(response.Make);
                                self.model(response.Model);
                                self.carModel(null);
                                if ($year.hasClass('input-validation-error')) {
                                    $year.removeClass('input-validation-error');
                                }
                                if ($make.hasClass('input-validation-error')) {
                                    $make.removeClass('input-validation-error');
                                }
                                if ($model.hasClass('input-validation-error')) {
                                    $model.removeClass('input-validation-error');
                                }
                            }
                            else {
                                $vin.removeClass('check').addClass('alert').css('border-color', '#cd0a0a');
                                $alert.qtip({
                                    content: 'not found',
                                    style: "pdrinvalidstyle"
                                });
                            }
                        },
                        error: function () {
                        }
                    });
                }
            }
            //});
        }
    });

//    self.setupEffort = function () {
//        $('.estimate-car-info-year').bind('change', function () { self.carModel(null); });
//        $('.estimate-car-info-make').bind('change', function () { self.carModel(null); });
//        $('.estimate-car-info-model').bind('change', function () { self.carModel(null); });
//    };
}