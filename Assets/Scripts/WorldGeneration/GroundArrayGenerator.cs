using System.Collections.Generic;
using UnityEngine;

public class GroundArrayGenerator : MonoBehaviour {
    public GroundArray GenerateGA(int maxDimensions, int maxValue, int random, float randomMultiplier, int trueCondition){
        int width,height, targetFloor;
        HashSet<Vector2Int> grounds;
        string s = "";
        int d = Mathf.ClosestPowerOfTwo(Random.Range(1,maxDimensions + 1)) + 1;
        width = d;
        height = d;
        d-=1;
        grounds = new HashSet<Vector2Int>();
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
                    grounds.Add(new Vector2Int(x,y));
                }
                s += $"[{ints[x,y]}]";
            }
            s += "\n";
        }
        
        Debug.Log(s);
        return new GroundArray{width = width,height = height, grounds = grounds, targetFloor = targetFloor};
    }
}