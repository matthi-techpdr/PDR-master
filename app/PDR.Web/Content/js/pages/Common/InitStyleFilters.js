$(document).ready(function () {
    $('#Customers, #Teams, #TeamsInv, #Team, #team').children().filter(':odd').addClass('greyFilter');
    $('#Customers, #Teams, #TeamsInv, #Team, #team').change(function () {
        $('.selectBox-label').removeClass('greyFilter');
    });
});



