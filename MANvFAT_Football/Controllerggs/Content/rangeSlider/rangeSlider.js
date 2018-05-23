

    var _height = 0.00;
var _weight = 0.00;
//var _heightCalc = 30.01818181818182;
var _heightFeetCalc = 30.48;
var _heightInchCalc = 2.54;
var _lbCalc = 2.204623; //for kg to lb pounds
var _stCalc = 0.1574730; //for kg to stones st
var _BMI = 0.00;

$(function () {
    $("#slider_Weight").slider({
        range: "min",
        step: 0.1,
        max: 200,
        animate: "fast",
        classes: {
            "ui-slider": "highlight"
        }
    });

    $("#slider_Weight").on("slide", function (event, ui) {

        SliderWeight_OnSlide(ui.value);
    });

    $("#slider_Weight").on("slidestop", function (event, ui) {
        SliderWeight_OnStop(ui.value);
    });

    $("#slider_BMI").slider({
        range: "min",
        step: 0.1,
        max: 200,
        animate: "fast",
        classes: {
            "ui-slider": "highlight"
        }
    });

    $("#slider_BMI").on("slide", function (event, ui) {
        SliderBMI_OnSlide(ui.value);

    });

    $("#slider_BMI").on("slidestop", function (event, ui) {
        SliderBMI_OnStop(ui.value);
    });

    CalculateBMI($("#txtWeight").data("kendoNumericTextBox").value());
    //Chaneg Text and Callback Event
    //$('#chkMeasuringUnit').on('switchChange.bootstrapSwitch', function (event, state) {
    //    chkMeasuringUnit_OnChange();
    //});

    //$.fn.bootstrapSwitch.defaults.onText = 'Imperial';
    //$.fn.bootstrapSwitch.defaults.offText = 'Metric';
});

function SetWeightTextBox(val, calcBMI) {

    // alert("im eher")
    var txtWeight = $("#txtWeight").data("kendoNumericTextBox");

    txtWeight.value(val);

    if (calcBMI == true)
        CalculateBMI(val);

}

function SliderWeight_OnSlide(val) {
    $(".txtWeight_SliderResult").text(val + " kg");
    $(".clsFinalWeight").text(kendo.toString(val, "N2") + " kg");
    CalculateBMI(val);
}

function SliderWeight_OnStop(val) {
    SetWeightTextBox(val, false);

}

function SliderBMI_OnSlide(val) {
    // $(".clsOutput").text(val);
    $(".clsFinalBMI").text("BMI "+val);
    CalculateWeight(val);
}

function SliderBMI_OnStop(val) {
    SetBMITextBox(val, false);
    CalculateWeight(val);
   

}

function txtWeight_OnChange() {

    var weight = $("#txtWeight").val();

    $("#slider_Weight").slider("option", "value", weight);
    SliderWeight_OnSlide(weight);

    // _weight = weight;
    CalculateBMI(weight, true);
}

function SetBMITextBox(val, calcweight) {

    var txtBMI = $("#txtBMI").data("kendoNumericTextBox");

    txtBMI.value(val);

    if (calcweight == true)
        CalculateWeight(val);

}

function txtBMI_OnChange() {

    var bmi = $("#txtBMI").val();

    $("#slider_BMI").slider("option", "value", bmi);
    SliderBMI_OnSlide(bmi);

    // _weight = weight;
    CalculateWeight(bmi, true);
}

function CalculateBMI(weight) {

    //var height_combine = $("#txtHeightFeet").val() + "." + $("#txtHeightInches").val();

   // var flHeight = parseFloat(height_combine);

    var heightFeet_cm = parseFloat($("#txtHeightFeet").val()) * _heightFeetCalc;
    var heightInch_cm = parseFloat($("#txtHeightInches").val()) * _heightInchCalc;

    // var height = (flHeight * _heightCalc) / 100.00;

    var height = (heightFeet_cm + heightInch_cm)/100.00;

    // alert("_height = " + _height + " _weight = " + _weight);

    _BMI = (weight / _height) / _height;
   // alert("_height = " + _height + " _weight = " + _weight + " finalVal = " + finalVal + " _BMI = " + _BMI);
    $("#slider_BMI").slider("option", "value", _BMI);
    // SliderBMI_OnSlide(_BMI);
    
    if (isNaN(_BMI) == false)
    {
        SetBMITextBox(_BMI, false);
        $(".clsFinalBMI").text("BMI "+kendo.toString(_BMI, "N2"));
    }
    else {
       // Validation();
    }

    _height = height;
    _weight = weight;

    // alert("_height = " + _height + " _weight = " + _weight);

}

function CalculateWeight(bmi) {

    var heightFeet_cm = parseFloat($("#txtHeightFeet").val()) * _heightFeetCalc;
    var heightInch_cm = parseFloat($("#txtHeightInches").val()) * _heightInchCalc;

    // var height = (flHeight * _heightCalc) / 100.00;

    var height = (heightFeet_cm + heightInch_cm) / 100.00;

    // alert("_height = " + _height + " _weight" + _weight);

    var finalWeight = (bmi * _height) * _height;

    // alert("_height = " + _height + " _weight" + _weight + " finalVal = " + finalVal);

    //  $(".clsFinalBMI").text(kendo.toString(finalVal, "N2"));

    SetWeightTextBox(finalWeight, false);

    txtWeight_OnChange(finalWeight);

    _height = height;
    _weight = finalWeight;
    _BMI = bmi;

}

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

function Validation() {
    var status = true;
    var Msg = "Please correct errors to continue.<br />"
    if ($("#txtHeightFeet").val() == "") {
        Msg = Msg + "- Please enter Height<br />";
        status = false;
    }

    if ($("#txtWeight").val() == "") {
        Msg = Msg + "- Please enter Weight.<br />";
        status = false;
    }
    if ($("#txtFirstName").val() == "") {
        Msg = Msg + "- Please enter Your First name.<br />";
        status = false;
    }
    if ($("#txtLastName").val() == "") {
        Msg = Msg + "- Please enter Your Last name.<br />";
        status = false;
    }

    if ($("#txtEmailAddress").val() == "") {
        Msg = Msg + "- Please enter Your Email Address.<br />";
        status = false;
    } else if (isValidEmailAddress($("#txtEmailAddress").val()) == false) {
        Msg = Msg + "- Please enter Valid Email Address.<br />";
        status = false;
    }
    
    if (status == false) {
        $(".divErrorMsg").html(Msg);
        $(".divErrorMsg").css('display', '');
        return false;
    }
    else
    {
        $(".divErrorMsg").css('display', 'none');
        return true;
    }
}

function SetFinalValues() {
    txtWeight_OnChange();
    var strHeight = "";
    var strWeight = "";
    var strBMI = "";
    if (isNaN(_height) == false)
    {
        strHeight = $("#txtHeightFeet").val() + "ft " + $("#txtHeightInches").val() + " in Or (" + kendo.toString((_height * 100), "N2") + " cm)";
        strWeight = kendo.toString(_weight, "N2") + " kg";
    }
    else {
     //   Validation();
    }   
    $(".clsFinalHeight_Score").text(strHeight);
    $(".clsFinalWeight_Score").text(strWeight);

    if (isNaN(_BMI) == false) {

        strBMI = kendo.toString(_BMI, "N2");
        $(".clsFinalBMI_Score").text(strBMI);

        //Now call another function to send it to the email
        if(Validation())
        {

            SendBMIDetails();
        }
        

    }
    else {
       // Validation();
    }
}

function chkMeasuringUnit_OnChange() {
    var chkMeasuringUnit = $("#chkMeasuringUnit");
    if ($("#chkMeasuringUnit").is(":checked")) {
        $("#lblMeasuringUnit").text("Change measuring unit to Metric");
        SwitchToImperial();
    }
    else {
        $("#lblMeasuringUnit").text("Change measuring unit to Imperial");
        SwitchToMetric();
    }
}

function SwitchToMetric() {

}

function SwitchToImperial() {

}


function SendBMIDetails(height, weight, bmi) {

    if (Validation()) {

        $.ajax({
            url: "/MANvFAT/SendBMIDetails",
            data: { HeightFeet: $("#txtHeightFeet").val(), HeightInches: $("#txtHeightInches").val(), Weight: _weight, BMI: _BMI, Firstname: $("#txtFirstName").val(), Lastname: $("#txtLastName").val(), EmailAddress: $("#txtEmailAddress").val() },
            dataType: "json",
            type: "POST",
            error: function (e) {
                // alert("Error  = " + e.responseText);
                OpenAlertModal("Email Sent","Email Successfully Sent. Thank You.",  "btn-success", BootstrapDialog.TYPE_SUCCESS);
                //alert("data.RegFeeWithBook = " + data.RegFeeWithBook + " data.RegFeeWithBook = " + data.RegFeeWithBook);
            },
            success: function (data) {
                if (data == true) {

                    var windowElement = $('#Window');
                    windowElement.data('kendoWindow').title("Email Sent").content("<div class='PopupDiv'><div class='alert alert-success'>Email Successfully Sent. Thank You</div><div class='PopupBtn'><button id='btnClose' class='k-button' type='button' onclick='javascript:CloseWindow();' >OK</button> </div></div>").center().open();

                    //OpenAlertModal("Email Sent", "Email Successfully Sent. Thank You.",  "btn-success", BootstrapDialog.TYPE_SUCCESS);
                }
                else {
                    var windowElement = $('#Window');
                    windowElement.data('kendoWindow').title("Email Sent Failed").content("<div class='PopupDiv'><div class='alert alert-danger'>Email Sent Failed.Please contact us at football@manvfat.com</div><div class='PopupBtn'><button id='btnClose' class='k-button' type='button' onclick='javascript:CloseWindow();' >OK</button> </div></div>").center().open();
                }


            }
        });
    }

}
function CloseWindow() {
    var window = $('#Window').data('kendoWindow');
    window.close();
}