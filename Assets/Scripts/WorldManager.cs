using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour {
    delegate void OnPlaced();
    delegate void OnCanceled(GroundArray g);
    OnPlaced placed;
    OnCanceled canceled;
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
    [SerializeField] private Pathfinding pathfinding;
    [Header("Other")]
    GroundArray groundArray;
    EventSystem currentEventSystem;
    public Building building;
    public BuildMode buildMode;
    Camera mCamera;
    Vector3 camMovePosition;
    Vector3 fixedCameraPosition;
    
    void Awake(){
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
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

        for(int i = 0; i < 5; i++){
            groundUIManager.AddGroundArray(OnClickUICallBack);
        }
    }
    void Start(){
        Vector3 input = mCamera.transform.position;
        GroundArray ga = new GroundArray(new Vector3Int(10,10),0);
        floorManager.CreateGroundArray(input, ga);
        GroundArray ga1 = new GroundArray(new Vector3Int(5,2),1);
        Vector3 mid = input + Vector3.up * 5 + Vector3.right * 2.5f;
        floorManager.CreateGroundArray(mid, ga1);
        floorManager.CreateCastle(mid,castle);
    }
    void OnClickUICallBack(GroundUI uI, GroundArray g){
        canceled?.Invoke(groundArray);
        groundArray = g;
        UpdateGroundArrayVisuals();
        buildingManager.ChooseGround(g);
        temporalFloor.ActivateFloor();
        placed = uI.CreateGroundArray;
        placed += temporalFloor.DeactivateFloor;
        canceled = uI.SetGroundArray;
        canceled += (g) => temporalFloor.DeactivateFloor();
    }
    void Update(){
        Vector3 input = mCamera.ScreenToWorldPoint(Input.mousePosition);
        temporalFloor.MoveTempFloor(input);
        if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)){
            if(currentEventSystem.IsPointerOverGameObject()){ 
               // currentEventSystem.currentSelectedGameObject.GetComponentInChildren<Floor>().Animate();
                return;
            }
           buildingManager.Build(input);
        }else if(Input.GetMouseButtonDown(2)){
            camMovePosition = Input.mousePosition;
            fixedCameraPosition = mCamera.transform.position;
        }else if(Input.GetMouseButton(2)){
            Vector3 pos = Input.mousePosition;
            mCamera.transform.position = fixedCameraPosition + (camMovePosition - pos) * Time.fixedDeltaTime;
        }
        else if(Input.GetKeyDown(KeyCode.Space)){
            canceled = null;
            ClearGroundArrayVisuals();
            groundUIManager.Reset();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            if(groundUIManager.hided){
                groundUIManager.Show();
            }else{
                if(!(buildMode == BuildMode.Ground)){
                    groundUIManager.Hide();
                }else{
                    canceled?.Invoke(groundArray);
                    ClearGroundArrayVisuals();
                }
            }
        }
    }
    public void ChangeGroundArray(){
        groundArray = new GroundArray(8);
        UpdateGroundArrayVisuals();
    }
    public void UpdateGroundArrayVisuals(){
        temporalFloor.SetGroundArray(groundArray);
    }
    public void ClearGroundArrayVisuals(){
        buildMode = BuildMode.None;
        temporalFloor.ClearAllTiles();
    }
}
