// global window.repairOrderSupplementTemplate

$(function () {
    var addNewTemplate;

    function reCalculateSum() {

        var $sum = $('.repair-order-summury .total-sum input[type="text"]');
        var $newsum = $('#NewSum');
        var newsum = $newsum.val();

        //validationTextChange($newsum, newsum, 2);

        var estimateSum = parseFloat($('.totalWithoutDiscountAndLabor').val());

        newsum = isNaN(newsum) || newsum === '' ? 0 : +newsum;
        newsum += estimateSum;

        var supplementSum = newsum;
        var $removeSuppl = $(".repair-orders-supplements li .remove");

        $removeSuppl.each(function () {
            var s = parseFloat($(this).parent().find(".sum").val());
            var sum = isNaN(s) ? 0 : s;
            supplementSum += parseFloat(sum);
        });

        supplementSum = Math.round(supplementSum * 100) / 100;
        viewModel.orderSum(supplementSum);
         
        $sum.val(isNaN(supplementSum) ? '$0' : '$' + supplementSum);
    };

    function remove() {
        // remove item
        var $button = $(this);
        var $itemContainer = $button.parent();
        $itemContainer.remove();

        // update indexes
        reIndex();

        reCalculateSum();
    };

    function reIndex() {
        $('.repair-orders-supplements .remove').each(function (index) {
            var $container = $(this).parent();
            $container.find("input").each(function () {
                var $item = $(this);
                if ($item.attr("name")) {
                    var nameAttr = $item.attr("name");
                    nameAttr = nameAttr.replace(/\[\d+\]/, '[' + index + ']');
                    $item.attr("name", nameAttr);
                }
            });
        });
    };

    function add() {
        var $button = $(this);
        var $item = $button.parent();
        if (!addNewTemplate) {
            addNewTemplate = "<li>" + $item.html() + "</li>";
        }

        //addValidation($item);
        if ($item.find("input").valid()) {

            var index = $(".repair-orders-supplements li").length - 1; // all items except item for creation
            var newItemHtml = window.repairOrderSupplementTemplate.replace(/\[0\]/g, '[' + index + ']').replace(/_0_/g, '_' + index + '_');
            $item.before(newItemHtml);
            var $newButton = $(".repair-orders-supplements li .remove:last");
            $newButton.click(remove);
            var $newItem = $newButton.parent();
            $newItem.find(".description").val($item.find(".description").val());
            $newItem.find(".sum").val($item.find(".sum").val());

            $item.find(".description").val("");
            $item.find(".sum").val("");
            $item.remove();
            $newItem.after(addNewTemplate);
            $('#NewSum').val('');
            $('#NewDescription').val('');
            $(".repair-orders-supplements .new").click(add);

            //addValidation($newItem);
            $('.sum').bind('textchange', function (event, previousText) {
                var currentval = $(event.target).val();
                validationTextChange($(event.target), currentval, 2);
                reCalculateSum();
            });

            reCalculateSum();
        }
    };

    $(".repair-orders-supplements .new").click(add);
    $('.repair-orders-supplements .remove').click(remove);

    function validationTextChange(elem, newValue, precision) {
        var regDot = /^\.$/;
        var regDotLast = /\.$/;
        var regOneNull = /^[0]{1}$/;
        var regDotWithNull = /^(\d+)?(\.{1})?(\d{1,2})?$/;
        var regDotWithNullLast = /(\.[0]{1})$/;
        var current = elem;
        var valueToWrite = "";

        if (!regDotWithNull.test(newValue)) {
            var roundingMultiplier = Math.pow(10, precision);
            var newValueAsNum = isNaN(parseFloat(newValue)) ? "" : parseFloat(newValue);
            
            if (newValueAsNum !== "") {
                valueToWrite = (newValueAsNum === 0) ? "" : ((Math.floor(newValueAsNum * roundingMultiplier) / roundingMultiplier) + '');
            }

            current.val(valueToWrite);
        }
        else {
            if (regDot.test(newValue)) {
                current.val(0 + newValue);
            }
            else if (regDotWithNullLast.test(newValue)) {
                current.val(newValue);
            }
            else if (regOneNull.test(newValue)) {
                current.val('');
            }
            else if (regDotLast.test(newValue)) {
                current.val(newValue);
            }
            else {
                current.val(isNaN(parseFloat(newValue)) ? newValue : parseFloat(newValue) + '');
            }
        }
    };

    $(document).bind("onAfterSetup", function () {
        $('.sum').bind('textchange', function (event, previousText) {
            var currentval = $(event.target).val();
            validationTextChange($(event.target), currentval, 2);
            reCalculateSum();
        });

        $('.sum').live('focusout', function (event) {
            var currentval = $(event.target).val();
            var val = isNaN(parseFloat(currentval)) || currentval == 0 ? '' : parseFloat(currentval) + '';
            $(event.target).val(val);
        });
    });
});