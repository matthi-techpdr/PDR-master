var perform_acrobat_detection = function () {
    //
    // The returned object
    // 
    var browserInfo = {
        name: null,
        acrobat: null,
        acrobat_ver: null
    };

    if (navigator && (navigator.userAgent.toLowerCase()).indexOf("chrome") > -1) browserInfo.name = "chrome";
    else if (navigator && (navigator.userAgent.toLowerCase()).indexOf("msie") > -1) browserInfo.name = "ie";
    else if (navigator && (navigator.userAgent.toLowerCase()).indexOf("firefox") > -1) browserInfo.name = "firefox";
    else if (navigator && (navigator.userAgent.toLowerCase()).indexOf("msie") > -1) browserInfo.name = "other";


    try {
        if (browserInfo.name == "ie") {
            var control = null;
            
            //
            // load the activeX control
            //                
            try {
                // AcroPDF.PDF is used by version 7 and later
                control = new ActiveXObject('AcroPDF.PDF');
                
            }
            catch (e) { }

            if (!control) {
                try {
                    // PDF.PdfCtrl is used by version 6 and earlier
                    control = new ActiveXObject('PDF.PdfCtrl');
                }
                catch (e) { }
            }
            var pp = control.LoadFile("C:\Users\Demchenko\Desktop\savetopdf.pdf");
            control.printWithDialog();

            //control.print(pp);
            if (!control) {
                browserInfo.acrobat == null;
                return browserInfo;
            }
           
            version = control.GetVersions().split(',');
            version = version[0].split('=');
            browserInfo.acrobat = "installed";
            browserInfo.acrobat_ver = parseFloat(version[1]);
        }
        else if (browserInfo.name == "chrome") {
            for (key in navigator.plugins) {
                if (navigator.plugins[key].name == "Chrome PDF Viewer" || navigator.plugins[key].name == "Adobe Acrobat") {
                    browserInfo.acrobat = "installed";
                    browserInfo.acrobat_ver = parseInt(navigator.plugins[key].version) || "Chome PDF Viewer";
                }
            }
        }
        //
        // NS3+, Opera3+, IE5+ Mac, Safari (support plugin array):  check for Acrobat plugin in plugin array
        //    
        else if (navigator.plugins != null) {
            var acrobat = navigator.plugins['Adobe Acrobat'];
            if (acrobat == null) {
                browserInfo.acrobat = null;
                return browserInfo;
            }
            browserInfo.acrobat = "installed";
            browserInfo.acrobat_ver = parseInt(acrobat.version[0]);
        }


    }
    catch (e) {
        browserInfo.acrobat_ver = null;
    }

    return browserInfo;
};
$(document).ready(function () {
    perform_acrobat_detection();
});
