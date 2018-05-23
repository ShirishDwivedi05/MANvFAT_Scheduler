
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
                else if (action == "MakeDefaultPlayerImage") {
                    MakeDefaultPlayerImage(param);
                }
                else if (action == "MakeImageDisplay") {
                    MakeImageDisplay(param);
                }
                else if (action == "DeletePlayer") {
                    DeletePlayer(param);
                }
                else if (action == "Create_SendPaymentLink") {
                    Create_SendPaymentLink(param);
                }
                else if (action == "DeleteNews") {
                    DeleteNews(param);
                }
                else if (action == "DeleteNAG") {
                    DeleteNAG(param1);
                }
                else if (action == "cmdAddAutoUsers") { // /Views/NAGs/tab_NAGUsers
                    AddNAGAutoUsers(param1);
                }
                else if (action == "cmdAddAllLeagues") { // /Views/NAGs/tab_NAGUsers
                    AddNAGAutoAllLeagues(param1);
                } else if (action == "CopyNAG") {
                    CopyNAG(param1);
                }
                else if (action == "cmdDone_UnDoneALLNAGUsers") { // /Views/NAGs/tab_NAGUsers
                    MarkDone_UnDoneAllUsers(param1);
                }
                else if (action == "cmdDone_UnDoneALLNAGLeagues") { // /Views/NAGs/tab_NAGUsers
                    MarkDone_UnDoneAllLeagues(param1);
                }
                else if (action == "Export_LeagueData") { // /Views/Leagues/Details
                    var AdditionalUsers = $("#txtAdditionalUsers").val();
                    Export_LeagueData(param, AdditionalUsers);
                }
                else if (action == "ResetLeague") { // /Views/Leagues/Details
                    ResetLeague(param);
                }
                else if (action == "SetupGhostTeam") { // /Views/Leagues/Details
                    SetupGhostTeam(param);
                }
                else if (action == "DismissAlert") { // /Views/Alerts/Index
                    DismissAlert(param);
                }
                else if (action == "DismissSelectedAlerts") { // /Views/Alerts/Index
                    DismissSelectedAlerts();
                }
                else if (action == "AutoDismissRedFlagAlerts") { // /Views/Alerts/Index
                    AutoDismissRedFlagAlerts();
                }
                else if (action == "cmdNAGLeagueDone") {
                    MarkNAGLeagueDone_UnDone(param1, true);
                }
                else if (action == "cmdNAGLeagueUnDone") {
                    MarkNAGLeagueDone_UnDone(param1, false);
                } else if (action == "cmdNAGUserDone") {
                    MarkNAGUserDone_UnDone(param1, false);
                }
                else if (action == "cmdNAGUserUnDone") {
                    MarkNAGUserDone_UnDone(param1, false);
                }
                //else if (action == "DeleteBGImage") {
                //    DeleteBGImage(param);
                //}
                else if (action == "MakeDefaultTeamImage") {
                    MakeDefaultTeamImage(param);
                }
                //else if (action == "MakeImageDisplay") {
                //    MakeImageDisplay(param);
                //}
                else if (action == "DeleteTeam") {
                    DeleteTeam(param);
                }
                else if (action == "RestorePlayer") {
                    RestorePlayer(param);
                }
                else if (action == "DeleteLeauge") {
                    DeleteLeague(param);
                }
                else if (action == "cmdDeleteDocument") { //Documents/Details when Document is Signed and Admin wants to Un-sign the document
                    DeleteDocument(param);
                }
                else if (action == "cmdMarkDocumentUnsigned") { //Documents/Details when Document is Signed and Admin wants to Un-sign the document
                    MarkDocumentUnSigned(param);
                }
                else if (action == "cmdReSendEmail") { 
                    ReSendEmail(param);
                } else if (action == "cmdRemovePlayerFromLeague") {
                    RemovePlayerFromLeague(param);
                } else if (action == "MakeImageFrontSide") { 
                    MakeImageFrontSide(param);
                }
                else if (action == "MarkImageDelete") {
                    MarkImageDelete(param);
                } else if (action == "UnMarkForDeletion") {
                    UnMarkForDeletion(param);
                }
                else if (action == "DismissPlayerWeightWeek_LossGain_5") {
                    DismissPlayerWeightWeek_LossGain_5(param);
                }
                else if (action == "DismissPlayerWeightWeek_LossGain_5") {
                    DismissPlayerWeightWeek_LossGain_5(param);
                }
                else if (action == "RemovePlayerFromMaintenance") {
                    RemovePlayerFromMaintenance(param);
                }
                else if (action == "RemoveLinkedProfile") {
                    RemoveLinkedProfile(param);
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