using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private int enemyPoolCount;
    public Enemy[] enemies;
    int Count;
    int lowestInactive = 0;
    [SerializeField] private float timeToSpawn = 10f;
    // [SerializeField] private int EnemyCount = 10;
    float time;
    bool active;
    void Start(){
        enemies = new Enemy[enemyPoolCount];
        Reset();
    }
    public void Update(){
        if(!active) return;
        time += Time.deltaTime;
        if(time >= timeToSpawn){
            time = 0;
            SpawnEnemy();
        }
    }
    private void FixedUpdate(){
        if(!active) return;
        float delta = Time.fixedDeltaTime;
        for(int x = 0; x < enemies.Length; x++){
            if(!enemies[x].active) continue;
            enemies[x].Tick(delta);
        }
    }
    private void SpawnEnemy(){
        if(lowestInactive >= Count){
            Debug.Log($"{lowestInactive}, {Count}");
            lowestInactive = 0;
        }
        Enemy enemy = enemies[lowestInactive++];
        enemy.gameObject.SetActive(true);
        var path = pathfinding.GetRandomPath();
        enemy.Init(path, path.Peek(),true, RemoveEnemy, playerManager.Damage);
        enemy.SetEnemyPool(buildingManager.bs.ToArray());
    }
    public void Activate(){
        active = true;
    }
    public void Deactivate(){
        active = false;
    }
    public void Reset(){
        Deactivate();
        time = 0;
        Count = enemyPoolCount;
        foreach(Enemy enemy in enemies){
            if(enemy != null)Destroy(enemy.gameObject);
        }
        for(int i = 0; i < enemyPoolCount; i++){
            int x = Random.Range(0, enemyPrefabs.Length); 
            enemies[i] = Instantiate(enemyPrefabs[x], transform);
            enemies[i].index = i;
            if(enemies[i].attackType == AttackType.Projectile){
                projectileManager.AddProjectile(enemies[i].GetProjectile());
            }
        }
    }
    void RemoveEnemy(int index){
        enemies[index].gameObject.SetActive(false);
        enemies[index].active = false;
    }
}