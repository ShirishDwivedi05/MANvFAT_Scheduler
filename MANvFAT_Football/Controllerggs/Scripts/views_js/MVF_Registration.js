function SubmitRegistrationForm() {
    if (ValidateRegForm()) {
        $("body").css('cursor', 'wait');
        $(".divSignMeUp").slideUp(); //Hide Sign Me up Button so user will never hit twice
        $(".divProcessing").slideDown();
        $("#RegistrationFormnew").submit();
    }
}

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

function isDate(txtDate) {
     alert("txtDate = " + txtDate);
    var currVal = txtDate;
    if (currVal == '')
        return false;

    var rxDatePattern = /^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/; //Declare Regex
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;

    //Checks for dd/mm/yyyy format.
    dtDay = dtArray[5];
    dtMonth = dtArray[3];
    dtYear = dtArray[1];

     alert("dtMonth = " + dtMonth + " dtDay = " + dtDay + " dtYear = " + dtYear);
    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

function ValidateRegForm() {
    var status = true;
    var Msg = "<div class='alert alert-danger'> Please correct the errors below to continue <br/> <ul>";
    if ($("#FirstName").val() == "") {
        Msg = Msg + "<li> Please enter your First name</li>";
        status = false;
    }

    if ($("#LastName").val() == "") {
        Msg = Msg + "<li> Please enter your Last name</li>";
        status = false;
    }

  

    if ($("#DOB").val() == "") {
        Msg = Msg + "<li> Please enter your Date Of Birth</li>";
        status = false;
    }
    //else if (isDate($("#DOB").val()) == false) {
    //    Msg = Msg + "<li> Date Of Birth is not valid it should be DD/MM/YYYY</li>";
    //    status = false;
    //}

    if ($("#HeightID").val() == "") {
        Msg = Msg + "<li> Please select your Height</li>";
        status = false;
    }

   


    if ($("#EmailAddress").val() == "") {
        Msg = Msg + "<li> Please enter your Email Address</li>";
        status = false;
    }
    else if (isValidEmailAddress($("#EmailAddress").val()) == false) {
        Msg = Msg + "<li> Please enter Valid Email Address</li>";
        status = false;
    }

    if ($("#Mobile").val() == "") {
        Msg = Msg + "<li> Please enter your Phone/Mobile Number</li>";
        status = false;
    }

    if ($("#PostCode").val() == "") {
        Msg = Msg + "<li> Please enter your Post Code</li>";
        status = false;
    }

    //if ($(".clsHowToPayRegFee").is(":visible") && $("#ddlHowToPayRegFee").val()=="") {
    //    Msg = Msg + "<li> Please choose How do you want to pay your registration fee?</li>";
    //    status = false;
    //}

   
    if ($("#chkTnC").is(":checked") == false) {
        Msg = Msg + "<li> Please accept term & conditions to Continue.</li>";
        status = false;
    }

    Msg = Msg + "</ul></div>";

    if (status == false)
        OpenAlertModal("Error", Msg, "btn-warning", BootstrapDialog.TYPE_WARNING);

    return status;
}

function League_OnChange() {
    var leagueiD = $("#SelectedLeagueID").val();
    if (leagueiD != "" && leagueiD != null) {
        SetRegistrationPage(leagueiD);
        $(".a_ListAllLeagues").hide();
    }
    else {
        $(".div_LiveLeague").slideUp();
        $(".div_PlannedLeague").slideUp();
        $(".div_FreeLeague").slideUp();
        $(".clsPaymentConfirmationText").html("I understand after submission I'll be redirected to<br />a payment page where I'll add my payment details for registration.");
    }
}

function SetRegistrationPage(LeagueID) {
    //Validate if selected league is Local Authority League Scheme?

    $.ajax({
        url: "/Leagues/IsLeagueLocalAuthWithDeposit",
        data: { LeagueID: LeagueID },
        dataType: "json",
        type: "GET",
        error: function (e) {
            alert("Error  = " + e.responseText);
            //alert("data.RegFeeWithBook = " + data.RegFeeWithBook + " data.RegFeeWithBook = " + data.RegFeeWithBook);
        },
        success: function (data) {
            var IsLocalAuth = data.Item1;
            var IsDepositRequired = data.Item2;
            var DepositAmount = data.Item3;
            var IsLive = data.Item4;
            var IsPlanned = data.Item5;
            var LeagueName = data.Item6;
            var PostCode = data.Item7;
            var depositStr = kendo.toString(DepositAmount, "N2");
            // alert("data.IsLocalAuth = " + IsLocalAuth + " data.IsDepositRequired = " + IsDepositRequired + " DepositAmount = " + DepositAmount);
            //If it is live league
            $(".h4_LeagueName").text(LeagueName);

            if (IsLive == true) {
                //if Live and Local Auth
                if (IsLocalAuth == true) {
                    $(".clsPaymentConfirmationText").html("I understand after submission I'll be redirected to a payment page where I'll add my payment details for Deposit of £" + depositStr + " - this deposit is returned at the end of the league season.");
                    $(".sp_DepositAmount").text("£" + depositStr);
                    //Now Deposit will also be paid using GoCardless
                    $(".sp_FreeLeaguePostCode").text(PostCode);
                    $(".div_LiveLeague").slideUp();
                    $(".div_PlannedLeague").slideUp();
                    $(".div_FreeLeague").slideDown();
                }
                else { //Only Live league
                    $(".clsPaymentConfirmationText").html("I understand after submission I'll be redirected to a payment page where I'll add my payment details for registration.");
                    $(".div_LiveLeague").slideDown();
                    $(".div_PlannedLeague").slideUp();
                    $(".div_FreeLeague").slideUp();
                }
            }

            //if it is Planned League
            else {
                $(".clsPaymentConfirmationText").html("I understand after submission I'll be redirected to a payment page where I'll add my payment details for £1 registration fee. This is to verify my identity and to reserve my place on the league");

                $(".div_LiveLeague").slideUp();
                $(".div_PlannedLeague").slideDown();
                $(".div_FreeLeague").slideUp();
            }

            $(".divPaymentConfirmtation").slideDown();
            $(".clsPayLogo_GoCardless").slideDown();
        }
    });
}

function Advertisement_OnChange() {
    var AdvertisementID = $("#AdvertisementID").val();

    //15 = Others or 16 = MANvFAT Coach
    if (AdvertisementID == 15 || AdvertisementID == 16) {
        $(".divOtherDetails").slideDown();

        if (AdvertisementID == 15) {
            $("#sp_OtherDetails").text("Other details");
            $("#AdvertisementOtherDetails").attr("placeholder", "Please enter detail, i.e. how did you hear about MAN v FAT Football");
        }
        else if (AdvertisementID == 16) {
            $("#sp_OtherDetails").text("Coach Name/Reference");
            $("#AdvertisementOtherDetails").attr("placeholder", "Please enter coach name or Reference");
        }
    }
    else {
        $(".divOtherDetails").slideUp();
        $("#AdvertisementOtherDetails").val("");
    }
}

$(window).on("load", function () {
    League_OnChange();
});