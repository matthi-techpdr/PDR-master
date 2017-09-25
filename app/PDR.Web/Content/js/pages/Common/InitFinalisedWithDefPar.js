$(document).bind('finaliseGridCustomize', function(event, grid) {
    grid.navButtonAdd('#pager', { caption: "Define participation", onClickButton: function () { RO.DefineParticipation(grid); }, position: "last", title: "" });
})
