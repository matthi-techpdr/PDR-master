function InsuranceViewModel(parent) {
    var self = this;

    self.parent = parent;
    self.insurance = $('#insurance');
    self.fieldset = $('.fieldset11');
    self.insuranceInputs = self.insurance.find('input[type="text"]');
    self.insuranceCompanyNames = $('#Insurance_CompanyNames');

    self.insuranceHide = function () {
        self.insurance.hide().attr('disabled', 'disabled');
        self.fieldset.hide();
        self.insuranceInputs.attr('disabled', 'disabled');
        self.insuranceCompanyNames.attr('disabled', 'disabled');
        $('input[name="Insurance.CompanyName"]').attr('disabled', 'disabled');
        window.globals.hasInsurance = "False";
    };

    self.insuranceShow = function () {
        self.insurance.show().removeAttr('disabled');
        self.fieldset.show();
        self.insuranceInputs.removeAttr('disabled');
        self.insuranceCompanyNames.removeAttr('disabled');
        $('input[name="Insurance.CompanyName"]').removeAttr('disabled');
        window.globals.hasInsurance = "True";
    };

    self.checkWCustomersForInsurance = function (select) {
        var customers = $(select);

        if (customers.data("checkInsurance").contains(customers.val())) {
            self.insuranceShow();
        }
        else {
            self.insuranceHide();
        }
    };
}