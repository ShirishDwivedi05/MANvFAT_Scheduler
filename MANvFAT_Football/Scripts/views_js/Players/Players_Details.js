function SetIsApply() {
    $("#IsApply").val("true");

    if (IsPrev_CurrentEmail_Different()) {
        alert("*Warning* You have changed the Email Address of this Player. You have to inform \"craig@manvfat.com\" and \"hello@manvfat.com\" about email changes otherwise it could cause system errors.");
    }

    return true;
}
function SetIsSubmit() {
    if (IsPrev_CurrentEmail_Different() && $("#PlayerID").val()!="0") {
        alert("*Warning* You have changed the Email Address of this Player. You have to inform \"craig@manvfat.com\" and \"hello@manvfat.com\" about email changes otherwise it could cause system errors.");
    }
    return true;
}
function IsPrev_CurrentEmail_Different() {
    if ($("#hndPlayerPrevEmail").val() != $("#EmailAddress").val()) {
        return true;
    }
    else {
        return false;
    }
}
function BackToList() {
    location.href = "/Players";
    return false;
}

function cmdDeletePlayer(id) {
    //Function defination in tab_PlayerImages.cshtml
    OpenConfirmationModal("DeletePlayer", "Delete Player", "Are you sure you want to Delete this Player?", id);
    return false;
}

function DeletePlayer(id) {

    $.ajax({
        type: "POST",
        url: '/Players/DeletePlayer',
        data: { PlayerID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                location.href = '/Players';
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });


}
function PlayersTab_OnSelect(e) {

    //if User SElected tab "Players" then refresh so all the columns should be aligned
    if ($(e.item).index() == 2) {
        $("#PlayerWeightLogsGrid").data("kendoGrid").dataSource.read();
    }

};



function cmdRemovePlayerFromLeague(id) {
    //Function defination in tab_PlayerImages.cshtml
    OpenConfirmationModal("cmdRemovePlayerFromLeague", "Remove Player from League", "Are you sure you want to Remove this Player from League?", id);
    return false;
}

function RemovePlayerFromLeague(id) {

    $.ajax({
        type: "POST",
        url: '/Players/RemovePlayerFromLeague',
        data: { PlayerID: id },
        beforeSend: function () {
            waitingDialog.show('Processing please wait', { dialogSize: 'md', progressType: 'info' });
        },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                location.href = '/Players/Details/' + id;
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });


}