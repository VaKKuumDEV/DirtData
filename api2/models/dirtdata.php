<?php
class DirtDataModel{
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
	
	public function getById(int $id): array{
		$dd = $this->db->getQuery('dirtdata', ['data_id' => $id]);
		if($dd == null) throw new \Exception("Указанная запись не найдена");
		
		return $dd;
	}
	
	public function getForUser(string $hash): array{
		$user = UsersModel::newInstance()->getByHash($hash);
		
		$dd = $this->db->getQuery('dirtdata', ['data_sender' => $user['user_id']]);
		if($dd == null) $dd = [];
		if((count($dd) - count($dd, COUNT_RECURSIVE) == 0) and (count($dd) > 0)) $dd = [$dd];
		
		$newDd = [];
		foreach($dd as $d){
			$attachments = [];
			$attIds = explode(',', $d['data_attachments']);
			foreach($attIds as $id){
				try{
					$att = AttachmentsModel::newInstance()->GetAttachmentById($id);
					$attachments[] = $att;
				}catch(\Exception $ex){ }
			}
			
			$dataContent = json_decode(base64_decode($d['data_content']));
			
			$newD = [
				'id' => $d['data_id'],
				'longitude' => $d['data_longitude'],
				'latitude' => $d['data_latitude'],
				'attachments' => $attachments,
				'content' => $dataContent,
				'date' => $d['data_date'],
			];
			
			$newDd[] = $newD;
		}
		
		usort($newDd, function($a, $b){
			$res = 0;
			$dateA = $a['date'];
			$dateB = $b['date'];
			
			$dA = new \DateTime($dateA);
			$dB = new \DateTime($dateB);
			
			$res = ($dA < $dB) ? 1 : -1;
			
			return $res;
		});
		
		return $newDd;
	}
	
	public function send(string $hash, array $dataContent, float $longitude, float $latitude, array $attachmentsIds): void{
		$user = UsersModel::newInstance()->getByHash($hash);
		
		if(count($dataContent) == 0) throw new \Exception('Передан пустой массив значений');
		
		$checkedAttachmentsIds = [];
		foreach($attachmentsIds as $id){
			try{
				$att = AttachmentsModel::newInstance()->GetAttachmentById($id);
				$checkedAttachmentsIds[] = $att['id'];
			}catch(\Exception $ex){ }
		}
		
		$insertData = [
			'data_sender' => $user['user_id'],
			'data_content' => base64_encode(json_encode($dataContent)),
			'data_longitude' => $longitude,
			'data_latitude' => $latitude,
			'data_attachments' => implode(',', $checkedAttachmentsIds),
		];
		
		$this->db->insert('dirtdata', $insertData);
	}
}
?>