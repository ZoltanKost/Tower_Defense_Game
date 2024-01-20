using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class StaticTiles{

    private static Dictionary<int,TileBase> dictionary = new Dictionary<int, TileBase>();
    public static void Bind(TileBase tile){
        int num = dictionary.Count;
        dictionary[num] = tile;
    }
    public static TileBase GetTile(int num){
        return dictionary[num];
    }
}