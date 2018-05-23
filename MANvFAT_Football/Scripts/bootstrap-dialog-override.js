
function OpenConfirmationModal(action, heading, Msg, param) {

    BootstrapDialog.confirm({
        title: heading,
        message: Msg,
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        closable: true, // <-- Default value is false
        draggable: true, // <-- Default value is false
        btnCancelLabel: 'No', // <-- Default value is 'Cancel',
        btnOKLabel: 'Yes', // <-- Default value is 'OK',
        btnOKClass: 'btn-primary', // <-- If you didn't specify it, dialog type will be used,
        callback: function (result) {
            // result will be true if button was click, while it will be false if users close the dialog directly.
            if (result) {
                if (action == "DeleteBGImage") {
                    DeleteBGImage(param);
                }
               
            }  else {
                //alert('Nope.'); //Don't do anything and close
            }
        }
    });

    return false;
}


function OpenAlertModal(title, Msg, css, type) {
    var alertDialog = BootstrapDialog.show({
        title: title,
        message: Msg,
        type: type,
        buttons: [{
            label: 'OK (Enter)',
            hotkey: 13, // Keycode of keyup event of key 'enter' is 13.
            cssClass: css,
            action: function () {
                alertDialog.close();
            }
        }]
    });
}

function OpenAlertModal_Location(title, Msg, css, type, RedirectLocation) {
    var alertDialog = BootstrapDialog.show({
        title: title,
        message: Msg,
        type: type,
        buttons: [{
            label: 'OK (Enter)',
            hotkey: 13, // Keycode of keyup event of key 'enter' is 13.
            cssClass: css,
            action: function () {
                location.href = RedirectLocation;
            }
        }]
    });
}