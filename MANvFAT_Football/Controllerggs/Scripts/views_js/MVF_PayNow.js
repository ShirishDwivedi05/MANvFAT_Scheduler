
//Called from PayNow View

//referralCrlID is the Referral Control ID, e.g. on the same page we have 4 same controls but given id as txtReferralCode_1

function GeneratePayNowURL(id, val, refctrlid) {

    //Validate the Referral Code from Database if Exists then Continue otherwise warn the user
    var _ReferralCode = $("#txtReferralCode_" + refctrlid).val();

     // alert("referralCrlID = " + refctrlid + " _ReferralCode = " + _ReferralCode);

    $.ajax({
        url: "/Home/Generate_PayNowURL",
        data: { id: id, selection: val, ReferralCode: _ReferralCode },
        dataType: "json",
        beforeSend: function () {
            waitingDialog.show('Processing please wait...', { dialogSize: 'md', progressType: 'info' });
        },
        type: "GET",
        error: function (e) {
            alert("Error  = " + e.responseText);
            // $("#divLatestNewsList").html(e.responseText);
        },
        success: function (data) {
            if (data.status == true) {
                location.href = data.url;
            }
            else {
                waitingDialog.hide();
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

    return false;
}

function ClaimMyFreePlace(id) {

    $.ajax({
        url: "/Home/ClaimMyFreePlace",
        data: { PayLinkID: id, lid: $("#PayNow_SelectedLeagueID").val() },
        dataType: "json",
        beforeSend: function () {
            waitingDialog.show('Processing please wait...', { dialogSize: 'md', progressType: 'info' });
        },
        type: "GET",
        error: function (e) {
            alert("Error  = " + e.responseText);
            // $("#divLatestNewsList").html(e.responseText);
        },
        success: function (data) {
            if (data) {
                location.href = "/Home/PayNow/"+id+"?claim=true";
            }
            else {
                waitingDialog.hide();
                OpenAlertModal("Error", "Unexpected error has occurred, Please contact us at football@manvfat.com", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}