using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy enemyPrefab;
    ArrayList<Enemy> enemies = new();
    float timeToSpawn = 10f;
    float time;
    bool active;
    public void Update(){
        time += Time.deltaTime;
        if(time >= timeToSpawn){
            time = 0;
            SpawnEnemy();
        }
    }
    private void FixedUpdate(){
        float delta = Time.fixedDeltaTime;
        for(int x = 0; x < enemies.lastIndex; x++){
            if(enemies[x].active)
            enemies[x].Move(delta);
        }
    }
    private void SpawnEnemy(){
        Enemy enemy = Instantiate(enemyPrefab);
        var path = pathfinding.GetRandomPath();
        enemy.Init(path, path.Peek(),true);
        enemies.Add(enemy);
    }
    private void KillEnemy(int index){
        enemies[index].active = false;
    }
}