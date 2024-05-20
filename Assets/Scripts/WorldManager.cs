using System;
using DG.Tweening;
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
    [SerializeField] private GroundPiecesUIManager groundUIManager;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private EnemyManager enemy;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private EventSubscribeButton nextWaweButton;
    [SerializeField] private EventSubscribeButton PauseButton;
    [SerializeField] private PlayerManager playerManager; 
    [SerializeField] private MenuUIManager defeatMenuManager;
    [SerializeField] private MenuUIManager menuUIManager;
    [SerializeField] private Transform ControlsMenu;
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

        Action buildingFailedCallback = () => 
        {
            playerInput.Deactivate();
            temporalFloor.GetAnimationTween().onKill += playerInput.Activate;
        };
        playerBuildingManager.Init(buildingFailedCallback, temporalFloor);
        playerInput.Init(temporalFloor,groundUIManager.Reset, playerBuildingManager.CancelBuildingAction, playerBuildingManager.ClickBuild, playerBuildingManager.HoldBuild);
        nextWaweButton.Init(StartLevel);
        PauseButton.Init(Pause);
        menuUIManager.Init(new Action[]{Unpause,Restart,Application.Quit});
        defeatMenuManager.Init(new Action[]{Restart,Application.Quit});
        playerManager.Init(Defeat);
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
    public void StartLevel(){
        if(!pathfinding.FindPathToCastle()) return;
        UIOff();
        playerBuildingManager.CancelBuildingAction();
        buildingManager.Activate();
        enemy.Activate();
        archerManager.ActivateArchers();
        gameState = GameState.Wave;
    }
    public void StopLevel(){
        UIOn();
        enemy.Reset();
        buildingManager.Deactivate();
        archerManager.DeactivateArchers();
        gameState = GameState.Idle;
    }
    public void Defeat(){
        UIOff();
        buildingManager.Deactivate();
        archerManager.DeactivateArchers();
        defeatMenuManager.gameObject.SetActive(true);
        gameState = GameState.Defeat;
    }
    public void Restart(){
        floorManager.ClearFloor();
        StopLevel();
        archerManager.DeactivateArchers();
        buildingManager.Reset();
        defeatMenuManager.gameObject.SetActive(false);
        menuUIManager.gameObject.SetActive(false);
        Start();
    }
    public void Pause(){
        UIOff();
        archerManager.DeactivateArchers();
        enemy.Deactivate();
        menuUIManager.gameObject.SetActive(true);
        gameState = gameState + 1;
    }
    public void Unpause(){
        if(gameState == GameState.WavePaused){
            archerManager.ActivateArchers();
            enemy.Activate();
        }else{
            UIOn();
        }
        menuUIManager.gameObject.SetActive(false);
        gameState = gameState - 1;
    }
    public void UIOn(){
        playerInput.Activate();
        buildingUI.ShowUI();
        groundUIManager.Show();
    }
    public void UIOff(){
        buildingUI.HideUI();
        groundUIManager.Hide();
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
