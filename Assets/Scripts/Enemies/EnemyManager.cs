using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Enemy[] enemyPrefabs;
    //[SerializeField] private 
    [SerializeField] private float spawnRate;
    Action onEnemyFinished;
    public Enemy[] enemies;
    Wave[] waves;
    public int lowestInactive = 0;
    bool active;
    float cellSize;
    Vector3 offset;
    int stage;
    public void Init(Action win){
        onEnemyFinished = win;
        offset = floorManager.offset;
        cellSize = floorManager.CellToWorld(Vector3.one).x;
    }
    public void Update(){
        if(!active) return;
        float delta = Time.deltaTime;
        TickSpawn(delta);
    }
    void FixedUpdate(){
        if(!active) return;
        float delta = Time.fixedDeltaTime;
        TickState(delta);
        TickMovement(delta);
        TickDetection();
        AnimatorTick(Time.fixedDeltaTime);
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
    public void TickState(float delta)
    {
        for(int i = 0; i < lowestInactive; i++)
        {
            switch (enemies[i].state)
            {
                case EnemyState.dead:
                    break;
                case EnemyState.run:
                    enemies[i].animator.SetDirectionAnimation(0, (enemies[i].destination - enemies[i].transform.position).normalized);
                    enemies[i].time += delta;
                    enemies[i].detectFlag = enemies[i].time > enemies[i].attackPeriod;
                    break;
                case EnemyState.attack:
                    enemies[i].time = 0;
                    enemies[i].animator.SetDirectionAnimation(1, (enemies[i].currentTarget.position - enemies[i].transform.position).normalized);
                    break;
                default:
                    enemies[i].state = EnemyState.run;
                    break;
            }
        }
        
    }
    public void TickDetection()
    {
        BuildingObject[] buildings = buildingManager.bs;
        int length = buildingManager.Count;
        for (int i = 0; i < lowestInactive; i++)
        {
            if (enemies[i].HP <= 0 || !enemies[i].detectFlag) continue;
            float minDistance = enemies[i].attackRange * cellSize;
            for (int k = 1; k < length; k++)
            {
                if (buildings[k] == null || !(buildings[k].HP > 0)) continue;
                Vector2Int enemyGridPosition = new Vector2Int
                {
                    x = Mathf.FloorToInt((enemies[i].transform.position.x + offset.x) / cellSize),
                    y = Mathf.FloorToInt((enemies[i].transform.position.y + offset.y) / cellSize)
                };
                Vector2Int diff = enemyGridPosition - buildings[k].gridPosition;
                Vector2Int bDims = new Vector2Int
                {
                    x = Mathf.Clamp(diff.x,0, buildings[k].w - 1),
                    y = Mathf.Clamp(diff.y, 0, buildings[k].h - 1)
                };
                diff = diff - bDims;
                float distance = (new Vector3(diff.x * cellSize, diff.y * cellSize)).magnitude;
                //if(i == 0 && k == 1)Debug.Log($"pos: {pos}, distance: {distance}, bDimensions: {bDims}, diff: {diff}");
                if (distance > minDistance) continue;
                minDistance = distance;
                enemies[i].currentTarget = buildings[k];
            }
            if(enemies[i].currentTarget != null)
            {
                enemies[i].detectFlag = false;
                enemies[i].state = EnemyState.attack;
            }
        }
        
    }
    public void TickMovement(float delta)
    {
        for (int i = 0; i < lowestInactive; i++)
        {
            if (!(enemies[i].state == EnemyState.run) || enemies[i].HP <= 0) continue;
            enemies[i].transform.position += (enemies[i].destination - enemies[i].transform.position).normalized * delta * enemies[i].speed;
            if ((enemies[i].destination - enemies[i].transform.position).magnitude <= .1f)
            {
                if (enemies[i].currentPath.Count > 0) 
                {
                    PathCell next = enemies[i].currentPath.Dequeue();
                    enemies[i].destination = next.pos;
                    enemies[i].pointsLeft = enemies[i].currentPath.Count;
                    enemies[i].animator.SetSortingParams(6 + 1000 /next.gridY,next.floor);
                }
                else enemies[i].DamageCastle();
            }
        }
        
    }

    public void AnimatorTick(float delta)
    {
        for(int x = 0; x < lowestInactive; x++){
            enemies[x].UpdateAnimator(delta);
            if (enemies[x].ProjectileFlag)
            {
                projectileManager.SendProjectile(enemies[x].ProjectileData);
                enemies[x].ProjectileFlag = false;
            }
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
                enemy.gameObject.SetActive(false);
            }
        }
    }
    public void SpawnEnemies(int wave) {
        stage = 0;
        GenerateWave(wave);
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
            if (enemies[i].state == EnemyState.dead) continue;
            //float distance = Vector3.Distance(, position);
            float dX = Mathf.Abs(enemies[i].transform.position.x - position.x);
            float dY = Mathf.Abs(enemies[i].transform.position.y - position.y);

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
            Vector3 pos = temp.transform.position - position;
            pos.z = 0;
            if (pos.magnitude > radius) continue;
            archer = temp;
            return true;
        }
        return false;
    }
    public void GenerateWave(int wave)
    {
        List<Queue<PathCell>> paths = pathfinding.vectors;
        int max = paths.Count;
        int waveCount = UnityEngine.Random.Range(1,Mathf.Min(wave + 1, max + 1));
        waves = new Wave[max];
        for (int i = 0; i < max; i++)
        {
            Debug.Log($"Generating Wave; wave*path.Count:{wave * max}");
            waves[i] = new Wave(i,
                    UnityEngine.Random.Range(wave, wave * 5),
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
        enemy.Init(waves[ID].Prefab, ID, lowestInactive - 1, waves[ID].Path, RemoveEnemy, RegisterKill, playerManager.Damage);
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
    public Queue<PathCell> Path;
    public float time;
    public float spawnRate;
    public Wave(int id, int count, Enemy prefab, Queue<PathCell> path, float _spawnRate)
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