using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEditor;

public class WorldManager : MonoBehaviour {
    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;
    [SerializeField] private int maxDimensions,maxSeedValue,maxValue,random,randomReduce,trueCondition;

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
    [SerializeField] private MenuUIManager winScreen;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private PlayerShopUIManager playerShopUIManager;
    [SerializeField] private GameLoadManager gameLoadManager;
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
        temporalFloor.Init(0,"TempFloor");
        /*bool fullscreen = true;
        #if UNITY_WEBGL
            fullscreen = false;
        #endif
        Screen.SetResolution(1366,768, fullscreen);*/
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
        menuUIManager.Init(new Action[]{Unpause,Restart,Application.Quit,null, ResetWave, () => { gameLoadManager.ReadSaveData(); menuUIManager.gameObject.SetActive(false); }, Save});
        defeatMenuManager.Init(new Action[]{Restart,Application.Quit});
        Action playerDefeat = () => {
            Defeat();
            playerHealthBar.gameObject.SetActive(false);
        };
        playerManager.Init(playerDefeat,playerHealthBar.Set);
        winScreen.Init(new Action[]{Restart,Application.Quit});
        playerHealthBar.gameObject.SetActive(false);
        enemyManager.SetWinAction(Win);
        shop.Init(6);
    }
    void Start(){
        buildingManager.Init();
        projectileManager.Init();
        Vector3 input = Camera.main.transform.position;
        GroundArray ga = new GroundArray(new Vector2Int(10,10),0);
        input -= new Vector3(10,10)*.5f;
        floorManager.CreateGroundArray_DontCheck(input, ga);
        GroundArray ga1 = new GroundArray(new Vector2Int(5,2),1);
        Vector3 mid = input + Vector3.up * 5 + Vector3.right * 2.5f;
        floorManager.CreateGroundArray_DontCheck(mid, ga1);
        floorManager.CreateCastle(mid,castle);
        archerManager.SwitchAnimation(true);
        gameState = GameState.Idle;
        playerActionManager.Switch(gameState);
    }
    public void Win(){
        ResetWave();
        // winScreen.gameObject.SetActive(true);
        playerHealthBar.gameObject.SetActive(false);
    }
    public void StartLevel(){
        if(!pathfinding.FindPathToCastle()) return;
        enemyManager.SpawnEnemies(wave++);
        playerActionManager.CancelBuildingAction();
        buildingManager.Switch(true);
        enemyManager.Switch(true);
        archerManager.Switch(true);
        projectileManager.Switch(true);
        gameState = GameState.Wave;
        playerActionManager.Switch(gameState);
        playerHealthBar.gameObject.SetActive(true);
        // playerHealthBar.Reset();
        playerShopUIManager.CloseAll();
    }
    public void StopLevel(){
        enemyManager.Switch(false);
        buildingManager.Switch(false);
        archerManager.Switch(false);
        archerManager.SwitchAnimation(false);
        projectileManager.Switch(false);
    }
    public void ResetWave(){
        enemyManager.Switch(false);
        projectileManager.ResetEntities();
        enemyManager.ResetEntities();
        archerManager.ResetEntities();
        archerManager.SwitchAnimation(true);
        shop.ResetGroundArrays();
        menuUIManager.gameObject.SetActive(false);
        playerHealthBar.gameObject.SetActive(false);
        gameLoadManager.CloseWindow();
        shop.Hide();
        playerShopUIManager.CloseAll();
        gameState = GameState.Idle;
        playerActionManager.Switch(gameState);
    }
    public void ResetLevel(){
        shop.Hide();
        playerShopUIManager.CloseAll();
        archerManager.ClearEntities();
        projectileManager.ClearEntities();
        buildingManager.ClearEntities();
        projectileManager.ClearEntities();
        enemyManager.ClearEntities();
        shop.ResetGroundArrays();
        playerResourceManager.Reset();
        playerHealthBar.gameObject.SetActive(false);
    }
    public void Defeat(){
        StopLevel();
        defeatMenuManager.gameObject.SetActive(true);
        playerHealthBar.gameObject.SetActive(false);
        gameState = GameState.Defeat;
        playerActionManager.Switch(gameState);
    }
    public void Restart(){
        floorManager.ClearFloor();
        ResetLevel();
        defeatMenuManager.gameObject.SetActive(false);
        menuUIManager.gameObject.SetActive(false);
        winScreen.gameObject.SetActive(false);
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
        ResetWave();
        floorManager.LoadFloorCells(data.floorCells, data.offset);
        buildingManager.ResetEntities();
        foreach (var b in data.buildings)
        {
            floorManager.PlaceBuilding_DontCheck(b);
        }
        pathfinding.ClearCastlePoint();
        pathfinding.SetCastlePoint(data.castlePositions[0].x, data.castlePositions[0].y,castle.width, castle.height);
        playerManager.currentHp = data.playerHP;
        playerResourceManager.SetResource(Resource.Gold, data.goldCount);
        //pathfinding.SetCastlePoint(,);
    }
    public void Save()
    {
        LevelData levelData = new LevelData
        {
            floorCells = FloorCellToSaveData().ToArray(),
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
        File.WriteAllText(dir + $"\\save{Directory.GetFiles(dir).Length}.json", save);
    }
    public List<FloorCellSaveData> FloorCellToSaveData()
    {
        List<FloorCellSaveData> result = new();// FloorCellSaveData[width * floorManager.floorCells.GetLength(1)];
        foreach (FloorCell cell in floorManager.floorCells)
        {
            if (cell.currentFloor < 0) continue;
            result.Add(new FloorCellSaveData
            {
                currentFloor = cell.currentFloor,
                bridgeData = cell.bridgeData, 
                bridge = cell.bridge,
                road = cell.road,
                ladder = cell.ladder,
                gridX = cell.gridX,
                gridY = cell.gridY,
            });
            if(cell.bridge && cell.bridgeData.start)
            {
                Debug.Log($"Cell wrote to CellData : {cell.gridX}, {cell.gridY}: " +
                $"currentFloor = {cell.currentFloor} " +
                $"bridgeData = {cell.bridgeData.floor}, {cell.bridgeData.bridgeDirection}, " +
                $"bridge = {cell.bridge} " +
                $"road = {cell.road} " +
                $"ladder = {cell.ladder} ");
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
    public Vector3Int offset;
    public BuildingSaveData[] buildings;
    public int goldCount;
    public int playerHP;
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
    public BridgeData bridgeData;
    public bool bridge;
    public bool road;
    public bool ladder;
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
