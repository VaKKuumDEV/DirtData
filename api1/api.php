<?php
ini_set('error_reporting', E_ALL);
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);

$answer = null;
if(isset($_REQUEST['method'])){
	$method = $_REQUEST['method'];
	include "database.php";
	include "servers.php";
	
	try{
		if($method == 'servers'){
			$servers = ServersModel::newInstance()->getAll();
			$answer = ['code' => 1, 'message' => 'Список доступных серверов', 'servers' => $servers];
		}
	}catch(\Exception $ex){
		$answer = ['code' => 0, 'message' => $ex->getMessage(), 'trace' => $ex->getTraceAsString()];
	}
}

header('Content-Type: application/json; charset=utf-8');
if($answer == null) $answer = ['code' => 0, 'message' => 'Переданы не все параметры.'];
echo json_encode($answer, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
?>