$(document).ready(function () {
    $('#VIN, #Stock, #CustRO').bind('keyup', function (e) {
        if (e.keyCode == 13 || e.which == '13') {
            $('#StartSearch').click();
        }
    });

    $('#VIN').keyup(function () {
        var $this = $(this);
        if ($this.val().length > 6) {
            $this.val($this.val().substr(0, 6));
        }
        if ($this.val().length == 0) {
            $('#vinHad').removeClass("black");
            $('#vinHad').addClass("gray");
            $this.removeClass("input-validation-error");
            $this.qtip('destroy');
        }
        if ($this.val().length == 6) {
            $this.removeClass("input-validation-error");
            $this.qtip('destroy');
        }
    });

    $('#CustRO').keyup(function () {
        var $this = $(this);
        if ($this.val().length > 20) {
            $this.val($this.val().substr(0, 20));
        }
        if ($this.val().length == 0) {
            $('#custROHad').removeClass("black");
            $('#custROHad').addClass("gray");
        }
    });

    $('#Stock').keyup(function () {
        var $this = $(this);
        if ($this.val().length > 20) {
            $this.val($this.val().substr(0, 20));
        }
        if ($this.val().length == 0) {
            $('#stockHad').removeClass("black");
            $('#stockHad').addClass("gray");
        }
    });
});

var GetParamForSearch = function () {
    var self = this;
    self.serchScope = {
        vin: $('#VIN').val(),
        stock: $('#Stock').val(),
        custRO: $('#CustRO').val()
    };

    self._changeClass = function(selector) {
        $(selector).removeClass("gray");
        $(selector).addClass("black");
    };

    self._isNullOrEmptyString = function (value, param) {
        var res = value != null && value != '';
        return param != undefined ? res && value != param : res;
    };

    self.param = '';
    self.param += "&vin=";
    if (self._isNullOrEmptyString(self.serchScope.vin, 'Last 6 characters')) {
        self.param += self.serchScope.vin;
        self._changeClass('#vinHad');
    }
    self.param += "&stock=";
    if (self._isNullOrEmptyString(self.serchScope.stock)) {
        self.param += self.serchScope.stock;
        $('#stockHad').removeClass("gray");
        $('#stockHad').addClass("black");
    }
    self.param += "&custRo=";
    if (self._isNullOrEmptyString(self.serchScope.custRO)) {
        self.param += self.serchScope.custRO;
        $('#custROHad').removeClass("gray");
        $('#custROHad').addClass("black");
    }
    return param;
};

var atLeastOneField = function () {
    var vin = $('#VIN').val();
    var stock = $('#Stock').val();
    var custRO = $('#CustRO').val();
    var result = (vin != null && vin != '') || (stock != null && stock != '') || (custRO != null && custRO != '');
    return result;
};

var isValidVin = function () {
    var vin = $('#VIN').val();
    var result = (vin != null && (vin.length == 0 || vin.length == 6 || vin == "Last 6 characters"));
    if (!result) {
        $('#VIN').addClass('input-validation-error').qtip({
            content: 'VIN must necessarily contain the last 6 characters!',
            position: {
                corner: {
                    target: 'topRight',
                    tooltip: 'bottomLeft'
                }
            },
            show: 'mouseover',
            hide: 'mouseout',
            style: 'pdrstyle'
        });
    } else {
        $("#VIN").removeClass("input-validation-error");
        if ($("#VIN").qtip()) {
            $("#VIN").qtip('destroy');
        }
    }
    return result;
};

var isValidSearchForm = function () {
    $("#VIN").removeClass("input-validation-error");
    if (!atLeastOneField()) {
        jAlert("You must fill in at least one field!", "Warning!");
        return false;
    }
    var vin = $('#VIN').val();
    if (vin != null && vin != '' && !isValidVin()) {
        return false;
    }
    return true;
};

var resetSearch = function () {
    $('#VIN').attr('value', '');
    $('#Stock').attr('value', '');
    $('#CustRO').attr('value', '');
    $('#vinHad').removeClass("black");
    $('#vinHad').addClass("gray");
    $('#stockHad').removeClass("black");
    $('#stockHad').addClass("gray");
    $('#custROHad').removeClass("black");
    $('#custROHad').addClass("gray");
    $("#VIN").removeClass("input-validation-error");
};
