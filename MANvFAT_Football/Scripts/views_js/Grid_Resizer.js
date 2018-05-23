function resizeGrid() {
    var gridElement = $(".body-content");
    var dataArea = gridElement.find(".k-grid-content");

    if (dataArea != undefined) {
        var WindowHeight = $(window).height();

        var newHeight = WindowHeight - 300;
        gridElement.height(newHeight);
        dataArea.height(newHeight);
    }
    //$("#divInfo").text(" WindowHeight = " + WindowHeight + " diff = " + diff);
}

$(window).resize(function () {
    resizeGrid();
});

$(window).on("load", function myfunction() {
    resizeGrid();
});

