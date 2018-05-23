
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


    GetYourWeightData();

    //if (isMobile.any()) {
    //    //alert("its mobile");
    //    $(window).on("swipeleft", function (event) {
    //        NextDate();
    //    });

    //    $(window).on("swiperight", function (event) {
    //        PreviousDate();
    //    });
    //}
   
});

function GetYourWeightData() {
    //using Date.js to Convert date from a string
   // alert("Before " + $("#dt_Activity").val());

    var newDate = moment($("#dt_Activity").val(),"DD MMM YYYY");
    var prevDate = moment($("#hdnActivityDate").val());

   // alert("act = " + (newDate > prevDate) + " prevDate = " + prevDate.format("DD/MM/YYYY") + " newDate = " + newDate.format("DD/MM/YYYY") + " hdn Date = " + $("#hdnActivityDate").val());
   
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
    $(".clsWeightData").hide("slide", { direction: hide_Effect }, 500).load("/ProgressDashboard/_GetPlayerYourWeight/" + $("#hdnDashboardURL").val(),
        { PlayerID: $("#hdnPlayerID").val(), ActivityDate: $("#dt_measurement").val() }).show("slide", { direction: show_Effect }, 500);

    //Set the Current Selected date in Hidden Control so we can compare to decide the transition for div
    $("#hdnActivityDate").val(newDate.format("DD MMM YYYY"));

    if (newDate.format("DD MMM YYYY") == moment().format("DD MMM YYYY")) {
        $(".spCurrentDate").text("Today");
    }
    else {
        //Display Current Selected Date as Text in Middle
        $(".spCurrentDate").text(newDate.format("DD MMM YYYY"));
    }
    //If Today's date is selected then hide the Today Date Button
    Hide_Show_TodayDateButton();

   // alert("After " + $("#dt_Activity").val());
}

function Hide_Show_TodayDateButton() {
    var newDate = moment($("#dt_Activity").val()).format("DD-MM-YYYY");
    var TodayDate = moment().format("DD-MM-YYYY");



    if (newDate == TodayDate) {
        $(".sp_btnTodayDate").slideUp();
    }
    else {
        $(".sp_btnTodayDate").slideDown();
    }
}

function SaveData(id) {
    
    debugger;
        var PlayerDailyActivityExt = {
            PlayerMeasurementID: $("#hdnPlayerDailyWeightActivityID").val(),
            PlayerID: $("#hdnPlayerID").val(),
            ActivityDate: $("#dt_measurement").val(),
            Neck: $("#txtNeck").val(),
            Waist: $("#txtWaist").val(),
            LeftThigh: $("#txtLeftthigh").val(),
            RightThigh: $("#txtRighttthigh").val(),
            LeftBicep: $("#txtLeftBicap").val(),
            RightBicep: $("#txtRightBicep").val(),
            Hips: $("#txtHips").val(),
        }
        debugger;
        $.ajax({
            url: "/ProgressDashboard/SaveWeightMeasurementData/" + $("#hdnDashboardURL").val(),
            data: PlayerDailyActivityExt,
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {debugger;
            },
            success: function (data) {debugger;
              //  debugger;
                $("#hdn" + id).val(New_val);
               // LoadAchievements();
                //LoadNotifications();
            }
        });
    
}

function SetTodayDate() {
    //Get Today's Date
    var TodayDate = moment().format("DD MMM YYYY");

    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(TodayDate);
   
    //Load and Refresh the data
    GetYourWeightData();
}

function NextDate() {
  //  alert("dt_activity = " + $("#dt_Activity").val());

    var currentDate = moment($("#dt_Activity").val());

    var NewDate = currentDate.add(1, "days").format("DD MMM YYYY");

    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(NewDate)
    //Load and Refresh the data
    GetYourWeightData();
}

function PreviousDate() {
    var currentDate = moment($("#dt_Activity").val(), "DD MMM YYYY");

    var NewDate = currentDate.add(-1, "days").format("DD MMM YYYY");
    //Set value to Date Picker
    $("#dt_Activity").data("kendoDatePicker").value(NewDate)
    //Load and Refresh the data
    GetYourWeightData();
}


