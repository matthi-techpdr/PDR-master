$(function() {
    $.ui.multiselect.prototype._getOptionNode =  function (option) {
            option = $(option);
            var text = option.text().length > 28 ? option.text().substring(0, 25) + '...' : option.text();
            var node = $('<li class="ui-state-default ui-element" title="' + option.text() + '"><span class="ui-icon"/>' + text + '<a href="#" class="action"><span class="ui-corner-all ui-icon"/></a></li>').hide();
            node.data('optionLink', option);
            return node;
        };
});
