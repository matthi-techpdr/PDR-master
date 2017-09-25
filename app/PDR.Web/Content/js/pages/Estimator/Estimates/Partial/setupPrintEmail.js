//@reference ~/Content/js/plugins/jquery.textchange.min.js

function setupEmail() {
    $('.email').click(function () {
        var emailUrl = $('.emailUrl').val();
        var ids = $('.hidden').val();
        $.ajax({
            type: "POST",
            url: emailUrl,
            data: "ids=" + ids,
            beforeSend: function () {
                $loader.show();
            },
            complete: function () {
                $loader.hide();
            },
            success: function (data) {
                if (data == "Error") {
                    jAlert("Send estimates of different customers in the same message can not be.\nPlease select the estimates relating to the same customer");
                }
                else {
                    Helpers.GetEmailDialog(data);
                }
            }
        });
    });
}

function setupPrint() {
    $('.print').click(function () {
        var printUrl = $('.printUrl').val();
        var basicDocument = $('#basicDocument').length;
        if (basicDocument == 0) {
            jCustom("Please select type of print document", "Print", function(r) {
                switch (r) {
                case 'open':
                    printUrl = printUrl.replace('False', 'true');
                    window.open(printUrl, "_blank");
                    break;
                case 'completed':
                    window.open(printUrl, "_blank");
                    break;
                default:
                    Helpers.RemoveActiveButtons();
                    break;
                }

            }, "Basic", "Cancel", "Detailed");
    } else {
            printUrl = printUrl.replace('False', 'true');
            window.open(printUrl, "_blank");
        }
    });
}