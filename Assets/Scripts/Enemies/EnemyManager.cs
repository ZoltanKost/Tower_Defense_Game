using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager singleton{get;private set;}
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
        if(singleton == null) singleton = this;
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
            if(enemies[x].active)
            enemies[x].Move(delta);
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
        enemy.Init(path, path.Peek(),true, RemoveEnemy);
    }
    private void KillEnemy(int index){
        enemies[index].active = false;
    }
    public void Activate(){
        active = true;
    }
    public void Deactivate(){
        active = false;
    }
    void RemoveEnemy(int index){
        enemies[index].active = false;
        enemies[index].gameObject.SetActive(false);
    }
}