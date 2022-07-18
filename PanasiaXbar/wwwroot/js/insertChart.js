function AddXbarChart(id, data, title) {
    var echart = echarts.init(document.getElementById(id));
    //console.log(data);
    var xAxis = new Array();
    var xbar = new Array();
    var xbarUcl = new Array();
    var xbarCl = new Array();
    var xbarLcl = new Array();
    data.map(x => {
        xAxis.push(x.subGroupNo);
        xbar.push(x.xbar);
        xbarUcl.push(x.xbarUcl);
        xbarCl.push(x.xbarCl);
        xbarLcl.push(x.xbarLcl);
    });
    //console.log(xbar);
    //console.log(xbarUcl);
    var uclMax = Math.max(...xbarUcl);
    var lclMin = Math.min(...xbarLcl);
    var xbarMax = Math.max(...xbar);
    var xbarMin = Math.min(...xbar);

    var yAxisMax = Math.max(uclMax, xbarMax); //UCL과 측정값중 큰값
    var yAxisMin = Math.min(lclMin, xbarMin); //LCL과 측정값중 작은값
    
    //console.log(yAxisMax);
    var option = {
        title: {
            text: title
        },
        tooltip: {},
        legend: {
            data: ['Xbar', 'UCL', 'CL', 'LCL'],
            orient: 'vertical',
            right: 10,
            top:'center'

        },
        xAxis: {
            data: xAxis
        },
        yAxis: {
            type: 'value',
            min: yAxisMin,
            max: yAxisMax
        },
        series: [
            {
                name: 'Xbar',
                type: 'line',
                data: xbar
            },
            {
                name: 'UCL',
                type: 'line',
                data: xbarUcl,
                showSymbol:false,
                lineStyle: {
                    normal: {
                        type: 'solid'
                    }
                }
            },
            {
                name: 'CL',
                type: 'line',
                showSymbol: false,
                data: xbarCl
            },
            {
                name: 'LCL',
                type: 'line',
                showSymbol: false,
                data: xbarLcl
            },
        ]
    };
    echart.setOption(option);
}

function AddRangeChart(id, data, title) {
    var echart = echarts.init(document.getElementById(id));
    //console.log(data);
    var xAxis = new Array();
    var range = new Array();
    var rangeUcl = new Array();
    var rangeCl = new Array();
    var rangeLcl = new Array();
    data.map(x => {
        xAxis.push(x.subGroupNo);
        range.push(x.range);
        rangeUcl.push(x.rangeUcl);
        rangeCl.push(x.rangeCl);
        rangeLcl.push(x.rangeLcl);
    });
    //console.log(xbar);
    //console.log(xbarUcl);
    var uclMax = Math.max(...rangeUcl);
    var lclMin = Math.min(...rangeLcl);
    var xbarMax = Math.max(...range);
    var xbarMin = Math.min(...range);

    var yAxisMax = Math.max(uclMax, xbarMax);

    var yAxisMin = Math.min(lclMin, xbarMin);

    //console.log(yAxisMax);
    var option = {
        title: {
            text: title
        },
        tooltip: {},
        legend: {
            data: ['Range', 'UCL', 'CL', 'LCL'],
            orient: 'vertical',
            right: 10,
            top: 'center'
        },
        xAxis: {
            data: xAxis
        },
        yAxis: {
            type: 'value',
            min: yAxisMin,
            max: yAxisMax
        },
        series: [
            {
                name: 'Range',
                type: 'line',
                data: range
            },
            {
                name: 'UCL',
                type: 'line',
                data: rangeUcl,
                showSymbol: false,
                lineStyle: {
                    normal: {
                        type: 'solid'
                    }
                }
            },
            {
                name: 'CL',
                type: 'line',
                showSymbol: false,
                data: rangeCl
            },
            {
                name: 'LCL',
                type: 'line',
                showSymbol: false,
                data: rangeLcl
            },
        ]
    };
    echart.setOption(option);
}