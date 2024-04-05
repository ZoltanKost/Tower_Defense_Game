using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour {
    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;
    bool playing;
    [SerializeField] private int maxDimensions,maxSeedValue,maxValue,random,randomReduce,trueCondition;
    [SerializeField] private bool test;

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
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private EnemyManager enemy;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private NextWaveButton nextWaweButton;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Transform DefeatImage;
    
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

        PlayerInputManager.Callback callback = buildingManager.ResetMode;
        callback += groundUIManager.Reset;
        callback += temporalFloor.DeactivateFloor;

        playerInput.Init(temporalFloor,callback, buildingManager.Build, StopLevel);
        nextWaweButton.Init(StartLevel);
        playerManager.Init(Defeat);
        if(test)for(int i = 0; i < 100; i++){
            var v = new GroundArray(maxDimensions,maxSeedValue,maxValue,random,randomReduce,trueCondition);
        }
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
        playing = true;
    }
    void Update(){
        if(playing) return;
        if(Input.GetKeyDown(KeyCode.Space)){
            floorManager.ClearFloor();
            Start();
            StopLevel();
            DefeatImage.gameObject.SetActive(false);
        }
    }
    public void StartLevel(){
        if(!pathfinding.FindPathToCastle()) return;
        archerManager.ActivateArchers();
        buildingUI.HideUI();
        groundUIManager.Hide();
        playerInput.Deactivate();
        enemy.Activate();
    }
    public void StopLevel(){
        buildingUI.ShowUI();
        groundUIManager.Show();
        playerInput.Activate();
        enemy.Deactivate();
        archerManager.DeactivateArchers();
    }
    public void Defeat(){
        buildingUI.HideUI();
        groundUIManager.Hide();
        playerInput.Deactivate();
        archerManager.DeactivateArchers();
        DefeatImage.gameObject.SetActive(true);
        playing = false;
    }
}
