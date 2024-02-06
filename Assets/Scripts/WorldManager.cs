using System.Linq;
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
        foreach(GroundStruct g in groundArray.grounds){
            nextTile.CreateGroundArray(Vector3Int.zero + g.position, g.width, g.height);
        }
        
        temporalFloor.SetGroundArray(groundArray);
        Vector3 pos = new Vector3{x = Mathf.Min(((float)-groundArray.width)/2, -.5f), y = ((float)-groundArray.height)/2 + .5f, z = 0};
        nextTile.transform.localPosition = pos;
    }
}

public struct GroundArray{
    public readonly float width;
    public readonly float height;
    public int floor;
    public readonly GroundStruct[] grounds; 
    public GroundArray(int maxExclusive, int maxLayer){
        width = 0;
        height = 0;
        floor = Random.Range(0,maxLayer);
        grounds = new GroundStruct[Random.Range(1,3)];
        for(int i =0; i < grounds.Length; i++){
            grounds[i] = new GroundStruct(){
                position = i == 0? Vector3Int.zero : new Vector3Int(){
                    x = Random.Range(0,maxExclusive),
                    y = Random.Range(0,maxExclusive)
                },
                size = new Vector3Int(){
                    x = Random.Range(1,maxExclusive),
                    y = Random.Range(1,maxExclusive)
                }
            };
            Vector3Int middle = (grounds[i].size + grounds[i].position)/2;
            width += middle.x;
            height += middle.y; 
        }
        Debug.Log(width + " " + height);
        width /= grounds.Length;
        height /= grounds.Length;
        Debug.Log(width + " " + height);
    }
}
public struct GroundStruct{
    public Vector3Int position;
    public Vector3Int size;
    public int width => size.x;
    public int height => size.y;
    public int xMin => Mathf.Min(position.x, position.x + size.x);
    public int yMin => Mathf.Min(position.y, position.y + size.y);
    public int xMax => Mathf.Max(position.x, position.x + size.x);
    public int yMax => Mathf.Max(position.y, position.y + size.y);
}