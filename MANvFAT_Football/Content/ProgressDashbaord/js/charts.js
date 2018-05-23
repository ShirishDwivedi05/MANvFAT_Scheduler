window.chartColors = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};

/*** Charts Vertical ****/

var OptionBarchart = {
    responsive: true,
};


function CreateWeeklyActivityChart(timePeriod, canvasId) {
    var ctx = document.getElementById(canvasId).getContext('2d');
    var labels = [];
    var overs = [];
    var unders = [];
    var target150Mins = [];

    $.get("/Charts/GetWeeklyActivityData?period=" + timePeriod, function (result, status) {

        $(result.data).each(function (index, value) {
            labels.push(value.Week);
            overs.push(value.Over);
            unders.push(value.Under);
            target150Mins.push(value.Target150Mins);
        });

        var VBarData = {
            labels: labels,
            datasets: [{
                label: 'Target 150 Mins',
                data: target150Mins,
                backgroundColor: "#d7d7d7"
            }, {
                label: 'Over',
                data: overs,
                backgroundColor: "#4bbb08"
            }, {
                label: 'Under',
                data: unders,
                backgroundColor: "#ff0000"
            }],
        };


        var Barchart = new Chart(ctx, {
            type: 'bar',
            data: VBarData,
            options: OptionBarchart,
        });
    })

}


/*** Charts Horizontal ****/

var OptionHorizontzl = {
    responsive: false,

    scales: {
        xAxes: [{
            stacked: true,
        }],
        yAxes: [{
            stacked: true
        }]
    },
};


function CreateChart(timePeriod, canvasId) {
    var ctx = document.getElementById(canvasId).getContext('2d');
    var labels = [];
    var currentInches = [];
    var inchesLost = [];

    $.get("/Charts/GetInchesData?period=" + timePeriod, function (result, status) {

        $(result.data).each(function (index, value) {
            labels.push(value.PartName);
            currentInches.push(value.CurrentInch);
            inchesLost.push(value.InchLost);
        });

        var barChartData = {
            labels: labels,
            datasets: [{
                label: 'Current Inches',
                data: currentInches,
                backgroundColor: "#ff0000"
            }, {
                label: 'Inches Lost',
                data: inchesLost,
                backgroundColor: "#4bbb08"
            }],
        };
        var horizontalBarchart = new Chart(ctx, {
            type: 'horizontalBar',
            data: barChartData,
            options: OptionHorizontzl,
        });
    })

}


/*** Charts Horizontal ****/

var optionLine = {
    responsive: true,
    legend: {
        position: 'bottom',
    },
    hover: {
        mode: 'index'
    },
    scales: {
        xAxes: [{
            display: true,
            scaleLabel: {
                display: true,
                labelString: 'Actual Weight'
            }
        }],
        yAxes: [{
            display: true,
            scaleLabel: {
                display: true,
                labelString: 'Target Weight'
            }
        }]
    },
    title: {
        display: true,
        text: ''
    }
};


function CreateWeightChart(timePeriod, canvasId) {
    var ctx = document.getElementById(canvasId).getContext('2d');
    var actualWeights = [];
    var targetWeights = [];
    var projectedWeights = [];

    $.get("/Charts/GetWeightData?period=" + timePeriod, function (result, status) {

        $(result.data).each(function (index, value) {
            actualWeights.push(value.ActualWeight);
            targetWeights.push(value.TargetWeight);
            projectedWeights.push(value.ProjectedWeight);
        });
        var healths = ["Healthy", "Overweight", "Obese", "Obese II", "Obese III"]
        var lineChartData = {
            labels: healths,
            datasets: [
                {
                    label: 'Weight',
                    data: actualWeights,
                    backgroundColor: window.chartColors.red,
                    borderColor: window.chartColors.red,
                    fill: false,
                    borderDash: [5, 5],
                    pointRadius: 15,
                    pointHoverRadius: 10,
                    backgroundColor: "#ff0000"
                },
                {
                    label: 'Target Weight',
                    data: targetWeights,
                    backgroundColor: window.chartColors.blue,
                    borderColor: window.chartColors.blue,
                    fill: false,
                    borderDash: [5, 5],
                    pointRadius: [2, 4, 6, 18, 0, 12, 20],
                }

            ],
        };
        var linechart = new Chart(ctx, {
            type: 'line',
            data: lineChartData,
            options: optionLine,
        });
    })

}
