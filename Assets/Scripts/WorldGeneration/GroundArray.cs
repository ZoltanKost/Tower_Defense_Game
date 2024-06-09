using System.Collections.Generic;
using UnityEngine;
public struct GroundArray{
    public int width;
    public int height;
    public int targetFloor;
    public HashSet<Vector2Int> grounds;
    public GroundArray(Vector2Int dimensions, int floor){
        width = dimensions.x;
        height = dimensions.y;
        targetFloor = floor;
        grounds = new HashSet<Vector2Int>();
        string s = "";
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                    grounds.Add(new Vector2Int(x,y));
                s += $"[{100}]";
            }
            s += "\n";
        }
    }
}