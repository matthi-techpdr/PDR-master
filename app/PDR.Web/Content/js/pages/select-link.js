$(function () {
    var isSelected = false;
    $("div.links a").each(function () {
        if (!isSelected) {
            var a = $(this);
            if (a.attr("href").indexOf(window.location.pathname) != -1) {
                a.addClass("active");
                isSelected = true;
            }
        }
    });
});