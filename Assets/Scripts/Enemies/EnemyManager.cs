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
    [SerializeField] private float spawnRate;
    Action onEnemyFinished;
    public Enemy[] enemies;
    Wave[] waves;
    public int lowestInactive = 0;
    bool active;
    public void SetWinAction(Action win){
        onEnemyFinished = win;
    }
    public void Update(){
        if(!active) return;
        TickSpawn(Time.deltaTime);
    }
    void FixedUpdate(){
        if(!active) return;
        AnimatorTick(Time.fixedDeltaTime);
        Tick(Time.fixedDeltaTime);
        if (lowestInactive == 0)
        {
            bool left = false;
            foreach (Wave wave in waves)
            {
                if(wave.Count > 0)
                {
                    left = true;
                }
            }
            if (left) return;
            onEnemyFinished?.Invoke();
        }
    }
    public void RegisterKill(int index){
        playerResourceManager.AddResource(Resource.Gold, enemies[index].killReward);
    }

    public void Tick(float delta)
    {
        for (int x = 0; x < lowestInactive; x++)
        {
            if (!enemies[x].active) continue;
            enemies[x].Tick(delta);
            if (enemies[x].ProjectileFlag)
            {
                projectileManager.SendProjectile(enemies[x].ProjectileData);
                enemies[x].ProjectileFlag = false;
            }
        }
    }

    public void AnimatorTick(float delta)
    {
        for(int x = 0; x < lowestInactive; x++){
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
        foreach(Enemy enemy in enemies){
            enemy.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        lowestInactive = 0;
        foreach(Enemy enemy in enemies){
            if(enemy != null){
                enemy.active = false;
                enemy.gameObject.SetActive(false);
            }
        }
    }
    public void SpawnEnemies(int wave) {
        GenerateWave(wave + 1);
        enemies = new Enemy[32];
        for (int i = 0; i < 32; i++)
        {
            enemies[i] = Instantiate(enemyPrefabs[0], transform);
        }
    }

    public void ClearEntities()
    {
        foreach(Enemy enemy in enemies){
            if(enemy != null) Destroy(enemy.gameObject);
        }
        lowestInactive = 0;
    }

    public void AreaSpell(SpellData spell, Vector3 position)
    {
        string s = "Area spell Cast...";
        for (int i = 0; i < lowestInactive; i++)
        {
            if (!enemies[i].active) continue;
            //float distance = Vector3.Distance(, position);
            float dX = Mathf.Abs(enemies[i].position.x - position.x);
            float dY = Mathf.Abs(enemies[i].position.y - position.y);

            if (dX > spell.radius || dY > spell.radius) continue;
            enemies[i].Damage(spell.damage);
            s += $"Damage id{i}: distanceX = {dX}, distanceY = {dY}";
        }
        Debug.Log(s);
    }
    public bool TryHighlightEntity(Vector3 position, out Enemy archer, float radius)
    {
        archer = null;
        if (!active) return false;
        for (int i = 0; i < lowestInactive; i++)
        {
            var temp = enemies[i];
            Vector3 pos = temp.position - position;
            pos.z = 0;
            if (pos.magnitude > radius) continue;
            archer = temp;
            return true;
        }
        return false;
    }
    public void GenerateWave(int wave)
    {
        List<Queue<Vector3>> paths = pathfinding.vectors;
        waves = new Wave[paths.Count];
        for (int i = 0; i < paths.Count; i++)
        {
            Debug.Log($"Generating Wave; wave*path.Count:{wave * paths.Count}");
            waves[i] = new Wave(i,
                    wave * paths.Count,
                    enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)],
                    paths[i], spawnRate);
        }
    }
    public void TickSpawn(float delta)
    {
        for(int i = 0; i < waves.Length; i++)
        {
            if (waves[i].Count <= 0) continue;
            waves[i].time += delta;
            if (waves[i].time >= waves[i].spawnRate )
            {
                SpawnEnemy(i);
                waves[i].time = 0;
            }
        }
    }
    public void SpawnEnemy(int ID)
    {
        Debug.Log("EnemySpawned");
        Enemy enemy = enemies[lowestInactive++];
        enemy.Init(waves[ID].Prefab, ID, lowestInactive - 1, waves[ID].Path, waves[ID].Path.Peek(),true, RemoveEnemy, RegisterKill, playerManager.Damage, buildingManager.bs);
        waves[ID].Count--;
        enemy.gameObject.SetActive(true);
        if (lowestInactive >= enemies.Length)
        {
            Resize();
        }
    }
    public void Resize()
    {
        int count = enemies.Length;
        Array.Resize(ref enemies, enemies.Length * 2);
        for (int i = count; i < enemies.Length; i++) 
        {
            enemies[i] = Instantiate(enemyPrefabs[0], transform);
        }
    }
    public void RemoveEnemy(int ID, int WaveID)
    {
        Enemy temp = enemies[ID];
        enemies[ID] = enemies[--lowestInactive];
        enemies[ID].index = ID;
        enemies[lowestInactive] = temp;
        temp.active = false;
        temp.gameObject.SetActive(false);
        Debug.Log("EnemyRemoved");
    }
}
[Serializable]
public struct Wave
{
    public int ID;
    public Enemy Prefab;
    public int Count;
    public int Spawned;
    public Queue<Vector3> Path;
    public float time;
    public float spawnRate;
    public Wave(int id, int count, Enemy prefab, Queue<Vector3> path, float _spawnRate)
    {
        Spawned = 0;
        Prefab = prefab;
        Count = count;
        ID = id;
        Path = path;
        time = 0;
        spawnRate = _spawnRate;
    }
}