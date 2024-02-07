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
    public TileBase ROCK_ROAD;
    public TileBase GRASS_SHADOW;
    public TileBase SAND;
    [SerializeField] private FloorManager floorManager;
    // [SerializeField] private Floor nextTile;
    [SerializeField] private TemporalFloor temporalFloor;
    [SerializeField] private GroundPiecesUIManager uIManager;
    bool chosenGround;
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
        // nextTile.Init(100,TileID.Ground,TileID.Grass);
        temporalFloor.Init(0,100);
        // ChangeGroundArray();
        for(int i = 0; i < 5; i++){
            uIManager.AddGroundArray(OnClickUICallBack);
        }
    }
    void OnClickUICallBack(object s, GroundArray g){
        canceled?.Invoke(groundArray);
        groundArray = g;
        UpdateGroundArrayVisuals();
        chosenGround = true;
        GroundUI uI = s as GroundUI;
        if(uI == null) return;
        placed = uI.CreateGroundArray;
        canceled = uI.SetGroundArray;
    }
    void Update(){
        Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(chosenGround)temporalFloor.MoveTempFloor(input);
        if(Input.GetMouseButtonDown(0)){
            if(!chosenGround) return;
            if(currentEventSystem.IsPointerOverGameObject()){ 
                currentEventSystem.currentSelectedGameObject.GetComponentInChildren<Floor>().Animate();
                return;
            }
            if(floorManager.CreateGround(input, groundArray)){
                placed?.Invoke();
                canceled = null;
                ClearGroundArrayVisuals();
            }
        }else if(Input.GetMouseButtonDown(1)){
            if(currentEventSystem.IsPointerOverGameObject()) return;
            floorManager.CreateRoad(input);
        }
        else if(Input.GetKeyDown(KeyCode.Space)){
            canceled?.Invoke(groundArray);
            ClearGroundArrayVisuals();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            canceled = null;
            ClearGroundArrayVisuals();
            uIManager.Reset();
        }
    }
    public void ChangeGroundArray(){
        groundArray = new GroundArray(6,2);
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

public struct GroundArray{
    public float width{get; private set;}
    public float height{get; private set;}
    public int layer;
    public readonly GroundStruct[] grounds;
    // public GroundStruct[] roads;
    public GroundArray(int maxExclusive, int maxLayer){
        width = 0;
        height = 0;
        layer = Random.Range(0,maxLayer);
        grounds = new GroundStruct[Random.Range(1,maxExclusive)];
        for(int i = 0; i < grounds.Length; i++){
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
            if(i == 0){
                width += grounds[i].width;
                height += grounds[i].height;
                continue;
            }
            width += grounds[i-1].xMax > grounds[i].xMin
                ?grounds[i].width - (grounds[i-1].xMax - grounds[i].xMin)  
                :grounds[i].width + (grounds[i].xMin - grounds[i-1].xMax);
            height += grounds[i-1].yMax > grounds[i].yMin
                ?grounds[i].height - (grounds[i-1].yMax - grounds[i].yMin) 
                :grounds[i].height + (grounds[i].yMin - grounds[i-1].yMax);
        }
        // roads = new GroundStruct[Random.Range(0,maxExclusive/2)];
        // if(roads.Length < 1) return;     
        // bool right = Random.Range(0,2) == 0? true : false;
        // roads[0] = new GroundStruct(){
        //     position = new Vector3Int(){
        //         x = Random.Range(0,Mathf.FloorToInt(width)/2), 
        //         y = Random.Range(0,Mathf.FloorToInt(height)/2)
        //     },
        //     size = new Vector3Int(){
        //         x = right ? Random.Range(0,Mathf.FloorToInt(width)/2) : 1, 
        //         y = right? 1 : Random.Range(0,Mathf.FloorToInt(height)/2)
        //     }
        // };
        // for(int i = 1; i < roads.Length; i++){
        //     right = Random.Range(0,2) == 0? true : false;             
        //     roads[i] = new GroundStruct(){
        //         position = new Vector3Int(){
        //             x = roads[i-1].xMax, 
        //             y = roads[i-1].yMax
        //         },
        //         size = new Vector3Int(){
        //             x = right ? Random.Range(0,Mathf.FloorToInt(width)/2) : 1, 
        //             y = right? 1 : Random.Range(0,Mathf.FloorToInt(height)/2)
        //         }
        //     };
        // }
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