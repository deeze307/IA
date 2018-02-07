<?php
// CLASE DE CONEXION
class MysqlConn  {
    var $server = "10.30.10.22";
    var $database = "cogiscan";
    var $user = "root";
    var $pass = "apisql";

    var $sql = null;

    function set($SERVER,$USER,$PASS,$DATABASE) {
        $this->server = $SERVER;
        $this->database = $DATABASE;
        $this->user = $USER;
        $this->pass = $PASS;

    }

    function connect() {
        $this->sql = new mysqli($this->server , $this->user , $this->pass, $this->database);
        if ($sql->connect_errno) {
            echo "Failed to connect to MySQL: (" . $sql->connect_errno . ") " . $sql->connect_error;
            die;
        }
        return $this->sql;
    }

    function query($query){
        $result = $this->sql->query($query);
        return $result;
    }
    function close(){
        $this->sql->close();
    }
}
?>

<?php

function  printTemp(){
    // EJECUCION DE CLASE
    $mysql = new MysqlConn();
    $mysql->connect();

    $sqlQuery = "SELECT (unix_timestamp(timestamp)*1000) as fechaUnix, temp FROM controlmsd ";
    $result = $mysql->query($sqlQuery);

    if($result->num_rows>0) {
        $values = array();
        while($row = $result->fetch_array(MYSQLI_ASSOC)) {
            if($row['temp']!=0){
                $values[] = "[".$row['fechaUnix'].",".str_replace(",", ".",$row['temp'])."]";
            }
        }
        $result->free();
    }
    echo "[".join(',',$values)."]";
}

function  printHumedad(){
    // EJECUCION DE CLASE
    $mysql = new MysqlConn();
    $mysql->connect();

    $sqlQuery = "SELECT (unix_timestamp(timestamp)*1000) as fechaUnix, hum FROM controlmsd";
    $result = $mysql->query($sqlQuery);

    if($result->num_rows>0) {
        $values = array();
        while($row = $result->fetch_array(MYSQLI_ASSOC)) {
            if($row['hum']!=0){
                $values[] = "[".$row['fechaUnix'].",".str_replace(",", ".",$row['hum'])."]";
            }
        }
        $result->free();
    }
    echo "[".join(',',$values)."]";
}
?>

<HTML>
<TITLE>Control</TITLE>
<BODY>

<meta charset="utf-8"> 
<?php
require_once("RandomClass.php");

$maquinas = new RandomTable();
$rawMaquinas = $maquinas->getAllInfoFromMaquinas();

$i=0;

?>

<div id="container" style="height: 400px; min-width: 310px"></div>

<script src="downloads/jquery.js"></script>
<script src="downloads/highstock.js"></script>
<script src="downloads/exporting.js"></script>
<script>

    $(function () {

        Highcharts.setOptions({
            global: {
                useUTC: false
            }
        });

        $('#container').highcharts('StockChart', {
            chart: {
                zoomType: 'x'
            },
            title: {
                text: 'Control de Temperatura - Humedad'
            },

            rangeSelector : {
                selected : 0
            },

            subtitle: {
                text: 'Planta 3 - Nave 2'
            },

            xAxis: {
                type: 'datetime'
            },
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '{value}%',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                offset: 90,
                title: {
                    text: 'Humedad',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                opposite: true
            }, { // Secondary yAxis
                title: {
                    text: 'Temperatura',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value} °C',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                }
            }],
            tooltip: {
                shared: true,
                valueDecimals: 2
            },
            legend: {
                layout: 'vertical',
                align: 'left',
                x: 90,
                verticalAlign: 'top',
                y: 50,
                floating: true,
                backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
            },
            series: [{
                name: 'Temperatura',
                type: 'spline',
                yAxis: 1,
                data:
                    <?php
                    printHumedad();
                    ?>,
                tooltip: {
                    valueSuffix: '°C'
                }

            }, {
                name: 'Humedad',
                type: 'spline',
                data:
                    <?php
                    printTemp();
                    ?>,

                tooltip: {
                    valueSuffix: '%'
                }
            }]
        });
    });

/*    var contador=0;
    $.each(names, function (i, name) {

        $.getJSON('valoresJSon.php?host=' + name , function (data) {

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
    });*/

</script>
</BODY>

</html>


