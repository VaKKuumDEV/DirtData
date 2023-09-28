<?php
class ServersModel{
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
	
	public function getAll(): array{
		$servers = $this->db->getQuery('servers', ['server_status' => 1]);
		if($servers == null) $servers = [];
		if((count($servers) - count($servers, COUNT_RECURSIVE) == 0) and (count($servers) > 0)) $servers = [$servers];
		
		$newServers = [];
		foreach($servers as $server){
			$newServers[] = [
				'name' => $server['server_name'],
				'address' => $server['server_address'],
			];
		}
		
		return $newServers;
	}
}
?>