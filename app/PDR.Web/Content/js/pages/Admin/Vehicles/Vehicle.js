//@reference ~/Content/js/plugins/jqueryui/jquery.ui.js
//@reference ~/Content/js/plugins/jqueryui/jquery.ui.widget.js
//@reference ~/Content/js/plugins/jquery.multi-accordion.js
//@reference ~/Content/js/plugins/jquery.selectBox.js
/*global window.globals.isReadOnlyMode = @Html.IsReadOnlyMode().ToString().ToLower();*/
var $loader;

$().ready(function () {
    $(document).bind('onAfterSetup', function () {
        $.ajaxSetup({ async: true });

        $("#accordion").multiAccordion({ active: '0' });
        $("#tabsSections").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
        $("#tabsSections li").removeClass("ui-corner-top").addClass("ui-corner-left");
        $loader = $('#loader');

        var value = $("#Info_VehicleType").val();
        if (value.toLowerCase() == "Car".toLowerCase()) {
            $('.tabsSections-8').addClass('hideTab');
            $('.tabsSections-11').addClass('hideTab');
        }

        $('#Info_VehicleType').bind("change", function () {
            value = $(this).val();
            if (value.toLowerCase() == "Car".toLowerCase()) {
                $("#tabsSections").find('li').removeClass('ui-tabs-selected ui-state-active');
                $('.tabsVehicle').addClass('ui-tabs-hide');
                $('.tabsSections-1').addClass('ui-tabs-selected ui-state-active');
                $('.tabsSections-8').addClass('hideTab');
                $('.tabsSections-11').addClass('hideTab');
                $('#tabsSections-1').removeClass('ui-tabs-hide');
                $('#tabsSections-8').find('input[type="text"]').val('');
                $('#tabsSections-11').find('input[type="text"]').val('');
            }
            else {
                $('.tabsSections-8').removeClass('hideTab');
                $('.tabsSections-11').removeClass('hideTab');
            }
        });

        UniqueVehicleBinding();

        $.validator.addMethod("onlyLetters", function (value, element) {
            return this.optional(element) || /^[a-zA-z]+([ ]?[A-Za-z]+)+$/i.test(value);
        }, "Can enter only letters and spaces");

        var targetInput = $('fieldset.fieldset4 input.ui-autocomplete-input');
        targetInput.attr('name', 'Info.Make')
            .addClass('required')
            .attr('maxlength', '50');

        var button = $('button[title="Show All Items"]');
        button.addClass(navigator.userAgent.match(/iPad/i) != null ? 'buttonForCombobox1' : 'buttonForCombobox');

        if (window.globals.isReadOnlyMode) {
            if (window.globals.isReadOnlyMode != 'false') {
                targetInput.attr("readonly", "readonly");
                button.attr("disabled", "disabled");
            }
        }
        else {
            $("form").validate();
        }
    });

});

function UniqueVehicleBinding() {
    $('input[name="Info.Make"]').bind('change', function () { UniqueVehicle(); });
    $('#Info_Model').bind('change', function () { UniqueVehicle(); });
    $('#Info_YearFrom').bind('change', function () { UniqueVehicle(); });
    $('#Info_YearTo').bind('change', function () { UniqueVehicle(); });
}

function UniqueVehicle(callback) {
    var make = $('input[name="Info.Make"]').val();
    var model = $('#Info_Model').val();
    var from = $('#Info_YearFrom').val();
    var to = $('#Info_YearTo').val();
    var id = $('#Info_Id').val();
    var url = $("#urlUniqueVehicle").val();

    if (make != '' && model != '' && from != '' && to != '') {
        Helpers.SendAjax(
            "GET",
            url,
            'make=' + make + '&model=' + model + '&from=' + from + '&to=' + to +"&id=" + id,
            false,
            function (data) {
                if (data) {
                    jAlert("This vehicle is already exists in the database", "Warning!", function() {
                        window.globals.uniqueVehicle = true;
                    });
                }
                else {
                    window.globals.uniqueVehicle = false;
                }
                if (callback) {
                    callback();
                }
            });
    }
    else {
        if (callback) {
            callback();
        }
    }
}