<?php
	class AttachmentsModel{
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
		
		public function GetThumbnailPath(string $original): string{
			$parts = pathinfo($original);
			$thumb = $parts['dirname'] . '/' . $parts['filename'] . '_thumbnail.' . $parts['extension'];
			return $thumb;
		}
		
		public function GetAttachmentById(int $id): array{
			$att = $this->db->getQuery('attachments', ['attachment_id' => $id]);
			if($att == null) throw new \Exception("Вложения с таким ID не найдено.");
			
			$originalPath = $_SERVER['DOCUMENT_ROOT'] . '/assets/default_file_type.png';
			$link = 'http://' . $_SERVER['SERVER_NAME'] . '/assets/default_file_type.png';
			
			if($att['attachment_type'] == 'photo'){
				$originalPath = $_SERVER['DOCUMENT_ROOT'] . '/uploads/' . $att['attachment_type'] . '/' . $att['attachment_link'];
				$link = 'http://' . $_SERVER['SERVER_NAME'] . '/uploads/' . $att['attachment_type'] . '/' . $att['attachment_link'];
			}
			
			$newAtt = [
				'id' => $att['attachment_id'],
				'owner' => $att['attachment_owner'],
				'photo' => $link,
				'type' => $att['attachment_type'],
			];
			
			return $newAtt;
		}
		
		public function Upload(int $ownerId): array{
			$count = FilesModel::newInstance()->GetFilesToUpload('file');
			if($count > 0){
				$newNames = [];
				for($i = 0; $i < $count; $i++){
					$newNames[] = UsersModel::newInstance()->randomPassword(10);
				}
				
				$files = FilesModel::newInstance()->UploadFiles($newNames);
				$attIds = [];
				$fileIndex = 0;
				foreach($files as $value){
					if(isset($value['error'])) $attIds[] = $value['error'];
					else{
						$attLink = $value['name'];
						$type = $value['type'];
						
						$insertData = [
							'attachment_owner' => $ownerId,
							'attachment_link' => $attLink,
							'attachment_type' => $type,
						];
						
						$att = $this->db->insertAndSelect('attachments', $insertData, 'attachment_id');
						$attIds[] = $att['attachment_id'];
					}
					
					$fileIndex++;
				}
				
				$newAtts = [];
				foreach($attIds as $attId){
					if(is_numeric($attId) && $attId > 0){
						$att = $this->GetAttachmentById((int)$attId);
						$newAtts[] = $att;
					}else $newAtts[] = $attId;
				}
				
				return $newAtts;
			}
			
			throw new \Exception('Не найдено файлов для загрузки');
		}
	}
?>