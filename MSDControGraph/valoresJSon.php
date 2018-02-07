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

// EJECUCION DE CLASE
$mysql = new MysqlConn();
$mysql->connect();

$numMaquina = $_GET['host'];

$sqlQuery = "SELECT (unix_timestamp(timestamp)*1000) as fechaUnix, temp FROM `controlmsd` where host = '".$numMaquina."'";

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

header('Content-type: application/json');
echo "[".join(',',$values)."]";

$mysql->close();

?>
