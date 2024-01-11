using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour{
    private Tilemap[] maps;
    public const int FLOOR_SHADOW = 0;
    public const int FLOOR_GROUND = 1;
    public const int FLOOR_GRASS = 2;
    public const int FLOOR_ROAD = 0;
    void Awake(){
        maps = GetComponentsInChildren<Tilemap>();
    }
    public Vector3Int WorldToCell(Vector3 position){
        return maps[0].WorldToCell(position);
    }
    public void SetTile(Vector3Int position, TileBase tile, int layer){
        maps[layer].SetTile(position,tile);
    }
    public void SetTile(Vector3 position, TileBase tile, int layer){
        maps[layer].SetTile(WorldToCell(position),tile);
    }
    public bool HasTile(Vector3 position){
        return maps[FLOOR_GROUND].HasTile(WorldToCell(position));
    }
    public bool HasTile(int x, int y){
        Vector3Int position = new(x,y);
        return maps[FLOOR_GROUND].HasTile(position);
    }
}