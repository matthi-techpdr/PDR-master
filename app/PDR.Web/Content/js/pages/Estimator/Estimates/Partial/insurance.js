/*global window.globals.isReadOnlyMode = @Html.IsReadOnlyMode().ToString().ToLower();*/

$().ready(function () {
    $(document).bind('onAfterSetup', function () {
        $.validator.addMethod("onlyLetters", function (value, element) {
            return this.optional(element) || /^[a-z A-z]+$/i.test(value);
        }, "Can enter only letters");

        var targetInput = $('fieldset.fieldset3 input.ui-autocomplete-input');
        targetInput.attr('name', 'Insurance.CompanyName')
            .addClass('required')
            .attr('maxlength', '50');

        var button = $('button[title="Show All Items"]');
        button.addClass('buttonForCombobox');

        if (window.globals.isReadOnlyMode) {
            targetInput.attr("readonly", "readonly");
            button.attr("disabled", "disabled");
        }
        else {
            $("form").validate();
        }
    });
});
