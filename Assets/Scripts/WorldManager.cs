using UnityEngine;
using UnityEngine.EventSystems;
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
    public TileBase ROCK_ROAD;
    public TileBase GRASS_SHADOW;
    public TileBase SAND;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private Floor nextTile;
    [SerializeField] private TemporalFloor temporalFloor;
    GroundArray groundArray;
    EventSystem currentEventSystem;
    
    void Awake(){
        currentEventSystem = FindObjectOfType<EventSystem>();
        StaticTiles.Init();
        StaticTiles.Bind(SHADOW, TileID.Shadow);
        StaticTiles.Bind(GROUND, TileID.Ground);
        StaticTiles.Bind(GRASS, TileID.Grass);
        StaticTiles.Bind(FOAM, TileID.Shadow | TileID.Alt);
        StaticTiles.Bind(ROCK, TileID.Ground | TileID.Alt);
        StaticTiles.Bind(GRASS_SHADOW, TileID.Grass | TileID.Alt);
        StaticTiles.Bind(SAND, TileID.Sand);
        StaticTiles.Bind(ROCK_ROAD, TileID.Sand | TileID.Alt); 
        nextTile.Init(100,TileID.Ground,TileID.Grass);
        temporalFloor.Init(100, TileID.Ground,TileID.Grass);
        ChangeGroundArray();
    }
    void Update(){
        Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temporalFloor.MoveTempFloor(input);
        if(Input.GetMouseButtonDown(0)){
            if(currentEventSystem.IsPointerOverGameObject()){ 
                currentEventSystem.currentSelectedGameObject.GetComponentInChildren<Floor>().Animate();
                return;
            }
            if(floorManager.CreateGround(input, groundArray))
                ChangeGroundArray();
        }else if(Input.GetMouseButtonDown(1)){
            if(currentEventSystem.IsPointerOverGameObject()) return;
            floorManager.CreateRoad(input);
        }else if(Input.GetKeyDown(KeyCode.Space)){
            ChangeGroundArray();
        }
    }
    public void ChangeGroundArray(){
        groundArray = new GroundArray(4,2);
        nextTile.ClearAllTiles();
        nextTile.CreateGroundArray(Vector3Int.zero,groundArray.width,groundArray.height);
        temporalFloor.SetGroundArray(groundArray);
        Vector3 pos = new Vector3{x = Mathf.Min(((float)-groundArray.width)/2, -.5f), y = ((float)-groundArray.height)/2 + .5f, z = 0};
        nextTile.transform.localPosition = pos;
    }
}

public struct GroundArray{
    public int width;
    public int height;
    public int floor;
    GroundArray[] appendixes; 
    public GroundArray(int maxExclusive, int maxLayer){
        width = Random.Range(1,maxExclusive);
        height = Random.Range(1,maxExclusive);
        floor = Random.Range(0,maxLayer);
        appendixes = null;
    }
    public GroundArray(int maxExclusive, int maxLayer, int maxAppendix){
        width = Random.Range(1,maxExclusive);
        height = Random.Range(1,maxExclusive);
        floor = Random.Range(0,maxLayer);
        appendixes = new GroundArray[Random.Range(0,maxExclusive)];
        for(int i =0; i < appendixes.Length; i++){
            appendixes[i] = new GroundArray(maxAppendix, maxLayer);
            appendixes[i].floor = floor;
        }
    }
}