function FooterSignUp() {

    if (ValidateFooterSignup()) {
        $.ajax({
            url: "/Home/Subscribe_FooterSignup",
            data: { EmailAddress: $("#txtFooterSignUp_Email").val() },
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
                waitingDialog.hide();
                if (data.status == true) {
                    OpenAlertModal("Subscribed Successfully", "Thank you for Subscribing.", "btn-success", BootstrapDialog.TYPE_SUCCESS);
                    $("#txtFooterSignUp_Email").val("");
                }
                else {
                    OpenAlertModal("Error", "Something went wrong, please try again or contact us at football@manvfat.com", "btn-danger", BootstrapDialog.TYPE_DANGER);
                }
            }
        });
    }

}


function isValid_FooterSignupEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

function ValidateFooterSignup() {
    var status = true;
    var Msg = "<div class='alert alert-danger'> Please correct the errors below to continue <br/> <ul>";

    if ($("#txtFooterSignUp_Email").val() == "") {
        Msg = Msg + "<li> Please enter your Email Address</li>";
        status = false;
    }
    else if (isValid_FooterSignupEmailAddress($("#txtFooterSignUp_Email").val()) == false) {
        Msg = Msg + "<li> Please enter Valid Email Address</li>";
        status = false;
    }

    Msg = Msg + "</ul></div>";

    if (status == false)
        OpenAlertModal("Error", Msg, "btn-warning", BootstrapDialog.TYPE_WARNING);

    return status;
}