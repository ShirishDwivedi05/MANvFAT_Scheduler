
function BGImages_OnSuccess(e) {
    Refresh_BgImagesList();
}

function Refresh_BgImagesList() {
    $("#lstPlayerImages").data("kendoListView").dataSource.read();
}

function DeleteBGImage(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/DeleteImage',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                // OpenAlertModal("Information", "Image Deleted Successfully", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                BGImages_OnSuccess();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}

function MakeDefaultPlayerImage(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/MakeDefaultImage',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                //  OpenAlertModal("Information", "Selected image Successfully set as Default Player Image", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                BGImages_OnSuccess();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}

function MakeImageDisplay(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/MakeImageDisplay',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                //  OpenAlertModal("Information", "Selected image Successfully Display/Hide on/From Front-End Website", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                BGImages_OnSuccess();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}

function MakeImageFrontSide(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/MakeImageFront_SideDisplay',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                // OpenAlertModal("Information", "Selected image Successfully Display/Hide on/From Front-End Website", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                BGImages_OnSuccess();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}

function UnMarkForDeletion(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/UnMarkForDeletion',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
                // OpenAlertModal("Information", "Selected image Successfully Display/Hide on/From Front-End Website", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                BGImages_OnSuccess();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}
function EditImageDate(id, val) {

    OpenImageDateWindow(id, val);

    return false;
}





function CreatePlayerImagesList(playerid) {
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/PlayerImages/GetPlayerImages/" + playerid + "?All=true",
                type: "GET",
                dataType: "json",
                contentType: 'application/json; charset=utf-8'
            }
        },
        pageSize: 50
    });

    $("#pager_PlayerImages").kendoPager({
        dataSource: dataSource
    });

    $("#lstPlayerImages").kendoListView({
        dataSource: dataSource,
        selectable: "single", //multiple
        dataBound: onDataBound,
        change: onChange,
        template: kendo.template($("#template_PlayerImages").html())
    });

    function onDataBound(e) {

        //$("#imgSelection").html("ListView data bound")
        //kendoConsole.log("ListView data bound");
    }

    function onChange() {

    }
}

//////////////////////////////////////////////////////////////
//******************For Animated GIF************************//
//////////////////////////////////////////////////////////////

var arr = [];
function PlayerAnimImgs_OnSelect(e) {
    var dataItem = e.dataItem;

    arr.push(dataItem.PlayerImageID);

    //$(".clsResult").html("event :: select (" + dataItem.Name + " : " + dataItem.ID + ") <br /> <h2> " + arr + "</h2>");
}

function PlayerImage_OnSelect(e) {
    arr.push(e);

    //$(".clsResult").html("event :: select (" + dataItem.Name + " : " + dataItem.ID + ") <br /> <h2> " + arr + "</h2>");
}

function PlayerAnimImgs_OnDeSelect(e) {
    var dataItem = e.dataItem;

    arr.splice($.inArray(dataItem.PlayerImageID, arr), 1);

    // $(".clsResult").html("event :: select (" + dataItem.Name + " : " + dataItem.ID + ") <br /> <h2> " + arr + "</h2>");
    // arr.pop(dataItem.ID);
}

function PlayerImage_OnDeSelect(e) {
    arr.splice($.inArray(e, arr), 1);

    // $(".clsResult").html("event :: select (" + dataItem.Name + " : " + dataItem.ID + ") <br /> <h2> " + arr + "</h2>");
    // arr.pop(dataItem.ID);
}


function CreateGifImg() {
    var PlayerAnimImgs = arr;
    //alert("PlayerAnimImgs = " + PlayerAnimImgs);
    if (PlayerAnimImgs != null && PlayerAnimImgs.length > 1) {
        startAnimation();
        $.ajax({
            url: "/PlayerImages/CreateGif",
            data: { PlayerImageIDs: PlayerAnimImgs },
            dataType: "json",
            type: "POST",
            tradition: true,
            error: function (e) {
                alert("Error is here = " + e.responseText);
                OpenErrorWindow("unhandled exception has occurred ", "Error");
                return false;
            },
            success: function (data) {
                if (data.status) {
                    $(".all-step .step-four").removeClass("error");
                    $(".all-step .step-four").removeClass("active");
                    $(".all-step .gif-step-nx:nth-of-type(1)").addClass("active");
                    LoadYourGifs();
                }
                else {
                    $(".all-step .step-four").removeClass("error").removeClass("active");
                    $(".all-step .gif-step-nx:nth-of-type(3)").addClass("active");
                    OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);

                }
            }
        });
    }
    else {
        $(".all-step .step-four").addClass("error");
        OpenAlertModal("Error", "Please select at least 2 Images", "btn-danger", BootstrapDialog.TYPE_DANGER);
        return false;
    }

    return false;
}
// doc and refference http://progressbarjs.readthedocs.io/en/1.0.0/api/shape/
function startAnimation() {
    var bar = new ProgressBar.Circle(divProgress, {
        color: '#4cba11',
        // This has to be the same size as the maximum width to
        // prevent clipping
        strokeWidth: 2,
        trailWidth: 10,
        easing: 'easeInOut',
        duration: 5000,
        text: {
            autoStyleContainer: false
        },
        from: { color: '#4cba11', width: 10 },
        to: { color: '#4cba11', width: 10 },
        // Set default step function for all animate calls
        step: function (state, circle) {
            circle.path.setAttribute('stroke', state.color);
            circle.path.setAttribute('stroke-width', state.width);

            var value = Math.round(circle.value() * 100);
            if (value === 0) {
                circle.setText('');
            } else {
                circle.setText(value);
            }

        }
    });
    bar.text.style.fontFamily = '"Raleway", Helvetica, sans-serif';
    bar.text.style.fontSize = '2rem';

    bar.animate(1.0);  // Number from 0.0 to 1.0
}
function CreateAnimatedImg() {

    var PlayerAnimImgs = arr;
    //alert("PlayerAnimImgs = " + PlayerAnimImgs);
    if (PlayerAnimImgs != null && PlayerAnimImgs.length > 1) {
        $.ajax({
            url: "/PlayerImages/CreateGif",
            data: { PlayerImageIDs: PlayerAnimImgs },
            dataType: "json",
            type: "POST",
            beforeSend: function () {
                waitingDialog.show('...Creating Animated GIF', { dialogSize: 'sm', progressType: 'info' });
            },
            tradition: true,
            error: function (e) {
                alert("Error is here = " + e.responseText);
                OpenErrorWindow("unhandled exception has occurred ", "Error");
                return false;
            },
            success: function (data) {
                if (data.status) {
                    waitingDialog.hide();
                    var tabStrip = $("#Players_tabstrip").data("kendoTabStrip");
                    if (tabStrip != undefined) {
                        tabStrip.select(3);
                    }
                    Refresh_BgImagesList();

                    var multi = $("#PlayerAnimImgs").data("kendoMultiSelect");
                    multi.value("");
                    multi.input.blur();
                }
                else {
                    OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
                }
            }
        });
    }
    else {
        OpenAlertModal("Error", "Please select at least 2 Images", "btn-danger", BootstrapDialog.TYPE_DANGER);
        return false;
    }

    return false;
}

//////////////////////////////////////////////////////////////
//******************For Update Image Date************************//
//////////////////////////////////////////////////////////////

var _PlayerImageID = "";

function OpenImageDateWindow(id, val) {
    _PlayerImageID = id;

    $("#ImageDateWindow").data("kendoWindow").center().open();
    $("#ImageDate").data("kendoDateTimePicker").value(val);
}

function CloseImageDateWindow() {
    $("#ImageDateWindow").data("kendoWindow").close();
    return false;
}

function UpdateImageDate() {
    if (ValidateImageDate()) {
        $.ajax({
            url: "/PlayerImages/UpdateImageDate",
            data: { PlayerImageID: _PlayerImageID, ImageDate: $("#ImageDate").val() },
            dataType: "json",
            type: "POST",
            beforeSend: function () {
                waitingDialog.show('...Processing Please wait', { dialogSize: 'sm', progressType: 'info' });
            },
            error: function (e) {
                waitingDialog.hide();
                onError();
            },
            success: function (data) {
                waitingDialog.hide();

                if (data.status) {
                    CloseImageDateWindow();

                    Refresh_BgImagesList();
                }
                else {
                    OpenAlertModal("Error", "Unexpected error occurred", "btn-danger", BootstrapDialog.TYPE_DANGER);
                }
            }
        });
    }
    return false;
}



function ValidateImageDate() {

    if ($("#ImageDate").val() == "") {

        var validationDiv = $("#lblImageDate");
        validationDiv.css('display', '');
        validationDiv.css('color', 'red');
        validationDiv.text('Please select Image Date');
        return false;
    }
    else {
        var validationDiv = $("#lblImageDate");
        validationDiv.css('display', 'none');
        return true;
    }

}//Validate Ends here


