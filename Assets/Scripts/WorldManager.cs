using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    [Header("Tiles")]
    public TileBase FOAM;
    public TileBase GROUND;
    public TileBase ROCK;
    public TileBase GRASS;
    public TileBase SHADOW;
    public TileBase LADDER;
    public TileBase GRASS_SHADOW;
    public TileBase SAND;
    public TileBase BRIDGE;
    public TileBase BRIDGE_ON_GROUND;
    public TileBase BRIDGE_SHADOW;
    public TileBase ATTACKRANGE_SHADOW;
    
    [Header("Tutorial")]
    [SerializeField] private bool tutorial;

    [Header("Managers")]
    [SerializeField] private FloorManager floorManager;
    // [SerializeField] private Floor nextTile;
    [SerializeField] private Building castle;
    [SerializeField] private TemporalFloor temporalFloor;
    [SerializeField] private Shop shop;
    [SerializeField] private ShopUI buildingUI;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private EventSubscribeButton PauseButton;
    [SerializeField] private PlayerManager playerManager; 
    [SerializeField] private MenuUIManager defeatMenuManager;
    [SerializeField] private MenuUIManager menuUIManager;
    //[SerializeField] private MenuUIManager winScreen;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private HealthBar playerManaBar;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private PlayerShopUIManager playerShopUIManager;
    [SerializeField] private GameLoadManager gameLoadManager;
    [SerializeField] private GroundArrayGenerator groundGenerator;
    [SerializeField] private GameSaveManager gameSaveManager;
    [SerializeField] private AudioManager musicManager;
    [SerializeField] private TutorialManager tutorialManager;
    private GameState gameState;
    int wave;
    
    void Awake(){
        StaticTiles.Init();
        StaticTiles.Bind(SHADOW, TileID.Shadow);
        StaticTiles.Bind(GROUND, TileID.Ground);
        StaticTiles.Bind(GRASS, TileID.Grass);
        StaticTiles.Bind(FOAM, TileID.Foam);
        StaticTiles.Bind(ROCK, TileID.Rock);
        StaticTiles.Bind(GRASS_SHADOW, TileID.GrassPieces);
        StaticTiles.Bind(SAND, TileID.Sand);
        StaticTiles.Bind(LADDER, TileID.Ladder);
        StaticTiles.Bind(BRIDGE, TileID.Bridge);
        StaticTiles.Bind(BRIDGE_ON_GROUND, TileID.BridgeOnGround);
        StaticTiles.Bind(BRIDGE_SHADOW, TileID.BridgeShadow);
        StaticTiles.Bind(ATTACKRANGE_SHADOW, TileID.AttackRangeShadow);
        temporalFloor.Init(0,"UI");
        bool fullscreen = true;
#if UNITY_WEBGL
        fullscreen = false;
#endif
        //Screen.SetResolution(1024, 576, fullscreen);
        playerHealthBar.gameObject.SetActive(true);
        playerResourceManager.Init();
        playerShopUIManager.Init(StartLevel);
        Action actionFailedCallback = () => 
        {
            playerInput.Deactivate();
            temporalFloor.GetAnimationTween().onKill += playerInput.Activate;
        };
        Action<int> destroyBuildingCb
        = (int ID) => {
            if(ID == 0) return;
            Debug.Log($"Removing{ID}");
            buildingManager.RemoveBuilding(ID, out int gX, out int gY, out int w, out int h);
            Debug.Log($"Removed{ID} on {gX},{gY}");
            floorManager.DestroyBuilding(gX,gY,w,h);
        };
        playerActionManager.Init(
            actionFailedCallback,
            temporalFloor,
            playerResourceManager.EnoughtResource,
            playerManager.RemoveMana,
            playerResourceManager.RemoveResource,
            null,
            destroyBuildingCb
        );
        Action cancelActionCallback = playerShopUIManager.CloseAll;
        cancelActionCallback += playerActionManager.CancelBuildingAction;
        playerInput.Init(
            temporalFloor,
            shop.ResetGroundArrays, 
            cancelActionCallback
        );
        PauseButton.Init(Pause);
        gameLoadManager.Init(Load);
        menuUIManager.Init(
            new Action[]
            {
                Unpause,Restart,Application.Quit,null, null, 
                () => { gameLoadManager.ReadSaveData(); menuUIManager.gameObject.SetActive(false); },
                ()=>{menuUIManager.gameObject.SetActive(false); gameSaveManager.gameObject.SetActive(true); }
            });
        defeatMenuManager.Init(new Action[]{Restart,Application.Quit});
        playerManager.Init(Defeat, playerHealthBar.Set, playerManaBar.Set);
        //playerHealthBar.gameObject.SetActive(false);
        floorManager.Init();
        enemyManager.SpawnEnemies();
        enemyManager.Init(FinishWave);
        shop.Init(6);
        //musicManager.PlayMenuMusic();
    }
    void Start(){
        wave = 1;
        buildingManager.Init();
        projectileManager.Init();
        Camera.main.transform.position = new Vector3(0,0,-100);
        Vector3 input = default;
        input -= new Vector3(5,5);
        Vector3Int pos = floorManager.WorldToCell(input);
        floorManager.FloodFloorInt(pos, pos + new Vector3Int(8,5));
        Vector3 mid = input + Vector3.up * 3 + Vector3.right * 3;
        floorManager.FloodFloorInt(pos + Vector3Int.up, pos + new Vector3Int(8,5));
        //pathfinding.ClearRoads();
        floorManager.CreateCastle(mid,castle);
        archerManager.SwitchAnimation(true);
        gameState = GameState.Idle;
        playerActionManager.Switch(gameState);
        enemyManager.GenerateWave(1,true);
        //enemyManager.GenerateWave(1,false);
        StartLevel();
        if(tutorial) tutorialManager.StartTutorial();
        //pathfinding.CreatePossibleStart();
    }
    public void FinishWave(){
        //ResetWave();
        //playerShopUIManager.FinishLevel();
        /*winScreen.gameObject.SetActive(true);
        playerHealthBar.gameObject.SetActive(false);*/
    }
    public void StartLevel(){
        //if(pathfinding.enemyPaths.Count <= 0) return;
        //musicManager.PlayWaveMusic();
        //enemyManager.GenerateWave(wave++);
        playerActionManager.CancelBuildingAction();
        buildingManager.Switch(true);
        enemyManager.Switch(true);
        archerManager.Switch(true);
        projectileManager.Switch(true);
        gameState = GameState.Wave;
        playerActionManager.Switch(gameState);
        //playerShopUIManager.StartLevel();
    }
    public void StopLevel(){
        enemyManager.Switch(false);
        buildingManager.Switch(false);
        archerManager.Switch(false);
        archerManager.SwitchAnimation(false);
        projectileManager.Switch(false);
    }
    public void StopLevelTutorialStep()
    {
        enemyManager.Switch(false);
        buildingManager.Switch(false);
        archerManager.Switch(false);
    }
    public void StartLevelTutorialStep()
    {
        enemyManager.Switch(true);
        buildingManager.Switch(true);
        archerManager.Switch(true);
    }
    /*public void ResetWave(){
        if ((int)gameState >= 2)
        {
            playerShopUIManager.FinishLevel();
        }
        enemyManager.Switch(false);
        projectileManager.ResetEntities();
        enemyManager.ResetEntities();
        archerManager.ResetEntities();
        archerManager.SwitchAnimation(true);
        shop.ResetGroundArrays();
        menuUIManager.gameObject.SetActive(false);
        //playerHealthBar.gameObject.SetActive(false);
        gameLoadManager.CloseWindow();
        shop.Hide();
        gameState = GameState.Idle;
        playerActionManager.Switch(gameState);
        //musicManager.PlayMenuMusic();
    }*/
    public void ResetLevel(){
        /*if ((int)gameState >= 2)
        {
            playerShopUIManager.FinishLevel();
        }*/
        shop.Hide();
        playerShopUIManager.CloseAll();
        archerManager.ClearEntities();
        projectileManager.ResetEntities();
        buildingManager.ResetEntities();
        enemyManager.ResetEntities();
        shop.ResetGroundArrays();
        playerResourceManager.Reset();
        //musicManager.PlayMenuMusic();
        playerHealthBar.Reset();
    }
    public void Defeat(){
        musicManager.PlayLostMusic();
        StopLevel();
        playerActionManager.CancelBuildingAction();
        defeatMenuManager.gameObject.SetActive(true);
        gameState = GameState.Defeat;
        playerActionManager.Switch(gameState);
    }
    public void Restart(){
        musicManager.StopMusic();
        floorManager.ClearFloor();
        pathfinding.Start();
        ResetLevel();
        defeatMenuManager.gameObject.SetActive(false);
        menuUIManager.gameObject.SetActive(false);
        //winScreen.gameObject.SetActive(false);
        playerShopUIManager.CloseAll();
        Start();
    }
    public void Pause(){
        StopLevel();
        menuUIManager.gameObject.SetActive(true);
        gameState = gameState + 1;
        playerActionManager.Switch(gameState);
    }
    public void Unpause(){
        if(gameState == GameState.WavePaused){
            enemyManager.Switch(true);
        }
        buildingManager.Switch(true);
        archerManager.Switch(true);
        archerManager.SwitchAnimation(true);
        projectileManager.Switch(true);
        menuUIManager.gameObject.SetActive(false);
        gameState = gameState - 1;
        playerActionManager.Switch(gameState);
    }
    public void Load(LevelData data)
    {
        //ResetWave();
        pathfinding.ClearCastlePoint();
        pathfinding.SetTargetPoint(data.castlePositions[0].x, data.castlePositions[0].y,castle.width, castle.height);
        floorManager.LoadFloorCells(data.floorCells, data.offset, data.roads, data.ladders, data.bridgeStarts, data.bridges);
        groundGenerator.LoadParameters(data.RandomParameters);
        buildingManager.ResetEntities();
        foreach (var b in data.buildings)
        {
            floorManager.PlaceBuilding_DontCheck(b);
        }
        playerManager.currentHp = data.playerHP;
        playerResourceManager.SetResource(Resource.Gold, data.goldCount);
        //pathfinding.UpdatePaths();
        //pathfinding.SetCastlePoint(,);
    }
    public void Save(string name)
    {
        LevelData levelData = new LevelData
        {
            floorCells = FloorCellToSaveData(out List<Vector3Int> roads,
                                            out List<Vector3Int> ladders,
                                            out List<BridgeSaveData> bridgeStarts,
                                            out List<BridgeSaveData> bridges).ToArray(),
            roads = roads.ToArray(),
            ladders = ladders.ToArray(),
            bridgeStarts = bridgeStarts.ToArray(),
            bridges = bridges.ToArray(),
            RandomParameters = SaveRandomParameters(),
            castlePositions = new Vector3Int[1],
            offset = floorManager.offset,
            buildings = BuildingToSaveData(),
            goldCount = playerResourceManager.storage[Resource.Gold],
            playerHP = playerManager.currentHp
        };
        levelData.castlePositions[0] = (Vector3Int)buildingManager.bs[0].gridPosition;
        string save = JsonUtility.ToJson(levelData);
        string dir = Application.persistentDataPath + "\\saves";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        var fileNames = Directory.GetFiles(Application.persistentDataPath + "/saves");
        name = dir + $"\\{name}.json";
        File.WriteAllText(name, save);
    }
public RandomParameters SaveRandomParameters()
{
        return new RandomParameters
        {
            maxDimensions = groundGenerator.maxDimensions, 
            maxValue = groundGenerator.maxValue, 
            random = groundGenerator.random, 
            groundCondition = groundGenerator.groundCondition, 
            floorCondition = groundGenerator.floorCondition,
            randomMultiplier = groundGenerator.randomMultiplier,
            firstFloorOnly = groundGenerator.firstFloorOnly
        };
}
    public List<FloorCellSaveData> FloorCellToSaveData(out List<Vector3Int> roads,
                                                       out List<Vector3Int> ladders,
                                                       out List<BridgeSaveData> bridgeStarts,
                                                       out List<BridgeSaveData> bridges)     
    {
        List<FloorCellSaveData> result = new();// FloorCellSaveData[width * floorManager.floorCells.GetLength(1)];
        roads = new();
        ladders = new();
        bridgeStarts = new();
        bridges = new();
        foreach (FloorCell cell in floorManager.floorCells)
        {
            if (cell.bridge)
            {
                if (cell.bridgeData.start)
                    bridgeStarts.Add(new BridgeSaveData
                    {
                        gridX = cell.gridX,
                        gridY = cell.gridY,
                        floor = cell.bridgeData.floor,
                        direction = (int)cell.bridgeData.bridgeDirection
                    });
                else bridges.Add(new BridgeSaveData
                {
                    gridX = cell.gridX,
                    gridY = cell.gridY,
                    floor = cell.bridgeData.floor,
                    direction = (int)cell.bridgeData.bridgeDirection
                });
            }
            if (cell.currentFloor < 0) continue;
            result.Add(new FloorCellSaveData
            {
                currentFloor = cell.currentFloor,
                gridX = cell.gridX,
                gridY = cell.gridY,
            });
            
            if (cell.road)
            {
                roads.Add(new Vector3Int(cell.gridX, cell.gridY));
            }
            if (cell.ladder)
            {
                ladders.Add(new Vector3Int(cell.gridX, cell.gridY));
            }
        }
        return result;
    }
    public BuildingSaveData[] BuildingToSaveData()
    {
        BuildingSaveData[] result = new BuildingSaveData[buildingManager.Count];
        for(int i = 0; i < buildingManager.Count; i++) 
        {
            BuildingObject building  = buildingManager.bs[i];
            result[building.index] = new BuildingSaveData
            {
                AssetID = building.AssetID,
                index = building.index,
                gridPosition = building.gridPosition,
                position = building.transform.position,
                currentHP = building.HP,
                active = building.active,
                width = building.w, 
                height = building.h
            };
            Debug.Log($"Building wrote to buildingData[] : {building.index} : " +
                $"AssetID = {building.AssetID}" +
                $"index = {building.index}" +
                $"gridPosition = {building.gridPosition}" +
                $"position = {building.transform.position}" +
                $"currentHP = {building.HP}" +
                $"active = {building.active}" +
                $"width = {building.w}" +
                $"height = {building.h}");
        }
        return result;
    }
}
[Serializable]
public class LevelData
{
    public FloorCellSaveData[] floorCells;
    public Vector3Int[] castlePositions;
    public Vector3Int[] roads;
    public Vector3Int[] ladders; 
    public BridgeSaveData[] bridgeStarts;
    public BridgeSaveData[] bridges;
    public BuildingSaveData[] buildings;
    public RandomParameters RandomParameters;
    public Vector3Int offset;
    public int goldCount;
    public float playerHP;
}
[Serializable] public struct BridgeSaveData
{
    public int gridX, gridY;
    public int floor;
    public int direction;
}
[Serializable]
public struct RandomParameters
{
    public int maxDimensions, maxValue, random, groundCondition, floorCondition;
    public float randomMultiplier;
    public bool firstFloorOnly;
}
[Serializable]
public struct BuildingSaveData
{
    public int AssetID;
    public int width, height;
    public int index;
    public Vector2Int gridPosition;
    public Vector3 position;
    public int currentHP;
    public bool active; // indicates if a building is active and visible;
}
[Serializable]
public struct FloorCellSaveData
{
    public int currentFloor;

    public int gridX;
    public int gridY;
}
public enum GameState
{
    Idle,
    IdlePaused,
    Wave,
    WavePaused,
    Defeat
}
