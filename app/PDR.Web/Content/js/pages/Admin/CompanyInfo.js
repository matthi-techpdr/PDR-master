$(function () {
    window.uploader = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus',
        browse_button: 'upload',
        max_file_count: 1,
        max_file_size: '5mb',
        resize: { width: 400, height: 400, quality: 90 },
        init: {
            FileUploaded: function () {
                $("#logo").attr('src', $('#logo').attr('src')+ "?" + new Date().getTime());
            },
            FilesAdded: function (up) {
                while (up.files.length > 1) {
                    up.removeFile(up.files[0]);
                }
                uploader.start();
            }
        },
        url: 'companyInfo/saveexample',
        flash_swf_url: '/content/js/plugins/plupload/plupload.flash.swf',
        silverlight_xap_url: '/content/js/plugins/plupload/plupload.silverlight.xap',
        filters: [{ title: "Image files", extensions: "jpg,gif,png"}]
    });
    uploader.init();
    setTimeout(function () { $('label.m').remove(); }, 3000);
});
