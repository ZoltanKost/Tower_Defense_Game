using System;
using UnityEngine;
using System.Collections.Generic;

/*
    Event 0: Remove
    Event 1: Attack
    Event 2: ResetState
 */

public class EnemyManager : MonoBehaviour {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private WaveHighlighting waveHighlighting;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Character[] enemyPrefabs;
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private float waveDefaultSpawnRate = 10f;
    [SerializeField] private float spawnRate;
    private int shipSpawnRate;
    public Vector3 shipLookRotation = new Vector3(0, 0.5f, -1f);
    public Vector3 shipLookRotationOffset = new Vector3(0, 1f, -1f);
    public float basicShipSpeed = 5f;
    public Character[] enemies;
    public List<Wave> waves = new();
    public List<Ship> ships = new();
    public int lowestInactive = 0;
    bool active;
    float cellSize;
    Vector3 offset;
    public int waveNumber;
    public void Init(Action win, int _waveNumber = 0){
        waveNumber = _waveNumber;
        //GenerateWave(++waveNumber);
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
        TickShipMovement(delta);
        TickState(delta);
        TickMovement(delta);
        TickDetection();
        AnimatorTick(delta);
    }
    public void TickShipMovement(float dt)
    {
        int c = ships.Count;
        for (int i = 0; i < c; i++)
        {
            if (!ships[i].active) continue;
            ships[i].visual.position += (ships[i].destination - ships[i].visual.position).normalized * dt * basicShipSpeed;//ships[i].speed;
            ships[i].visual.rotation = Quaternion.LookRotation((ships[i].destination - ships[i].visual.position), shipLookRotation);// ().normalized * dt * ships[i].speed;
            if ((ships[i].destination - ships[i].visual.position).magnitude <= .1f)
            {
                if (ships[i].pathIndex > 1)
                {
                    var ship = ships[i];
                    var cell = floorManager.WorldToCell(ship.destination) + floorManager.offset;
                    ship.gridPosition = new Vector2Int(cell.x,cell.y);
                    ship.pathIndex--;
                    //Debug.Log(ship.path.Count + " " + ship.pathIndex);
                    PathCell next = ship.path[ship.pathIndex];
                    ship.destination = next.pos;
                    ships[i] = ship;
                }
                else
                {
                    ships[i].wave.ID = waves.Count;
                    waves.Add(ships[i].wave);
                    ships.RemoveAt(i);
                    waveHighlighting.SetHighlighting(waves, ships);
                    playerActionManager.locked = true;
                    i--;
                    c--;
                }
            }
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
                case CharState.Dead:
                    break;
                case CharState.Moving:
                    float degree = Vector2.SignedAngle(Vector2.right, (enemies[i].destination - enemies[i].transform.position).normalized);
                    if (degree < 0) degree += 360;
                    degree %= 360;
                    enemies[i].animator.Play(0);
                    enemies[i].time += delta;
                    enemies[i].detectFlag = enemies[i].time > enemies[i].attackPeriod;
                    break;
                case CharState.Attacking:
                    enemies[i].time = 0;
                    degree = Vector2.SignedAngle(Vector2.right, (enemies[i].buildingTarget.position - enemies[i].transform.position).normalized);
                    if (degree < 0) degree += 360;
                    degree %= 360;
                    enemies[i].animator.Play(1);
                    break;
                default:
                    enemies[i].state = CharState.Moving;
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
            enemies[i].buildingTarget = null;
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
                enemies[i].buildingTarget = buildings[k];
            }
            if(enemies[i].buildingTarget != null)
            {
                enemies[i].detectFlag = false;
                enemies[i].state = CharState.Attacking;
            }
        }
        
    }
    public void TickMovement(float delta)
    {
        for (int i = 0; i < lowestInactive; i++)
        {
            if (!(enemies[i].state == CharState.Moving) || enemies[i].HP <= 0) continue;
            enemies[i].transform.position += (enemies[i].destination - enemies[i].transform.position).normalized * delta * enemies[i].moveSpeed;
            if ((enemies[i].destination - enemies[i].transform.position).magnitude <= .1f)
            {
                if (enemies[i].pointsLeft > 0)
                {
                    enemies[i].pointsLeft--;
                    PathCell next = enemies[i].currentPath[enemies[i].pointsLeft];
                    enemies[i].destination = next.pos;
                    //enemies[i].animator.SetSortingParams(6 + 1000 / next.gridY, next.floor);
                }
                else 
                {
                    playerManager.Damage(enemies[i].castleDamage);
                    Debug.Log(enemies[i].castleDamage);
                    RemoveEnemy(i, enemies[i].waveIndex);
                }
            }
        }
    }

    public void AnimatorTick(float delta)
    {
        for(int x = 0; x < lowestInactive; x++){
            //enemies[x].animator.UpdateAnimator(delta);
            if (enemies[x].ProjectileFlag)
            {
                projectileManager.SendProjectile(enemies[x].projectileData);
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
        foreach(Character enemy in enemies){
            enemy.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        lowestInactive = 0;
        foreach(Character enemy in enemies){
            if(enemy != null){
                enemy.gameObject.SetActive(false);
            }
        }
        ships.Clear();
        waves.Clear();
        waveHighlighting.SetHighlighting(waves,ships);
    }
    public void SpawnEnemies() {
        Debug.Log(enemies.Length);
        if (enemies.Length == 0)
        {
            Debug.Log("Spawning 32 enemies");
            enemies = new Character[32];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = Instantiate(enemyPrefabs[0], transform);
            }
        }
    }

    public void DestroyEntities()
    {
        foreach(Character enemy in enemies){
            if(enemy != null) Destroy(enemy.gameObject);
        }
        lowestInactive = 0;
    }

    public void AreaSpell(SpellData spell, Vector3 position)
    {
        int reached = 0;
        for (int i = 0; i < lowestInactive; i++)
        {
            if (enemies[i].state == CharState.Dead) continue;
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
    public bool TryHighlightEntity(Vector3 position, out Character archer, float radius)
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
    public void GenerateWave(int wave, bool active)
    {
        waveNumber = wave;
        //waveHighlighting.ClearWaves();
        //pathfinding.CreatePossibleStarts(wave/5 + 1);
        //pathfinding.UpdatePaths();
        GenerateShips(wave, active);
        waveHighlighting.SetWaves(waves);
        waveHighlighting.SetShips(ships);
    }
    public void GenerateShips(int wave, bool active)
    {

        /* 
        1. Define Ship's Side: top,down,left or right
        2. Find a nearest land point starting from the castle
        3. Construct a path of ship to that possible start
        */
        int waveCount = wave / 10 + 1;
        int maxHalfTime = 0;
        for(int i = 0; i < waveCount; i++)
        {
            Camera cam = Camera.main;
            float Ysize = cam.orthographicSize * 2;
            float height = cam.pixelHeight / (Ysize);
            float width = cam.pixelWidth / (Ysize * cam.aspect);

            bool vertical = UnityEngine.Random.Range(0, 2) == 1;
            var floor = floorManager;
            int posX = - 1, posY = -1;
            bool fixedAxisMinimum = UnityEngine.Random.Range(0, 2) == 1;
            if (!vertical)
            {
                posX = UnityEngine.Random.Range(floor.edgeStartX, floor.edgeEndX + 1);
                posY = (fixedAxisMinimum?floor.edgeStartY - Mathf.FloorToInt(height) / 2 : floor.edgeEndY + Mathf.FloorToInt(height) / 2) ;
            }
            else
            {
                posY = UnityEngine.Random.Range(floor.edgeStartY, floor.edgeEndY + 1);
                posX = (fixedAxisMinimum ? floor.edgeStartX - Mathf.FloorToInt(width) / 2 : floor.edgeEndX + Mathf.FloorToInt(width) / 2) ;
            }
            //pathfinding.ships.Add(new Vector2Int(posX/10,posY/10));
            var path = new List<PathCell>(); 
            PossibleStart enemyStart = 
                pathfinding.GenereratePossibleStart(vertical,fixedAxisMinimum,out List<PathCell> enemyPath);
            posX = Mathf.Clamp(posX,0,99);
            posY = Mathf.Clamp(posY,0,99);
            var shipStart = floorManager.floorCells[posX, posY];
            Debug.Log($"{path.Count}, {enemyStart.pos}:{shipStart.gridX}:{shipStart.gridY}");
            pathfinding.FindPathBoat(
                shipStart,
                floor.floorCells[enemyStart.pos.x, enemyStart.pos.y],
                path);
            var visual = Instantiate(shipPrefab).transform;
            int enemyCount = (2 * waveNumber) / waveCount;
            var ship = new Ship
            {
                visual = visual,
                wave = new Wave(-1,
                    enemyCount,
                    enemyStart,
                    enemyPath, spawnRate, visual),
                gridPosition = new Vector2Int(shipStart.gridX, shipStart.gridY),
                //occupiedPositions;
                path = path,
                pathIndex = path.Count - 1,
                speed = basicShipSpeed,
                destination = path[path.Count - 1].pos,
                active = active
            };
            int value = enemyCount + path.Count * 2 / 3 ;
            if (value > maxHalfTime) maxHalfTime = value;
            Debug.Log("maxHalfTime is " + maxHalfTime);
            ship.visual.transform.position = ship.destination;
            ships.Add(ship);
            Debug.Log($"{path.Count}; {posX},{posY}: {width},{height}: {floor.edgeStartX},{floor.edgeEndX}: " +
                $"{floor.edgeStartY},{floor.edgeEndY}");
        }
        shipSpawnRate = maxHalfTime;
        Debug.Log(shipSpawnRate);
    }
    /*void OnDrawGizmos()
    {
        if (ships != null && ships.Count > 0)
        {
            foreach (var shipStruct in ships)
            {
                var ship = shipStruct.path;
                var offset = new Vector3(0, 0, 5);
                if (ship.Count < 1) continue;
                Vector3 prevCell = ship[0].pos;
                foreach (var nextCell in ship)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(prevCell + offset, nextCell.pos + offset);
                    prevCell = nextCell.pos;
                }
                foreach (var nextCell in shipStruct.wave.Path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(prevCell + offset, nextCell.pos + offset);
                    prevCell = nextCell.pos;
                }
            }
        }
        if (waves != null && waves.Count > 0)
        {
            foreach (var wave in waves)
            {
                var path = wave.Path;
                var offset = new Vector3(0, 0, 5);
                if (path.Count < 1) continue;
                Vector3 prevCell = path[0].pos;
                foreach (var nextCell in path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(prevCell + offset, nextCell.pos + offset);
                    prevCell = nextCell.pos;
                }
            }
        }
    }*/
    public void UpdateShips()
    {
        for (int i = 0; i < ships.Count; i++)
        {
            var ship = ships[i];
            var pos = ship.gridPosition;
            //Debug.Log(pos);
            var start = floorManager.floorCells[pos.x, pos.y];
            
            ship.wave.start = pathfinding.NeedMovePossibleStart(ship.wave.start,ship.wave.Path);
            pos = ship.wave.start.pos;
            ship.path.Clear();
            pathfinding.FindPathBoat(
                start, floorManager.floorCells[pos.x,pos.y],ship.path);
            ship.pathIndex = ship.path.Count;
            ships[i] = ship;
        }
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].Path.Clear();
            var start = waves[i].start;
            var cell = floorManager.floorCells[start.pos.x, start.pos.y];
            pathfinding.FindPath(cell, pathfinding.castlePosition, waves[i].Path);
        }
        waveHighlighting.SetHighlighting(waves,ships);
    }
    public void UpdatePaths()
    {
        for (int i = 0; i < ships.Count; i++)
        {
            var wave = ships[i].wave;

            wave.Path.Clear();
            var start = wave.start.pos;
            var cell = floorManager.floorCells[start.x, start.y];
            pathfinding.FindPath(
                cell, pathfinding.castlePosition, wave.Path);
        }
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].Path.Clear();
            var start = waves[i].start;
            var cell = floorManager.floorCells[start.pos.x, start.pos.y];
            pathfinding.FindPath(cell, pathfinding.castlePosition, waves[i].Path);
        }
        for (int i = 0; i < lowestInactive; i++)
        {
            enemies[i].currentPath.Clear();
            Vector3Int gridPos = floorManager.WorldToCell(enemies[i].position);
            gridPos.x += Mathf.FloorToInt(offset.x);
            gridPos.y += Mathf.FloorToInt(offset.y);
            FloorCell cell = floorManager.floorCells[gridPos.x, gridPos.y];
            pathfinding.FindPath(cell, pathfinding.castlePosition, enemies[i].currentPath);
            enemies[i].pointsLeft = enemies[i].currentPath.Count - 2;
            enemies[i].destination = enemies[i].currentPath[enemies[i].pointsLeft].pos;
        }
        waveHighlighting.SetHighlighting(waves,ships);
    }

    public void TickSpawn(float delta)
    {
        if(ships.Count < 1 && waves.Count < 1 && lowestInactive == 0)
        {
            playerActionManager.locked = false;
            GenerateWave(++waveNumber,true);
        }
        for (int i = 0; i < waves.Count; i++)
        {
            if (waves[i].count <= 0) 
            {
                waves[i].shipPrefabToDestroy.gameObject.SetActive(false);
                waves.RemoveAt(i);
                i--;
                continue;
            }
            waves[i].time += delta;
            if (waves[i].time >= waves[i].spawnRate)
            {
                waves[i].count--;
                SpawnEnemy(i);
                waves[i].time = 0;
            }
        }
    }
    public void SpawnEnemy(int ID)
    {
        Character enemy = enemies[lowestInactive++];
        enemy.Init(
            enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)],
            -1,-1,-1,ID, lowestInactive - 1, new Vector2Int(-1,-1), 
            waves[ID].Path, RemoveEnemy, RegisterKill,
            CharacterType.Enemy);
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
    public void RemoveEnemy(int ID, int waveID)
    {
        Debug.Log("removed");
        Character temp = enemies[ID];
        enemies[ID] = enemies[--lowestInactive];
        enemies[ID].index = ID;
        enemies[lowestInactive] = temp;
        temp.gameObject.SetActive(false);
    }
}
[Serializable]
public class Wave
{
    public int ID;
    public Transform shipPrefabToDestroy;
    public int count;
    public int Spawned;
    public PossibleStart start;
    public List<PathCell> Path;
    public float time;
    public float spawnRate;
    public Wave(int id, int count, PossibleStart _start, List<PathCell> path, float _spawnRate, Transform _shipPrefabToDestroy)
    {
        Spawned = 0;
        //Prefab = prefab;
        this.count = count;
        ID = id;
        Path = path;
        time = 0;
        spawnRate = _spawnRate;
        start = _start;
        shipPrefabToDestroy = _shipPrefabToDestroy;
    }
}
public struct Ship
{
    public Transform visual;
    public Wave wave;
    public Vector2Int gridPosition;
    public Vector2Int[] occupiedPositions;
    public Vector3 destination;
    public List<PathCell> path;
    public int pathIndex;
    public float speed;
    public bool active;
}