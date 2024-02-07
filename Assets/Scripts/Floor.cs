using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Data;

public class Floor : MonoBehaviour{
    [SerializeField] Tilemap[] visuals;
    private const TileID ROCK_ROAD = TileID.Sand | TileID.Alt;
    private const int FOAMLAYER = 0;
    private const int SHADOWLAYER = 1;
    private const int GROUNDLAYER = 2;
    private const int GRASSLAYER = 3;
    private const int SANDLAYER = 4;
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
    public void Init(int layer, int RenderOrderOffset){
        this.layer = layer;
        for(int i = 0; i < visuals.Length; i++){
            visuals[i].GetComponent<TilemapRenderer>().sortingOrder = RenderOrderOffset * 5 + i;
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
        if(layer == 0) SetTile(pos, FOAMLAYER, TileID.Shadow | TileID.Alt);
        SetTile(pos, SHADOWLAYER, TileID.Shadow);
        SetTile(pos,GROUNDLAYER, ground);
        if(layer > 0 && !HasTile(pos + Vector3Int.down)){
            SetTile(pos + Vector3Int.down,GROUNDLAYER, ground | TileID.Alt);
            SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.Shadow);
        }
        SetTile(pos,GRASSLAYER, grass);
    }
    // Spawns a single road ontop of the floor
    public bool PlaceRoad(Vector3Int pos){
        pos.z = 0;
        if(visuals[GROUNDLAYER].GetTile(pos) == StaticTiles.GetTile(TileID.Ground | TileID.Alt)){
            SetTile(pos, SANDLAYER,ROCK_ROAD);          
        }else {
            SetTile(pos, SANDLAYER,TileID.Sand);
        }
        return true;
    }
    public void PlaceRoadArray(Vector3Int pos, int w, int h){
        pos.x+=w;
        pos.y+=h;
        PlaceRoad(pos);
    }    public void CreateGroundArray(Vector3Int start, int w, int h){
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