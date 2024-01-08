using System.IO.Compression;
using System.Xml.Serialization;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour{
    public Tilemap[] maps;
    void Awake(){
        maps = GetComponentsInChildren<Tilemap>();
    }
    public Vector2 GridPositionToWorld(Tilemap map, int x, int y){
        Vector3Int positionInt = new(){x=x,y=0,z=y};
        Vector3 position = map.CellToWorld(positionInt);
        position.z = position.y;
        position.y = 0;
        return position;
    }
    public Vector2 GridPositionToWorld(int x, int y, int layer){
        return GridPositionToWorld(maps[layer],x, y);
    }
    public void WorldPositionToGrid(Vector2 position, out int x, out int y, int layer){
        Vector3 position3 = new Vector3(){x = position.x, y = 0, z = position.y};
        Vector3Int gridPosition = maps[layer].WorldToCell(position3);
        x = gridPosition.x;
        y = gridPosition.z;
    }
    public Vector3Int WorldToGrid(Vector3 position, int layer){
        return maps[layer].WorldToCell(position);
    }
    public void SetTile(int x, int y, TileBase tile, int layer){
        Vector3Int pos= new Vector3Int(){x = x, y = 0, z = y};
        maps[layer].SetTile(pos,tile);
    }
    public void SetTile(Vector3 position, TileBase tile, int layer){
        Vector3Int pos = WorldToGrid(position,layer);
        SetTile(pos.x, pos.y, tile,layer);
    }

}