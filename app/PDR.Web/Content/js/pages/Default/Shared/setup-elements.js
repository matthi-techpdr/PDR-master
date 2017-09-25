//@reference ~/Content/js/plugins/jquery.combobox.js
//@reference ~/Content/js/plugins/jquery.selectBox.js

jQuery(function () {
    //jQuery('#accordion > div').accordion({ autoHeight: false, collapsible: true });
    // jQuery(document).ready(function () {

    var isiPad = navigator.userAgent.match(/iPad/i) != null;
    var isiOS5 = navigator.userAgent.match(/10(A|B)*/i) != null;
    var isSafari = navigator.userAgent.match(/Safari*/i) != null;

    setupCombobox();
    if (isiPad) {
        $('head').append('<link href="/Content/css/iPad-styles.css" type="text/css" rel="stylesheet">');

        var selects = $('select').not('.multiselect').not('.combobox');
        $.each(selects, function () {
            $(this).wrap('<div class="wrapperBlock"></div>');
        });
        selects.after('<span class="selectBox-arrow"></span>');

        var td = $('#pager_right table tbody').find('tr:first-child').find('td:last-child');
        $('#pager_right table tbody tr').remove('td:last-child');
        $('#pager_right table tbody').find('tr:first-child').find('td:first-child').before(td);
        //$('#pager_right table tbody').find('tr:first-child').find('td:first-child').append('<span class="pagerSel selectBox-arrow"></span>');
//        if (!isiOS5) {
//            $('#filelist1').show();
//            $('#container').find('a').hide();
//        }
 //       changeStyleiPad('#Customers', 22);
 //       changeStyleiPad('#Teams', 14);
    }
    else {
        $('select').not('.multiselect').not('.combobox').selectBox();
        if (isSafari) {
            $('#Customer_Retail_State').parent().find('a').css('width', '55px');
            $('#EstimateModel_Customer_Retail_State').parent().find('a').css('width', '55px');
            $('#CarInfo_Type').parent().find('a').css('width', '78px');
            $('#EstimateModel_CarInfo_Type').parent().find('a').css('width', '78px');
            $('#CarInfo_State').parent().find('a').css('width', '55px');
            $('#EstimateModel_CarInfo_State').parent().find('a').css('width', '55px');
        }
    }

    $('#pager_right table tbody tr td[dir="ltr"]').last().prepend('<span class="showLabel">Show:</span>').css('text-align', 'right');
    //$('.from').not("[readonly='readonly']").datepicker({ maxDate: "+0D" });
    $(function () {
        $(".fromDate").datepicker({
            maxDate: "+0D",
            //defaultDate: "+1w",
            //changeMonth: true,
            numberOfMonths: 1,
            onSelect: function (selectedDate) {
                $(".toDate").datepicker("option", "minDate", selectedDate);
            },
            beforeShow: function (input, inst) {
                var readonly = input.hasAttribute('readonly');
                if (readonly) {
                    inst.dpDiv = $('<div style="display: none;"></div>');
                }
            }
        });
        $(".toDate").datepicker({
            //defaultDate: "+1w",
            maxDate: "+0D",
            //changeMonth: true,
            numberOfMonths: 1,
            onSelect: function (selectedDate) {
                $(".fromDate").datepicker("option", "maxDate", selectedDate);
            },
            beforeShow: function (input, inst) {
                var readonly = input.hasAttribute('readonly');
                if (readonly) {
                    inst.dpDiv = $('<div style="display: none;"></div>');
                }
            }
        });
    });
    selectActiveLinkInMenu();
    handleUnauthorizedAjaxRequest();
    addContainMethodToArray();
    addRemoveMethodToArray();
    setTrimForIE();
    setIndexOfIE();
    addFirstMethodToArray();
    addLastMethodToArray();
    setCustomsValidationRules();
    getHexColorSetup();
    $.ajaxSetup({ async: false });
    $('[placeholder]').placeholder();
    
    $.event.trigger('onAfterSetup'); //always must be in the end.
});

function changeStyleiPad(selector, length) {
    var opt = $(selector).find('option');
    $.each(opt, function () {
        var elem = this;
        var val = $(elem).text();
        if (val.length > length) {
            val = val.substring(0, length) + '...';
            $(elem).text(val);
        }
    });
}

function setupCombobox() {
    $(".combobox").not('.select-wholesale').not('.select-employee').combobox();
    $("#toggle").click(function () {
        $(".combobox").toggle();
    });
}

function selectActiveLinkInMenu() {
    var currentUrl = window.location.pathname;
    var exactPath = "div.links a[href='" + currentUrl + "']";
    var pathwithParameters = "div.links a[href^='" + currentUrl + "?']";
    var selectLink = $(exactPath+", "+pathwithParameters);
    if (selectLink.length == 0) {
        selectLink = $("div.links a.default");
    }
    
    selectLink.addClass("active");
}

function handleUnauthorizedAjaxRequest() {
    $(function () {
        $(document).ajaxError(function (e, xhr, settings) {
            if (xhr.status == 401) {
                var company = location.pathname.split('/')[1];
                window.location = location.protocol + '//' + location.host + '//' + company;
            }
        });
    });
}

function addContainMethodToArray() {
    Array.prototype.contains = function (obj) {
        var i = this.length;
        while (i--) {
            if (this[i] === obj) {
                return true;
            }
        }
        return false;
    };
}

function addRemoveMethodToArray() {
    Array.prototype.remove = function (item) {
        var index = this.indexOf(item);
        this.splice(index, 1);
    };
}

function addLastMethodToArray() {
    Array.prototype.last = function () {
        return this[this.length - 1];
    };
}

function addFirstMethodToArray() {
    Array.prototype.first = function () {
        return this[0];
    };
}

function setCustomsValidationRules() {
    $.validator.addMethod("onlyNumbers", function (value, element) {
        return this.optional(element) || /^[0-9]{0,4}\.?[0-9]{0,2}$/i.test(value);
    }, "Can enter only numbers.");

    $.validator.addMethod('isNotGretterCurrentYear', function (value, element) {
        var currentYear = new Date().getFullYear()+5;
        return this.optional(element) || (parseInt(value) <= currentYear && parseInt(value) > 1900);
    }, "You can enter only values from 1901 to current year + 5");

    $.validator.addMethod('onlyLetters', function (value, element) {
        return this.optional(element) || /^[A-za-z\. ]$/i .test(value); 
    }, "Can enter only letters, dot and escape");

    $.validator.addMethod('notSelectedLocation', function (value, element) {
        return this.optional(element) || (value != null && value.length > 0 && value != "0");
    }, "You must select any Option.");

    $.validator.addMethod('notEmptyField', function (value, element) {
        var elem = element.parentElement;
        if ($(element).hasClass('mainLineText')) {
            elem = $(elem).find('.small');
        } else {
            elem = $(elem).find('.priceLine');
        }

        var val = parseFloat($(elem).val());
        var val1 = isNaN(val) ? '' : val.toString();
        val = isNaN(val) ? '0' : val.toString();

        if (value.length > 0 && val.length >= 0) {
            return true;
        }
        if (value.length == 0 && val1.length == 0) {
            return true;
        }

        return false;
    }, "Field can't be empty");

    $.validator.addMethod('notEmptyFieldSupplement', function (value, element) {
        var elem = $(element.parentElement).find('.sum');
        var flag = false;
        var val = parseFloat($(elem).val());
        var val1 = isNaN(val) ? '' : val.toString();
        val = isNaN(val) ? '0' : val.toString();

        if (value.length > 0 && val.length >= 0) {
            flag = true;
        }
        if (value.length == 0 && val1.length == 0) {
            flag = true;
        }

        return flag;
    }, "Field can't be empty");

    $.validator.addMethod('maxLengthDescription', function (value, element) {
        var flag = true;

        if (value.length > 100) {
            flag = false;
        }

        return flag;
    }, "Please enter no more than 100 characters.");

    $.validator.addMethod('changeSum', function (value, element) {
        var descrip = $(element.parentElement).find('.description');

        if (!$(descrip).valid()) {
            $.validator.unobtrusive.parse(document);
            $("form").validate();
        }
        return true;
    });

    $.validator.addMethod('incorrectFormat', function (value, element) {
        var s = /^[^ ]/i.test(value);
        var e = /[^ ]$/i.test(value);
        return value.length > 0 ? s && e : true;
    }, 'Incorrect data format');

    $.validator.addMethod('yearToNotMustGretterThenYearFrom', function (value, element) {
        var to = parseInt(value);
        var from = parseInt($('#Info_YearFrom').val());
        return to >= from;
    }, "Year 'To' mustn't be less than year 'From'");

    $.validator.addMethod('customerandMatrixRequire', function (value, element) {
        var flag = true;
        if (value == null || value == '') {
            flag = false;
        }

        return flag;
    }, "Field can't be empty");

    $.validator.addMethod('LessThen', function (value, element) {
        var to = value !== '' ? parseInt(value) : 0;
        return to <= 100;
    }, "Discount mustn't be equals or less then 100%.");

    $.validator.addMethod('AddDiscountCannotMoreThenSumRO', function (value, element) {
        var val = parseInt($(".grandTotal").text().replace("$", ""));
        return val > 0;
    }, "Discount mustn't be a cause of negative Total repair order sum.");

    $.validator.addMethod('isChoose', function (value, element) {
        if (value.toLowerCase().indexOf("choose") == -1) {
            $('.isChoose').css("border-color", "");
            return true;
        }
        
        $('.isChoose').css("border-color", "red");
        return false;
    }, "Owner Name field is required");
}

function setTrimForIE() {
    if (typeof String.prototype.trim !== 'function') {
        String.prototype.trim = function() {
            return this.replace( /^\s+|\s+$/g , '');
        };
    }
}

function setIndexOfIE() {
    if (!('indexOf' in Array.prototype)) {
        Array.prototype.indexOf = function (find, i /*opt*/) {
            if (i === undefined) i = 0;
            if (i < 0) i += this.length;
            if (i < 0) i = 0;
            for (var n = this.length; i < n; i++)
                if (i in this && this[i] === find)
                    return i;
            return -1;
        };
    }
}

function getHexColorSetup(){
    $.fn.getHexColor = function () {
        var rgb = $(this).css('background-color');
        rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
        return "#" +
            ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
            ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
            ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2);
    };
}

function ScrollTo(jEl) {
    $("html, body").scrollTop($(jEl).offset().top);
}

function ScrollToBottom() {
    $("html, body").scrollTop($(document).height());
}

function DisableFormSubmissionByEnter() {
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            return false;
        }
    });
}