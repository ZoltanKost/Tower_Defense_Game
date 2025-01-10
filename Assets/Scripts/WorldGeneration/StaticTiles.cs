using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class StaticTiles{
    private static Dictionary<TileID,TileBase> ID_Tile;
    public static void Init(){
        if(ID_Tile != null) return;
        ID_Tile = new Dictionary<TileID, TileBase>();
        ID_Tile.Add(TileID.None, null);
    }
    public static void Bind(TileBase tile, TileID ID){
        if(ID_Tile.ContainsKey(ID)) return;
        ID_Tile[ID] = tile;
    }
    public static TileBase GetTile(TileID layer){
        return ID_Tile[layer];
    }
    public static TileID GetID(TileBase tile)
    {
        foreach(var pair in ID_Tile)
        {
            if(pair.Value ==  tile) return pair.Key;
        }
        return TileID.None;
    }
}

public enum TileID{
    None,
    Foam,
    Shadow,
    Ground,
    Rock,
    Ladder,
    Grass,
    GrassPieces,
    Sand,
    SandPieces,
    Bridge,
    BridgeShadow,
    BridgeOnGround
}
