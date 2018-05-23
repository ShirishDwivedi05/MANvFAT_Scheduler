function Submit_Registration(id) {
    if (ValidateRegistration_Form(id)) {

        var RegistrationExt = {
            FirstName: $("#txtFirstName_" + id).val(),
            LastName: $("#txtLastName_" + id).val(),
            EmailAddress: $("#txtEmailAddress_" + id).val()
        };

        $.ajax({
            type: "POST",
            url: '/Home/Registration',
            data: RegistrationExt,
            beforeSend: function () {
                waitingDialog.show('Processing please wait', { dialogSize: 'md', progressType: 'success' });
            },
            dataType: "json",
            error: function (e) {
                return false;
            },
            success: function (data) {
                waitingDialog.hide();

                if (data.status) {
                    OpenAlertModal("Information", "Registration Success", "btn-success", BootstrapDialog.TYPE_SUCCESS);
                }
                else {
                    OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
                }
            }
        });

    }

    return false;
}

function ValidateRegistration_Form(id) {
    
    var status = true;
    var errorMsg = "Please correct the following to continue:<br/><ul>";

    if ($("#txtFirstName_" + id).val() == "") {
        $("#txtFirstName_" + id).addClass("txt_error");
        $("#txtFirstName_" + id).focus();
        status = false;
        errorMsg = errorMsg + "<li>Please enter your First name</li>";
    }
    else {
        $("#txtFirstName_" + id).removeClass("txt_error");
    }

    if ($("#txtLastName_" + id).val() == "") {
        $("#txtLastName_" + id).addClass("txt_error");
        $("#txtLastName_" + id).focus();
        status = false;
        errorMsg = errorMsg + "<li>Please enter your Last name</li>";
    }
    else {
        $("#txtLastName_" + id).removeClass("txt_error");
    }

    if ($("#txtEmailAddress_" + id).val() == "") {
        $("#txtEmailAddress_" + id).addClass("txt_error");
        $("#txtEmailAddress_" + id).focus();
        status = false;
        errorMsg = errorMsg + "<li>Please enter your Email Address</li>";
    }
    else {
        if (isValidEmailAddress($("#txtEmailAddress_" + id).val())) {
            $("#txtEmailAddress_" + id).removeClass("txt_error");
        }
        else {
            $("#txtEmailAddress_" + id).addClass("txt_error");
            $("#txtEmailAddress_" + id).focus();
            status = false;
            errorMsg = errorMsg + "<li>Please enter your Valid Email Address</li>";
        }
    }

    if ($("#chk_tns_" + id).is(":checked") == false) {
        $("#chk_tns_" + id).addClass("txt_error");
        $("#chk_tns_" + id).focus();
        status = false;
        errorMsg = errorMsg + "<li>Please tick the checkbox for Terms and Conditions</li>";
    }
    else {
        $("#chk_tns_" + id).removeClass("txt_error");
    }

    if (status == false) {
        errorMsg = errorMsg + "</ul>";

        OpenAlertModal("Error", errorMsg, "btn-danger", BootstrapDialog.TYPE_DANGER);
    }

    return status;
}

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};