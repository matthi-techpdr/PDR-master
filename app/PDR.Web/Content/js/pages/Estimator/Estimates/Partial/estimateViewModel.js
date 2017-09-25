function EstimateViewModel() {
    var self = this;
    self.partsTemp = new PartsTemp();
    self.selectBox = $('select.select-wholesale');
    self.retailCustomer = new CustomerRetailInfo(self);
    self.wholesale = new CustomerWholesaleInfo(self);
    self.inspection = new CarInspectionsViewModel(self);
    self.car = new CarViewModel(self);
    self.insurance = new InsuranceViewModel(self);
    self.effortsModel = new EffortsModel();
    self.sections = [];

    self.initDefaultData = function () {
        $('#matrixId').val(window.globals.defaultMatrix);
        self.partsTemp.remove();

        self.inspection.hourlyRate(window.globals.defaultHourlyRate);
        self.inspection.aluminiumPercent(window.globals.defaultAluminium);
        self.inspection.maxCorProtect(window.globals.defaultMaxCorProtect);
        self.inspection.doubleMetallPercent(window.globals.defaultDoubleMetall);
        self.inspection.oversizedRoofPercent(window.globals.defaultOversizedRoof);
        self.inspection.oversizedDents(window.globals.defaultOversizedDents);
        self.inspection.corProtectPart(window.globals.defaultCorProtect);
        self.inspection.maxPercentPart(window.globals.defaultMaxPercent);

        var parts = self.inspection.parts();
        self.partsTemp.addRange(parts);
    };

    self.initWholesaleCurrentData = function () {
        $('#matrixId').val(window.globals.currentMatrix);
        self.partsTemp.remove();
        
        self.inspection.hourlyRate(parseFloat(window.globals.hourlyRate));
        self.inspection.aluminiumPercent(parseFloat(window.globals.currentAluminium));
        self.inspection.maxCorProtect(parseFloat(window.globals.maxCorProtect));
        self.inspection.doubleMetallPercent(parseFloat(window.globals.currentDoubleMetall));
        self.inspection.oversizedRoofPercent(parseFloat(window.globals.currentOversizedRoof));
        self.inspection.oversizedDents(parseFloat(window.globals.currentOversizedDents));
        self.inspection.corProtectPart(parseFloat(window.globals.currentCorProtect));
        self.inspection.maxPercentPart(parseFloat(window.globals.currentMaxPercent));
        var parts = self.inspection.parts();
        self.partsTemp.addRange(parts);
    };

    self.initAffiliateCurrentData = function () {
        $('#matrixId').val(window.globals.defaultMatrix);
        self.partsTemp.remove();

        self.inspection.hourlyRate(parseFloat(window.globals.hourlyRate));
        self.inspection.aluminiumPercent(parseFloat(window.globals.currentAluminium));
        self.inspection.maxCorProtect(parseFloat(window.globals.maxCorProtect));
        self.inspection.doubleMetallPercent(parseFloat(window.globals.currentDoubleMetall));
        self.inspection.oversizedRoofPercent(parseFloat(window.globals.currentOversizedRoof));
        self.inspection.oversizedDents(parseFloat(window.globals.currentOversizedDents));
        self.inspection.corProtectPart(parseFloat(window.globals.currentCorProtect));
        self.inspection.maxPercentPart(parseFloat(window.globals.currentMaxPercent));
        var parts = self.inspection.parts();
        self.partsTemp.addRange(parts);
    };


    self.setupCustomerTypeChange = function () {
        var isiPad = navigator.userAgent.match(/iPad/i) != null;

        if (isiPad) {
            var selects = self.selectBox;
            $.each(selects, function () {
                $(this).css('height', '28px').css('width', '300px');
                $(this).wrap('<div class="wrapperBlock"></div>');
            });
            selects.after('<span class="selectBox-arrow" style="top:1px;right:-3px;"></span>');
        }
        else {
            self.selectBox.selectBox();
        }

        self.wholesale.wholesaleId.change(function () {
            self.insurance.checkWCustomersForInsurance(this);
            self.wholesale.wholesaleCustomerDataBindChange();
            self.updateData();
        });

        $('#estimate-customer label input[type="radio"]').change(function () {

            var type = $(this).attr('value');
            var $retailState = $('#Customer_Retail_State');
            var $retailInputs = $('#retail-customer input[type="text"]');
            var $affiliate = $('#Customer_Retail_AffiliateId');

            window.globals.type = type;

            if (type == 'Retail') {

                viewModel.customerInfo(' - ' + $retailFirstName.val() + ' ' + $retailLastName.val());
                self.insurance.insuranceShow();
                self.inspection.discount(0);
                self.inspection.laborRate(0);
                //                self.inspection.hourlyRate(window.globals.defaultHourlyRate);
                //                self.inspection.maxCorProtect(window.globals.defaultMaxCorProtect);
               
                self.initDefaultData();
                self.updateCarInspectionsData();
                $retailInputs.removeAttr('disabled');
                $retailState.selectBox('enable');
                $affiliate.selectBox('enable');
                self.wholesale.wholesaleDisable();
                self.wholesale.wholesaleCustomerDataUnBindChange();
                self.retailCustomer.affiliateCustomerDataBindChange();
                viewModel.hasEstimateSignature('True');
            }
            else if (type == 'Wholesale') {
                if (window.globals.order == 'True') {
                    self.wholesale.wholesaleDisable();
                }
                else {
                    self.wholesale.wholesaleEnable();
                }
                var text = $('#Customer_Wholesale_CustomerId :selected').text();
                viewModel.customerInfo(text.length == 0 ? '' : ' - ' + text);
                $retailInputs.attr('disabled', 'disabled');
                $retailState.attr('disabled', 'disabled');
                $affiliate.attr('disabled', 'disabled');
                self.insurance.checkWCustomersForInsurance('#Customer_Wholesale_CustomerId');
                self.wholesale.wholesaleCustomerDataUnBindChange();
                self.wholesale.wholesaleCustomerDataBindChange();
                self.retailCustomer.affiliateCustomerDataUnBindChange();
                self.updateData();

            }
        });
    };

    self.updateCarInspectionsData = function () {
        var temp = self.partsTemp.parts;
        var parts = self.inspection.parts();

        //if (parts.length == 16) {
        for (var i = 0, len = parts.length; i < len; i++) {
            var part = parts[i];

            part.selectedAverageSize(temp[i].averagesize);
            part.selectedTotalDents(temp[i].totaldents);
            part.change();

            part.Aluminium(temp[i].aluminium);
            part.DoubleMetal(temp[i].doublemetal);
            part.OversizedRoof(temp[i].oversizedroof);

            part.amountOversizedDents(temp[i].amountoversizeddents);
            part.oversizedDentsCost(temp[i].oversizeddentscost);
            part.CorrosionProtection(temp[i].corrosionprotection);
            part.CorrosionProtectionCost(temp[i].corrosionprotectioncost);
        }
        //}

        self.partsTemp.remove();
    };

    self.updateData = function () {
        var customerId = self.wholesale.wholesaleId.val();
        var matrixId = self.wholesale.wholesalaMatrixId.val();
        window.globals.currentMatrix = matrixId;
        self.insurance.checkWCustomersForInsurance('#Customer_Wholesale_CustomerId');
        if ((customerId != null || customerId != '0') && (matrixId != null || matrixId != '0')) {
            $.ajax({
                type: 'POST',
                url: window.globals.UrlGetWholesaleCustomerData,
                cache: false,
                data: { customer: customerId, matrix: matrixId },
                beforeSend: function () {
                    $loader.show();
                },
                complete: function () {
                    $loader.hide();
                },
                success: function (data) {
                    if (data) {
                        window.globals.maxCorProtect = data.MaxCorProtect;
                        window.globals.laborRate = data.LaborRate;
                        window.globals.discount = data.Discount;
                        window.globals.hourlyRate = data.HourlyRate;
                        window.globals.currentAluminium = data.Aluminium;
                        window.globals.currentMaxCorProtect = data.MaxCorProtect;
                        window.globals.currentDoubleMetall = data.DoubleMetall;
                        window.globals.currentOversizedRoof = data.OversizedRoof;
                        window.globals.currentOversizedDents = data.OversizedDents;
                        window.globals.currentCorProtect = data.CorProtectPart;
                        window.globals.currentMaxPercent = data.Maximum;

                        self.inspection.discount(data.Discount);
                        self.inspection.laborRate(data.LaborRate);
                        self.inspection.maxCorProtect(data.MaxCorProtect);
                        viewModel.hasEstimateSignature(data.HasEstimateSignature ? 'True' : 'False');

                        self.initWholesaleCurrentData();

                        self.updateCarInspectionsData();    
                    }
                }
            });
        }
    };

    //Update data for Affiliates 
    self.updateAffiliate = function () {
        var affiliateId = self.retailCustomer.affiliateId.val();
        window.globals.currentAffiliate = affiliateId;
        
        if (affiliateId != null || affiliateId != '0') {
            $.ajax({
                type: 'POST',
                url: window.globals.UrlGetAffiliateData,
                cache: false,
                data: { affiliateId: affiliateId },
                beforeSend: function () {
                    $loader.show();
                },
                complete: function () {
                    $loader.hide();
                },
                success: function (data) {
                    if (data) {
                        window.globals.maxCorProtect = data.MaxCorProtect;
                        window.globals.laborRate = data.LaborRate;
                        window.globals.discount = data.Discount;
                        window.globals.hourlyRate = data.HourlyRate;
                        window.globals.currentAluminium = data.Aluminium;
                        window.globals.currentMaxCorProtect = data.MaxCorProtect;
                        window.globals.currentDoubleMetall = data.DoubleMetall;
                        window.globals.currentOversizedRoof = data.OversizedRoof;
                        window.globals.currentOversizedDents = data.OversizedDents;
                        window.globals.currentCorProtect = data.CorProtectPart;
                        window.globals.currentMaxPercent = data.Maximum;

                        self.inspection.discount(data.Discount);
                        self.inspection.laborRate(data.LaborRate);
                        self.inspection.maxCorProtect(data.MaxCorProtect);
                        viewModel.hasEstimateSignature(data.HasEstimateSignature ? 'True' : 'False');


                        self.initAffiliateCurrentData();

                        self.updateCarInspectionsData();
                    }
                }
            });
        }

    };


    self.updateMatrixData = function () {
        var customerId = self.wholesale.wholesaleId.val();
        var text = $('#Customer_Wholesale_CustomerId :selected').text();
        viewModel.customerInfo(text.length == 0 ? '' : ' - ' + text);

        $.ajax({
            type: 'POST',
            url: window.globals.UrlGetWholesaleMatrices,
            data: { customer: customerId },
            beforeSend: function () {
                $loader.show();
            },
            complete: function () {
                $loader.hide();
            },
            success: function (data) {
                var text = '';
                $.each(data, function () {
                    var d = this;
                    text += '<option value="' + d.Value + '">' + d.Text + '</option>';
                });
                self.wholesale.wholesalaMatrixId.find('option').remove();
                self.wholesale.wholesalaMatrixId.append(text);
                self.wholesale.wholesalaMatrixId.selectBox('refresh');
                self.updateData();
            }
        });
    };

    self.initSectionCost = function () {
        for (var i = 0, len = 19; i < len; i++) {
            self.sections[i] = { Cost: 0 };
        }
    };

    self.setNewSectionCost = function () {
        var parts = self.inspection.partsNames();
        for (var i = 0, len = 19; i < len; i++) {
            var part = parts[i];
            if (self.sections.length != 0) {
                if (self.sections[i] != undefined) {
                    part.newsectioncost(self.sections[i].Cost);
                }
                else {
                    part.newsectioncost(0);
                }
            } else {
                part.newsectioncost(0);
            }
        }
    };

    self.updateEfforts = function () {
        for (var i = 0, len = 19; i < len; i++) { // After added Truck's new sections change amount section for loop 

            var efforts = self.sections[i].EffortItems;

            var effortLine = self.effortsModel.effortsArray[i];
            var effortItems = effortLine.effortItems();

            if (effortItems != null) {
                for (var x = 0, lenX = effortItems.length; x < lenX; x++) {

                    effortItems[x].operations(efforts[x].HoursR_I == null ? 1 : efforts[x].HoursR_R == null ? 0 : 0);
                    effortItems[x].hoursR_I(efforts[x].HoursR_I);
                    effortItems[x].hoursR_R(efforts[x].HoursR_R);
                    effortItems[x].id(efforts[x].Id);

                    effortItems[x].effortType(efforts[x].EffortType);
                    effortItems[x].baseEffortType(efforts[x].EffortType);
                    //effortItems[x].currentEffortType(efforts[x].EffortType);
                    effortItems[x].selectedEffortItem(false);
                    if (efforts[x].HoursR_I != null && efforts[x].HoursR_R == null) {
                        effortItems[x].hoursInput(efforts[x].HoursR_I);
                    }
                    else if (efforts[x].HoursR_I == null && efforts[x].HoursR_R != null) {
                        effortItems[x].hoursInput(efforts[x].HoursR_R);
                    }
                    else if (efforts[x].HoursR_I != null && efforts[x].HoursR_R != null) {
                        effortItems[x].hoursInput(efforts[x].HoursR_I);
                    }
                }
            }
        }
    };
}