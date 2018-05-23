jQuery(document).ready(function ($) {

    CalculateHealthyWeightRange();

    emojione.imageType = 'svg';
    emojione.sprints = true;
    emojione.imagePathSVGSprites = 'https://github.com/Ranks/emojione/raw/master/assets/sprites/emojione.sprites.svg';

    // Change translations
    $('#DashboardPassword').password({
        animate: true,
        minimumLength: 6,
        enterPass: emojione.unicodeToImage('Type your password 🔜'),
        shortPass: emojione.unicodeToImage('You can do it better, dude! 🤕'),
        badPass: emojione.unicodeToImage('Still needs improvement! 😷'),
        goodPass: emojione.unicodeToImage('Yeah! That\'s better! 👍'),
        strongPass: emojione.unicodeToImage('Yup, you made it 🙃'),
        showPercent: true
    });

    $("#rdDaily").change(function () {
        rdFrequency_OnClick()
    });

    $("#rdWeekly").change(function () {
        rdFrequency_OnClick()
    });

    $("#rdMonthly").change(function () {
        rdFrequency_OnClick()
    });

    

    $.fn.bootstrapSwitch.defaults.onText = 'Yes';
    $.fn.bootstrapSwitch.defaults.offText = 'No';
    $.fn.bootstrapSwitch.defaults.onColor = 'success';

    $("#rd_SendDailyReminder").bootstrapSwitch();

    $("#rd_SendDailyReminder").on('switchChange.bootstrapSwitch', function (event, state) {
        //console.log(this); // DOM element
        //console.log(event); // jQuery event
        //console.log(state); // true | false
        if (state == true) {
            $(".div_ReminderTime").slideDown();
        }
        else {
            $(".div_ReminderTime").slideUp();
            $("#ReminderTime").val("");
        }

    });

    $('#ReminderTime').mdtimepicker({

        // format of the time value (data-time attribute)
        timeFormat: 'hh',

        // format of the input value
        format: 'hh:00',

        // theme of the timepicker
        // 'red', 'purple', 'indigo', 'teal', 'green'
        theme: 'blue',

        // determines if input is readonly
        readOnly: true,

        // determines if display value has zero padding for hour value less than 10 (i.e. 05:30 PM); 24-hour format has padding by default
        hourPadding: false

    }); 

});

$(window).on("load", function () {
    if ($("#ReminderTime").val() != "") {

        $("#rd_SendDailyReminder").bootstrapSwitch("state", true, true);
        $(".div_ReminderTime").slideDown();
    }
});

function ValidateDashboardURL() {
    if (ValidateSystemSettings()) {
        $.ajax({
            url: "/Member/ValidateDashboardURL/" + $("#hdnDashboardURL").val(),
            data: { DashboardURL: $("#DashboardURL").val() },
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
                    $(".clsDashboardValidation").slideUp();
                }
                else {
                    $(".clsDashboardValidation").text(data.Msg);
                    $(".clsDashboardValidation").slideDown();
                }
            }
        });
    }
}

function SaveSettings() {

 

    if (ValidateSystemSettings()) {

        var rdFrequency = $('input[name=rdFrequency]:checked').val();
        var rdShareTo = $('input[name=rdShareTo]:checked').val();
        var txtAdditionalRecipients = $("#txtAdditionalRecipients").val();
        var txtMsgBody = $("#txtMsgBody").val();
        var drpDayOfWeek = $("#drpDayOfWeek").val();

        

       // alert("rdFrequency = " + rdFrequency + " rdShareTo = " + rdShareTo + " txtAdditionalRecipients = " + txtAdditionalRecipients + " txtMsgBody = " + txtMsgBody + " drpDayOfWeek = " + drpDayOfWeek);

        var DashboardSettings = {
            PlayerDashboardID: $("#hdnPlayerDashboardID").val(),
            PlayerID: $("#hdnPlayerID").val(),
            DashboardURL: $("#DashboardURL").val(),
            DashboardPassword: $("#DashboardPassword").val(),
            TargetWeight: $("#TargetWeight").val(),
          
            ShareDataFrequency: rdFrequency,
            ShareDataWith: rdShareTo,
            DayOfWeek: drpDayOfWeek,
            AdditionalRecipients: txtAdditionalRecipients,
            OptionalMessage: txtMsgBody,

            ReminderTime: $("#ReminderTime").val().replace(":00", "")

        }; 

        $.ajax({
            url: "/Member/UpdateSettings/" + $("#hdnDashboardURL").val(),
            data: DashboardSettings,
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
                    location.href = "/Member/" + $("#DashboardURL").val();
                }
                else {
                    $(".clsErrorMsg").html(data.Msg);
                    $(".clsErrorMsg").slideDown();
                }
            }
        });
    }
    return false;
}

function ValidateSystemSettings() {
    var errorMsg = "Please correct the following to continue...<br/><br/>";
    var status = true;
    if ($("#DashboardURL").val() == "") {
        errorMsg = errorMsg + "Please enter Dashboard URL <br/>";
        status = false;
    }


    if ($('#DashboardPassword').val() != "") {
        if ($('#DashboardPassword').val().length < 6) {
            errorMsg = errorMsg + "Password should be atleast 6 characters <br/>";
            status = false;
        }
    }

    if ($("#hdnIsFirstLogin").val() == "True" && $('#DashboardPassword').val() == "")
    {
        errorMsg = errorMsg + "You must Change your password on First Login <br/>";
        status = false;
    }

    if ($("#rd_SendDailyReminder").is(":checked") && $("#ReminderTime").val()=="")
    {
        errorMsg = errorMsg + "Please set when would like to receive Reminder? <br/>";
        status = false;
    }
   // alert("is First Login = " + );


    if (status == false) {
        $(".clsErrorMsg").html(errorMsg);
        $(".clsErrorMsg").slideDown();
    }
    else {
        $(".clsErrorMsg").slideUp();
    }

    return status;

}

//Calculate Player's Healthy BMI and display Healthy Weight Range
//ON THE SETTINGS PAGE THE SYSTEM SHOULD SIMPLY SAY - FOR YOUR HEIGHT A HEALTHY BMI RANGE WOULD BE (THEN THE SYSTEM CALCULATES WHAT A PLAYER'S WEIGHT WOULD BE AT BMI 18.5 (E.G. 75KG) TO 24.9 (E.G. 87KG) SO ABOVE THAT TARGET THE SYSTEM GENERATES A LINE THAT SAYS - FOR YOUR HEIGHT A HEALTHY WEIGHT RANGE WOULD BE 75KG - 87KG WHAT WOULD YOU LIKE YOUR TARGET TO BE? 

function CalculateHealthyWeightRange() {

    var playerHeight_cm = parseFloat($("#hdnPlayerHeight_cm").val())/100.00;

    var BMI_A = 18.50;
    var BMI_B = 24.90;

    // alert("_height = " + _height + " _weight" + _weight);

    var Weight_A = (BMI_A * playerHeight_cm) * playerHeight_cm;
    var Weight_B = (BMI_B * playerHeight_cm) * playerHeight_cm;

    $(".clsHealthyBMIWeight").html("For your Height a Healthy Weight Range would be " + Weight_A.toFixed(0) + "KG - " + Weight_B.toFixed(0)+"KG what would you like your Target to be?");
}

//Share Your Data JQuery Functions

function rdFrequency_OnClick()
{
    if ($("#rdWeekly").is(":checked"))
    {
        $(".divDayOfWeek").slideDown();
    }
    else
    {
        $(".divDayOfWeek").slideUp();
    }
}

function SaveShareDataSettings() {

}