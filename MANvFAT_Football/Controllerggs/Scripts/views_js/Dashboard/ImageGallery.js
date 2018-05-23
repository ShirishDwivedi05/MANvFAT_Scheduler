
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

function onChange_First(arg) {

    var ds = $("#listView").data("kendoListView");
    var index = ds.select().index(),
        dataItem = ds.dataSource.view()[index];


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

function onChange_Second(arg) {

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

function ProceedToCombine() {

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