/*global $, jQuery, console, alert, prompt */

$(document).ready(function () {

    //imgs gifs Selector  
    $(".select-image-gif img").imgCheckbox({
        scaleSelected: false
    });


    // Date Picker 

    $('#datetimepicker1,#datetimepicker4,#datetimepicker2,#datetimepicker3').datetimepicker({
        language: 'en',
        pickTime: false
    });
    //Img Upload
   // $('.dropzone').html5imageupload();
    $('.dropzone').html5imageupload({ url: 'PlayerImages/SavePlayerImages', data: { ParamPlayerID: '4' } });

    // custemscroll bar for news
    $(window).on("load", function () {
        $("#scroll-1").mCustomScrollbar({
            scrollButtons: { enable: true },
            theme: "3d-thick"
        });
        $(".news").mCustomScrollbar({
            scrollButtons: { enable: true, scrollType: "stepped" },
            keyboard: { scrollType: "stepped" },
            mouseWheel: { scrollAmount: 662, normalizeDelta: true },
            theme: "rounded-dark",
            autoExpandScrollbar: true,
            snapAmount: 662,
            snapOffset: 65
        });
    });

    // Lead progresss for gif image creat
    //$("#divProgress").circularloader({
    //    backgroundColor: "transparent",//background colour of inner circle
    //    fontColor: "#000000",//font color of progress text
    //    fontSize: "40px",//font size of progress text
    //    radius: 60,//radius of circle
    //    progressBarBackground: "#ffffff",//background colour of circular progress Bar
    //    progressBarColor: "#4cba11",//colour of circular progress bar
    //    progressBarWidth: 15,//progress bar width
    //    progressPercent: 0,//progress percentage out of 100
    //    progressValue: 0,//diplay this value instead of percentage
    //    showText: false,//show progress text or not 
    //    title: "M",//show header title for the progress bar
    //});


    /*** Modal ***/
    // Quick & dirty toggle to demonstrate modal toggle behavior
    
    $('.modale-close').on('click', function (e) {
        $(this).parents('.photo_aft-bef,.photo_libraryPic').find('.modal-bf-aft').removeClass('is-visible');
    });

    
    // Clear all input 
    $('.clear-all').on('click', function () {
        $('.input-fields').find('input:text').val('').removeClass('filled-background');
    });




    // Steps Gifs 

    $(".all-step .gif-step-nx:nth-of-type(1)").addClass("active");
    $(".gif-step-nx").on("click", ".add-gifs", function (e) {
      
        if ($(this).parents(".gif-step-nx").hasClass('step-one')) {
            $(this).parents(".gif-step-nx").removeClass("active").next().next().addClass("active");
        }
        else {
            $(this).parents(".gif-step-nx").removeClass("active").next().addClass("active");
        }
  
        var hasLastStep = $(".all-step .step-four").hasClass("active");
        var hasError = $(".all-step .step-four").hasClass("error");
        if (hasError) {
            $(".all-step .step-four").removeClass("error").removeClass("active");
            $(".all-step .gif-step-nx:nth-of-type(3)").addClass("active");
        }

        else if (hasLastStep) {
            //$(".all-step .step-four").removeClass("active");
            //$(".all-step .gif-step-nx:nth-of-type(1)").addClass("active");
        }

    });


    // Accept only number 
    $(".numberTextOnly").on("keypress keyup blur", function (event) {
 
        //this.value = this.value.replace(/[^0-9\.]/g,'');
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $(".btn-remove-gif").on('click', function () {
        $(this).parent(".gifs-box").remove();
    });

    $('.arrow-links').click(function () {
        var collapsed = $(this).find('i').hasClass('fa-angle-up');

        $('.arrow-links').find('i').removeClass('fa-angle-down');

        $('.arrow-links').find('i').addClass('fa-angle-up');
        if (collapsed)
            $(this).find('i').toggleClass('fa-angle-up fa-2x fa-angle-down fa-2x')
    });




    $("#modelLink").click(function () {
      
        var link=$(this).attr('href');
        if(link!='#')
        {
            $(this).prop('href','#');
            var height = 700;
            var width = 900;
            var top = window.innerHeight - height;
            var left = window.innerHeight - width;
            window.open(link, '_blank', 'location=yes, height=700,width=900, top=' + top + ', left=' + left + ', scrollbars=yes,status=yes');
       
        }

    });



    $('.fa-times').on('click', function () {
        $(this).parents('.slider_bg_Head').hide();
    });

    $('.fa-question-circle2').on('click', function () {
        $('.fa-question-circle2').hide();
        $('.fa-question-circle2').hide();
    });

    $(".fa-times2").click(function () {
        $(".fa-question-circle").show();
    });

});

/*
CircularLoader-v1.3.js
Developed by http://technoplugin.com
For support or web development drop a mail at developer@technoplugin.com
*/
!function (r) { function t(r) { return r * Math.PI / 180 } function e(e, a, s) { var o = r("#" + s + "canvas")[0], n = r("#" + s + "canvas"), i = o.getContext("2d"), l = o.width / 2, d = o.height / 2; i.beginPath(), i.arc(l, d, r(n).attr("data-radius"), 0, 2 * Math.PI, !1), i.fillStyle = "transparent", i.fill(), i.lineWidth = r(n).attr("data-width"), i.strokeStyle = r(n).attr("data-progressBarBackground"), i.stroke(), i.closePath(), i.beginPath(), i.arc(l, d, r(n).attr("data-radius"), -t(90), -t(90) + t(e / 100 * 360), !1), i.fillStyle = "transparent", i.fill(), i.lineWidth = r(n).attr("data-width"), i.strokeStyle = r(n).attr("data-stroke"), i.stroke(), i.closePath(), "true" == r(n).attr("data-text").toLocaleLowerCase() && r("#" + s + " .clProg").val(a + ("true" == r(n).attr("data-percent").toLocaleLowerCase() ? "%" : "")) } r.fn.circularloader = function (t) { function a() { h.beginPath(), h.arc(u, f, i, 0, 2 * Math.PI, !1), h.fillStyle = n.backgroundColor, h.fill(), h.lineWidth = l, h.strokeStyle = n.progressBarBackground, h.stroke(), h.closePath(), c > 0 && e(c, d, o) } var s = this[0], o = s.id; if (0 == r("#" + o + "canvas").length) { var n = r.extend({ backgroundColor: "#ffffff", fontColor: "#000000", fontSize: "40px", radius: 70, progressBarBackground: "#cdcdcd", progressBarColor: "#aaaaaa", progressBarWidth: 25, progressPercent: 0, progressValue: 0, showText: !0, title: "" }, t), i = parseInt(n.radius), l = parseInt(n.progressBarWidth), d = parseInt(parseInt(n.progressValue) > 0 ? n.progressValue : n.progressPercent), c = parseInt(n.progressPercent), p = "color:" + n.fontColor + ";font-size:" + parseInt(n.fontSize) + "px;width:" + 2 * (i + l) + "px;vertical-align:middle;position:relative;background-color:transparent;border:0 none;transform:translateY(-48%);-webkit-transform: translateY(-48%);-ms-transform: translateY(-48%);height:" + 2 * (i + l) + "px;margin-left:-" + 2 * (i + l) + "px;text-align:center;padding:0;" + (n.showText ? "" : "display:none;"); r('<canvas data-width="' + l + '" data-radius="' + i + '" data-stroke="' + n.progressBarColor + '" data-progressBarBackground="' + n.progressBarBackground + '" data-backgroundColor="' + n.backgroundColor + '" data-text=' + n.showText + " data-percent=" + (void 0 == t.progressValue ? !0 : !1) + ' id="' + o + 'canvas" width=' + 2 * (i + l) + " height=" + 2 * (i + l) + "></canvas>").appendTo(s), r('<input class="clProg" style="' + p + '" value="' + d + (void 0 == t.progressValue ? "%" : "") + '"></input>').appendTo(s), "" == n.title ? r("#" + o).css("height", 2 * (i + l)) : (r("#" + o).css("height", 2 * (i + l) + 20), r("#" + o + "canvas").before("<div class='titleCircularLoader' style='height:19px;text-align:center;'>" + n.title + "</div>"), r(".titleCircularLoader").css("width", 2 * (i + l))); { var g = r("#" + o + "canvas")[0], h = g.getContext("2d"), u = g.width / 2, f = g.height / 2; r("#" + o + "canvas").offset().left, r("#" + o + "canvas").offset().top } a() } else if (void 0 != t.progressPercent || void 0 != t.progressValue) { var c = 0, d = 0; c = void 0 == t.progressPercent ? parseInt(t.progressValue) > 100 ? 100 : parseInt(t.progressValue) : parseInt(t.progressPercent) > 100 ? 100 : parseInt(t.progressPercent), d = void 0 == t.progressValue ? parseInt(t.progressPercent) > 100 ? 100 : parseInt(t.progressPercent) : parseInt(t.progressValue), e(c, d, o) } return this } }(jQuery);