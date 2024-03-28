using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int enemyPoolCount;
    public Enemy[] enemies;
    int lowestInactive;
    [SerializeField] private float timeToSpawn = 10f;
    // [SerializeField] private int EnemyCount = 10;
    float time;
    bool active;
    void Awake(){
        enemies = new Enemy[enemyPoolCount];
        for(int i = 0; i < enemyPoolCount; i++){
            enemies[i] = Instantiate(enemyPrefab, transform);
            enemies[i].index = i;
        }
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
        float delta = Time.fixedDeltaTime;
        for(int x = 0; x < enemies.Length; x++){
            if(!enemies[x].active) continue;
            enemies[x].Tick(delta);
        }
    }
    private void SpawnEnemy(){
        if(lowestInactive >= enemies.Length){
            Debug.Log($"{lowestInactive}, {enemies.Length}");
            lowestInactive = 0;
        }
        Enemy enemy = enemies[lowestInactive++];
        enemy.gameObject.SetActive(true);
        var path = pathfinding.GetRandomPath();
        enemy.Init(path, path.Peek(),true, RemoveEnemy, playerManager.Damage);
    }
    public void Activate(){
        active = true;
    }
    public void Deactivate(){
        active = false;
        time = 0;
        lowestInactive = 0;
        foreach(var enemy in enemies){
            RemoveEnemy(enemy.index);
        }
    }
    void RemoveEnemy(int index){
        enemies[index].active = false;
        enemies[index].gameObject.SetActive(false);
    }
}