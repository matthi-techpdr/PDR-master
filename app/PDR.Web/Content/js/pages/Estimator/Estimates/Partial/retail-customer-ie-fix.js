// fix for inserting inline style 'width=2px' in IE8
$(document).bind('onAfterSetup', function () {
    $("#retail-customer .selectBox-dropdown").css("width", "50px");
});
