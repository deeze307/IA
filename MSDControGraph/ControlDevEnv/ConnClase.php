<?php
/**
 * Created by JetBrains PhpStorm.
 * User: sromero
 * Date: 13/11/14
 * Time: 10:51
 * To change this template use File | Settings | File Templates.
 */

class ConnClase {

    var $server = "10.30.51.51";
    var $database = "correctivos";
    var $user = "proevo";
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

    function getJsonDataFromDB($numMaquina){
        //Creamos la consulta
        $sql = "SELECT (unix_timestamp(fechaValor) * 1000) as  fechaValor, valorMaquina FROM maquinas where numeroMaquina ='".$numMaquina."'";
        //obtenemos el array con toda la informaci�n

        return $this->getArraySQL($sql);
    }
}