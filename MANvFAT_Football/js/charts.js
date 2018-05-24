    /*** Charts Vertical 1  ****/
    

    var OptionBarchartl = { 
            responsive: true, 
    };

    /*** For All Time  ****/
    var VBarData1 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [70, 70, 70, 70],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [98, 0, 98, 0],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [0, 50, 0, 0],
            backgroundColor: "#ff0000"
        }],
    };

     
    /*** For 7 days Time  ****/
    var VBarData2 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [73, 90, 80, 10],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [ 28, 45, 58, 20],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [ 20, 60, 70, 50],
            backgroundColor: "#ff0000"
        }],
    };

    /*** For 1 moth Time  ****/
    var VBarData3 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [173, 40, 30, 60],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [ 28, 25, 48, 80],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [ 75, 30, 67, 30],
            backgroundColor: "#ff0000"
        }],
    };

    /*** For 3 month Time  ****/
    var VBarData4 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [70, 70, 70, 70],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [98, 0, 98, 0],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [0, 50, 0, 0],
            backgroundColor: "#ff0000"
        }],
    };

    /*** For 6 month Time  ****/
    var VBarData5 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [89, 190, 120, 140],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [ 28, 145, 88, 97],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [ 70, 54, 89, 110],
            backgroundColor: "#ff0000"
        }],
    };

    /*** For 1 year Time  ****/
    var VBarData6 = {
        labels: ["week 1", "week 2", "Week 3", "week4",],
        datasets: [{ 
            label: 'Target 150 Mins',
            data: [173, 40, 30, 60],
            backgroundColor: "#d7d7d7"
        }, { 
            label: 'Over',
            data: [ 28, 25, 48, 80],
            backgroundColor: "#4bbb08"
        }, { 
            label: 'Under',
            data: [ 75, 30, 67, 30],
            backgroundColor: "#ff0000"
        }],
    };

    var ctx1 = document.getElementById("barchart").getContext('2d');
    var barChart = new Chart(ctx1, {
        type: 'bar',
        data:  VBarData1,
        options: OptionBarchartl,
    });

    var ctxOne2 = document.getElementById("barchart2").getContext('2d');
    var barChart = new Chart(ctxOne2, {
        type: 'bar',
        data:  VBarData2,
        options: OptionBarchartl,
    });

    var ctxOne3 = document.getElementById("barchart3").getContext('2d');
    var barChart = new Chart(ctxOne3, {
        type: 'bar',
        data:  VBarData3,
        options: OptionBarchartl,
    });

    var ctxOne4 = document.getElementById("barchart4").getContext('2d');
    var barChart = new Chart(ctxOne4, {
        type: 'bar',
        data:  VBarData4,
        options: OptionBarchartl,
    });

    var ctxOne5 = document.getElementById("barchart5").getContext('2d');
    var barChart = new Chart(ctxOne5, {
        type: 'bar',
        data:  VBarData5,
        options: OptionBarchartl,
    });

    var ctxOne6 = document.getElementById("barchart6").getContext('2d');
    var barChart = new Chart(ctxOne6, {
        type: 'bar',
        data:  VBarData6,
        options: OptionBarchartl,
    });

    /*** Charts Horizontal 2 ****/
    
    var OptionHorizontzl = { 
            responsive: true,

            scales: {
                xAxes: [{
                    stacked: true,
                }],
                yAxes: [{
                    stacked: true
                }]
            }, 
    };

    /*** For All Time  ****/
    var barChartData = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [10, 67, 23, 36, 24, 78, 34],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [78, 34, 56, 78, 34, 78, 89],
            backgroundColor: "#4bbb08"
        }],
    };

    /*** For 7 days Time  ****/
    var barChartData2 = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [40, 27, 23, 16, 64, 93, 45],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [186, 128, 74, 125, 97, 43, 162],
            backgroundColor: "#4bbb08"
        }],
    };

    /*** For 1 month Time  ****/
    var barChartData3 = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [180, 127, 73, 16, 14, 93, 45],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [186, 198, 74, 125, 97, 43, 162],
            backgroundColor: "#4bbb08"
        }],
    };

    /*** For 3 month Time  ****/
    var barChartData4 = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [120, 127, 173, 116, 124, 193, 95],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [86, 98, 174, 95, 127, 143, 162],
            backgroundColor: "#4bbb08"
        }],
    };

    /*** For 6 month Time  ****/
    var barChartData5 = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [ 140, 187, 173, 16, 14, 93, 45],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [ 286, 198, 74, 125, 97, 43, 162],
            backgroundColor: "#4bbb08"
        }],
    };

    /*** For Year Time  ****/
    var barChartData6 = {
        labels: ["Neck", "Right Bicep", "Left bicep", "Waist", "Hips", "Right Thigh", "Left Thigh"],
        datasets: [{
            label: 'Current Inches',
            data: [ 80, 97, 173, 16, 14, 93, 425],
            backgroundColor: "#ff0000"
        }, {
            label: 'Inches Lost',
            data: [186, 198, 74, 125, 97, 43, 162],
            backgroundColor: "#4bbb08"
        }],
    };

    var ctx2 = document.getElementById("Hrzntchart").getContext('2d');
    var horizontalBarchart = new Chart(ctx2, {
        type: 'horizontalBar',
        data: barChartData,
        options: OptionHorizontzl,
    });

    var ctx3 = document.getElementById("Hrzntchart2").getContext('2d');
    var horizontalBarchart = new Chart(ctx3, {
        type: 'horizontalBar',
        data: barChartData2,
        options: OptionHorizontzl,
    });

    var ctx4 = document.getElementById("Hrzntchart3").getContext('2d');
    var horizontalBarchart = new Chart(ctx4, {
        type: 'horizontalBar',
        data: barChartData3,
        options: OptionHorizontzl,
    });

    var ctx5 = document.getElementById("Hrzntchart4").getContext('2d');
    var horizontalBarchart = new Chart(ctx5, {
        type: 'horizontalBar',
        data: barChartData4,
        options: OptionHorizontzl,
    });

    var ctx6 = document.getElementById("Hrzntchart5").getContext('2d');
    var horizontalBarchart = new Chart(ctx6, {
        type: 'horizontalBar',
        data: barChartData5,
        options: OptionHorizontzl,
    });

    var ctx7 = document.getElementById("Hrzntchart6").getContext('2d');
    var horizontalBarchart = new Chart(ctx7, {
        type: 'horizontalBar',
        data: barChartData6,
        options: OptionHorizontzl,
    });
    
     

        

