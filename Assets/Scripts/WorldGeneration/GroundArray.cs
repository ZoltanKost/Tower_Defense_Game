using UnityEngine;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Numerics;

public struct GroundArray{
    public float width{get; private set;}
    public float height{get; private set;}
    public int targetFloor;
    public readonly GroundStruct[] grounds;
    public HashSet<Vector3Int> roads;
    public HashSet<Vector3Int> bridges;
    public GroundArray(int maxExPieces, int maxExDimensions, int minPiexes, int minDimensions){
        width = 0;
        height = 0;
        targetFloor = Random.Range(0,2);
        // int min = targetFloor == 0 ? 2 : 1; 
        int num = Random.Range(minPiexes, maxExPieces);
        grounds = new GroundStruct[num];
        roads = new();
        bridges = new();
        grounds[0] = new GroundStruct(){
            position = Vector3Int.zero,
            size = new Vector3Int(){
                x = Random.Range(minDimensions,maxExDimensions),
                y = Random.Range(minDimensions,maxExDimensions)
            }
        };
        // grounds[0].Init((grounds[0].width + grounds[0].height)/2);
        // roads.UnionWith(grounds[0].roads);
        // bridges.UnionWith(grounds[0].bridges);
        width = grounds[0].width;
        height = grounds[0].height;
        for(int i = 1; i < grounds.Length; i++){
            bool x = Random.Range(0,2) == 1?true:false;
            grounds[i] = new GroundStruct(){
                position = new Vector3Int{
                    x = x?Random.Range(grounds[i-1].xMin,grounds[i-1].xMax):grounds[i-1].xMax,
                    y = x?grounds[i-1].yMax:Random.Range(grounds[i-1].yMin,grounds[i-1].yMax)
                },
                size = new Vector3Int(){
                    x = Random.Range(minDimensions,maxExDimensions),
                    y = Random.Range(minDimensions,maxExDimensions)
                }
            };
            // grounds[i].Init((grounds[i].width + grounds[i].height)/2);
            // roads.UnionWith(grounds[i].roads);
            // bridges.UnionWith(grounds[i].bridges);
            width += grounds[i-1].xMax > grounds[i].xMin
                ?grounds[i].width - (grounds[i-1].xMax - grounds[i].xMin)  
                :grounds[i].width + (grounds[i].xMin - grounds[i-1].xMax);
            height += grounds[i-1].yMax > grounds[i].yMin
                ?grounds[i].height - (grounds[i-1].yMax - grounds[i].yMin) 
                :grounds[i].height + (grounds[i].yMin - grounds[i-1].yMax);
        }
    }
    public GroundArray(Vector3Int dimensions, int layer){
        grounds = new GroundStruct[1];
        grounds[0] = new GroundStruct(){
            position = Vector3Int.zero,
            size = dimensions
        };
        
        // grounds[0].Init((grounds[0].width + grounds[0].height)/2);
        bridges = new();
        roads = new();
        targetFloor = layer;
        width = grounds[0].width;
        height = width = grounds[0].height;
    }
}
public struct GroundStruct{
    public Vector3Int position;
    public Vector3Int size;
    public HashSet<Vector3Int> roads;
    public HashSet<Vector3Int> bridges;
    public int width => size.x;
    public int height => size.y;
    public int xMin => Mathf.Min(position.x, position.x + size.x);
    public int yMin => Mathf.Min(position.y, position.y + size.y);
    public int xMax => Mathf.Max(position.x, position.x + size.x);
    public int yMax => Mathf.Max(position.y, position.y + size.y);
    // public void Init(int maxRoads){
    //     roads = new();
    //     bridges = new();
    //     bool w = Random.Range(0,2) == 0;
    //     bridges.Add(new Vector3Int(){
    //         x = w?Random.Range(xMin, xMax):0,
    //         y = w?0:Random.Range(yMin,yMax)
    //     });
    //     Vector3Int pos = new(){
    //         x = Random.Range(xMin,xMax),
    //         y = Random.Range(yMin, yMax)
    //     };
    //     int m = Random.Range(0 ,Mathf.Min(width, height)/2);
    //     roads.Add(pos);
    //     for(int i = 0; i < m; i++){
    //         pos += new Vector3Int(){
    //             x = Random.Range(-1,2),
    //             y = Random.Range(-1,2)
    //         };
    //         if(bridges.Contains(pos)) continue;
    //         if(pos.x > xMax) {
    //             Debug.Log(pos.x + " " + xMax);
    //             pos.x--;
    //         }
    //         if(pos.x < xMin - 1){ 
    //             Debug.Log(pos.x + " " + xMin);
    //             pos.x++;
    //         }
    //         if(pos.y > yMax){  
    //             Debug.Log(pos.y + " " + yMax);
    //             pos.y--;
    //         }
    //         if(pos.y < yMin - 1){ 
    //             Debug.Log(pos.y + " " + yMin);
    //             pos.y++;
    //         }
    //         roads.Add(pos);
    //     }
    // }
}