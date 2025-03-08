using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private WaveHighlighting waveHighlighting;
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
    public List<Wave> waves = new();
    public int lowestInactive = 0;
    int enemyNumber = 0;
    bool active;
    float cellSize;
    Vector3 offset;
    public int waveNumber;
    public void Init(Action win, int _waveNumber = 0){
        waveNumber = _waveNumber;
        //GenerateWave(++waveNumber);
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
        AnimatorTick(delta);
        if (lowestInactive == 0)
        {
            bool left = false;
            foreach (Wave wave in waves)
            {
                if(wave.count > 0)
                {
                    left = true;
                }
            }
            if (left) return;
            onEnemyFinished?.Invoke();
            GenerateWave(++waveNumber);
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
            enemies[i].currentTarget = null;
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
                if (enemies[i].pointsLeft > 0) 
                {
                    enemies[i].pointsLeft--;
                    PathCell next = enemies[i].currentPath[enemies[i].pointsLeft];
                    enemies[i].destination = next.pos;
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
    public void SpawnEnemies() {
        Debug.Log(enemies.Length);
        if (enemies.Length == 0)
        {
            Debug.Log("Spawning 32 enemies");
            enemies = new Enemy[32];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = Instantiate(enemyPrefabs[0], transform);
            }
        }
    }

    public void DestroyEntities()
    {
        foreach(Enemy enemy in enemies){
            if(enemy != null) Destroy(enemy.gameObject);
        }
        lowestInactive = 0;
    }

    public void AreaSpell(SpellData spell, Vector3 position)
    {
        int reached = 0;
        for (int i = 0; i < lowestInactive; i++)
        {
            if (enemies[i].state == EnemyState.dead) continue;
            Vector3 enemyPos = enemies[i].transform.position;
            float dX = Mathf.Abs(enemyPos.x - position.x);
            float dY = Mathf.Abs(enemyPos.y - position.y);

            if (dX > spell.radius || dY > spell.radius) continue;
            enemies[i].Damage(spell.damage);
            reached++;
        }
        string s = $"Area spell Cast, reached: { reached}spell.damage = {spell.damage}, spell.radius = {spell.radius}";
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
    /*public void RegenerateWave()
    {
        GenerateWave(waveNumber);
    }*/
    public void GenerateWave(int wave)
    {
        waveNumber = wave;
        //waveHighlighting.ClearWaves();
        pathfinding.CreatePossibleStarts(wave/10 + 1);
        //pathfinding.UpdatePaths();
    }
    public void ShowWave()
    {
        //Debug.Log("CalculatingWave");
        enemyNumber = waveNumber * 2 + waveNumber / 2;
        List<List<PathCell>> paths = pathfinding.paths;
        if (paths.Count < 1) 
        {
            waveHighlighting.ClearWaves();
            return; 
        }
        waves.Clear();
        foreach (var path in paths)
        {
            if (path.Count < 1) 
            {
                Debug.Log("path is shorter then one"); continue;
            }
            //int maxCells = path.Count;
            //Debug.Log($"maxCells: {maxCells}");
            var wave = new Wave(waves.Count,
                    enemyNumber,
                    path, spawnRate);
            waves.Add(wave);
            //Debug.Log($"wave {waves.Count - 1}, count: {wave.count} {enemyNumber} {numForWave}, paths: {wave.Paths.Count}");
        }
        waveHighlighting.SetWaves(waves);
    }
    public void TickSpawn(float delta)
    {
        for(int i = 0; i < waves.Count; i++)
        {
            if (waves[i].count <= 0) continue;
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
        enemy.Init(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], ID, lowestInactive - 1, waves[ID].Path, RemoveEnemy, RegisterKill, playerManager.Damage);
        waves[ID].count--;
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
public class Wave
{
    public int ID;
    //public Enemy Prefab;
    public int count;
    public int Spawned;
    public List<PathCell> Path;
    public float time;
    public float spawnRate;
    public Wave(int id, int count, List<PathCell> path, float _spawnRate)
    {
        Spawned = 0;
        //Prefab = prefab;
        this.count = count;
        ID = id;
        Path = path;
        time = 0;
        spawnRate = _spawnRate;
    }
}