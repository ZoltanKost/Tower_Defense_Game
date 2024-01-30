using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour {
    public static WorldManager singleton{get;private set;}
    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;

    [Header("Tiles")]
    public TileBase FOAM;
    public TileBase GROUND;
    public TileBase ROCK;
    public TileBase GRASS;
    public TileBase SHADOW;
    public TileBase FLOOR_CONNECTION;
    public TileBase GRASS_SHADOW;
    public TileBase SAND;
    [SerializeField] private Tilemap prefab;
    private Floor floor;
    
    void Start(){
        singleton = this;
        Tilemap map = Instantiate(prefab,transform);
        map.transform.localPosition = Vector3.zero;
        floor = new Floor(map, halfWidth * 2, halfHeight * 2);
        StaticTiles.Init();
        StaticTiles.Bind(SHADOW, TileID.Shadow);
        StaticTiles.Bind(GROUND, TileID.Ground);
        StaticTiles.Bind(GRASS, TileID.Grass);
        StaticTiles.Bind(FOAM, TileID.Shadow | TileID.Alt);
        StaticTiles.Bind(ROCK, TileID.Ground | TileID.Alt);
        StaticTiles.Bind(GRASS_SHADOW, TileID.Grass | TileID.Alt);
        StaticTiles.Bind(SAND, TileID.Sand);
        StaticTiles.Bind(null, TileID.Sand | TileID.Alt);
    }
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(input);
            floor.CreateGround(input);
        }
    }
}