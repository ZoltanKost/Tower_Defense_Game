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
    [SerializeField] private FloorManager floorManager;
    // [SerializeField] private Floor nextTile;
    [SerializeField] private TemporalFloor temporalFloor;
    [SerializeField] private GroundPiecesUIManager uIManager;
    bool chosenGround;
    GroundArray groundArray;
    EventSystem currentEventSystem;
    public Building building;
    public bool choosenBuilding;
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
        temporalFloor.Init(0,100);

        for(int i = 0; i < 5; i++){
            uIManager.AddGroundArray(OnClickUICallBack);
        }
    }
    void OnClickUICallBack(GroundUI uI, GroundArray g){
        canceled?.Invoke(groundArray);
        groundArray = g;
        UpdateGroundArrayVisuals();
        chosenGround = true;
        if(uI == null) return;
        placed = uI.CreateGroundArray;
        canceled = uI.SetGroundArray;
    }
    void Update(){
        Vector3 input = mCamera.ScreenToWorldPoint(Input.mousePosition);
        if(chosenGround)temporalFloor.MoveTempFloor(input);
        if(Input.GetMouseButtonDown(0)){
            if(currentEventSystem.IsPointerOverGameObject()){ 
               // currentEventSystem.currentSelectedGameObject.GetComponentInChildren<Floor>().Animate();
                return;
            }
            if(chosenGround){
                if(floorManager.CreateGround(input, groundArray)){
                    foreach(GroundStruct g in groundArray.grounds){
                        string s = "";
                        foreach(Vector3Int r in g.roads){
                            s += r + ", ";
                        }
                        Debug.Log(g.position + " " + g.size + "\n" + s);
                    }
                    placed?.Invoke();
                    canceled = null;
                    ClearGroundArrayVisuals();
                }
            }else if(choosenBuilding){
                floorManager.PlaceBuilding(input,building);
            }
            
            
        }else if(Input.GetMouseButton(1)){
            if(currentEventSystem.IsPointerOverGameObject()) return;
            floorManager.CreateRoad(input);
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
            uIManager.Reset();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            if(uIManager.hided){
                uIManager.Show();
            }else{
                if(!chosenGround){
                    uIManager.Hide();
                }else{
                    canceled?.Invoke(groundArray);
                    ClearGroundArrayVisuals();
                }
            }
        }
    }
    public void ChangeGroundArray(){
        groundArray = new GroundArray(8,2);
        UpdateGroundArrayVisuals();
    }
    public void UpdateGroundArrayVisuals(){
        temporalFloor.SetGroundArray(groundArray);
    }
    public void ClearGroundArrayVisuals(){
        chosenGround = false;
        temporalFloor.ClearAllTiles();
    }
}