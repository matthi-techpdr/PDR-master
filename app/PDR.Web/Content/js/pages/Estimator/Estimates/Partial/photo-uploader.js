//@reference ~/Content/js/browserplus.js
//@reference ~/Content/js/plugins/jquery.prettyPhoto.js
//@reference ~/Content/js/plugins/plupload/plupload.full.js
//@reference ~/Content/js/json2.js

var uploader;
var count = 0;
var newPhotoCount = 0;

function removeFile(obj) {
    var $fileInfo = $(obj).parent();
    var file = uploader.getFile($fileInfo.attr("id"));
    uploader.removeFile(file);
    $fileInfo.remove();
    uploader.refresh();
}

function getIndex(name) {
    var indexStartPos = name.indexOf('[');
    var indexEndPos = name.indexOf(']');
    var index = name.substring(indexStartPos + 1, indexEndPos);

    return index;
}

function setIndex(name, index) {
    var oldIndex = getIndex(name);
    var newName = name.replace("[" + oldIndex + "]", "[" + index + "]");

    return newName;
}

function removeImage(obj) {
    jConfirm("Are you sure you want to delete this photo?", "Warning!", function (t) {
        if (t) {
            var $fileInfo = $(obj).parent();
            var $container = $fileInfo.parent();
            var id = $fileInfo.attr('file-id');
            $.ajax({
                method: 'GET',
                url: '/Images/RemoveTemp?id=' + id,
                cache: false,
                complete: function () {
                }
            });
            $fileInfo.remove();
            var files = $container.find("> div");
            $.each(files, function (fileIndex, file) {
                var inputs = $(file).find("input");
                $.each(inputs, function (inputIndex, input) {
                    var $input = $(input);
                    var name = $input.attr("name");
                    $input.attr("name", setIndex(name, fileIndex));
                });
            });

            var amount = parseInt(viewModel.amountStoredPhoto());
            amount--;
            count--;
            newPhotoCount--;
            viewModel.amountStoredPhoto(amount);

            uploader.refresh();
        }
    });
}

// Custom example logic
$(function () {
    var runtimes = "silverlight,html5,html4,browserplus,gears";
    var safari = ($.browser.safari && /safari/.test(navigator.userAgent.toLowerCase())) ? true : false;
    var msie8 = ($.browser.msie && /msie 8/.test(navigator.userAgent.toLowerCase())) ? true : false;
    var msie9 = ($.browser.msie && /msie 9/.test(navigator.userAgent.toLowerCase())) ? true : false;
    var chrome = (/chrome/.test(navigator.userAgent.toLowerCase())) ? true : false;
    var isiPad = navigator.userAgent.match(/iPad/i) != null;
    if (safari && !isiPad) {
        runtimes = "flash";
    }
    if (msie8 || msie9) {
        runtimes = "flash";
    }
    if (chrome) {
        runtimes = "html5";
    }

    uploader = new plupload.Uploader({
        runtimes: runtimes,
        headers: { 'Cache-Control': 'no-cache, must-revalidate', 'Pragma': 'no-cache' },
        browse_button: 'pickfiles',
        container: 'container',
        max_file_size: '5mb',
        url: '/Images/UploadToTemp',
        flash_swf_url: '/content/js/plugins/plupload/plupload.flash.swf',
        silverlight_xap_url: '/content/js/plugins/plupload/plupload.silverlight.xap',
        filters: [{ title: "Image files", extensions: "jpg,png"}]
    });

    uploader.bind('Init', function (up, params) {
        $('#filelist').html('');
    });

    $('#uploadfiles').click(function (e) {

        count = $("#uploaded-files > div").length + $("#storedfiles > div").length;
        var filesMaxCount = 10;
        var fileUploadedNowMaxCount = filesMaxCount - count;
        var uploadingFilesCount = $("#filelist > div span[onclick]").length;
        var $message = $(".photo-uploader-message");
        if (uploadingFilesCount > fileUploadedNowMaxCount) {
            $message.html("You can upload only up to " + filesMaxCount + " files");
            e.preventDefault();
        } else {
            $message.html("");
            $("#filelist span.ui-icon-trash").remove();
            $('.plupload').attr('disabled', true);

            uploader.start();
            e.preventDefault();
        }

        uploader.refresh();
    });

    uploader.init();

    $('#pickfiles').on("mouseover", function () {
        if ($(".plupload").hasClass("flash")) {
            uploader.refresh();
        }
    });

    uploader.bind('FilesAdded', function (up, files) {
        var error = false;
        $.each(files, function (i, file) {
            if (file.size > 5120000) {
                error = true;
            }

            if (error) {
                return jAlert("You cannot upload files that are larger than 5 Mb", "Warning!", null);
            }

            $('#filelist').append(
                    '<div class="file" id="' + file.id + '">' +
                        '<span class="file-info">' + file.name + ' (' + plupload.formatSize(file.size) + ')</span>' +
                        '<b></b>' +
                        '<span class="ui-icon ui-icon-trash" onclick="removeFile(this);"></span>' +
                    '</div>');
            up.refresh();
        });

        // Reposition Flash/Silverlight
    });

    uploader.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html(file.percent + "%");
    });

    uploader.bind('Error', function (up, err) {
        if (err.file) {
            if (err.code == plupload.FILE_SIZE_ERROR) {
                jAlert("You cannot upload files that are larger than 5 Mb", "Warning!", null);
            }
        }
        if (err.code != -500) {
            var $fileInfo = $('#' + err.file.id);
            $fileInfo.find("span.file-info").addClass("input-validation-error");
            $fileInfo.find("b").html('');
            up.refresh(); // Reposition Flash/Silverlight
        }
    });

    uploader.bind('FileUploaded', function (up, file, resp) {
        var $file = $('#' + file.id + "");
        $file.find("b").html("");
        $file.append('<span class="ui-icon ui-icon-check"></span>');
        var respObj = JSON.parse(resp.response);
        var $uploadedFiles = $("#uploaded-files");
        for (var i = 0; i < respObj.files.length; i++) {
            var img = respObj.files[i];
            var collectionName = 'UploadPhotos';
            $uploadedFiles.append('<div file-id="' + img.id + '">' +
                                        '<a href="' + img.fullSizeUrl + '" rel="prettyPhoto"><img src="' + img.thumbnailUrl + '" width="60" height="60" /></a>' +
                                        '<span class="ui-icon ui-icon-trash" onclick="removeImage(this);"></span>' +
                                        '<input type="hidden" name="' + collectionName + '[' + newPhotoCount + '].Id" value="' + img.id + '" />' +
                                        '<input type="hidden" name="' + collectionName + '[' + newPhotoCount + '].FullSizeUrl" value="' + img.fullSizeUrl + '" />' +
                                        '<input type="hidden" name="' + collectionName + '[' + newPhotoCount + '].ThumbnailUrl" value="' + img.thumbnailUrl + '" />' +
                                        '<input type="hidden" name="' + collectionName + '[' + newPhotoCount + '].ContentType" value="' + img.contentType + '" />' +
                                    '</div>');
            $uploadedFiles.find("div[file-id='" + img.id + "'] > a").prettyPhoto({ social_tools: false });
        }
        newPhotoCount++;
        count++;

        viewModel.amountStoredPhoto(count);

        $('.plupload').removeAttr('disabled');

        $file.hide('drop', {}, 1000);
        if ($('.plupload').hasClass('silverlight')) {
            if (parseInt(viewModel.amountEstimatePhoto()) > 0 && parseInt(viewModel.amountStoredPhoto()) > 0) {
                $('.plupload').css('top', '200px');
            }
            else if (parseInt(viewModel.amountStoredPhoto()) > 0 && parseInt(viewModel.amountEstimatePhoto()) == 0) {
                $('.plupload').css('top', '135px');
            }
            else {
                $('.plupload').css('top', '88px');
            }
        }
        else if ($('.plupload').hasClass('flash')) {
            var s = isNaN(parseInt(viewModel.amountEstimatePhoto())) ? 0 : parseInt(viewModel.amountEstimatePhoto());
            var d = isNaN(parseInt(viewModel.amountStoredPhoto())) ? 0 : parseInt(viewModel.amountStoredPhoto());

            if (s > 0 && d > 0) {
                $('.plupload').css('width', '50px !important').css('height', '33px !important').css('top', '200px');
            }
            else if (d > 0 && s == 0) {
                $('.flash').css('top', '68px');
            }
            else {
                $('.plupload').css('width', '50px !important').css('height', '33px !important').css('top', '0px');
            }
        }
    });
});