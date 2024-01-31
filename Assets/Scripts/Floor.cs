using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour{
    [SerializeField] Tilemap[] visuals;
    private const TileID FLOOR_CONNECTION = TileID.None | TileID.Alt;
    private const int FOAMLAYER = 0;
    private const int SHADOWLAYER = 1;
    private const int ROCKLAYER = 2;
    private const int GROUNDLAYER = 3;
    private const int GRASSLAYER = 4;
    private const int SANDLAYER = 5;
    private TileID ground;
    private TileID grass;
    private TileID grassAlternate;
    public int layer;
    void Awake(){
        visuals = GetComponentsInChildren<Tilemap>();
    }
    public void Init(int layer, TileID groundID, TileID grass = TileID.None, TileID grassAlternate = TileID.Grass | TileID.Alt){
        this.layer = layer;
        ground = groundID;
        this.grassAlternate = grassAlternate;
        this.grass = grass;
        for(int i = 0; i < visuals.Length; i++){
            visuals[i].GetComponent<TilemapRenderer>().sortingOrder = layer * 5 + i;
        }
    }
    public bool HasTile(Vector3Int tile){
        return visuals[GROUNDLAYER].HasTile(tile);
    }
    public bool CreateGround(Vector3Int pos){
        pos.z = 0;
        if(HasTile(pos)) return false;
        if(layer == 0) SetTile(pos, FOAMLAYER, TileID.Shadow | TileID.Alt);
        SetTile(pos, SHADOWLAYER, TileID.Shadow);
        SetTile(pos,GROUNDLAYER, ground);
        if(layer > 0 && !HasTile(pos + Vector3Int.down)){
            SetTile(pos + Vector3Int.down,ROCKLAYER, ground | TileID.Alt);
            SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.Shadow);
        }
        SetTile(pos,GRASSLAYER, grass);
        return true;
    }
    public bool PlaceRoad(Vector3Int pos){
        pos.z = 0;
        if(!HasTile(pos)) return false;
        if(visuals[ROCKLAYER].GetTile(pos) == StaticTiles.GetTile(TileID.Ground | TileID.Alt)){
            SetTile(pos, SANDLAYER,FLOOR_CONNECTION);          
        }else {
            SetTile(pos, SANDLAYER,TileID.Sand);
        }
        return true;
    }
    public bool CreateGroundArray(Vector3Int start, int w, int h){
        for(int posX = 0; posX < w; posX++){
            for(int posY = 0; posY < h; posY++){
                if(HasTile(new Vector3Int{x = start.x + posX, y = start.y + posY})) return false;
            }
        }
        for(int posX = 0; posX < w; posX++){
            for(int posY = 0; posY < h; posY++){
                CreateGround(new Vector3Int{x = start.x + posX, y = start.y + posY});
            }
        }
        return true;
    }
    void SetTile(Vector3Int pos, int LAYER, TileID ID){
        visuals[LAYER].SetTile(pos,StaticTiles.GetTile(ID));
        Debug.Log(pos + " " + LAYER + " " + ID);
    }
    public Vector3Int WorldToCell(Vector3 input){
        return visuals[0].WorldToCell(input);
    }
}