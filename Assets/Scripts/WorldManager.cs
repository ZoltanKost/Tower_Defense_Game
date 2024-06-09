using System;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private EventSubscribeButton nextWaweButton;
    [SerializeField] private EventSubscribeButton PauseButton;
    [SerializeField] private PlayerManager playerManager; 
    [SerializeField] private MenuUIManager defeatMenuManager;
    [SerializeField] private MenuUIManager menuUIManager;
    [SerializeField] private MenuUIManager controlsMenu;
    [SerializeField] private MenuUIManager winScreen;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private HealthBar playerHealthBar;
    private GameState gameState;
    
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
        Screen.SetResolution(1920,1080, true);
        Action buildingFailedCallback = () => 
        {
            playerInput.Deactivate();
            temporalFloor.GetAnimationTween().onKill += playerInput.Activate;
        };
        playerBuildingManager.Init(buildingFailedCallback, temporalFloor);
        playerInput.Init(
            temporalFloor,
            shop.ResetGroundArrays, 
            playerBuildingManager.CancelBuildingAction, 
            playerBuildingManager.ClickBuild, 
            playerBuildingManager.HoldBuild, 
            playerBuildingManager.CanBuild
        );
        nextWaweButton.Init(StartLevel);
        PauseButton.Init(Pause);
        menuUIManager.Init(new Action[]{Unpause,Restart,Application.Quit,OpenControls, ResetWave});
        defeatMenuManager.Init(new Action[]{Restart,Application.Quit});
        Action playerDefeat = () => {
            Defeat();
            playerHealthBar.gameObject.SetActive(false);
        };
        playerManager.Init(playerDefeat,playerHealthBar.Set);
        controlsMenu.Init(new Action[]{Unpause});
        winScreen.Init(new Action[]{Restart,Application.Quit});
        playerHealthBar.gameObject.SetActive(false);
        enemyManager.SetWinAction(Win);
        shop.Init(6);
    }
    void Start(){
        Vector3 input = Camera.main.transform.position;
        GroundArray ga = new GroundArray(new Vector2Int(10,10),0);
        input -= new Vector3(10,10)*.5f;
        floorManager.CreateGroundArray(input, ga);
        GroundArray ga1 = new GroundArray(new Vector2Int(5,2),1);
        Vector3 mid = input + Vector3.up * 5 + Vector3.right * 2.5f;
        floorManager.CreateGroundArray(mid, ga1);
        floorManager.CreateCastle(mid,castle);
        archerManager.SwitchAnimation(true);
        gameState = GameState.Idle;
    }
    void Update(){
        bool esc = Input.GetKeyDown(KeyCode.Escape);
        switch(gameState){
            case GameState.Idle:
                if(esc) {
                    UIOff();
                    Pause();
                }
                break;
            case GameState.IdlePaused:
                if(esc) {
                    Unpause();
                }
                break;
            case GameState.Wave:
                if(esc) {
                    Pause();
                }
                break;
            case GameState.WavePaused:
                if(esc) {
                    Unpause();
                }
                break;
            case GameState.Defeat:
                if(esc) {
                    Restart();
                }
                break;
        }
    }
    public void Win(){
        StopLevel();
        winScreen.gameObject.SetActive(true);
        playerHealthBar.gameObject.SetActive(false);
    }
    public void OpenControls(){
        controlsMenu.gameObject.SetActive(true);
        menuUIManager.gameObject.SetActive(false);
    }
    public void StartLevel(){
        if(!pathfinding.FindPathToCastle()) return;
        UIOff();
        playerBuildingManager.CancelBuildingAction();
        buildingManager.Switch(true);
        enemyManager.Switch(true);
        archerManager.Switch(true);
        projectileManager.Switch(true);
        gameState = GameState.Wave;
        playerHealthBar.gameObject.SetActive(true);
        playerHealthBar.Reset();
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
        UIOn();
        gameState = GameState.Idle;
    }
    public void ResetLevel(){
        archerManager.ClearEntities();
        projectileManager.ClearEntities();
        buildingManager.ClearEntities();
        projectileManager.ClearEntities();
        enemyManager.ClearEntities();
        shop.ResetGroundArrays();
        playerHealthBar.gameObject.SetActive(false);
    }
    public void Defeat(){
        UIOff();
        StopLevel();
        defeatMenuManager.gameObject.SetActive(true);
        playerHealthBar.gameObject.SetActive(false);
        gameState = GameState.Defeat;
    }
    public void Restart(){
        floorManager.ClearFloor();
        UIOn();
        ResetLevel();
        defeatMenuManager.gameObject.SetActive(false);
        menuUIManager.gameObject.SetActive(false);
        winScreen.gameObject.SetActive(false);
        Start();
    }
    public void Pause(){
        UIOff();
        StopLevel();
        menuUIManager.gameObject.SetActive(true);
        gameState = gameState + 1;
    }
    public void Unpause(){
        if(gameState == GameState.WavePaused){
            enemyManager.Switch(true);
        }else{
            UIOn();
        }
        buildingManager.Switch(true);
        archerManager.Switch(true);
        archerManager.SwitchAnimation(true);
        projectileManager.Switch(true);
        menuUIManager.gameObject.SetActive(false);
        controlsMenu.gameObject.SetActive(false);
        gameState = gameState - 1;
    }
    public void UIOn(){
        playerInput.Activate();
        buildingUI.ShowUI();
    }
    public void UIOff(){
        buildingUI.HideUI();
        playerInput.Deactivate();
    }
    enum GameState{
        Idle, 
        IdlePaused,
        Wave, 
        WavePaused,
        Defeat
    } 
}
