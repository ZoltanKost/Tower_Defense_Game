using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour{
    [SerializeField] public Tilemap[] visuals;
    protected TweenAnimator tweenAnimator;
    private const int FOAMLAYER = 0;
    private const int SHADOWLAYER = 1;
    private const int GROUNDLAYER = 2;
    private const int GRASSLAYER = 3;
    private const int SANDLAYER = 4;
    private const int BRIDGELAYER = 5;
    private TileID ground{
        get
        {
            return floor == 0? TileID.Sand : TileID.Ground;
        } 
    }
    private TileID grass{
        get{
            return floor == 0? TileID.None : TileID.Grass;
        }
    }
    public int floor;
    protected void Awake(){
        visuals = GetComponentsInChildren<Tilemap>();
        tweenAnimator = GetComponent<TweenAnimator>();
    }
    public void Init(int layer, string sortingLayerName){
        this.floor = layer;
        for(int i = 0; i < visuals.Length; i++){
            TilemapRenderer mapRenderer = visuals[i].GetComponent<TilemapRenderer>();
            mapRenderer.sortingLayerName = sortingLayerName;
            mapRenderer.sortingOrder = i;
        }
    }
    // Checks if there's a ground or a road on the floor.
    public bool HasTile(Vector3Int start){
        start.z = 0;
        return visuals[GROUNDLAYER].GetTile(start) == StaticTiles.GetTile(ground) 
            || visuals[SANDLAYER].HasTile(start);
    }
    // Spawns one cell on the floor
    public void CreateGround(Vector3Int pos){
        pos.z = 0;
        if(floor == 0) SetTile(pos, FOAMLAYER, TileID.Foam);
        SetTile(pos, SHADOWLAYER, TileID.Shadow);
        SetTile(pos,GROUNDLAYER, ground);
        if(floor > 0 && !HasTile(pos + Vector3Int.down)){
            SetTile(pos + Vector3Int.down,GROUNDLAYER, TileID.Rock);
            SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.Shadow);
        }
        SetTile(pos,GRASSLAYER, grass);
    }
    public void RemoveGround(Vector3Int pos, bool replaceWithRock)
    {
        pos.z = 0;
        if(floor == 0) SetTile(pos, FOAMLAYER, TileID.None);
        if(!replaceWithRock)
        {
            SetTile(pos, SHADOWLAYER, TileID.None);
            SetTile(pos,GROUNDLAYER, TileID.None);
            if(floor > 0)
            {
                SetTile(pos + Vector3Int.down,GROUNDLAYER, TileID.None);
                SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.None);
            }
        }
        else
        {
            SetTile(pos,GROUNDLAYER, TileID.Rock);
            SetTile(pos, SHADOWLAYER, TileID.Shadow);
            if(floor > 0)
            {
                SetTile(pos + Vector3Int.down,GROUNDLAYER, TileID.None);
                SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.None);
            }
        }
        SetTile(pos,GRASSLAYER, TileID.None);
    }
    public void PlaceRoad(Vector3Int pos){
        pos.z = 0;
        SetTile(pos, SANDLAYER,TileID.Sand);
    }
    public void RemoveRoad(Vector3Int pos)
    {
        pos.z = 0;
        SetTile(pos,SANDLAYER,TileID.None);
    }
    public void PlaceStairs(Vector3Int pos){
        pos.z = 0;
        SetTile(pos, GROUNDLAYER,TileID.Ladder);
    }
    public void RemoveStairs(Vector3Int pos)
    {
        pos.z = 0;
        SetTile(pos, GROUNDLAYER,TileID.Rock);
    }
    public void PlaceBridge(Vector3Int pos){
        pos.z = 0;
        SetTile(pos,BRIDGELAYER, TileID.Bridge);
    }
    public void RemoveBridge(Vector3Int pos)
    {
        pos.z = 0;
        SetTile(pos,BRIDGELAYER,TileID.None);
    }
    public void SetBridgeSpot(Vector3Int pos){
        pos.z = 0;
        SetTile(pos, BRIDGELAYER, TileID.BridgeOnGround);
        SetTile(pos,SANDLAYER,TileID.Sand);
    }
    public void RemoveBridgeSpot(Vector3Int pos)
    {
        pos.z = 0;
        SetTile(pos, BRIDGELAYER, TileID.None);
        SetTile(pos, SANDLAYER, TileID.None);
    }
    public void CreateGroundArray(Vector3Int start, int w, int h){
        for(int posX = 0; posX < w; posX++){
            for(int posY = 0; posY < h; posY++){
                CreateGround(new Vector3Int{x = start.x + posX, y = start.y + posY});
            }
        }
    }
    void SetTile(Vector3Int pos, int LAYER, TileID ID){
        visuals[LAYER].SetTile(pos,StaticTiles.GetTile(ID));
    }
    public Vector3Int WorldToCell(Vector3 input){
        return visuals[0].WorldToCell(input);
    }
    public Vector3 CellToWorld(Vector3Int cell){
        return visuals[0].CellToWorld(cell);
    }
    public virtual void Animate(){
        tweenAnimator.JellyAnimation();
    }
    public virtual Tween GetAnimationTween(){
        return tweenAnimator.JellyAnimation();   
    }
    public void ClearAllTiles(){
        foreach(Tilemap map in visuals){
            map.ClearAllTiles();
        }
    }
}