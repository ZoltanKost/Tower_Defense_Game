using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
public struct GroundArray{
    // public float width{get; private set;}
    // public float height{get; private set;}
    public int width;
    public int height;
    public int targetFloor;
    public string s;
    public HashSet<Vector3Int> grounds;
    // public HashSet<Vector3Int> roads;
    // public HashSet<Vector3Int> bridges;
    public GroundArray(int maxDimensions, int maxSeedValue, int maxValue, int random, int randomReduce, int trueCondition){
        // width = Random.Range(2, maxDimensions + 1);
        // height = Random.Range(2, maxDimensions + 1);
        width = maxDimensions;
        height = maxDimensions;
        grounds = new HashSet<Vector3Int>();
        int[,] ints = new int[width,height];
        targetFloor = Random.Range(0,2);
        Vector2Int[] seeds = new Vector2Int[maxSeedValue];//new Vector2Int[Random.Range(1,maxSeedValue + 1)];
        for(int i = 0; i < seeds.Length; i++){
            seeds[i] = new Vector2Int{
                x = Random.Range(0,width),
                y = Random.Range(0,height)
            };
        }
        HashSet<Vector2Int> open = new(seeds);
        HashSet<Vector2Int> closed = new();
        HashSet<Vector2Int> temp = new();
        int avg = maxValue;
        while(open.Count > 0){
            foreach(var v in open){
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(- random,random + 1), 0, maxValue);
                int num = Random.Range(1,4);
                List<Vector2Int> neigs = new List<Vector2Int>();
                for(int i = 0; i < num; i++){
                    Vector2Int add = new Vector2Int(){
                        x = Random.Range(-1,2),
                        y = Random.Range(-1,2)
                    };
                    Vector2Int res = v + add;
                    if(res.x < 0 || res.x >= width || res.y < 0 || res.y >= height) continue;
                    if(closed.Contains(v + add) || open.Contains(v + add)) continue;
                    neigs.Add(v + add);
                }
                temp.UnionWith(neigs);
                closed.Add(v);
            }
            avg = 0;
            foreach(var vector in closed){
                avg += ints[vector.x, vector.y];
            }
            avg /= closed.Count;
            open.Clear();
            open.UnionWith(temp);
            temp.Clear();
            random -= randomReduce;
        }
        s = "";
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                if(ints[x,y] > trueCondition){
                    grounds.Add(new Vector3Int(x,y));
                }
                s += $"[{ints[x,y]}]";
            }
            s += "\n";
        }
        // Debug.Log(s);
    }
    public GroundArray(Vector2Int dimensions, int floor){
        width = dimensions.x;
        height = dimensions.y;
        targetFloor = floor;
        grounds = new HashSet<Vector3Int>();
        s = "";
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                    grounds.Add(new Vector3Int(x,y));
                s += $"[{100}]";
            }
            s += "\n";
        }
    }
}