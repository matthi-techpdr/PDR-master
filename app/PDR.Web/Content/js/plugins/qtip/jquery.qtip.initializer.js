function QtipInitializer() { }
QtipInitializer.errorsCount = 0;
QtipInitializer.Init = function () {
    if (QtipInitializer.errorsCount > 0) {
        $('.field-validation-error').each(function () {
            var inputElem = '#' + $(this).attr('data-valmsg-for').replace('.', '_').replace('[', '_').replace(']', '_');
            $(this).hide();
            $(inputElem).filter(':not(.valid)').qtip({
                content: { text: $(this).text() },
                position: {
                    corner: {
                        target: 'topRight',
                        tooltip: 'bottomLeft'
                    }
                },
                show: 'mouseover',
                hide: 'mouseout',
                style: "pdrstyle"
            });
        });
    }
};


$(function () {
    QtipInitializer.Init();
});

$.fn.qtip.styles.pdrstyle = { // Last part is the name of the style
    background: '#FFA2A2',
    'font-size': "12px",
    color:'#696969',
    border:{
        width: 1,
        radius: 4,
        color:'#FFA2A2'
    },
    tip: {
         corner: 'bottomLeft', 
         color: '#FFA2A2',
         size: {
            x: 10, 
            y : 8 
         }
    }
};

$.fn.qtip.styles.pdrinvalidstyle = { // Last part is the name of the style
    background: '#FFA2A2',
    'font-size': "12px",
    color: '#696969',
    border: {
        width: 1,
        radius: 4,
        color: '#FFA2A2'
    },
    tip: true
};

$.fn.qtip.styles.pdrvalidstyle = { // Last part is the name of the style
    background: '#15cd0a',
    'font-size': "12px",
    color: 'white',
    border: {
        width: 1,
        radius: 4,
        color: '#15cd0a'
    },
    tip: true
};