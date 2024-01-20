using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {
    public static WorldManager singleton{get;private set;}

    [Header("Dependencies")]
    // private List<Floor> visuals;
    [SerializeField] private BuildingManager buildingManager;

    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;

    [Header("Tiles")]
    public TileBase FOAM;
    public TileBase GROUND;
    public TileBase GRASS;
    public TileBase SHADOW;
    public TileBase FLOOR_CONNECTION;
    [SerializeField] private Tilemap prefab;
    private Floor lastFloor;

    //[Header("private")] 
    
    Cell[,] grid;
    
    void Start(){
        singleton = this;
        grid = new Cell[halfWidth*2,halfHeight*2];
        Tilemap map = Instantiate(prefab,transform);
        map.transform.localPosition = Vector3.zero;
        lastFloor = new Floor(map);
        StaticTiles.Bind(SHADOW);
        StaticTiles.Bind(GROUND);
        StaticTiles.Bind(GRASS);
        
    }
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            // Tilemap map = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            // lastFloor = new Floor(map);
            lastFloor.ClearAllTiles();
            lastFloor.SetFloor(-halfWidth,-halfHeight,halfWidth*2,halfHeight*2);
        }
    }
    void VisualToLogic(int x, int y, out int lx, out int ly){
        lx = x - halfWidth;
        ly = y - halfHeight;
    }
    void VisualToLogic(Vector3Int pos, out int x, out int y){
        x = pos.x - halfWidth;
        y = pos.y - halfHeight;
    }
}