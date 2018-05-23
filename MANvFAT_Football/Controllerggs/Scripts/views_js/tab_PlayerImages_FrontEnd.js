

function Refresh_WeeklyImagesList() {

    $("#lstPlayerImages").data("kendoListView").dataSource.read();
}

function Refresh_GIFImagesList() {
    $("#lstPlayerGIFImages").data("kendoListView").dataSource.read();
}

function MarkImageDelete(id) {

    $.ajax({
        type: "POST",
        url: '/PlayerImages/MarkImageDelete',
        data: { PlayerImageID: id },
        dataType: "json",
        error: function (e) {
            return false;
        },
        success: function (data) {

            if (data.status) {
               // OpenAlertModal("Information", "Image Deleted Successfully", "btn-primary", BootstrapDialog.TYPE_PRIMARY);
                Refresh_WeeklyImagesList();
                Refresh_GIFImagesList();
            }
            else {
                OpenAlertModal("Error", data.Msg, "btn-danger", BootstrapDialog.TYPE_DANGER);
            }
        }
    });

}

function DownloadImage(id) {
    location.href = "/PlayerImages/DownloadImage/"+id;
}

function CreatePlayerImagesList(playerid) {
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/PlayerImages/GetPlayerImages/" + playerid +"?Anim=false&All=false",
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


function CreatePlayerImagesGIFList(playerid) {
    var dataSource_GIF = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/PlayerImages/GetPlayerGIFImages/" + playerid + "?All=false",
                type: "GET",
                dataType: "json",
                contentType: 'application/json; charset=utf-8'
            }
        },
        pageSize: 50
    });

    $("#pager_PlayerGIFImages").kendoPager({
        dataSource: dataSource_GIF
    });

    $("#lstPlayerGIFImages").kendoListView({
        dataSource: dataSource_GIF,
        selectable: "single", //multiple
        dataBound: onDataBound_GIF,
        change: onChange_GIF,
        template: kendo.template($("#template_PlayerGIFImages").html())
    });

    function onDataBound_GIF(e) {

        //$("#imgSelection").html("ListView data bound")
        //kendoConsole.log("ListView data bound");
    }

    function onChange_GIF() {

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

function PlayerAnimImgs_OnDeSelect(e) {
    var dataItem = e.dataItem;

    arr.splice($.inArray(dataItem.PlayerImageID, arr), 1);

   // $(".clsResult").html("event :: select (" + dataItem.Name + " : " + dataItem.ID + ") <br /> <h2> " + arr + "</h2>");
    // arr.pop(dataItem.ID);
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

                    Refresh_GIFImagesList();

                    LoadAchievements();

                    LoadNotifications();

                    var multi = $("#PlayerAnimImgs").data("kendoMultiSelect");
                    multi.value("");
                    multi.input.blur();

                    $("#liUseYourPhotots").removeClass("active");
                    $("#liYourPhotoLibrary").addClass("active");
                    $("#tab_UseYourPhotos").removeClass("active");
                    $("#tab_PhotoLibrary").addClass("active");
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
