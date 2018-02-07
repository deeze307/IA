<?php
class RandomTable{

    public $IDr = 0 ;
    //Funci�n que crea y devuelve un objeto de conexi�n a la base de datos y chequea el estado de la misma. 
    function conectarBD(){ 
            $server = "10.30.10.22";
            $usuario = "root";
            $pass = "apisql";
            $BD = "cogiscan";
            //variable que guarda la conexi�n de la base de datos
            $conexion = mysqli_connect($server, $usuario, $pass, $BD); 
            //Comprobamos si la conexi�n ha tenido exito
            if(!$conexion){ 
               echo 'Ha sucedido un error inexperado en la conexion de la base de datos<br>'; 
            } 
            //devolvemos el objeto de conexi�n para usarlo en las consultas  
            return $conexion; 
    }  
    /*Desconectar la conexion a la base de datos*/
    function desconectarBD($conexion){
            //Cierra la conexi�n y guarda el estado de la operaci�n en una variable
            $close = mysqli_close($conexion); 
            //Comprobamos si se ha cerrado la conexi�n correctamente
            if(!$close){  
               echo 'Ha sucedido un error inexperado en la desconexion de la base de datos<br>'; 
            }    
            //devuelve el estado del cierre de conexi�n
            return $close;         
    }

    //Devuelve un array multidimensional con el resultado de la consulta
    function getArraySQL($sql){
        //Creamos la conexi�n
        $conexion = $this->conectarBD();
        //generamos la consulta

        $result =  mysqli_query($conexion, $sql);

        $rawdata = array();
        //guardamos en un array multidimensional todos los datos de la consulta
        $i=0;
        while($row = mysqli_fetch_array($result))
        {   
            //guardamos en rawdata todos los vectores/filas que nos devuelve la consulta
            $rawdata[$i] = $row;
            $i++;
        }
        //Cerramos la base de datos
        $this->desconectarBD($conexion);
        //devolvemos rawdata
        return $rawdata;
    }
    //inserta en la base de datos un nuevo registro en la tabla usuarios
    function insertRandom(){
    	//Generamos un n�mero entero aleatorio entre 0 y 100
    	$ran = rand(0, 10);
        //creamos la conexi�n
        $conexion = $this->conectarBD();
        //Escribimos la sentencia sql necesaria respetando los tipos de datos
        $sql = "insert into random (valor) 
        values (".$ran.")";
        //hacemos la consulta y la comprobamos 
        $consulta = mysqli_query($conexion,$sql);
        if(!$consulta){
            echo "No se ha podido insertar en la base de datos<br><br>".mysqli_error($conexion);
        }
        //Desconectamos la base de datos
        $this->desconectarBD($conexion);
        //devolvemos el resultado de la consulta (true o false)
        return $consulta;
    }
    function getAllInfo(){
        //Creamos la consulta
        $sql = "SELECT * FROM random;";
        //obtenemos el array con toda la informaci�n
        return $this->getArraySQL($sql);
    }

    function getAllInfoFromMaquinas(){
        //Creamos la consulta
        $sql = "SELECT host FROM controlmsd group by host;";
        //obtenemos el array con toda la informaci�n
        return $this->getArraySQL($sql);
    }

    function getJsonData($numMaquina){
        //Creamos la consulta
        $sql = "SELECT (unix_timestamp(fechaValor) * 1000) as  fechaValor, valorMaquina FROM maquinas where numeroMaquina ='".$numMaquina."'";
        //obtenemos el array con toda la informaci�n
        return $this->getArraySQL($sql);
    }
}
    //-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/-*/
?>