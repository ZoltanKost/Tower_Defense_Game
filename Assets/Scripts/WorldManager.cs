using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour {
    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;

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
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private Pathfinding pathfinding;
    
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
        temporalFloor.Init(0,100);

        PlayerInputManager.ResetCallback callback = buildingManager.ResetMode;
        callback += groundUIManager.Reset;
        callback += temporalFloor.DeactivateFloor;

        inputManager.Init(temporalFloor,callback, buildingManager.Build);
    }
    void Start(){
        Vector3 input = Camera.main.transform.position;
        GroundArray ga = new GroundArray(new Vector3Int(10,10),0);
        floorManager.CreateGroundArray(input, ga);
        GroundArray ga1 = new GroundArray(new Vector3Int(5,2),1);
        Vector3 mid = input + Vector3.up * 5 + Vector3.right * 2.5f;
        floorManager.CreateGroundArray(mid, ga1);
        floorManager.CreateCastle(mid,castle);
    }
    public void ChangeGroundArray(){
        var groundArray = new GroundArray(8);
        temporalFloor.SetGroundArray(groundArray);
    }
}
