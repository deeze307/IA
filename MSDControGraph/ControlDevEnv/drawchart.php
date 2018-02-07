<HTML>
<TITLE>Reporte Correctivo</TITLE> 
<BODY>

<meta charset="utf-8"> 
<?php
require_once("RandomClass.php");

$maquinas = new RandomTable();
$rawMaquinas = $maquinas->getAllInfoFromMaquinas();

$i=0;
$array = array();
foreach($rawMaquinas as $maquina=>$valor){
    if($i!=20){
    if(in_array($valor[0], $array)){

    }elseif($valor[0]!=0){
        $i++;
        $array[] = trim($valor[0]);
        }
    }
}
?>

<div id="container" style="height: 400px; min-width: 310px"></div>

<script src="../Downloads/jquery.js"></script>
<script src="../highstock.js"></script>
<script src="../exporting.js"></script>
<script>

$(function () {
    var seriesOptions = [],
        seriesCounter = 0,
        names = ["<?php echo join('","', $array); ?>"];
        createChart = function () {

            $('#container').highcharts('StockChart', {

                rangeSelector: {
                    selected: 4
                },

                legend: {
                    enabled: true,
                    align: 'right',
                    backgroundColor: '#FCFFC5',
                    borderColor: 'black',
                    borderWidth: 2,
                    layout: 'vertical',
                    verticalAlign: 'top',
                    y: 100,
                    shadow: true
                },

                yAxis: {
                    labels: {
                        formatter: function () {
                            return (this.value > 0 ? ' + ' : '') + this.value + '%';
                        }
                    },
                    plotLines: [{
                        value: 0,
                        width: 2,
                        color: 'silver'
                    }]
                },

                plotOptions: {
                    series: {
                        compare: 'percent'
                    }
                },

                tooltip: {
                    pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.change}%)<br/>',
                    valueDecimals: 2
                },

                series: seriesOptions
            });
        };

    var contador=0;
    $.each(names, function (i, name) {

        $.getJSON('valoresJSon.php?idMaquina=' + name , function (data) {

            seriesOptions[i] = {
                name: name,
                data: data,
                visible: true
            };

            function incrementar() {
                if(contador==6)
                    alert('Maximo permitido alcanzado: 3');
                else {
                    contador++;
                    alert('El contador ahora vale :' + contador);}
            }

            seriesCounter += 1;
            if (seriesCounter === names.length) {
                createChart();
            }
        });
    });
});

</script>
</BODY>

</html>

