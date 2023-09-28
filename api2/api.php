<?php
ini_set('error_reporting', E_ALL);
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);

$answer = null;
if(isset($_REQUEST['method'])){
	$method = $_REQUEST['method'];
	include "models/database.php";
	include "models/users.php";
	include "models/categories.php";
	include "models/files.php";
	include "models/attachments.php";
	include "models/dirtdata.php";
	
	try{
		if($method == 'login'){
			if(isset($_REQUEST['login']) and isset($_REQUEST['password'])){
				$login = $_REQUEST['login'];
				$password = $_REQUEST['password'];
				
				$hash = UsersModel::newInstance()->login($login, $password);
				
				$answer = ['code' => 1, 'message' => 'Успещная авторизация.', 'hash' => $hash];
			}
		}else if($method == 'register'){
			if(isset($_REQUEST['login']) and isset($_REQUEST['password'])){
				$login = $_REQUEST['login'];
				$password = $_REQUEST['password'];
				
				$hash = UsersModel::newInstance()->register($login, $password);
				$answer = ['code' => 1, 'message' => 'Успешная регистрация.', 'hash' => $hash];
			}
		}else if($method == 'account'){
			if(isset($_REQUEST['hash'])){
				$hash = $_REQUEST['hash'];
				
				$user = UsersModel::newInstance()->getAccount($hash);
				$answer = ['code' => 1, 'message' => 'Ваш аккаунт', 'user' => $user];
			}
		}else if($method == 'categories'){
			$categories = CategoriesModel::newInstance()->getAll();
			$answer = ['code' => 1, 'message' => 'Поля для ввода', 'categories' => $categories];
		}else if($method == 'upload'){
			if(isset($_REQUEST['hash'])){
				$hash = $_REQUEST['hash'];
				
				$user = UsersModel::newInstance()->getByHash($hash);
				$loaded = AttachmentsModel::newInstance()->Upload($user['user_id']);
				
				$answer = ['code' => 1, 'message' => 'Файлы вложений загружены', 'attachments' => $loaded];
			}
		}else if($method == 'mydata'){
			if(isset($_REQUEST['hash'])){
				$hash = $_REQUEST['hash'];
				
				$data = DirtDataModel::newInstance()->getForUser($hash);
				$answer = ['code' => 1, 'message' => 'Ваши выгрузки', 'data' => $data];
			}
		}else if($method == 'send_data'){
			if(isset($_REQUEST['hash']) and isset($_REQUEST['content']) and isset($_REQUEST['longitude']) and isset($_REQUEST['latitude']) and isset($_REQUEST['attachments'])){
				$hash = $_REQUEST['hash'];
				$longitude = $_REQUEST['longitude'];
				$latitude = $_REQUEST['latitude'];
				$attachments = explode(',', $_REQUEST['attachments']);
				
				$content = json_decode($_REQUEST['content'], true);
				if(!is_array($content)) $content = [];
				else if($content == null) $content = [];
				
				DirtDataModel::newInstance()->send($hash, $content, floatval($longitude), floatval($latitude), $attachments);
				$answer = ['code' => 1, 'message' => 'Отчет успешно загружен'];
			}
		}
	}catch(\Exception $ex){
		$answer = ['code' => 0, 'message' => $ex->getMessage(), 'trace' => $ex->getTraceAsString()];
	}
}

header('Content-Type: application/json; charset=utf-8');
if($answer == null) $answer = ['code' => 0, 'message' => 'Переданы не все параметры.'];
echo json_encode($answer, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
?>