using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour{
    private Tilemap map;
    void Awake(){
        map = GetComponentInChildren<Tilemap>();
    }
    public TileBase GetTileBase(int x, int y, int z){
        return map.GetTile(new Vector3Int(x,y,z));
    }
    public bool HasTile(int x, int y, int z){
        return map.HasTile(new Vector3Int(x,y,z));
    }
    public Vector3 GridPositionToWorld(int x, int y, int z){
        Vector3Int positionInt = new Vector3Int(x,y,z);
        return map.CellToWorld(positionInt);
    }
    public Vector3 GridPositionToWorld(Vector3Int pos){
        Vector3Int positionInt = new Vector3Int(pos.x,pos.y,pos.z);
        return map.CellToWorld(positionInt);
    }
    public void WorldPositionToGrid(Vector3 position, out int x, out int y, out int z){
        Vector3Int gridPosition = map.WorldToCell(position);
        x = gridPosition.x;
        y = gridPosition.y;
        z = gridPosition.z;
    }
    public Vector3Int WorldToGrid(Vector3 position){
        return map.WorldToCell(position);
    }
    public void SetTile(int x, int y, int z, TileBase tile){
        Vector3Int pos= new Vector3Int(x,y,z);
        map.SetTile(pos,tile);
    }
    public void SetTile(Vector3 position, TileBase tile){
        Vector3Int pos = WorldToGrid(position);
        map.SetTile(pos,tile);
    }
    public void SetTile(Vector3Int position, TileBase tile){
        map.SetTile(position,tile);
    }

}