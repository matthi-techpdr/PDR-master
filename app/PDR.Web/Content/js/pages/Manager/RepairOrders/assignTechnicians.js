$(function () {
    var addNewTemplate;
    var isiPad = navigator.userAgent.match(/iPad/i) != null;
    var heightPartialView = 70;
    var heightLi = 33;

    //=============================================== Add =================================================
    var add = function (event) {

        var t = event.target || event.srcElement;
        var elem = t.parentElement.parentElement;
        var $select = $(elem).find('select');
        var $selectedOption = $(elem).find('select option:selected');
        var $selectedOptionValue = $($selectedOption).val();
        var $selectedOptionText = $($selectedOption).text();
        $($selectedOption).remove();

        var $selectTemp = $('.technicians li').find('select');
        if (isiPad) {
            $($selectTemp).removeAttr('id').attr('name', 'TechnicianIds[0]');
        }
        else {
            $($selectTemp).removeAttr('id').attr('name', 'TechnicianIds[0]').selectBox();
        }

        $('.new').find('.ui-icon').addClass('TechnicianIds[0]');
        
        addNewTemplate = '<li>' + $('.technicians li').last().html() + '</li>';

        $($select).after('<input type="text" readonly="readonly" value="' + $selectedOptionText + '" class="readonlyInput"/><input type="hidden" value="' + $selectedOptionValue + '" class="technician"/>');
        $($select).hide();
        if (isiPad) {
            $('.technicians li').find('.wrapperBlock').find('.selectBox-arrow').remove();
            $($select).unwrap();
        }
        else {
            $($select).selectBox('destroy');
        }
        $($select).remove();

        var heightUl = $('#assignTechniciansForm ul').height();

        var index = $('.technicians li').length; // all items except item for creation

        $('.technicians li').last().after(addNewTemplate);
        if (isiPad) {
            $('.technicians li').last().find('select').removeAttr('id').attr('name', 'TechnicianIds[' + index + ']');
        }
        else {
            $('.technicians li').last().find('select').removeAttr('id').attr('name', 'TechnicianIds[' + index + ']')
                .selectBox(); 
        }

        $('.technicians li').last().find('a:last-child').remove();
        $('.new').hide();
        $('.remove').show();
        $('.technicians li').last().find('.remove').hide();
        $('.technicians li').last().find('.new').show();


        $('#assignTechniciansForm ul').height(heightUl + heightLi + 15);
        var $newSelect = $('.technicians li').last().find('select');
        var lengthNewSelect = $($newSelect).find('option').length;
        if (lengthNewSelect == 1) {
            $('.technicians li').last().find('.new').hide();
            var $selectedNewOption = $($newSelect).find('option:selected');
            var $selectedNewOptionValue = $($selectedNewOption).val();
            var $selectedNewOptionText = $($selectedNewOption).text();
            if (isiPad) {
                $('.technicians li').last().find('.wrapperBlock').find('.selectBox-arrow').remove();
                $($newSelect).unwrap();
            }
            else {
                $($newSelect).selectBox('destroy');
            }

            $($newSelect).after('<input type="text" readonly="readonly" value="' + $selectedNewOptionText +
                '"class="readonlyInput" style="margin-left:44px;"/><input type="hidden" value="' + $selectedNewOptionValue + '" class="technician"/>');
            $($newSelect).hide();
            $($newSelect).remove();
        }
        var $curSelect = $('.technicians li').last().find('select').selectBox().val();
        riOperations($curSelect);
    };

    var reindex = function () {
        var sel = $('select.technician');
        $('select.technician').removeAttr('name');
        $.each(sel, function (i) {
            $(this).attr('name', 'TechnicianIds[' + i + ']');
        });
    };
    //=============================================== Remove =================================================
    var checkCountRi = function () {
        var $lastchild = $('.technicians li').last();
        var countRi = 0;
        var all = $('.technicians li input[type="hidden"]');
        for (s = 0; s < all.length; s++) {
            if ($(all[s]).val().split(':')[1] == 'RITechnician') {
                countRi++;
            }
        }
        var $selectedOption = $($lastchild).find('select option:selected');
        var $selectedOptionValue = $($selectedOption).val();
        
        if ($selectedOptionValue.split(':')[1] == 'RITechnician') {
            countRi++;
        }
        if (countRi < 1) {
            var ul = $('#assignTechniciansForm ul').height();
            $('#assignTechniciansForm ul').height(ul - heightPartialView);
            var riDiv = $('#RiPayment');
            $(riDiv).css('visibility', 'hidden');
        }
    };

    var remove = function (event) {
        var t = event.target || event.srcElement;
        var elem = $(t).is('span') ? $(t).parent().parent() : $(t).parent();
        var $input = $(elem).find('input[type="text"]');
        var $inputHidden = $(elem).find('input[type="hidden"]');
        var $inputText = $($input).val();
        var $inputValue = $($inputHidden).val();
        var $lastchild = $('.technicians li').last();

        if ($inputValue.split(':')[1] == 'RITechnician') {
            checkCountRi();
        }
        var $current;
        var $select = $lastchild.find('select');
        var $inputTextLast = $lastchild.find('input[type="text"]');
        var $inputHiddenLast = $lastchild.find('input[type="hidden"]');
        
        if ($select.length != 0) {
            $($select).find('option:last-child').after('<option value="' + $inputValue + '">' + $inputText + '</option>');
            $current = $($select).val();
            Sort($select);
            
            if (!isiPad) {
                $($select).selectBox('refresh');
            }
        }
        else {
            var value = $($inputHiddenLast).val();
            var text = $($inputTextLast).val();
            $($inputHiddenLast).after('<select class="technician" style="width:100px;"><option value="' + value + '">' + text + '</option></select>');
            $($inputHiddenLast).remove();
            $($inputTextLast).remove();
            var $newselect = $('.technicians li').last().find('select');
            $($newselect).find('option:last-child').after('<option value="' + $inputValue + '">' + $inputText + '</option>');
            Sort($newselect);
            if (isiPad) {
                $($newselect).wrap('<div class="wrapperBlock"></div>');
                $($newselect).after('<span class="selectBox-arrow"></span>');
            }
            else {
                $($newselect).selectBox();
            }
        }
        var heightUl = $('#assignTechniciansForm ul').height();
        riBlock = $(elem).find('div.RiPayment');
        if (riBlock.size()) {
            $('#assignTechniciansForm ul').height(heightUl - heightPartialView);
        }
        
        $(elem).remove();

        var $new = $('.technicians li').last().find('.new');
        $($new).show();
        heightUl = $('#assignTechniciansForm ul').height();
        $('#assignTechniciansForm ul').height(heightUl - heightLi -15);

        reindex();
        var $curSelect = $('.technicians li').last().find('select');
        $($curSelect).selectBox('value', $current);
        cur = $('.technicians li').last().find('select').selectBox().val();
        riOperations(cur);
    };

    $(".new").live('click', function (event) {
        add(event);
    });

    $(".remove").live('click', function (event) {
        remove(event);
    });

    $('.percentInput').live('keyup', function (event) {
        var $target = $(event.target);
        var val = $($target).val();
        validationTextChange($target, val, 2);
    });

    $('.percentInput').live('focusout', function (event) {
        var $target = $(event.target);
        var currentval = $(event.target).val();
        var val = isNaN(parseFloat(currentval)) || currentval == 0 ? '' : parseFloat(currentval) + '';
        $(event.target).val(val);
    });

    $('.technician').selectBox().live('change', function(event) {
        riOperations(event.target.value);
    });

    $('select.technician').live('change',
        function (event) {
            if (isiPad) {
                riOperations(event.target.value);
            }
        });

    $(':radio').live('change',
    function (event) {
        var val = $(this).val();
        if (val == "RiOperations") {
            $("#FieldFlatFee").attr("disabled", "disabled");
            var sum = $("#riOperationsSum").val();
            $("#sum").html(sum);
        } else {
            $("#FieldFlatFee").removeAttr("disabled");
            $("#sum").html(0);
        }
    });

    var riOperations = function (currentVal) {
        var riDiv = $('#RiPayment');
        if (riDiv.css('visibility') == 'hidden') {
            var heightUl = $('#assignTechniciansForm ul').height();
            var val = currentVal;
            var role = val.split(':')[1];
            if (role == 'RITechnician') {
                $('#assignTechniciansForm ul').height(heightUl + heightPartialView);
                $(riDiv).css('visibility', 'visible');
            }
        } else {
            checkCountRi();
        }
    };

    $('#FieldFlatFee').live(
        'keypress', function (event) {
            if ($.browser.mozilla == true) {
                if (event.which == 8 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 46) {
                    return true;
                }
            }
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });

    $('#FieldFlatFee').live(
        'keyup', function(event) {
            var ch = $('#FieldFlatFee').val();
            var pos1 = ch.indexOf('.');
            if (pos1 != -1) {
                if ((ch.length - pos1) > 3) {
                    ch = ch.slice(0, -1);
                }
            }
            $('#FieldFlatFee').val(ch);
        });
    var validationTextChange = function (elem, newValue, precision) {
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
});

function Sort($select) {
    var selectOptions = $($select).find("option");
    selectOptions.sort(function (a, b) {
        if (a.text.toLowerCase() > b.text.toLowerCase()) {
            return 1;
        }
        else if (a.text.toLowerCase() < b.text.toLowerCase()) {
            return -1;
        }
        else {
            return 0;
        }
    });
    $($select).empty().append(selectOptions);
};

