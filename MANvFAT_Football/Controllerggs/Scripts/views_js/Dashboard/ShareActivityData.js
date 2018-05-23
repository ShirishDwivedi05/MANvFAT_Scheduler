//Share Activity Data
//1 = Daily Activity 2 = Weekly Activity
function ShowShareDiv(id) {
    if (id == 1) {
        $(".clsActivityData").slideUp();

        $(".clsShareData_Daily").slideDown();

        $(".clsShare").hide();
        $(".clsData").show();
    }
    else if (id == 2) {
        $(".clsWeeklyActivityData").slideUp();

        $(".clsShareData_Weekly").slideDown();

        $(".clsWeeklyShare").hide();
        $(".clsWeeklyData").show();
    }
}

function ShowActivityDiv(id) {
    if (id == 1) {
        $(".clsShareData_Daily").slideUp();

        $(".clsActivityData").slideDown();

        $(".clsData").hide();
        $(".clsShare").show();
    }
    else if (id == 2) {
        $(".clsShareData_Weekly").slideUp();

        $(".clsWeeklyActivityData").slideDown();

        $(".clsWeeklyData").hide();
        $(".clsWeeklyShare").show();
    }
}

function ShareFrequency_OnChange_Daily() {
    var ShareFrequencyID = $("#ShareFrequencyID_Daily").val();

    if (ShareFrequencyID == 4) //Selection Date Range
    {
        $(".divDateRangeSelection_Daily").slideDown();
    }
    else {
        $(".divDateRangeSelection_Daily").slideUp();
    }
}

function ShareFrequency_OnChange_Weekly() {
    var ShareFrequencyID = $("#ShareFrequencyID_Weekly").val();

    if (ShareFrequencyID == 4) //Selection Date Range
    {
        $(".divDateRangeSelection_Weekly").slideDown();
    }
    else {
        $(".divDateRangeSelection_Weekly").slideUp();
    }
}



function Activity_ShareDateFrom_OnChange_Daily() {
    var Activity_ShareDateFrom = $("#Activity_ShareDateFrom_Daily").data("kendoDatePicker");
    var Activity_ShareDateTo = $("#Activity_ShareDateTo_Daily").data("kendoDatePicker");

    Activity_ShareDateTo.min(Activity_ShareDateFrom.value());
    Activity_ShareDateTo.value(Activity_ShareDateFrom.value());
}

function Activity_ShareDateFrom_OnChange_Weekly() {
    var Activity_ShareDateFrom = $("#Activity_ShareDateFrom_Weekly").data("kendoDatePicker");
    var Activity_ShareDateTo = $("#Activity_ShareDateTo_Weekly").data("kendoDatePicker");

    Activity_ShareDateTo.min(Activity_ShareDateFrom.value());
    Activity_ShareDateTo.value(Activity_ShareDateFrom.value());
}



function chkSendToCoach_OnSelect(id) {
    if ($("#chkSendToCoach_" + id+"_").is(":checked")) {
        $.ajax({
            url: "/Member/GetPlayerLeagueCoach/" + $("#hdnDashboardURL").val(),
            data: { PlayerID: $("#hdnPlayerID").val() },
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {
            },
            success: function (data) {
                $("#txtEmail_"+id).val(data.CoachEmail);
            }
        });
    }
    else {
        $("#txtEmail_"+id).val("");
    }
}

function chkSendToTeam_Coach_OnSelect(id) {
    if ($("#chkSendToTeam_Coach_" + id + "_").is(":checked")) {
        $("#txtEmail_"+id).val("");

        $.ajax({
            url: "/Member/GetPlayerLeagueCoach/" + $("#hdnDashboardURL").val(),
            data: { PlayerID: $("#hdnPlayerID").val() },
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {
            },
            success: function (data) {
                var AllEmailAddresses = "";
                AllEmailAddresses = data.CoachEmail;

                $.ajax({
                    url: "/Member/GetPlayerTeamEmails/" + $("#hdnDashboardURL").val(),
                    data: { PlayerID: $("#hdnPlayerID").val() },
                    dataType: "json",
                    type: "POST",
                    //beforeSend: function () {
                    //    $(".divButtons").slideUp();
                    //    $(".divProcessing").slideDown();
                    //},
                    error: function (e) {
                    },
                    success: function (data) {
                        AllEmailAddresses = AllEmailAddresses + ";" + data.TeamEmailAddresses
                        $("#txtEmail_"+id).val(AllEmailAddresses);
                    }
                });
            }
        });
    }
    else {
        $("#txtEmail_"+id).val("");
    }
}

function chkSendToTeam_OnSelect(id) {
    if ($("#chkSendToTeam_" + id + "_").is(":checked")) {
        $.ajax({
            url: "/Member/GetPlayerTeamEmails/" + $("#hdnDashboardURL").val(),
            data: { PlayerID: $("#hdnPlayerID").val() },
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {
            },
            success: function (data) {
                var appendedEmailAddresses = $("#txtEmail_"+id).val() + data.TeamEmailAddresses
                $("#txtEmail_"+id).val(appendedEmailAddresses);
            }
        });
    }
    else {
        $("#txtEmail_"+id).val("");
    }
}

function ShareActivityData(id) {
    //ActivityTypeId = 1 >  Activity & 2 > Weekly Activity

    if (ValidateActivityShareForm(id)) {
        var _DateFrom = null;
        var _DateTo = null;
        if (Date.parse($("#Activity_ShareDateFrom_" + id).val()) != null) {
            _DateFrom = Date.parse($("#Activity_ShareDateFrom_" + id).val()).toString("dd-MM-yyyy");
        }

        if (Date.parse($("#Activity_ShareDateTo_" + id).val()) != null) {
            _DateTo = Date.parse($("#Activity_ShareDateTo_" + id).val()).toString("dd-MM-yyyy");
        }

        var _ActivityTypeID = 0;

        if (id == "Daily") {
            _ActivityTypeID = 1;
        }
        else if (id == "Weekly") {
            _ActivityTypeID = 2;
        }

        var ShareActivity = {
            PlayerID: $("#hdnPlayerID").val(),
            ShareFrequencyID: $("#ShareFrequencyID_" + id).val(),
            Activity_ShareDateFrom: _DateFrom,
            Activity_ShareDateTo: _DateTo,
            EmailAddress: $("#txtEmail_" + id).val(),
            MsgBody: $("#txtMsgBody_" + id).val(),
            ActivityTypeId: _ActivityTypeID
        };
        // alert("From " + ShareActivity.Activity_ShareDateFrom + " To " + ShareActivity.Activity_ShareDateTo);
        $.ajax({
            url: "/Member/ShareActivityData/" + $("#hdnDashboardURL").val(),
            data: ShareActivity,
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {
            },
            success: function (data) {
                $(".clsShareActivityMsg_" + id).html("Your Activity Data Shared Successdully!");
                $(".clsShareActivityMsg_" + id).addClass("alert").addClass("alert-success")
                $(".clsShareActivityMsg_" + id).slideDown();
            }
        });
    }
}

function ValidateActivityShareForm(id) {
    var ErrorMsg = "Please correct the following to continue <ul>";
    var status = true;
    var ShareFrequencyID = $("#ShareFrequencyID_"+id).val();

    if (ShareFrequencyID == 4 && ($("#Activity_ShareDateFrom_"+id).val() == "" || $("#Activity_ShareDateTo_"+id).val() == "")) {
        ErrorMsg = ErrorMsg + "<li>Please select Date Rage</li>";
        status = false;
    }

    if ($("#txtEmail_" + id).val() == "") {
        ErrorMsg = ErrorMsg + "<li>Please enter Email Address to share activity data</li>";
        status = false;
    }

    if (status == false) {
        ErrorMsg = ErrorMsg + "</ul>"

        $(".clsShareActivityMsg_" + id).html(ErrorMsg);
        $(".clsShareActivityMsg_" + id).addClass("alert").addClass("alert-danger")
        $(".clsShareActivityMsg_" + id).slideDown();
    }
    else {
        $(".clsShareActivityMsg_" + id).removeClass("alert").removeClass("alert-danger")
        $(".clsShareActivityMsg_" + id).slideUp();
    }

    return status;
}

function additionalData(id) {
    if (id == "Daily") {
        return {
            ActivityTypeID: 1
        };
    }
    else if (id == "Weekly") {
        return {
            ActivityTypeID: 2
        };
    }
}