function DismissNotification(id)
{
    $.ajax({
        url: "/Member/DismissNotification/" + $("#hdnDashboardURL").val(),
        data: { PlayerID: $("#hdnPlayerID").val(), DashboardNotificationID: id},
        dataType: "json",
        type: "POST",
        //beforeSend: function () {
        //    $(".divButtons").slideUp();
        //    $(".divProcessing").slideDown();
        //},
        error: function (e) {
        },
        success: function (data) {

            LoadNotifications();
        }
    });
}