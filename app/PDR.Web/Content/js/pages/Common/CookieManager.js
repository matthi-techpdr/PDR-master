function CookieManager() {

}

/////////////////////////////////////////////////////
CookieManager.SetCookie = function(cookieName, value) {
    var cookieValue = null;
    switch (cookieName) {
        case 'customer':
            cookieValue = value;
            break;
        case 'team':
            cookieValue = value;
            break;
        case 'state':
            cookieValue = value;
            break;
        default:
            return;
    }
    $.cookie(cookieName, cookieValue, { expires: 30, path: '/' });
};



/////////////////////////////////////////////////////
$(document).ready(function () {
    $('#tabs > ul > li').click(function () {
        $.cookie('customer', null, { expires: -5, path: '/' });
        $.cookie('team', null, { expires: -5, path: '/' });
        $.cookie('state', null, { expires: -5, path: '/' });
    });
});


/////////////////////////////////////////////////////

CookieManager.GetCookie = function (cookieName) {
    var cookieValue = $.cookie(cookieName);
    return cookieValue;
};
