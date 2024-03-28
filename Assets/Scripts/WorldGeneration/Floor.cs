using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Floor : MonoBehaviour{
    [SerializeField] public Tilemap[] visuals;
    private const int FOAMLAYER = 0;
    private const int SHADOWLAYER = 1;
    private const int GROUNDLAYER = 2;
    private const int GRASSLAYER = 3;
    private const int SANDLAYER = 4;
    private const int BRIDGELAYER = 5;
    private TileID ground{
        get
        {
            return layer == 0? TileID.Sand : TileID.Ground;
        } 
    }
    private TileID grass{
        get{
            return layer == 0? TileID.None : TileID.Grass;
        }
    }
    public int layer;
    protected void Awake(){
        visuals = GetComponentsInChildren<Tilemap>();
    }
    public void Init(int layer, string sortingLayerName){
        this.layer = layer;
        for(int i = 0; i < visuals.Length; i++){
            TilemapRenderer mapRenderer = visuals[i].GetComponent<TilemapRenderer>();
            mapRenderer.sortingLayerName = sortingLayerName;
            mapRenderer.sortingOrder = i;
        }
    }
    // Checks if there's a ground or a road on the floor. Refactor.
    public bool HasTile(Vector3Int start){
        start.z = 0;
        return visuals[GROUNDLAYER].GetTile(start) == StaticTiles.GetTile(ground) 
            || visuals[SANDLAYER].HasTile(start);
    }
    // Spawns one cell on the floor
    public void CreateGround(Vector3Int pos){
        pos.z = 0;
        if(layer == 0) SetTile(pos, FOAMLAYER, TileID.Foam);
        SetTile(pos, SHADOWLAYER, TileID.Shadow);
        SetTile(pos,GROUNDLAYER, ground);
        if(layer > 0 && !HasTile(pos + Vector3Int.down)){
            SetTile(pos + Vector3Int.down,GROUNDLAYER, TileID.Rock);
            SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.Shadow);
        }
        SetTile(pos,GRASSLAYER, grass);
    }
    // Spawns a single road ontop of the floor
    public void PlaceRoad(Vector3Int pos){
        pos.z = 0;
        SetTile(pos, SANDLAYER,TileID.Sand);
    }
    public void PlaceStairs(Vector3Int pos){
        pos.z = 0;
        if(visuals[GROUNDLAYER].GetTile(pos) == StaticTiles.GetTile(TileID.Rock))
            SetTile(pos, GROUNDLAYER,TileID.Ladder);
    }
    public void PlaceBridge(Vector3Int pos){
        pos.z = 0;
        SetTile(pos,BRIDGELAYER, TileID.Bridge);
    }
    public void SetBridgeSpot(Vector3Int pos){
        pos.z = 0;
        SetTile(pos, BRIDGELAYER, TileID.BridgeOnGround);
        SetTile(pos,SANDLAYER,TileID.Sand);
    }
    public void CreateGroundArray(Vector3Int start, int w, int h){
        for(int posX = 0; posX < w; posX++){
            for(int posY = 0; posY < h; posY++){
                CreateGround(new Vector3Int{x = start.x + posX, y = start.y + posY});
            }
        }
    }
    // Shortcut. After refactoring probably isn't required.
    void SetTile(Vector3Int pos, int LAYER, TileID ID){
        visuals[LAYER].SetTile(pos,StaticTiles.GetTile(ID));
        // Debug.Log(pos + " " + LAYER + " " + ID);
    }
    // Shortcut
    public Vector3Int WorldToCell(Vector3 input){
        return visuals[0].WorldToCell(input);
    }
    public Vector3 CellToWorld(Vector3Int cell){
        return visuals[0].CellToWorld(cell);
    }
    // Jelly Animation. Probably should be moved somewhere else
    public void Animate(){
        Tween tween = transform.DOScale(.99f, .05f);
        tween.onComplete += () => {
            Tween tween1 = transform.DOScale(1.01f, .1f);
            tween1.onComplete += () => transform.DOScale(1, .05f);
        };
    }
    // Shortcut.
    public void ClearAllTiles(){
        foreach(Tilemap map in visuals){
            map.ClearAllTiles();
        }
    }
}