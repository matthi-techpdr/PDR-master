$(function () {
    var id = "matrixId";
    $('.m-status').click(function () {
        var currentElement = $(this);
        $.ajax({
            type: 'POST',
            cache: false,
            url: "matrixes/changestatus/" + currentElement.attr(id),
            success: function (data) {
                currentElement.html(data);
            }
        });
    });

    $('.m-edit').click(function () {
        var matrixId = $(this).attr('matrixId');
        $.ajax({
            url: "matrixes/rendermatrix/" + matrixId,
            cache: false,
            success: function (data) {
                $('.main-tt').html(data);
                ValidateAjaxForm($('#m_form'));
            }
        });
    });

    $('#create').click(function () {
        $.ajax({
            url: "matrixes/rendermatrix/",
            cache:false,
            success: function (data) {
                $('.main-tt').html(data);
                ValidateAjaxForm($('#m_form'));
            }
        });
    });
});

function SaveMatrix() {
    var sendJson = true;
    if ($('#m_form').valid()) {
    var model = {
        "Name": $('#Name').val(),
        "Description": $('#Description').val(),
        "AluminumPanel": $('#AluminumPanel').val(),
        "DoubleLayeredPanels": $('#DoubleLayeredPanels').val(),
        "OversizedDents": $('#OversizedDents').val(),
        "Suv": $('#Suv').val(),
        "Max": $('#Max').val(),
        "Id": $('#Id').val(),
        "PerBodyPart": $('#PerBodyPart').val(),
        "PerWholeCar": $('#PerWholeCar').val(),
        "MatrixPrices" : []
    };
    $('span.m_price').each(function (i, v) {
        var costVal = $(v).find('input[type="text"]').val();
        if (costVal.match(/^[0-9]{0,4}\.?[0-9]{0,2}$/) == null || costVal == "") {
            sendJson = false;
            $(v).find('input[type="text"]').addClass("input-validation-error");
        }
        var price = {
            "Id": $(v).attr('pid'),
            "Cost": costVal
        };
        model.MatrixPrices.push(price);
    });
    if (sendJson) {
        $.ajax({
            url: "matrixes/NameIsUnique?name=" + $('#Name').val() + "&matrixId=" + $("#Id").val(),
            cache:false,
            success: function (data) {
                if (data == "True") {
                    Helpers.SendJsonModelBase(model, "matrixes/save", function () {
                        var msg;
                        if ($("#Id").val() == "") {
                            msg = "New price matrix is created successfully.";
                        }
                        else {
                            msg = "Price matrix was edited successfully.";
                        }

                        jAlert(msg, "", function() { CloseMatrix(); });
                    });
                }
                else {
                    jAlert("Matrix name must be unique.", "Warning!");
                }
            }
        });
        }
    }
}

function CloseMatrix() {
    $('.main-tt').contents().remove();
    window.scrollTo(0, 0);
    window.location.reload();
}

function InitUploader() {
    window.uploader = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus,html4',
        browse_button: 'import',
        max_file_count: 1,
        max_file_size: '10mb',
        headers: { "id": $("#Id").val(), 'Cache-Control': 'no-cache, must-revalidate', 'Pragma': 'no-cache' },
        init: {
            FileUploaded: function (up, file, response) {
                $('.main-tt').html(response.response);
                ValidateAjaxForm($('#m_form'));
                jAlert("Price matrix was imported successfully", "", null);
            },
            FilesAdded: function (up) {
                while (up.files.length > 1) {
                    up.removeFile(up.files[0]);
                }

                this.settings.headers.name = $("#Name").val();
                this.settings.headers.description = $("#Description").val();
                uploader.start();
            }
        },
        url: 'matrixes/ImportFromXls',
        silverlight_xap_url: '/content/js/plugins/plupload/plupload.silverlight.xap',
        flash_swf_url: '/content/js/plugins/plupload/plupload.flash.swf',
        filters: [{ title: "XLS files", extensions: "xls, xlsx"}]
    });
    uploader.init();
}