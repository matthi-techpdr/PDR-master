function CustomerRetailInfo(parent) {
    var self = this;
    self.parent = parent;
    self.affiliateId = $('#Customer_Retail_AffiliateId');

    self.affiliateCustomerDataBindChange = function () {
        self.affiliateId.bind('change', function () { self.parent.updateAffiliate(); });
    };

    self.affiliateCustomerDataUnBindChange = function () {
        self.affiliateId.unbind('change');
    };


    self.firstName = ko.observable();
    self.lastName = ko.observable();

    self.fullName = ko.computed(function () {
        var firstName = self.firstName();
        var lastName = self.lastName();
        var str = '';
        if ((firstName != undefined && firstName != '') && (lastName != undefined && lastName != '')) {
            str = ' - ' + firstName + ' ' + lastName;
        }
        else if ((firstName != undefined && firstName != '') && (lastName == undefined || lastName == '')) {
            str = ' - ' + firstName;
        }
        else if ((firstName == undefined || firstName == '') && (lastName != undefined && lastName != '')) {
            str = ' - ' + lastName;
        }

        return str;
    });
}
$(function () {
    $.validator.addMethod("uniqueEmail", function (value, element) {
        value = value.toLowerCase();
        var emails = [];
        for (var i = 0; i < $(element).data('emails').length; i++) {
            var item = $(element).data('emails')[i];
            emails.push(item.toLowerCase());
        }
        //var result = this.optional(element) || !emails.contains(value);
        var result = this.optional(element) || emails.indexOf(value) < 0;
        return result;
    }, "Email already exist.");
});
