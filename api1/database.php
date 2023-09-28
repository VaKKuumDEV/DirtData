<?php
class Database{
	private $db;
	
	public function __construct(){
		mysqli_report(MYSQLI_REPORT_ERROR | MYSQLI_REPORT_STRICT);
		$this->db = new mysqli("localhost", "admin", "vfiyALc4oUIXGAabx31mBWjZWNmI5xBL", "krem");
		$this->db->set_charset('utf8mb4');
		
		if ($this->db->connect_errno) {
			throw new \RuntimeException('Ошибка соединения mysqli: ' . $this->db->connect_error);
		}
	}
	
	public function insert(string $table, array $data): bool{
		$keys = [];
		$values = [];
		
		foreach($data as $key => $value){
			$keys[] = '`' . $key . '`';
			if(is_numeric($value)) $values[] = $value;
			else $values[] = "'" . $this->real_string($value) . "'";
		}
		
		$sql = "INSERT INTO `" . $table . "`(" . implode(", ", $keys) . ") VALUES (" . implode(", ", $values) . ")";
		$result = $this->db->query($sql);
		return $result;
	}
	
	public function insertAndSelect(string $table, array $data, string $idColumn): ?array{
		$keys = [];
		$values = [];
		
		foreach($data as $key => $value){
			$keys[] = '`' . $key . '`';
			if(is_numeric($value)) $values[] = $value;
			else $values[] = "'" . $this->real_string($value) . "'";
		}
		
		$sql = "INSERT INTO `" . $table . "`(" . implode(", ", $keys) . ") VALUES (" . implode(", ", $values) . ")";
		$result = $this->db->query($sql);
		
		if($result){
			$sql = "SELECT * FROM " . $table . " ORDER BY " . $idColumn . " DESC LIMIT 1";
			$latest = $this->query($sql);
			if(is_array($latest)) return $latest;
		}
		
		return null;
	}
	
	public function query(string $sql): ?array{
		$result = $this->db->query($sql);
		if($result->num_rows == 1){
			$rows = $result->fetch_assoc();
			return $rows;
		}else if($result->num_rows > 1){
			$rows = [];
			while ($row = $result->fetch_assoc()) {
				$rows[] = $row;
			}
			return $rows;
		}
		
		return null;
	}
	
	public function update(string $sql): bool{
		$result = $this->db->query($sql);
		return $result;
	}
	
	public function real_string(string $str): string{
		return $this->db->real_escape_string($str);
	}
	
	public function getQuery($table, array $where = [], array $orders = [], ?int $limit = null): ?array{
		$sql = "SELECT * FROM " . $table;
		
		if(count($where) > 0){
			$w = [];
			foreach($where as $key => $value){
				if(!is_numeric($value)) $value = "'" . $value . "'";
				$w[] = $key . " = " . $value;
			}
			
			$sql .= " WHERE " . implode(" AND ", $w);
		}
		
		if(count($orders) > 0){
			$o = [];
			foreach($orders as $key => $value){
				$order = "DESC";
				if(strtolower($value) == "asc") $order = "ASC";
				$o[] = $key . " " . $order;
			}
			
			$sql .= " ORDER BY " . implode(" AND ", $o);
		}
		
		if(is_numeric($limit)){
			$sql .= " LIMIT " . $limit;
		}
		
		$result = $this->query($sql);
		return $result;
	}
}
?>