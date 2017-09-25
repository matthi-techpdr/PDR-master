//@reference ~/Content/js/knockout.js

//================BindingHandlers=====================================
ko.bindingHandlers['class'] = {
    'update': function (element, valueAccessor) {
        if (element['__ko__previousClassValue__']) {
            ko.utils.toggleDomNodeCssClass(element, element['__ko__previousClassValue__'], false);
        }
        var value = ko.utils.unwrapObservable(valueAccessor());
        ko.utils.toggleDomNodeCssClass(element, value, true);
        element['__ko__previousClassValue__'] = value;
    }
};

//================= Extend ============================================

ko.extenders.numeric = function (target, precision) {
    var regDot = /^\.$/;
    var regDotLast = /\.$/;
    var regOneNull = /^[0]{1}$/;
    var regDotWithNullLast = /(\.[0]{1})$/;
    var regDotWithNull = /^(\d+)?(\.{1})?(\d{1,2})?$/;
    var regPrecisionNull = /^\d+$/;

    var result = ko.computed({
        read: target,
        write: function (newValue) {

            var current = target();
            var valueToWrite = "";

            if (precision == 2) {

                if (!regDotWithNull.test(newValue)) {
                    var roundingMultiplier = Math.pow(10, precision);
                    var newValueAsNum = isNaN(parseFloat(newValue)) ? "" : parseFloat(newValue);
                    if (newValueAsNum !== "") {
                        valueToWrite = (newValueAsNum === 0) ? "" : ((Math.floor(newValueAsNum * roundingMultiplier) / roundingMultiplier) + '');
                    }
                }
                else {
                    if (regDot.test(newValue)) {
                        valueToWrite = 0 + newValue;
                    }
                    else if (regDotWithNullLast.test(newValue)) {
                        valueToWrite = newValue;
                    }
                    else if (regOneNull.test(newValue)) {
                        valueToWrite = "";
                    }
                    else if (regDotLast.test(newValue)) {
                        valueToWrite = newValue;
                    }
                    else {
                        valueToWrite = isNaN(parseFloat(newValue)) ? newValue : parseFloat(newValue) + '';
                    }
                }
            }
            else if (precision == 0) {
                valueToWrite = !regPrecisionNull.test(newValue) ? '' : regOneNull.test(newValue) ? "" : parseFloat(newValue)+''; //isNaN(parseFloat(newValue)) ? '' : parseFloat(newValue)
            }

            if (valueToWrite !== current) {
                target(valueToWrite);
            } else {
                if (newValue !== current) {
                    target.notifySubscribers(valueToWrite);
                }
            }
        }
    });

    result(target());

    return result;
};