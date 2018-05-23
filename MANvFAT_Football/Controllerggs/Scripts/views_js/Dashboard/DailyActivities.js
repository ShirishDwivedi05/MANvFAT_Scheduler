
$(document).keydown(function (e) {
    //if (!args) args = []; // IE barks when args is null
    e = e || window.event;
    var keyCode = e.keyCode || e.which,
        arrow = { left: 37, up: 38, right: 39, down: 40 };

    if (e.ctrlKey) {
        switch (keyCode) {
            case arrow.left:
                PreviousDate();
                break;
            case arrow.right:
                NextDate();
                break;
        }
    }
});

$(document).ready(function () {
    debugger;
    GetDailyActivityData();

    if (isMobile.any()) {
        //alert("its mobile");
        $(window).on("swipeleft", function (event) {
            NextDate();
        });

        $(window).on("swiperight", function (event) {
            PreviousDate();
        });
    }
    $('.datetimepickerDaily').datetimepicker(
   { format: 'MM/DD/YYYY' }).on('dp.change', function (e) {
       var start = $(e).datetimepicker('getDate');
       $("#dt_Activity").val(start);
   });
});

function GetDailyActivityData() {
    //using Date.js to Convert date from a string
    var newDate = moment($("#dt_Activity").val());
    var prevDate = moment($("#hdnActivityDate").val());

    // alert("act = " + (newDate > prevDate) + "newDate = " + newDate.format("DD/MM/YYYY") + " prevDate = " + prevDate.format("DD/MM/YYYY") + "dt = " + $("#hdnActivityDate").val());

    //Default Effect of Transition
    var hide_Effect = "left";
    var show_Effect = "right";

    //Change the transision depends on Date Selection
    if (newDate > prevDate) {
        hide_Effect = "left";
        show_Effect = "right";
    }
    else {
        hide_Effect = "right";
        show_Effect = "left";
    }

    //Load Content from Partial View
    $(".clsActivityData").hide("slide", { direction: hide_Effect }, 500).load("/Member/_GetPlayerDailyActivity/" + $("#hdnDashboardURL").val(),
        { PlayerID: $("#hdnPlayerID").val(), ActivityDate: $("#dt_Activity").val() }).show("slide", { direction: show_Effect }, 500);

    //Set the Current Selected date in Hidden Control so we can compare to decide the transition for div
    $("#hdnActivityDate").val(newDate.format("dd MMM yyyy"));

    //Display Current Selected Date as Text in Middle
    $(".spCurrentDate").text(newDate.format("dd MMM yyyy"));

    //If Today's date is selected then hide the Today Date Button
    Hide_Show_TodayDateButton();
}

function Hide_Show_TodayDateButton() {
    var newDate = moment($("#dt_Activity").val()).format("DD-MM-YYYY");
    var TodayDate = moment().format("DD-MM-YYYY");

    // alert("newDate = " + newDate + " prevDate = " + TodayDate);

    if (newDate == TodayDate) {
        $(".sp_btnTodayDate").slideUp();
    }
    else {
        $(".sp_btnTodayDate").slideDown();
    }
}

function SaveData(id) {
    debugger;
    var ProceedToSave = false;

    var New_val = $("#" + id).val();
    var Old_val = $("#hdn" + id).val();


    if (New_val != Old_val) {
        ProceedToSave = true;
        // $("#" + id).css("background-color", "green");

        // $("#saved_" + id).show("pulsate").hide("pulsate");
        //$("#saved_" + id).show("bounce", { times: 3 }, "slow").hide("bounce", { times: 3 }, "slow");
        //$("#saved_" + id).show("puff", { times: 3 }, "slow").hide("puff", { times: 3 }, "slow");
    }


    //  alert("id = " + id+" New_val = " + New_val + " Old_val = " + Old_val+ " Proceed = "+ProceedToSave);

    if (ProceedToSave) {
        var PlayerDailyActivityExt = {
            PlayerDailyActivityID: $("#hdnPlayerDailyActivityID").val(),
            PlayerID: $("#hdnPlayerID").val(),
            ActivityDate: $("#hdnActivityDate").val(),
            Breakfast: $("#Breakfast").val(),
            Lunch: $("#Lunch").val(),
            Dinner: $("#Dinner").val(),
            Snacks: $("#Snacks").val(),
            Drink: $("#Drink").val(),
            HowHealthy: $("#HowHealthy").val(),
        };

        $.ajax({
            url: "/Member/SaveActivityData/" + $("#hdnDashboardURL").val(),
            data: PlayerDailyActivityExt,
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {
            },
            success: function (data) {

                $("#hdn" + id).val(New_val);
                LoadAchievements();
                LoadNotifications();
            }
        });
    }
}

function SetTodayDate() {
    //Get Today's Date
    var TodayDate = moment().format("DD MMM YYYY");

    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(TodayDate);

    //Load and Refresh the data
    GetDailyActivityData();
}

function NextDate() {

    var currentDate = moment($("#dt_Activity").val());

    var NewDate = currentDate.add(1, "days").format("DD MMM YYYY");

    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(NewDate);
    //Load and Refresh the data
    GetDailyActivityData();
}

function PreviousDate() {
    var currentDate = moment($("#dt_Activity").val());

    var NewDate = currentDate.add(-1, "days").format("DD MMM YYYY");
    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(NewDate);
    //Load and Refresh the data
    GetDailyActivityData();
}


