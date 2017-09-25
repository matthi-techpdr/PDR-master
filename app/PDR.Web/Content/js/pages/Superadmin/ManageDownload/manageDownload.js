$(function () {
    window.uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight',
        browse_button: 'upload',
        max_file_count: 1,
        max_file_size: '8mb',
        //resize: { width: 400, height: 400, quality: 90 },
        filters: [{ title: "All files", extensions: "ipa"}],
        init: {
            FileUploaded: function (up, file, info) {

                var obj = JSON.parse(info.response);
                if (obj.isError === true) {
                    jAlert(obj.msg);
                } else {
                    $('#iPhoneAppGrid').setGridParam({
                        url: 'manageDownload/getdata'
                    }).trigger("reloadGrid");
                    jAlert(obj.msg);
                } 
                   
                
              //$("#logo").attr('src', $('#logo').attr('src') + "?" + new Date().getTime());
            },
            FilesAdded: function (up) {
                while (up.files.length > 1) {
                    up.removeFile(up.files[0]);
                }
                uploader.start();
            }
        },
        url: 'managedownload/saveexample',
        flash_swf_url: '/content/js/plugins/plupload/plupload.flash.swf',
        silverlight_xap_url: '/content/js/plugins/plupload/plupload.silverlight.xap'
    });
    uploader.init();

    uploader.bind('Error', function (up, err) {
       jAlert("The file has an invalid format or damaged!");
     });

    //    uploader.bind('FileUploaded', function (up, file) {
    //        
    //    });

    //setTimeout(function () { $('label.m').remove(); }, 3000);
});

