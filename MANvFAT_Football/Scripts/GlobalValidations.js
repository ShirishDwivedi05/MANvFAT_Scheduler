

function onError(e) {

    $.ajax({
        url: "/Error/getErrorCode",
        data: { id: "lucky" },
        dataType: "json",
        type: "POST",
        error: function (e) {
            //alert("An error occurred." + e.responseText);
        },

        success: function (data) {
            if (data.errorCode == "UnauthorizedAccess") {
                OpenAlertModal("Error", "You have no rights to perform this Action", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "UnExpectedError") {
                OpenAlertModal("Error", "Unexpected error occurred", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "DateTimeFormat") {
                OpenAlertModal("Error", "Date/Time format is not valid. Please enter Date as dd/MM/yyyy and Time as HH:mm", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "2601") {
                OpenAlertModal("Error", "Data already exists, it must be unique.", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "2627") {
                OpenAlertModal("Error", "This data already exists, it must be unique.", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "547") {
                OpenAlertModal("Error", "You cannot delete this record. It is already in use.", "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
            else if (data.errorCode == "20047") {
                OpenAlertModal("Error", "You are not authorised to perform this operation.", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            }
            else if (data.errorCode == "EmailAlreadyExists") {
                OpenAlertModal("Error", "Email Already Exists, it should be unique.", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            }
            else if (data.errorCode == "FileExtensionNotAllowed") {
                OpenAlertModal("Error", "File Extension is not Allowed, you can upload any file with extension  .jpg, .jpeg, .gif, .bmp, .png", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            } else if (data.errorCode == "ImageDimentionsAreNotValid") {
                OpenAlertModal("Error", "Uploaded Image Dimensions are not valid.", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            }
            else if (data.errorCode == "OnlyExcelFilesAllowed") {
                OpenAlertModal("Error", "File Extension is not Allowed, you can upload an Excel file with extension of .xls or .xlsx", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            }
            else if (data.errorCode == "FileAlreadyImported") {
                OpenAlertModal("Error", "The League Data from uploaded file already Imported", "btn-danger", BootstrapDialog.TYPE_DANGER);
                return false;
            }
            var mygrid = $('#grid').data('kendoGrid');
            if (mygrid != undefined) { mygrid.dataSource.read(); }

        }
    });
    e.preventDefault();
}

function DisplayGridError(e, status) {
   // alert("Im here e.errors = " + e.errors)
    var err = e.errors;

    if (err == undefined)
    {
        err = "Unexpected error has been occurred, Session Expired or Connection to Server has been lost. Please refresh the page, if error persist contact System Administrator.";
    }

    OpenAlertModal("Error", err, "btn-danger", BootstrapDialog.TYPE_DANGER);
    return false;
}

function CloseWindow() {
    var window = $('#Window').data('kendoWindow');
    window.close();
}


function ProcessingWindow(title) {
    var windowElement = $('#Window');
    windowElement.data('kendoWindow').title(title).content("<div><center><img alt='Processing please wait' src='/Images/loadingbar.gif' /> </center>  </div></div>").center().open();

}


function OpenErrorWindow(msg) {
    OpenAlertModal("Error", msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
}

function OpenInformationWindow(msg, title) {
    OpenAlertModal(title, msg, "btn-info", BootstrapDialog.TYPEINFO);
}

var Redirectlocation;
function OpenWindow_Location(msg, title, location, buttonText) {
    Redirectlocation = location;
    var windowElement = $('#Window');
    windowElement.data('kendoWindow').title(title).content("<div class='PopupDiv'><div class='alert alert-success'>" + msg + "</div><div class='PopupBtn'><button id='btnClose' class='k-button' type='button' onclick='javascript:BackToLocation();' >" + buttonText + "</button> </div></div>").center().open();

}

var RedirectlocationYES;
var RedirectlocationNO;
function OpenWindow_LocationYesNo(msg, title, locationYes, locationNo, buttonTextYes, buttonTextNo) {
    RedirectlocationYES = locationYes;
    RedirectlocationNO = locationNo;

    var windowElement = $('#Window');
    windowElement.data('kendoWindow').title(title).content("<div class='PopupDiv'>" + msg +
                    "<br/><div class='PopupBtn'><button id='btnClose' class='k-button' type='button' onclick='javascript:BackToLocationYES();' >" + buttonTextYes + "</button> <button id='btnClose' class='k-button' type='button' onclick='javascript:BackToLocationNO();' >" + buttonTextNo + "</button> </div></div>").center().open();

}

///////////////// Bootstrap Dialogue Windows //////////////////

function OpenConfirmationDialog(Title, Msg, JqueyFunc) {

    $('.model_ConfirmationDialogue').modal('show');
    $('.model_ConfirmationDialogue').find('#dlgHeading').text(Title);
    $('.model_ConfirmationDialogue').find('#dlgMsg').text(Msg);
    $('.model_ConfirmationDialogue').find('#btnAction').attr("onclick", JqueyFunc);
    $('.model_ConfirmationDialogue').find('#btnAction').attr("value", "Confirm");

}

function CloseDialog() {
    $('.model_ConfirmationDialogue').modal('hide');
}


//////////////////////////////////////////////////////////////



function FilenameAlreadyExistsRenameIt() {
    OpenInformationWindow("One or more Filename(s) already exists.", "Warning");
}


function BackToLocationYES() {
    location.href = RedirectlocationYES;
}

function BackToLocationNO() {
    location.href = RedirectlocationNO;
}

function BackToLocation() {
    location.href = Redirectlocation;
}

function BackToHomePage() {
    location.href = "/Home";
}


function ddl_OnLoad_OnFocus() {
    var ddl = $(this);

    ddl.closest(".t-widget").focus(function () {
        ddl.data("tDropDownList").open();
    });

}

$(document).keypress(function (e) {
    if (e.which == 13) {

        if ($("#btnClose") != undefined)
            $("#btnClose").click();

        return true;
    }
});



function UpdateGoCardlessPaymentStatuses(id) {
    $.ajax({
        url: "/GoCardless/GoCardless_UpdatePaymentStatuses",
        data: { PlayerPaymentID: id },
        dataType: "json",
        type: "POST",
        beforeSend: function myfunction() {
            if (id == undefined) {
                ProcessingWindow("Processing please wait...");
            }
        },
        error: function (e) {
            OpenErrorWindow("An unexpected error occurred.");// + e.responseText);
        },

        success: function (data) {

            if (id != undefined) {
                RefreshPlayerPayments_Grid();
            }
            else {
                OpenAlertModal("Information", "GoCardless Payment Statuses Updated Successfully", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
            }
        }
    });
}



function UpdatePaymentStatuses() {
  
    $.ajax({
        url: "/PlayerPayments/UpdatePaymentStatuses",
        data: {},
        beforeSend:function () {
            ProcessingWindow("Updating Payment details, Please Wait...!!!");
        },
        dataType: "json",
        type: "POST",
        error: function (e) {
            //alert("Error  = "+e.responseText);
            $("#PlayersPaid1499_Grid").data("kendoGrid").dataSource.read();
            CloseWindow();
        },
        success: function (data) {
            $("#PlayersPaid1499_Grid").data("kendoGrid").dataSource.read();
            CloseWindow();
        }
    });

    return false;
}


kendo.ui.Grid.fn.options.columnMenuInit = function (e) {
    var menu = e.container.find(".k-menu").data("kendoMenu");
    menu.bind('activate', function (e) {
        if (e.item.is('.k-filter-item')) {
            // if an element in the submenu is focused first, the issue is not observed
            e.item.find('span.k-dropdown.k-header').first().focus();
            // e.item.find('input').first().focus();
        }
    });
}

//Following Function will be used to Fix the Navigation Header
//if Scroll approaches to more than 100 px then it will add "FixHeader" class to navbar class, which will cause it to be fixed on screen
$(window).scroll(function () {

    if ($(window).scrollTop() >= 1) {
        // alert($(this).scrollTop());
        $('.navbar').addClass('FixHeader');
    } else {
        $('.navbar').removeClass('FixHeader');
    }
});


//Will be Called from League Details or League home Page
function cmdFixtureCalc(id, calcType) {

    var ddlFixtureCalc = 3;

    if (calcType == undefined) {
        ddlFixtureCalc = $("#ddlFixtureCalc").val();
    }
    else {
        ddlFixtureCalc = calcType;
    }

    $.ajax({
        type: "POST",
        url: '/SystemSettings/GetCurrentDomain',

        data: {},
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            //data = Current Domain i.e. https://test.manvfatfootball.org or https://manvfatfootball.org
            var url = "";
            //Subscribe Fixtures to Google Calendar
            if (ddlFixtureCalc == 1) {
                //if (data.indexOf("test.manvfatfootball.org") > 0)
                //{
                    data = data.replace('https', 'http');
                //}
                url = "https://www.google.com/calendar/render?cid=" + data + "/ICalc/Index/" + id + "";
            }
            //Subscribe Fixtures to Outlook or MAC OS X Calendar
            else if (ddlFixtureCalc == 2) {
                data = data.replace('http://', '').replace('https://', '');

                url = "webcal://" + data + "/ICalc/Index/" + id + "";

            }
            //Download Fixtures Calendar
            else if (ddlFixtureCalc == 3) {
                url = data + "/ICalc/Index/" + id + "";
            }

            // alert("url = " + url);

            //window.open(url, "_blank");
            location.href = url;
        }
    });


    return false;

}
