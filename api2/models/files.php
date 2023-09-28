<?php
class FilesModel{
	private static $instance;
	private $db;
	
	public static function newInstance(){
		if(!self::$instance instanceof self){
			self::$instance = new self;
		}
		return self::$instance;
	}
	
	public function __construct(){
		$this->db = new Database();
	}
	
	public function GetFilesToUpload(string $input_name): int{
		$count = 0;
		if (isset($_FILES[$input_name])){
			$files = [];
			$diff = count($_FILES[$input_name]) - count($_FILES[$input_name], COUNT_RECURSIVE);
			if ($diff == 0) {
				$files = array($_FILES[$input_name]);
			} else {
				foreach($_FILES[$input_name] as $k => $l) {
					foreach($l as $i => $v) {
						$files[$i][$k] = $v;
					}
				}
			}
			
			$count = count($files);
		}
		
		return $count;
	}
	
	public function UploadFiles(array $targetNames = []): array{
		$answer = null;
		// Название <input type="file">
		$input_name = 'file';

		// Разрешенные расширения файлов.
		$allow = ['png', 'jpg', 'jpeg'];

		// Запрещенные расширения файлов.
		$deny = [];

		// Директория куда будут загружаться файлы.
		$path = $_SERVER['DOCUMENT_ROOT'] . '/uploads/';
		
		$types = ['photo'];

		if (isset($_FILES[$input_name])) {
			// Проверим директорию для загрузки.
			if (!is_dir($path)) {
				mkdir($path, 0777, true);
			}

			// Преобразуем массив $_FILES в удобный вид для перебора в foreach.
			$files = array();
			$diff = count($_FILES[$input_name]) - count($_FILES[$input_name], COUNT_RECURSIVE);
			if ($diff == 0) {
				$files = array($_FILES[$input_name]);
			} else {
				foreach($_FILES[$input_name] as $k => $l) {
					foreach($l as $i => $v) {
						$files[$i][$k] = $v;
					}
				}		
			}	
			
			$loadedFiles = [];
			$inited = 0;
			foreach ($files as $file) {
				$error = $success = '';
				$fileType = null;

				// Проверим на ошибки загрузки.
				if (!empty($file['error']) || empty($file['tmp_name'])) {
					switch (@$file['error']) {
						case 1:
						case 2: $error = 'Превышен размер загружаемого файла.'; break;
						case 3: $error = 'Файл был получен только частично.'; break;
						case 4: $error = 'Файл не был загружен.'; break;
						case 6: $error = 'Файл не загружен - отсутствует временная директория.'; break;
						case 7: $error = 'Не удалось записать файл на диск.'; break;
						case 8: $error = 'PHP-расширение остановило загрузку файла.'; break;
						case 9: $error = 'Файл не был загружен - директория не существует.'; break;
						case 10: $error = 'Превышен максимально допустимый размер файла.'; break;
						case 11: $error = 'Данный тип файла запрещен.'; break;
						case 12: $error = 'Ошибка при копировании файла.'; break;
						default: $error = 'Файл не был загружен - неизвестная ошибка.'; break;
					}
				} elseif ($file['tmp_name'] == 'none' || !is_uploaded_file($file['tmp_name'])) {
					$error = 'Не удалось загрузить файл.';
				} else {
					// Оставляем в имени файла только буквы, цифры и некоторые символы.
					$pattern = "[^a-zа-яё0-9,~!@#%^-_\$\?\(\)\{\}\[\]\.]";
					$name = mb_eregi_replace($pattern, '-', $file['name']);
					$name = mb_ereg_replace('[-]+', '-', $name);

					// Т.к. есть проблема с кириллицей в названиях файлов (файлы становятся недоступны).
					// Сделаем их транслит:
					$converter = [
						'а' => 'a',   'б' => 'b',   'в' => 'v',    'г' => 'g',   'д' => 'd',   'е' => 'e',
						'ё' => 'e',   'ж' => 'zh',  'з' => 'z',    'и' => 'i',   'й' => 'y',   'к' => 'k',
						'л' => 'l',   'м' => 'm',   'н' => 'n',    'о' => 'o',   'п' => 'p',   'р' => 'r',
						'с' => 's',   'т' => 't',   'у' => 'u',    'ф' => 'f',   'х' => 'h',   'ц' => 'c',
						'ч' => 'ch',  'ш' => 'sh',  'щ' => 'sch',  'ь' => '',    'ы' => 'y',   'ъ' => '',
						'э' => 'e',   'ю' => 'yu',  'я' => 'ya', 

						'А' => 'A',   'Б' => 'B',   'В' => 'V',    'Г' => 'G',   'Д' => 'D',   'Е' => 'E',
						'Ё' => 'E',   'Ж' => 'Zh',  'З' => 'Z',    'И' => 'I',   'Й' => 'Y',   'К' => 'K',
						'Л' => 'L',   'М' => 'M',   'Н' => 'N',    'О' => 'O',   'П' => 'P',   'Р' => 'R',
						'С' => 'S',   'Т' => 'T',   'У' => 'U',    'Ф' => 'F',   'Х' => 'H',   'Ц' => 'C',
						'Ч' => 'Ch',  'Ш' => 'Sh',  'Щ' => 'Sch',  'Ь' => '',    'Ы' => 'Y',   'Ъ' => '',
						'Э' => 'E',   'Ю' => 'Yu',  'Я' => 'Ya',
					];

					$name = strtr($name, $converter);
					$parts = pathinfo($name);

					if (empty($name) || empty($parts['extension'])) {
						$error = 'Недопустимый тип файла';
					} elseif (!empty($allow) && !in_array(strtolower($parts['extension']), $allow)) {
						$error = 'Недопустимый тип файла';
					} elseif (!empty($deny) && in_array(strtolower($parts['extension']), $deny)) {
						$error = 'Данное расширение файла запрещено к загрузке';
					} else {
						if(count($targetNames) > 0){
							if(isset($targetNames[$inited])){
								$name = strtr($targetNames[$inited], $converter);
								$parts = pathinfo($targetNames[$inited] . '.' . $parts['extension']);
							}else continue;
						}
						
						if($parts['extension'] == 'jpg' or $parts['extension'] == 'jpeg' or $parts['extension'] == 'png') $fileType = $types[0];
						else $error = 'Тип файла не одобряется для хранения на сервере';
						
						$filePath = $path . $fileType . '/';
						if(!is_dir($filePath)) mkdir($filePath, 0777, true);
						
						// Чтобы не затереть файл с таким же названием, добавим префикс.
						$i = 1;
						$prefix = '';
						while(is_file($filePath . $parts['filename'] . $prefix . '.' . $parts['extension'])) $prefix = '(' . ++$i . ')';
						$name = $parts['filename'] . $prefix . '.' . $parts['extension'];

						// Перемещаем файл в директорию.
						if ($fileType != null and move_uploaded_file($file['tmp_name'], $filePath . $name)){
							// Далее можно сохранить название файла в БД и т.п.
							$success = 'Файл \'' . $name . '\' успешно загружен.';
						}else if($error == null){
							$error = 'Не удалось загрузить файл.';
						}
					}
				}
				
				$inited++;
				
				if (!empty($success)) {
					$loadedFiles[] = ['path' => $filePath . $name, 'name' => $name, 'type' => $fileType];
				} else {
					$loadedFiles[] = ['error' => $error];
				}
			}
			
			return $loadedFiles;
		}else throw new \Exception("Не обнаружены файлы для загрузки.");
		
		return $answer;
	}
}
?>