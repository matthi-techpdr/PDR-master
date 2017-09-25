jQuery(function ($) {
    PhoneMask("#PhoneNumber");
});

function PhoneMask(phoneId)
{
    $(phoneId).mask("000 000 0000");
}