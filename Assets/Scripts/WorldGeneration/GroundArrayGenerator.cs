using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroundArrayGenerator : MonoBehaviour {
    [SerializeField] private TMP_InputField tMaxDimensions, tMaxValue, tRandom, tTrueCondition, tFloorCondition, tRandomMultiplier;
    [SerializeField] private TMP_Text tFirstFloorOnly;
    public int maxDimensions, maxValue, random, groundCondition, floorCondition;
    public bool firstFloorOnly;
    [SerializeField] private float randomMultiplier;
    public GroundArray GenerateGA(){
        int _maxDimensions = maxDimensions, _maxValue = maxValue, _random = random;
        int _groundCondition = groundCondition;
        int _floorCondition = floorCondition;
        float _randomMultiplier = randomMultiplier;
        int width,height, targetFloor;
        List<GACell> grounds;
        // string s = "";
        int d = Mathf.ClosestPowerOfTwo(Random.Range(1,_maxDimensions + 1)) + 1;
        width = d;
        height = d;
        d--;
        grounds = new List<GACell>();
        int[,] ints = new int[width,height];
        targetFloor = firstFloorOnly?1:Random.Range(0,2);
        Vector2Int start = Vector2Int.zero;
        List<Vector2Int> starts = new(){
            start
        };
        foreach(Vector2Int v in starts){
            ints[v.x, v.y] = Random.Range(0, _maxValue + 1);
            ints[v.x + d, v.y] = Random.Range(0, _maxValue + 1);
            ints[v.x, v.y + d] = Random.Range(0, _maxValue + 1);
            ints[v.x + d, v.y + d] = Random.Range(0, _maxValue + 1);
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
                ints[v.x + d, v.y + d] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                v += Vector2Int.right * d;
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                v += Vector2Int.one * d; 
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                v += (Vector2Int.left + Vector2Int.up)* d;
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                v += -Vector2Int.one * d;
                ints[v.x, v.y] = avg + Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
            }
            starts.Clear();
            foreach(Vector2Int v in temp){
                ints[v.x, v.y] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                ints[v.x + d, v.y] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                ints[v.x, v.y + d] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                ints[v.x + d, v.y + d] = Mathf.Clamp(avg + Random.Range(-_random, _random + 1), 0, _maxValue);
                starts.Add(v);
            }
            temp.Clear();
            d/=2;
            _random = Mathf.FloorToInt(_random * _randomMultiplier);
        }
        int small = _maxValue;
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
        //_trueCondition = Random.Range(0, _trueCondition + 1);
        /*float rel = (float)groundCondition/ _maxValue;
        int tCon = Mathf.RoundToInt(small + (big - small) * rel);*/
        _groundCondition = Mathf.RoundToInt(small + (big - small) * (float)groundCondition / _maxValue);
        _floorCondition = Mathf.RoundToInt(small + (big - small) * (float)floorCondition / _maxValue);

        //Debug.Log($"Max: {_maxValue}, Ground: {_groundCondition}, Sand:{_floorCondition} Small: {small}, Big: {big}, Rel: {rel}, tCon: {tCon}");
        for (int y = height - 1; y >= 0; y--){
            for(int x = 0; x < width; x++){
                if(ints[x,y] > _groundCondition)
                {
                    grounds.Add(new GACell { position = new Vector3Int(x, y), floor = 1 });
                }else if (ints[x, y] > _floorCondition)
                {
                    grounds.Add(new GACell { position = new Vector3Int(x, y), floor = 0 });
                }
                // s += $"[{ints[x,y]}]";
            }
            // s += "\n";
        }

        // Debug.Log(s);
        return new GroundArray { width = width, height = height, grounds = grounds, targetFloor = targetFloor, price = (targetFloor == 0 ? grounds.Count / 2 : grounds.Count) / Mathf.FloorToInt(Mathf.Sqrt(width - 1))};
    }
    public void ReadMaxDimensionsValue()
    {
        maxDimensions = int.Parse(tMaxDimensions.text);
    }
    public void ReadMaxValue()
    {
        maxValue = int.Parse(tMaxValue.text);
        Debug.Log(maxValue);
    }
    public void ReadRandom()
    {
        random = int.Parse(tRandom.text);
    }
    public void ReadTrueCondition()
    {
        groundCondition = int.Parse(tTrueCondition.text);
        Debug.Log(groundCondition);
    }
    public void ReadRandomMultiplier()
    {
        randomMultiplier = float.Parse(tRandomMultiplier.text);
    }
    public void ReadFloorCondition()
    {
        floorCondition = int.Parse(tFloorCondition.text);
    }
    public void SwitchFirstFloorOnly()
    {
        firstFloorOnly = !firstFloorOnly;
        tFirstFloorOnly.text = firstFloorOnly ? "Yes" : "No";
    }
}
public struct GACell
{
    public Vector3Int position;
    public int floor;
}