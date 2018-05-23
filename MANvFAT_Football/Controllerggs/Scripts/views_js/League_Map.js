// window.initMap = function () { };
$(window).on("load", function () {
    // alert("now do it");
    initMap();

    $('#txtLocationSearch').keyup(function (e) {
        if (e.keyCode == 13) {
            Set_MAP_SearchLocation();
        }
    });
});

var iconBase = '/Images/MapPins/';

function ShowMarkerTip(id) {
    //it will be used to Display Marker accossiaced with each pin
    google.maps.event.trigger(MyMarkers["" + id + ""], "click");
}

var geocoder;
var map;
var MyMarkers = [];
var SearchMarker = undefined;

function initMap() {
    geocoder = new google.maps.Geocoder();

    var mapOptions = {
        center: new google.maps.LatLng(52.8071132, -4.7897752),
        zoom: 6,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);

    DrawMap_Markers();

    var input = document.getElementById('txtLocationSearch');
    var autocomplete = new google.maps.places.Autocomplete(input);
}

function DrawMap_Markers() {
    var infowindow = new google.maps.InfoWindow();
    var bounds = new google.maps.LatLngBounds();
    var latlngbounds = new google.maps.LatLngBounds();

    var marker;

    $.ajax({
        url: "/GoogleMaps/GetLeagueGoogleMAP",
        data: { searchByLeague: $("#txtSearchLeagueMap").val() },
        dataType: "json",
        type: "POST",
        error: function (e) {
        },

        success: function (data) {
            var ListOfItems_Html = "<li><div class='input-group'> <input style='color:black;' type='text' class='form-control' placeholder='Search by League name / Postcode' id='txtSearchLeagueMap' /> <span class='input-group-addon'> <i style='cursor:pointer;' class='glyphicon glyphicon-search' onclick='initMap();'></i> </span></div></li>"

            ListOfItems_Html = ListOfItems_Html + data.menu;

            $.each(data.MapLocations, function () {
                var i = 1, j = 0;

                var _LeagueID = this.LeagueID;
                var _LeagueName = this.LeagueName;
                var _City = this.City;
                var _Country = this.Country;
                var _Address = this.Address;
                var _PinColour = this.PinColour;
                var _NEWSTag = this.NEWSTag;
                var _RegistrationDate_Str = this.RegistrationDate_Str;
                var _LaunchDate_Str = this.LaunchDate_Str;
                var _Lat = this.Latitude;
                var _Lng = this.Longitude;
                var _PinImageURL = iconBase + 'pin_' + _PinColour + '_32x32.png';

                //ListOfItems_Html = ListOfItems_Html + "<li><label class='clsTipLabel'  onclick=\"ShowMarkerTip('" + _NEWSTag + "');\"><img class='imgMapPin' src='"+_PinImageURL+"' alt='pin' /> - " + _LeagueName + "</label></li>";

                var myLatlng = new google.maps.LatLng(parseFloat(_Lat), parseFloat(_Lng));

                // alert("_Lat = " + _Lat + " _Lng = " + _Lng +" myLatlng = " + myLatlng);
                {
                    //  map.setCenter(myLatlng);

                    marker = new google.maps.Marker({
                        map: map,
                        draggable: false,
                        //animation: google.maps.Animation.DROP,
                        position: myLatlng,

                        // lat: _Lat,
                        // lng: _Lng,
                        icon: _PinImageURL
                    });

                    //  alert("marker = " + marker.getPosition());

                    // marker.setIcon('~\images\pin_green.png');
                    latlngbounds.extend(myLatlng);

                    google.maps.event.addListener(marker, 'click', (function (marker, j) {
                        MyMarkers["" + _NEWSTag + ""] = marker;

                        return function () {
                            // alert('Name: ' + _Name + ' PostCode:' + _City + " PinColour: " + _PinColour);

                            var panel_class = "panel-primary";
                            if (_PinColour == "Green") {
                                panel_class = "GreenPanel";
                            }
                            else if (_PinColour == "Red") {
                                panel_class = "RedPanel";
                            }
                            else if (_PinColour == "Yellow") {
                                panel_class = "YellowPanel";
                            }

                            MarkerInfoHtml = "<div class='clsMapInfoMarkerWindow' >" +

                                // "<div class='row'>" +

                                "<div class='col-md-12'>" +

                                "<div class='panel panel-primary'>" +

                                "<div class='panel-heading " + panel_class + "'>" +

                                "<strong>" + _City + " - " + _LeagueName + "</strong>" +
                                "</div>" +
                                "<div class='panel-body'>" +

                                "<p>" + _Address + "</p>" +

                                "<p>" +
                                "<div class='videoplayer'><iframe src='//www.youtube.com/embed/SosjkQVo1S4' framborder='0' allowfullscreen='' width='300' height='150'></iframe></div>" +
                                "</p>" +
                                "<div style='float:left;'>" +
                                "<img class='markerpic' src='https://manvfat.com/wp-content/uploads/2016/09/ManvFat-FC-Badge.png' alt='ManvFat-FC-Badge' />" +
                                "</div>" +
                                "<div>" +

                                "<p> <strong>Join This League</strong><a target='_blank' href='https://www.manvfatfootball.org/Home/Registration?lid=" + _LeagueID + "'> Sign up and get playing</a></p>" +
                                "<p><strong>Description</strong> Got a question about joining this league? Check it hasn't already <a target='_blank' href='http://bit.ly/MVFFFAQ'>been answered on our FAQ</a> then give us a call or email us and we'll do our best to help.</p>" +
                                "<p><strong>Registration Date</strong> " + _RegistrationDate_Str + "</p>" +
                                "<p><strong>Launch Date</strong> " + _LaunchDate_Str + "</p>" +
                                "<p><strong>Email</strong> <a target='_blank' href='mailto:football@manvfat.com'>football@manvfat.com</a></p>" +
                                "<p><strong>Telephone</strong> 0845 163 0042</p>" +
                                "<p><strong>League homepage</strong> <a target='_blank' href='https://www.manvfatfootball.org/" + _NEWSTag + "'>Click here now</a>.</p>" +
                                "</div>" +
                                "</div>" +
                                "</div>" +
                                "</div>" +
                                //"</div>" +
                                "</div>";

                            infowindow.setContent(MarkerInfoHtml);

                            infowindow.open(map, marker);
                        }
                    })(marker, j));
                    j = j + 1;
                }
            });
            // myLatlng = { lat: parseFloat(52.8071132), lng: parseFloat(-4.7897752) };

            //map.fitBounds(latlngbounds);

            $("#ListOfItems").html(ListOfItems_Html);
        }
    });
}

function Set_MAP_SearchLocation() {
    var infowindow = new google.maps.InfoWindow();
    var latlngbounds = new google.maps.LatLngBounds();

    var _SearchAddress = $("#txtLocationSearch").val();
    //alert("Set_MAP_Centre - _SearchAddress = " + _SearchAddress);
    $.ajax({
        url: "/GoogleMaps/Get_MAP_Searched_LatLng",
        data: { SearchAddress: _SearchAddress },
        dataType: "json",
        type: "POST",
        error: function (e) {
        },

        success: function (data) {
            var _Lat = data.Latitude;
            var _Lng = data.Longitude;

            var myLatlng = new google.maps.LatLng(parseFloat(_Lat), parseFloat(_Lng));

            // alert("myLatlng = " + myLatlng);

            //Clear the previous search marker
            if (SearchMarker != undefined) {
                SearchMarker.setMap(null);
            }

            //Place new markter
            marker = new google.maps.Marker({
                map: map,
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: myLatlng,
                icon: iconBase + 'search_pin_32x32.png'
            });

            //search current search marker so we can remove when any new search took place
            SearchMarker = marker;

            latlngbounds.extend(myLatlng);
            map.fitBounds(latlngbounds);

            var zoomChangeBoundsListener =
                google.maps.event.addListenerOnce(map, 'bounds_changed', function (event) {
                    if (this.getZoom()) {
                        this.setZoom(13);
                    }
                });
            setTimeout(function () { google.maps.event.removeListener(zoomChangeBoundsListener) }, 2000);
        }
    });

    return false;
}

function HideShowListOfItems(val) {
    if (val == 'hide') {
        $(".clsdivListOfItems").hide();
        $(".clsdivListOfItems").removeClass("col-md-4");
        $(".clsdivMaps").removeClass("col-md-8");
        $(".clsdivMaps").addClass("col-md-12");
        $(".clsdivShowListOfItems").slideDown();
    }
    else if (val == 'show') {
        $(".clsdivListOfItems").slideDown();
        $(".clsdivListOfItems").addClass("col-md-4");
        $(".clsdivMaps").removeClass("col-md-12");
        $(".clsdivMaps").addClass("col-md-8");
        $(".clsdivShowListOfItems").slideUp();
    }
}