using System.Collections.Generic;
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
    public GroundArray(int maxDimensions, int maxValue, int random, float randomMultiplier, int trueCondition){
        s = "";
        int d = Mathf.ClosestPowerOfTwo(Random.Range(1,maxDimensions + 1)) + 1;
        // width = Random.Range(2, maxDimensions + 1);
        // height = Random.Range(2, maxDimensions + 1);
        width = d;
        height = d;
        d-=1;
        grounds = new HashSet<Vector3Int>();
        int[,] ints = new int[width,height];
        targetFloor = Random.Range(0,2);
        Vector2Int start = Vector2Int.zero;
        List<Vector2Int> starts = new(){
            start
        };
        foreach(Vector2Int v in starts){
            ints[v.x, v.y] = Random.Range(0, maxValue + 1);
            ints[v.x + d, v.y] = Random.Range(0, maxValue + 1);
            ints[v.x, v.y + d] = Random.Range(0, maxValue + 1);
            ints[v.x + d, v.y + d] = Random.Range(0, maxValue + 1);
        }
        List<Vector2Int> temp = new();
        d/=2;
        
        while(d > 0){
            int avg = 0;
            foreach(var v in starts){
                avg += ints[v.x,v.y];
            }
            avg /= starts.Count;
            for(int i = 0; i < starts.Count; i++){
                Vector2Int v = starts[i];
                temp.Add(v);
                temp.Add(v + Vector2Int.right * d);
                temp.Add(v + Vector2Int.up * d);
                temp.Add(v + Vector2Int.one * d);
                ints[v.x + d, v.y + d] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                v += Vector2Int.right * d;
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                v += Vector2Int.one * d; 
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                v += (Vector2Int.left + Vector2Int.up)* d;
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                v += -Vector2Int.one * d;
                ints[v.x, v.y] = avg + Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
            }
            starts.Clear();
            foreach(Vector2Int v in temp){
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                ints[v.x + d, v.y] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                ints[v.x, v.y + d] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                ints[v.x + d, v.y + d] = Mathf.Clamp(avg + Random.Range(-random, random + 1), 0, maxValue);
                starts.Add(v);
            }
            temp.Clear();
            d/=2;
            random = Mathf.FloorToInt(random * randomMultiplier);
        }
        int small = maxValue;
        int big = 0;
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                if(ints[x,y] < small){
                    small = ints[x,y];
                }
                if(ints[x,y] > big){
                    big = ints[x,y];
                }
            }
        }
        trueCondition = Random.Range(0, trueCondition + 1);
        float rel = (float)trueCondition / maxValue;
        int tCon = Mathf.RoundToInt(small + (big - small) * rel);
        Debug.Log($"Max: {maxValue}, True: {trueCondition}, Small: {small}, Big: {big}, Rel: {rel}, tCon: {tCon}");
        for(int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                if(ints[x,y] > tCon){
                    grounds.Add(new Vector3Int(x,y));
                }
                s += $"[{ints[x,y]}]";
            }
            s += "\n";
        }
        
        Debug.Log(s);
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
    public GroundArray(Vector2Int dimensions){
        width = dimensions.x;
        height = dimensions.y;
        targetFloor = Random.Range(0,2);
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