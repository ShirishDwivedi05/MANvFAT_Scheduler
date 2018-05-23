$(document).keydown(function (e) {
    //if (!args) args = []; // IE barks when args is null
    e = e || window.event;
    var keyCode = e.keyCode || e.which,
        arrow = { left: 37, up: 38, right: 39, down: 40 };

    if (e.ctrlKey) {
        switch (keyCode) {
            case arrow.left:
                PreviousWeek();
                break;
            case arrow.right:
                NextWeek();
                break;
        }
    }
});
Date.prototype.getWeek = function () {
    var onejan = new Date(this.getFullYear(), 0, 1);
    var today = new Date(this.getFullYear(), this.getMonth(), this.getDate());
    var dayOfYear = ((today - onejan + 1) / 86400000);
    return Math.ceil(dayOfYear / 7)
};
$(document).ready(function () {
    GetWeeklyActivityData();

    ////Scripts/Views/MobileDetection.js
    //if (isMobile.any()) {
    //    //alert("its mobile");
    //    $(window).on("swipeleft", function (event) {
    //        NextWeek();
    //    });

    //    $(window).on("swiperight", function (event) {
    //        PreviousWeek();
    //    });
    //}

});

function GetWeeklyActivityData() {

    //using Date.js to Convert date from a string
    var newDate = moment($("#dt_WeeklyActivity").val(), "DD MMM YYYY");
  
    var prevDate = moment($("#hdnWeeklyActivityDate").val());

    // alert("newDate = " + newDate + " prevDate = " + prevDate);

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
   
    var nDate = new Date(newDate.format("YYYY-MM-DD"));
    $("#weekheader").text("Week " + nDate.getWeek());
    
   
    //Load Content from Partial View
    $(".clsWeeklyActivityData").hide("slide", { direction: hide_Effect }, 500).load("/Member/_GetPlayerWeeklyActivity/" + $("#hdnDashboardURL").val(),
        { PlayerID: $("#hdnPlayerID").val(), ActivityDate: $("#dt_WeeklyActivity").val() }).show("slide", { direction: show_Effect }, 500);

    //Set the Current Selected date in Hidden Control so we can compare to decide the transition for div
    $("#hdnWeeklyActivityDate").val($("#dt_WeeklyActivity").val());

    //Display Current Selected Date as Text in Middle
    $(".spCurrentWeekDate").text(newDate.format("dd MMM yyyy"));

    //If Today's date is selected then hide the Today Date Button
    Hide_Show_CurrentWeekDateButton();

}

function Hide_Show_CurrentWeekDateButton() {

    var SelectedWeekDate = moment($("#dt_WeeklyActivity").val()).format("DD-MM-YYYY");
    var CurrentWeekDate = moment().weekday(1).format("DD-MM-YYYY");

    //alert("newDate = " + newDate + " prevDate = " + TodayDate);

    if (SelectedWeekDate == CurrentWeekDate) {
        $(".sp_btnCurrentWeek").slideUp();
    }
    else {
        $(".sp_btnCurrentWeek").slideDown();
    }
}

function SaveWData(id) {
    debugger;
    // alert("id= "+id);
    var StrIndex = id.split("_");

    var ctrlId = StrIndex[0];
    var dbID = StrIndex[1];
    var activityDate = StrIndex[2];;

    var FullId = ctrlId + "_" + dbID;

    var value = $("#" + FullId).val();
   
    var isCompleted=false;
    if ("#ddlCompleted_" + dbID) {
        if ($("#ddlCompleted_" + dbID).val() == "true")
            isCompleted = true;
        }
    var ProceedToSave = false;

    var New_val = "";
    var Old_val = "";

    if (ctrlId == "chkCompleted") {
        New_val = $("#" + FullId).is(":checked").toString();
        Old_val = $("#hdn" + FullId).val();
        // alert("New_val = " + New_val);
        if (New_val == "true") {
            //  alert("im ehre = " + dbID);
            $("#divWeeklyActivity_" + dbID).addClass("complete");

        }
        else {
            $("#divWeeklyActivity_" + dbID).removeClass("complete");
        }

    }
    else {
        New_val = $("#" + FullId).val();
        Old_val = $("#hdn" + FullId).val();
    }

    // alert("id = " + id + "FullId = " + FullId + " ctrlId = " + ctrlId + " dbID = " + dbID + " activityDate = " + activityDate + " value = " + value + " New_val = " + New_val + " Old_val = " + Old_val + " ctrl ACtivityID =  " + ctrlId.indexOf("ActivityTimeID"));

    if (New_val != "") {
        if (New_val != Old_val) {
            ProceedToSave = true;
            // $("#saved_" + FullId).show("bounce", { times: 3 }, "slow").hide("bounce", { times: 3 }, "slow");
            //$("#saved_" + id).show("puff", { times: 3 }, "slow").hide("puff", { times: 3 }, "slow");
        }
    }
    else if (ctrlId.indexOf("ActivityTimeID") >= 0) {
        ProceedToSave = true;
    }

    // alert("id = " + id+" New_val = " + New_val + " Old_val = " + Old_val+ " Proceed = "+ProceedToSave);



    if (ProceedToSave) {
        var PlayerWeeklyActivityExt = {
            PlayerID: $("#hdnPlayerID").val(),
            PlayerWeeklyActivityID: dbID,
            ActivityDate: activityDate,
            Activity: $("#txtActivity_" + dbID).val(),
            ActivityTime: $("#ActivityTimeID_" + dbID).val(),
            Completed: isCompleted
        };



        $.ajax({
            url: "/Member/SaveWeeklyActivityData/" + $("#hdnDashboardURL").val(),
            data: PlayerWeeklyActivityExt,
            dataType: "json",
            type: "POST",
            //beforeSend: function () {
            //    $(".divButtons").slideUp();
            //    $(".divProcessing").slideDown();
            //},
            error: function (e) {

            },
            success: function (data) {
              
                $("#hdn" + FullId).val(New_val);
                $(".collapse1_" + dbID).removeClass("collapse")
                $(".collapse1_" + dbID).addClass("collapse in")
                if ($("#txtActivity_" + dbID).length > 0) {
                    activityName = $("#txtActivity_" + dbID).val();
                }
                if ($("#ActivityTimeID_" + dbID).length > 0) {
                    activity = $("#ActivityTimeID_" + dbID).val();
                }
                //if (isCompleted && activityName.length > 0 && activity.length > 0) {
                //    $(".card_" + dbID).prop('id', 'headingOne');
                //}
                debugger;
                var allSelect = true;
                $("#divWeeklyActivity_" + dbID + " .ddl_target").each(function () {
                    debugger;
                    var val = $(this).val();
                    if(val=="true")
                    {
                        allSelect = allSelect && true;
                    }
                    else
                    {
                        allSelect = allSelect && false;
                    }
                });
                if(allSelect)
                {
                    $(".card_" + dbID).prop('id', 'headingOne');
                }
                else {
                    $(".card_" + dbID).prop('id', 'headingTwo');
                }
                //LoadAchievements();
               // LoadNotifications();
               // if (ctrlId == "chkCompleted") {
                 //   LoadProgressCharts();
               // }
            }
        });
    }
}

function SetCurrentWeek() {

    var Start_WeekDate = moment().weekday(1);
    //Get Today's Date
    // var TodayDate = Date.parse('today').toString("dd MMM yyyy");

    //Set value to Date Picker
    $("#dt_WeeklyActivity").data("kendoDatePicker").value(Start_WeekDate.format("DD MMM YYYY"));

    //Load and Refresh the data
    GetWeeklyActivityData();
}

function NextWeek() {
    var Start_WeekDate = moment($("#dt_WeeklyActivity").val(), "DD MMM YYYY").weekday(1);

    var Next_WeekStartDate = Start_WeekDate.add(1, "weeks");

    //Set value to Date Picker
    $("#dt_WeeklyActivity").data("kendoDatePicker").value(Next_WeekStartDate.format("DD MMM YYYY"));
    //Load and Refresh the data
    GetWeeklyActivityData();
}

function PreviousWeek() {

    var Start_WeekDate = moment($("#dt_WeeklyActivity").val(), "DD MMM YYYY").weekday(1);

    var Prev_WeekStartDate = Start_WeekDate.add(-1, "weeks");

    //Set value to Date Picker
    $("#dt_WeeklyActivity").data("kendoDatePicker").value(Prev_WeekStartDate.format("DD MMM YYYY"));
    //Load and Refresh the data
    GetWeeklyActivityData();
}

function AddNewActivity(id) {
  
    // alert("im here id = "+id);
    var vmWeeklyActivity = {
        PlayerID: $("#hdnPlayerID").val(),
        ActivityDate: id
    };

    id = id.replace(/ /g, '');
    //  alert("id = "+id);
    //Load html and append to div
    $("#NewRow_" + id).load("/Member/_AddNewWeeklyActivity/" + $("#hdnDashboardURL").val(), vmWeeklyActivity, function () {

        //append another with same name so we can append at later time
        $("#NewRow_" + id).after('<div class="row" style="display:none;" id="NewRow_' + id + '")"></div>')

        //Display the new Row
        $("#NewRow_" + id).slideDown();

        //Rename the New Row, so next time it will not be used to append the row
        $("#NewRow_" + id).attr("id", "NewRow");
    });

    return false;
}

function RemoveActivity(id) {

    $.ajax({
        url: "/Member/RemoveWeeklyActivity/" + $("#hdnDashboardURL").val(),
        data: { PlayerWeeklyActivityID: id },
        dataType: "json",
        async: false,
        type: "POST",
        //beforeSend: function () {
        //    $(".divButtons").slideUp();
        //    $(".divProcessing").slideDown();
        //},
        error: function (e) {

        },
        success: function (data) {

            $(".clsWeeklyActivityData").hide("blind", 500).load("/Member/_GetPlayerWeeklyActivity/" + $("#hdnDashboardURL").val(),
                { PlayerID: $("#hdnPlayerID").val(), ActivityDate: $("#dt_WeeklyActivity").val() }).show("blind", 500);

            //GetWeeklyActivityData();
        }
    });
}

function ShowShareDiv() {


    $(".clsWeeklyActivityData").slideUp();

    $(".clsShareData").slideDown();

    $(".clsShare").hide();
    $(".clsData").show();
}

function ShowActivityDiv() {


    $(".clsShareData").slideUp();

    $(".clsWeeklyActivityData").slideDown();

    $(".clsData").hide();
    $(".clsShare").show();
}


