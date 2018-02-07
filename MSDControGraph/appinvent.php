<?php
/*
 * Written By: ShivalWolf
 * Date: 2011/06/03
 * Contact: Shivalwolf@domwolf.net
 *
 * UPDATE 2011/04/05
 * The code now returns a real error message on a bad query with the mysql error number and its error message
 * checks for magic_quotes being enabled and strips slashes if it is. Its best to disable magic quotes still.
 * Checks to make sure the submitted form is a x-www-form-urlencode just so people dont screw with a browser access or atleast try to
 * Forces the output filename to be JSON to conform with standards
 *
 * UPDATE 2011/06/03
 * Code updated to use the Web Module instead of tinywebdb
 *
 * UPDATE 2013/12/26 and 2014/02/18
 * minor modifications by Taifun, puravidaapps.com
 *
 * UPDATE 2014/07/11
 * mysql API (deprecated) replaced by mysqli by Taifun
 */

/************************************CONFIG****************************************/
//DATABSE DETAILS//
$DB_ADDRESS="10.30.51.51";
$DB_USER="proevo";
$DB_PASS="apisql";
$DB_NAME="correctivos";

//SETTINGS//
//This code is something you set in the APP so random people cant use it. 
$SQLKEY="santi";

/************************************CONFIG****************************************/

//these are just in case setting headers forcing it to always expire
header('Cache-Control: no-cache, must-revalidate');

error_log(print_r($_POST,TRUE));

if( isset($_POST['query']) && isset($_POST['key']) ){              //checks if the tag post is there and if its been a proper form post
  //set content type to CSV (to be set here to be able to access this page also with a browser)
  header('Content-type: text/csv');

  if($_POST['key']==$SQLKEY){                                      //validates the SQL key
    $query=urldecode($_POST['query']);
    if(get_magic_quotes_gpc()){     //checks if the worthless pile of crap magic quotes is enabled and if it is, strip the slashes from the query
      $query=stripslashes($query);
    }
    $conn = new mysqli($DB_ADDRESS,$DB_USER,$DB_PASS,$DB_NAME);    //connect

    if($conn->connect_error){                                   //checks connection
      header("HTTP/1.0 400 Bad Request on the connect_error)");
      echo "ERROR Database Connection Failed: " . $conn->connect_error, E_USER_ERROR;   //reports a DB connection failure
    } else {
      $result=$conn->query($query);                             //runs the posted query
      if($result === false){
        header("HTTP/1.0 400 Bad Request on the $result === false");                        //sends back a bad request error
        echo "Wrong SQL: " . $query . " Error: " . $conn->error, E_USER_ERROR;  //errors if the query is bad and spits the error back to the client
      } else {
        if (strlen(stristr($query,"SELECT"))>0) {              //tests if its a SELECT statement
          $total = $result->num_rows;
		  
		  while ($rowsFromDb = $result->fetch_assoc()) 
		  {
			echo $rowsFromDb['name'];
			echo "\n\r";
		  }
		  
		  
		  //$outstream = fopen("php://temp", 'r+');                 //opens up a temporary stream to hold the data

          //$header = array();
          //while ($fieldinfo = $result->fetch_field()) {
           // array_push($header, $fieldinfo->name);
         // }
         // fputcsv($outstream, $header, ',', '"');                 //prints header row

         // $result->data_seek(0);
         // while($row = $result->fetch_assoc()){
         //   fputcsv($outstream, $row, ',', '"');                  //prints all data rows
        //  }
        //  fpassthru($outstream);
          //fclose($outstream);
        } else {
          header("HTTP/1.0 201 Rows");
          echo "AFFECTED ROWS: " . $conn->affected_rows;       //if the query is anything but a SELECT, it will return the number of affected rows
        }
      }
      $conn->close();                                          //closes the DB
    }
  } else {
     header("HTTP/1.0 400 Bad Request");
     echo "Bad Request on the header 1";                                          //reports if the secret key was bad
  }
} else {
        header("HTTP/1.0 400 Bad Request");
        echo "Bad Request on the header 2";
}
?>