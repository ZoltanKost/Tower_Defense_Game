using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private int enemyPoolCount;
    Action onEnemyFinished;
    public List<Enemy> enemies;
    int lowestInactive = 0;
    int killed = 0;
    [SerializeField] private float timeToSpawn = 10f;
    float time;
    bool active;
    void Awake(){
        enemies = new List<Enemy>();
    }
    public void SetWinAction(Action win){
        onEnemyFinished = win;
    }
    public void Update(){
        if(!active) return;
        time += Time.deltaTime;
        if(time >= timeToSpawn){
            time = 0;
            SpawnEnemy();
        }
        AnimatorTick(Time.deltaTime);
    }
    void FixedUpdate(){
        if(!active) return;
        Tick(Time.fixedDeltaTime);
    }
    private void SpawnEnemy(){
        if(lowestInactive >= enemyPoolCount){
            return;
        }
        Enemy enemy = enemies[lowestInactive];
        enemy.gameObject.SetActive(true);
        var path = pathfinding.GetRandomPath();
        enemy.Init(path, path.Peek(),true, RemoveEnemy, RegisterKill, playerManager.Damage);
        enemy.SetEnemyPool(buildingManager.bs);
        lowestInactive++;
    }
    void RemoveEnemy(int index){
        enemies[index].gameObject.SetActive(false);
        enemies[index].active = false;
        killed++;
        // Debug.Log($"Removed enemy: {index}, removed total:{killed}");
        if(killed >= enemyPoolCount) onEnemyFinished?.Invoke();
    }
    public void RegisterKill(int index){
        playerResourceManager.AddResource(Resource.Gold, enemies[index].killReward);
    }

    public void Tick(float delta)
    {
        for(int x = 0; x < enemies.Count; x++){
            if(!enemies[x].active) continue;
            enemies[x].Tick(delta);
        }
    }

    public void AnimatorTick(float delta)
    {
        for(int x = 0; x < enemies.Count; x++){
            if(!enemies[x].active) continue;
            enemies[x].UpdateAnimator(delta);
        }
    }

    public void Switch(bool active)
    {
        this.active = active;
    }

    public void DeactivateEntities()
    {
        lowestInactive = 0;
        killed = 0;
        time = 0;
        foreach(Enemy enemy in enemies){
            enemy.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        lowestInactive = 0;
        killed = 0;
        time = 0;
        foreach(Enemy enemy in enemies){
            if(enemy != null){
                enemy.active = false;
                enemy.gameObject.SetActive(false);
            }
        }
    }
    public void SpawnEnemies(int complexity){
        enemyPoolCount = complexity * 5;
        foreach(var col in pathfinding.vectors){
            enemyPoolCount += col.Count;
        }
        killed = 0;
        lowestInactive = 0;
        time = 0;
        enemies.Clear();
        for(int i = 0; i < enemyPoolCount; i++){
            int x = UnityEngine.Random.Range(0, enemyPrefabs.Length);
            Enemy enemy = Instantiate(enemyPrefabs[x], transform);
            enemy.index = i; 
            enemies.Add(enemy);
        }
    }

    public void ClearEntities()
    {
        foreach(Enemy enemy in enemies){
            if(enemy != null) Destroy(enemy.gameObject);
        }
    }

    public void AreaSpell(SpellData spell, Vector3 position)
    {
        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(enemy.position, position) > spell.radius) continue;
            enemy.Damage(spell.damage);
        }
    }
}