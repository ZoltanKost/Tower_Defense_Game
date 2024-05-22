using UnityEngine;

public class EnemyManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private int enemyPoolCount;
    public Enemy[] enemies;
    int lowestInactive = 0;
    [SerializeField] private float timeToSpawn = 10f;
    // [SerializeField] private int EnemyCount = 10;
    float time;
    bool active;
    void Start(){
        enemies = new Enemy[enemyPoolCount];
        SpawnEnemies();
    }
    public void Update(){
        if(!active) return;
        time += Time.deltaTime;
        if(time >= timeToSpawn){
            time = 0;
            SpawnEnemy();
        }
        Tick(Time.deltaTime);
    }
    private void SpawnEnemy(){
        if(lowestInactive >= enemyPoolCount){
            Debug.Log($"{lowestInactive}, {enemyPoolCount}");
            lowestInactive = 0;
        }
        Enemy enemy = enemies[lowestInactive++];
        enemy.gameObject.SetActive(true);
        var path = pathfinding.GetRandomPath();
        enemy.Init(path, path.Peek(),true, RemoveEnemy, playerManager.Damage);
        enemy.SetEnemyPool(buildingManager.bs.ToArray());
    }
    void RemoveEnemy(int index){
        enemies[index].gameObject.SetActive(false);
        enemies[index].active = false;
    }

    public void Tick(float delta)
    {
        for(int x = 0; x < enemies.Length; x++){
            if(!enemies[x].active) continue;
            enemies[x].Tick(delta);
        }
    }

    public void AnimatorTick(float delta)
    {
        
    }

    public void Switch(bool active)
    {
        this.active = active;
    }

    public void DeactivateEntities()
    {
        time = 0;
        foreach(Enemy enemy in enemies){
            enemy.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        foreach(Enemy enemy in enemies){
            if(enemy != null) Destroy(enemy.gameObject);
        }
        enemies = new Enemy[enemyPoolCount];
        SpawnEnemies();
    }
    public void SpawnEnemies(){
        for(int i = 0; i < enemyPoolCount; i++){
            int x = Random.Range(0, enemyPrefabs.Length); 
            enemies[i] = Instantiate(enemyPrefabs[x], transform);
            enemies[i].index = i;
            if(enemies[i].attackType == AttackType.Projectile){
                projectileManager.AddProjectile(enemies[i].GetProjectile());
            }
        }
    }
}