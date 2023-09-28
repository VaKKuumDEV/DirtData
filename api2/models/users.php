<?php
class UsersModel{
	private static $instance;
	private $db;
	private $cachedUsers = [];
	
	public static function newInstance(){
		if(!self::$instance instanceof self){
			self::$instance = new self;
		}
		return self::$instance;
	}
	
	public function __construct(){
		$this->db = new Database();
	}
	
	private function get_cached_user(array $where): ?array{
		foreach($this->cachedUsers as $cachedUser){
			foreach($where as $key => $value){
				if(isset($cachedUser[$key])){
					if($cachedUser[$key] == $value) return $cachedUser;
				}
			}
		}
		
		return null;
	}
	
	private function update_cached_user(array $where, array $user): void{
		foreach($this->cachedUsers as $index => $cachedUser){
			foreach($where as $key => $value){
				if(isset($cachedUser[$key])){
					if($cachedUser[$key] == $value){ $this->cachedUsers[$index] = $user; break 2; }
				}
			}
		}
		
		$this->cachedUsers[] = $user;
	}
	
	private function CheckUserPassword($password, $hash): bool{
		return password_verify($password, $hash) ? true : (sha1($password) == $hash);
	}
	
	public function randomPassword($length = 60): string {
		$alphabet = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890';
		if($length < 6) $length = 6;
		
		$pass = array();
		$alphaLength = strlen($alphabet) - 1;
		for ($i = 0; $i < $length; $i++) {
			$n = rand(0, $alphaLength);
			$pass[] = $alphabet[$n];
		}
		return implode($pass);
	}
	
	public function getByHash(string $hash): array{
		if(($cachedUser = $this->get_cached_user(['user_hash' => $hash])) != null) return $cachedUser;
		
		$user = $this->db->getQuery('users', ['user_hash' => $hash]);
		if($user == null) throw new \Exception("Указанный пользователь не найден");
		
		$this->update_cached_user(['user_id' => $user['user_id']], $user);
		
		unset($user['user_password']);
		return $user;
	}
	
	public function getById(int $id): array{
		if(($cachedUser = $this->get_cached_user(['user_id' => $id])) != null) return $cachedUser;
		
		$user = $this->db->getQuery('users', ['user_id' => $id]);
		if($user == null) throw new \Exception("Указанный пользователь не найден");
		
		$this->update_cached_user(['user_id' => $user['user_id']], $user);
		
		unset($user['user_password']);
		return $user;
	}
	
	public function getAccount(string $hash): array{
		$user = $this->getByHash($hash);
		
		$newUser = [
			'id' => $user['user_id'],
			'login' => $user['user_login'],
			'reg_date' => $user['user_reg_date'],
			'status' => $user['user_status'],
		];
		
		return $newUser;
	}
	
	public function changePassword(int $id, string $newPassword): void{
		if(mb_strlen($newPassword) < 8) throw new \Exception('Минимальная длина пароля - 8 символов');
		$user = UsersModel::newInstance()->getById($id);
		$passwordHash = password_hash($newPassword, PASSWORD_BCRYPT);
		$hash = $this->randomPassword(60);
		
		$sql = 'UPDATE users SET user_password = \''. $passwordHash . '\', user_hash = \'' . $hash . '\' WHERE user_id = ' . $user['user_id'];
		$this->db->update($sql);
		
		$cacheUser = $this->db->getQuery('users', ['user_id' => $user['user_id']]);
		$this->update_cached_user(['user_id' => $cacheUser['user_id']], $cacheUser);
	}
	
	public function login(string $login, string $password): string{
		$user = $this->db->getQuery('users', ['user_login' => $login]);
		if($user == null) throw new \Exception("Пользователя с таким логином не существует");
		
		if(!$this->CheckUserPassword($password, $user['user_password'])) throw new \Exception("Указан неверный пароль.");
		if($user['user_status'] == 0) throw new \Exception('Ваш аккаунт заблокирован. Вход невозможен.');
		
		$hash = $user['user_hash'];
		return $hash;
	}
	
	public function register(string $login, string $password): string{
		$user = $this->db->getQuery('users', ['user_login' => $login]);
		if(is_array($user)) throw new \Exception("Пользователь с таким логином уже зарегистрирован.");
		
		if(mb_strlen($password) < 8) throw new \Exception('Минимальная длина пароля - 8 символов');
		if(mb_strlen($login) < 5) throw new \Exception('Минимальная длина логина - 5 символов');
		
		$passwordHash = password_hash($password, PASSWORD_BCRYPT);
		$hash = $this->randomPassword(60);
		
		$insertData = [
			'user_login' => $login,
			'user_password' => $passwordHash,
			'user_hash' => $hash,
		];
		
		$user = $this->db->insertAndSelect('users', $insertData, 'user_id');
		$this->update_cached_user(['user_id' => $user['user_id']], $user);
		
		return $hash;
	}
}
?>