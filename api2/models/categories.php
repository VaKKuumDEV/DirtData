<?php
class CategoriesModel{
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
		$category = $this->db->getQuery('categories', ['category_id' => $id]);
		if($category == null) throw new \Exception("Указанная категория не найдена");
		
		return $category;
	}
	
	public function getAll(): array{
		$categories = $this->db->getQuery('categories', ['category_status' => 1]);
		if($categories == null) $categories = [];
		if((count($categories) - count($categories, COUNT_RECURSIVE) == 0) and (count($categories) > 0)) $categories = [$categories];
		
		$newCats = [];
		foreach($categories as $c){
			if($c['category_parent'] == null) $newCats[$c['category_name']] = $this->recur($c['category_id'], $categories);
		}
		
		return $newCats;
	}
	
	private function recur(int $parentId, array &$allCategories): array{
		$children = [];
		
		foreach($allCategories as $c){
			if($c['category_parent'] == $parentId){
				$childChildren = $this->recur($c['category_id'], $allCategories);
				$children[$c['category_name']] = $childChildren;
			}
		}
		
		return $children;
	}
}
?>