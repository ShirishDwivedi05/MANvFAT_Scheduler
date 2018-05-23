//View / Refer/ReferralCode

$(window).on("load", function () {

    $('#txtEmailAddress').keyup(function (e) {
        if (e.keyCode == 13) {
            ValidateEmailAddress();
        }
    });
});
function ValidateReferralCode() {

    if (CheckReferralCode()) {
        $.ajax({
            url: "/Refer/ValidateReferralCode/" + $("#hdnReferralCode").val(),
            data: { ReferralCode: $("#ReferralCode").val(), PlayerID: $("#hdnPlayerID").val() },
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {

            },
            success: function (data) {

                if (data.status) {
                    $(".clsErrorMsg").slideUp();
                }
                else {
                    $(".clsErrorMsg").text(data.Msg);
                    $(".clsErrorMsg").slideDown();
                }
            }
        });
    }
}

function SaveReferralCode() {

    if (CheckReferralCode()) {
        $.ajax({
            url: "/Refer/UpdateReferralCode/" + $("#hdnReferralCode").val(),
            data: { ReferralCode: $("#ReferralCode").val(), PlayerID: $("#hdnPlayerID").val() },
            dataType: "json",
            type: "POST",
            beforeSend: function () {
                waitingDialog.show('Processing please wait...', { dialogSize: 'md', progressType: 'info' });
            },
            error: function (e) {

            },
            success: function (data) {

                if (data.status) {
                    location.href = '/refer/' + $("#ReferralCode").val();
                }
                else {
                    waitingDialog.hide();
                    $(".clsErrorMsg").text(data.Msg);
                    $(".clsErrorMsg").slideDown();
                }
            }
        });
    }
}

function CheckReferralCode() {
    var errorMsg = "Please correct the following to continue...<br/><br/>";
    var status = true;
    if ($("#ReferralCode").val() == "") {
        errorMsg = errorMsg + "Please enter Referral Code<br/>";
        status = false;
    }

    if ($("#ReferralCode").val() == $("#hdnReferralCode").val()) {
        errorMsg = errorMsg + "New and Previous Referral code cannot be same<br/>";
        status = false;
    }

    if (status == false) {
        $(".clsErrorMsg").html(errorMsg);
        $(".clsErrorMsg").slideDown();
    }
    else {
        $(".clsErrorMsg").slideUp();
    }

    return status;

}

//View /Home/ReferYourFriend

function ValidateEmailAddress() {

    if ($("#txtEmailAddress").val() == "") {
        $(".clsErrorMsg").text("Please enter your email address registered at MANvFAT Football.");
        $(".clsErrorMsg").slideDown();
    }
    else {
        $(".clsErrorMsg").text("");
        $(".clsErrorMsg").slideUp();

        $.ajax({
            url: "/Refer/ValidateEmailAddress/" + "123456",
            data: { EmailAddress: $("#txtEmailAddress").val() },
            dataType: "json",
            type: "POST",
            beforeSend: function () {
                waitingDialog.show('Processing please wait...', { dialogSize: 'md', progressType: 'info' });
            },
            error: function (e) {

            },
            success: function (data) {

                if (data.status) {
                    location.href = "/Refer/" + data.ReferralCode;
                }
                else {
                    waitingDialog.hide();
                    $(".clsErrorMsg").text(data.Msg);
                    $(".clsErrorMsg").slideDown();
                }
            }
        });
    }
}