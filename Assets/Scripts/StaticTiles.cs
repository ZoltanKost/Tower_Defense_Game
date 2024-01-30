using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public static class StaticTiles{
    private static Dictionary<TileID,TileBase> ID_Tile;
    public static void Init(){
        if(ID_Tile != null) return;
        ID_Tile = new Dictionary<TileID, TileBase>();
        ID_Tile.Add(TileID.None, null);
    }
    public static void Bind(TileBase tile, TileID ID){
        ID_Tile[ID] = tile;
        Debug.Log((int)ID);
    }
    public static TileBase GetTile(TileID layer){
        return ID_Tile[layer];
    }
}

public enum TileID{
    Shadow = 0, //0000
    Ground = 1, // 0001
    Grass = 2, // 0010
    Sand = 3, // 0011
    None = 4, // 0100
    Alt = 1 << 3 // 1000
}
