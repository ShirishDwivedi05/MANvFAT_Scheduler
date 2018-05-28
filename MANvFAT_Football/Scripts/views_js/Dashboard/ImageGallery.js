
function WeeklyImage_UploadSuccess() {

    $(".k-upload-files.k-reset").find("li").remove();
    $(".k-upload-status-total").hide();

    //Display Notification on Member Page

    LoadAchievements();
    LoadNotifications();
    Refresh_WeeklyImagesList();
}

function Show_CreateGif() {
    Refresh_GIFImagesList();
    $(".divCreateBeforeAndAfterPhoto").slideUp();
    $(".divCreateGif").slideDown();
}

function Show_CreateBeforeAndAfterPhoto() {
    Refresh_FirstImagesList();
    Refresh_SecondImagesList();
    $(".divCreateGif").slideUp();
    $(".divCreateBeforeAndAfterPhoto").slideDown();
}

function Refresh_FirstImagesList() {
    $("#listView").data("kendoListView").dataSource.read();
}

function Refresh_SecondImagesList() {
    $("#listView_second").data("kendoListView").dataSource.read();
}

var FirstImageId = 0;
var FirstImageSrc = "";
var SecondImageId = 0;
var SecondImageSrc = 0;
var count = 0;
function onChangeFirst(arg) {
    debugger
    count = count + 1;
    var ds = $("#listView").data("kendoListView");
    //var index = ds.select().index(),
    //    dataItem = ds.dataSource.view()[index];
    var data = ds.dataSource.view(),
        selected = $.map(this.select(), function (item) {
            return data[$(item).index()].PlayerImageID;
        });
    console.log(selected);
    //  alert("id = " + dataItem.PlayerImageID);
    FirstImageId = dataItem.PlayerImageID;
    FirstImageSrc = dataItem.ImageLink;
    $(".clsDivFirstImageSelection").slideUp();
    $(".clsDivSecondImageSelection").slideDown();

    $("#imgFirstImage_1").attr("src", dataItem.ImageLink);

    var listView = $("#listView_second").data("kendoListView");
    listView.dataSource.read();

    //$("#listView_second").data("KendoListView").dataSource.read();
    // Console.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
}
function onChange_First(arg) {
    count = count + 1;
    var ds = $("#listView").data("kendoListView");
    var data = ds.dataSource.view(),
        selected = $.map(this.select(), function (item) {
            return data[$(item).index()].PlayerImageID;
        });
    //  alert("id = " + dataItem.PlayerImageID);
    FirstImageId = data[0].PlayerImageID;
    FirstImageSrc = data[0].ImageLink;
    $(".clsDivFirstImageSelection").slideUp();
    $(".clsDivSecondImageSelection").slideDown();

    $("#imgFirstImage_1").attr("src", data[0].ImageLink);

    var listView = $("#listView_second").data("kendoListView");
    listView.dataSource.read();

    //$("#listView_second").data("KendoListView").dataSource.read();
    // Console.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
}
function onChange_Second(arg) {
    count = count + 1;
    var ds = $("#listView_second").data("kendoListView");
    var index = ds.select().index(),
        dataItem = ds.dataSource.view()[index];


    //  alert("id = " + dataItem.PlayerImageID);
    SecondImageId = dataItem.PlayerImageID;
    SecondImageSrc = dataItem.ImageLink
    // alert("FirstImageId = " + FirstImageId + " SecondImageId = " + SecondImageId);


    $(".clsDivSecondImageSelection").slideUp();
    $(".clsDivFinalImageSelection").slideDown();

    $("#imgFirstImage").attr("src", FirstImageSrc);
    $("#imgSecondImage").attr("src", SecondImageSrc);

}


function onSelectImage(playerImageId, playerImageLink, e) {
 
    count = count + 1;
    if (count > 2) {
        $(e).parent(".imgCheckbox0").remove();
        count = count - 1;
        OpenAlertModal("Error", "You can't select more than 2 Images", "btn-danger", BootstrapDialog.TYPE_DANGER);
        return;
    }
    if (count == 1) {
        FirstImageId = playerImageId;
        FirstImageSrc = playerImageLink;
    }
    else if (count == 2) {
        SecondImageId = playerImageId;
        SecondImageSrc = playerImageLink;
    }

}

function onDeselectImage(playerImageId, playerImageLink) {
    count = count - 1;
    if (playerImageId == 1) {
        SecondImageId = 0;
        SecondImageSrc = "";
    }
    else if (playerImageId == 0) {
        FirstImageId = 0;
        FirstImageSrc = "";
    }
}



function ProceedToCombine() {
    debugger;
    // alert("First Image ID = " + FirstImageId + " src = " + FirstImageSrc + " Second Image = " + SecondImageId + " src = " + SecondImageSrc +" PlayerID = " + $("#hdnPlayerID").val());

    $.ajax({
        url: "/PlayerImages/Combine_BeforeAfterImage/",
        data: { PlayerID: $("#hdnPlayerID").val(), FirstImagePath: FirstImageSrc, SecondImagePath: SecondImageSrc },
        dataType: "json",
        type: "POST",
        beforeSend: function () {
            waitingDialog.show('Processing please wait', { dialogSize: 'md', progressType: 'info' });
        },
        error: function (e) {
        },
        success: function (data) {
            waitingDialog.hide();
            if (data == true) {
                //location.href = '/Member/ImageGallery/' + $("#hdnDashboardURL").val();

                Refresh_WeeklyImagesList();

                $("#liUseYourPhotots").removeClass("active");
                $("#liYourPhotoLibrary").addClass("active");
                $("#tab_UseYourPhotos").removeClass("active");
                $("#tab_PhotoLibrary").addClass("active");
                ShowFirstImageSelector();

                LoadAchievements();
                LoadNotifications();
            }
        }
    });

}


function ProceedToCombineImages() {

    // alert("First Image ID = " + FirstImageId + " src = " + FirstImageSrc + " Second Image = " + SecondImageId + " src = " + SecondImageSrc +" PlayerID = " + $("#hdnPlayerID").val());
    if (count < 2) {
        $(".all-step .step-four").addClass("error");
        OpenAlertModal("Error", "Please select 2 Images", "btn-danger", BootstrapDialog.TYPE_DANGER);
        return;
    }


    $.ajax({
        url: "/PlayerImages/Combine_BeforeAfterImage/",
        data: { PlayerID: $("#hdnPlayerID").val(), FirstImagePath: FirstImageSrc, SecondImagePath: SecondImageSrc },
        dataType: "json",
        type: "POST",
        error: function (e) {
        },
        success: function (data) {
            if (data == true) {
                //location.href = '/Member/ImageGallery/' + $("#hdnDashboardURL").val();
                $('a[href="#4a"]').tab('show');
                $("#4a").parent("li").addClass("active");
                $("#4a").click();
                $("#3a").parent("li").removeClass("active");
                $("#2a").parent("li").removeClass("active");
                //ShowFirstImageSelector();
                HideAllImagesForBeforeAfter();
            }
        }
    });
    
}

// Default first step on first and after tab click
function GotoFirstStep() {
    $(".all-step .gif-step-nx:nth-of-type(3)").removeClass("active");
    $(".all-step .gif-step-nx:nth-of-type(1)").addClass("active");
}

//tabsId Id of the div containing the tab code.
//srcId Id of the tab whose id you are looking for
function id2Index(tabsId, srcId) {
    var index = -1;
    var i = 0, tbH = $(tabsId).find("li a");
    var lntb = tbH.length;
    if (lntb > 0) {
        for (i = 0; i < lntb; i++) {
            o = tbH[i];
            if (o.href.search(srcId) > 0) {
                index = i;
            }
        }
    }
    return index;
}

function AdditionalData() {
    return {
        ParamPlayerID: $("#hdnPlayerID").val(),
        ParamImageID: FirstImageId
    };

}

function AdditionalData_final() {
    return {
        FirstImageId: FirstImageId,
        SecondImageId: SecondImageId,
    };

}

function ShowSecondImageSelector() {

    $(".clsDivFinalImageSelection").slideUp();
    $(".clsDivSecondImageSelection").slideDown();

}

function ShowFirstImageSelector() {

    $(".clsDivFinalImageSelection").slideUp();
    $(".clsDivSecondImageSelection").slideUp();
    $(".clsDivFirstImageSelection").slideDown();

}