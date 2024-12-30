using System.Collections.Generic;
using UnityEngine;
public struct GroundArray{
    public int width;
    public int height;
    public int targetFloor;
    public List<GACell> grounds;
    public int price;
    public GroundArray(Vector2Int dimensions, int floor){
        width = dimensions.x;
        height = dimensions.y;
        targetFloor = floor;
        price = 2;
        grounds = new List<GACell>();
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                    grounds.Add(new GACell { position = new Vector3Int(x, y), floor = targetFloor });
            }
        }
    }
}