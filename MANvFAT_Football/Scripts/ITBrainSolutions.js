

//UA - We Don't need this here, we just need it in Home/Index where we have to load the comments, so document ready should be in Home/Index only, I have added it to Home/Index
//Now its working as Expected.

//$(document).ready(function () {
//    function specificPage(pageName) { 
//        if (pageName == 'Home/Index') {
//            alert('Inside Function - Muhammad Abdullah -');

//            loadComments();
//            CommentTimer = setInterval(function () { loadComments(); }, 5000); //Creating and Assigning Timer to Global Object CommentTimer at /Views/Home/Index View
//        }
//    }
//});



function openConfirmationBox(id, Type, action) {
   // alert("id = " + id + " Type = " + Type + " action = " + action);
    $("#hfdId").attr("value", id);

    //Set Model Parameters
    $("#btnAction").attr("value", "Yes");
    if (action == 111 || action == "gridComment")
    {
        if (Type == 1) {
            $("#dlgHeading").text("Dismiss Comment");
            $("#dlgMsg").text("Are you sure you want to Dismiss?");
        }
        else {
            $("#dlgHeading").text("Un Dismiss Comment");
            $("#dlgMsg").text("Are you sure you want to Un Dismiss?");
        }
    }
    else if (action == "detailTask" || action == "gridTask") {
        $("#dlgHeading").text("Delete Record? ID = " + Type);
        $("#dlgMsg").text("Are you sure you want to Delete?");
    }
    else {
        $("#dlgHeading").text("Delete Record? ID = " + id);
        $("#dlgMsg").text("Are you sure you want to Delete?");
    }
    $('#confModal').modal('show');
    $("#btnAction").attr('class', 'btn btn-danger');
    $("#btnAction").attr("onclick", "ConfirmAction(" + id + ", '" + Type + "','" + action + "')");

    return false;
}

function ConfirmAction(id, Type, action) {
    if (action == "111" || action == "gridComment")
    {
        dismiss(id, Type, action);
    }
    else if (action == "detailTask" || action == "gridTask") {
        //Block For Task
        $.ajax({
            url: "/Tasks/DeleteTaskRecord",
            data: { id: id },
            dataType: "json",
            type: "POST",
            error: function (e) {
                alert("Error is here = " + e.responseText);
                OpenErrorWindow("unhandled exception has occurred ", "Error");
                return false;
            },
            success: function (data) {
                if (data.Status == true) {
                    if (action == "gridTask") {
                        RefreshGrid();
                        $('#confModal').modal('hide');
                    }
                    else 
                    if (action == "detailTask") {
                        location.href = "/Tasks/Index";
                    }
                }
                else {
                    $("#dlgErrorMsg").text(data.Msg);
                    $("#dlgErrorMsg").attr('class', 'alert alert-danger');
                }

            }
        });
    }
    else {
        //Block For WorkRecords
        $.ajax({
            url: "/WorkRecords/DeleteWorkRecord",
            data: { WorkRecordID: id },
            dataType: "json",
            type: "POST",
            error: function (e) {
                alert("Error is here = " + e.responseText);
                //onError();
                OpenErrorWindow("unhandled exception has occurred ", "Error");
                return false;
            },
            success: function (data) {
                if (data.Status == true) {
                    if (action == "grid") {
                        RefreshGrid();
                        $('#confModal').modal('hide');
                    }
                    else if (action == "detail") {
                        location.href = "/WorkRecords/Index";
                    }
                }
                else {
                    $("#dlgErrorMsg").text(data.Msg);
                    $("#dlgErrorMsg").attr('class', 'alert alert-danger');
                }

            }
        });
    }
}

function loadComments()
{
    $.ajax({
        url: "/Comments/ReadAllUnDismissComments",
        dataType: "json",
        type: "POST",
        error: function (e) {
            alert("Error is here = " + e.responseText);
            //onError();
            OpenErrorWindow("unhandled exception has occurred ", "Error");
            return false;
        },
        success: function (data) {
            if (data.Status == true) {

                //Dispaly Count in Badge
                $("#countBadge").text(data.Count);
                //Display Comments
                var Finalhtml = '';
                var jsonResult = data.jsonTaskComments;

                Finalhtml = '<div id="commentSpace" style="height:270px; overflow:auto;">';
                $.each(jsonResult, function (index, value) {
                    Finalhtml += '<div class="alert alert-info alert-dismissable"><button type="button" class="close" onclick="openConfirmationBox(' + value.TaskCommentID + ',1,111);" arial-hidden="true"> &times; </button> <div id="TaskComment"> <a href="/Tasks/Details/' + value.TaskUniqueID + '?selectedTab=3" target="_blank"> ' + value.TaskUniqueID + ' - ' + value.TaskComment + ' </a> </div> </div>';
                });
                Finalhtml += '</div>';

                $("#commentSpace").html(Finalhtml);

               
            }
            else {
                $("#countBadge").text(data.Count);
                $("#commentSpace").html('');
            }

            //When First Time Comment Loads, it will execute it self after provide delay time
            setTimeout(loadComments, 5000);
            // setInterval(function () { loadComments }, 5000);  //5000 = 5 sec
        }
    });
}

//For Dismiss
function dismiss(TaskCommentID, Type, action)
{
    $.ajax({
        url: "/Comments/DismissTaskComment",
        data: { TaskCommentID: TaskCommentID, Type: Type },
        dataType: "json",
        type: "POST",
        error: function (e) {
            alert("Error is here = " + e.responseText);
            //onError();
            OpenErrorWindow("unhandled exception has occurred ", "Error");
            return false;
        },
        success: function (data) {
            if (data.Status == true) {
                if (action == "gridComment") {
                    RefreshCommentGrid();
                    $('#confModal').modal('hide');
                }
                else if (action == 111) {
                    loadComments();
                    $('#confModal').modal('hide');
                }
            }
        }
    });
}













