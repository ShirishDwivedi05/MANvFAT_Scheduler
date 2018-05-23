
 //10000 milisec = 10 Sec

$(document).ready(function () {

    //Set the Timer to Display Countdown and then execute the function on trigger
    $(".clsSiteMaintenance").css("display", '');
    RefreshCountDown_Maintenance();
  
});

function RefreshCountDown_Maintenance() {
    setInterval(function () {
        //Maintenacne_countDownTimer--;

        ////Display Upper right corner of the screen and it will update the timer after every 1 sec
        //$('.clsDownForMaintenanceRefreshTimer').html("Refresh In: " + Maintenacne_countDownTimer.toString() + "sec <span class='glyphicon glyphicon-time'></span>");

        //if (Maintenacne_countDownTimer === 0) {
        //    //Call the Function when CountdownTimer is zero and then again set the timer.
        //    Refresh_DownForMaintenance();

        //    Maintenacne_countDownTimer = (DownForMaintenance_refreshInterval / 1000);
        //}
        alert("im here");
        if (localStorage.getItem("DownForMaintenance_refreshInterval") == null) {
            alert("Item was not there created new");
            localStorage.DownForMaintenance_refreshInterval = "00:10";
        }
        
        var timer = localStorage.DownForMaintenance_refreshInterval.split(':');
        //by parsing integer, I avoid all extra string processing
        var minutes = parseInt(timer[0], 10);
        var seconds = parseInt(timer[1], 10);
        --seconds;
        minutes = (seconds < 0) ? --minutes : minutes;
        if (minutes < 0) clearInterval(interval);
        seconds = (seconds < 0) ? 59 : seconds;
        seconds = (seconds < 10) ? '0' + seconds : seconds;
        //minutes = (minutes < 10) ?  minutes : minutes;
        $('.clsDownForMaintenanceRefreshTimer').text(minutes + ':' + seconds);
        localStorage.DownForMaintenance_refreshInterval = minutes + ':' + seconds;

        if (minutes = 0 && seconds == 0)
        {
            alert("Now Im removing item");
            localStorage.removeItem("DownForMaintenance_refreshInterval");
        }

    }, 1000);
}