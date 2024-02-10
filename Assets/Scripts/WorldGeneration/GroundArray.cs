using UnityEngine;
using System.Collections.Generic;

public struct GroundArray{
    public float width{get; private set;}
    public float height{get; private set;}
    public int layer;
    public readonly GroundStruct[] grounds;
    public HashSet<Vector3Int> roads;
    public GroundArray(int maxExclusive, int maxLayer){
        width = 0;
        height = 0;
        layer = Random.Range(0,maxLayer);
        int min = layer == 0? 2 : 1; 
        int num = Random.Range(min, maxExclusive);
        grounds = new GroundStruct[num];
        roads = new();
        grounds[0] = new GroundStruct(){
            position = Vector3Int.zero,
            size = new Vector3Int(){
                x = Random.Range(2,maxExclusive),
                y = Random.Range(2,maxExclusive)
            }
        };
        grounds[0].Init();
        roads.UnionWith(grounds[0].roads);
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
                    x = Random.Range(2,maxExclusive),
                    y = Random.Range(2,maxExclusive)
                }
            };
            grounds[i].Init();
            roads.UnionWith(grounds[i].roads);
            width += grounds[i-1].xMax > grounds[i].xMin
                ?grounds[i].width - (grounds[i-1].xMax - grounds[i].xMin)  
                :grounds[i].width + (grounds[i].xMin - grounds[i-1].xMax);
            height += grounds[i-1].yMax > grounds[i].yMin
                ?grounds[i].height - (grounds[i-1].yMax - grounds[i].yMin) 
                :grounds[i].height + (grounds[i].yMin - grounds[i-1].yMax);
        }
    }
}
public struct GroundStruct{
    public Vector3Int position;
    public Vector3Int size;
    public HashSet<Vector3Int> roads;
    public int width => size.x;
    public int height => size.y;
    public int xMin => Mathf.Min(position.x, position.x + size.x);
    public int yMin => Mathf.Min(position.y, position.y + size.y);
    public int xMax => Mathf.Max(position.x, position.x + size.x);
    public int yMax => Mathf.Max(position.y, position.y + size.y);
    public void Init(){
        Debug.Log("Initialization!");
        roads = new();
        Vector3Int pos = new(){
            x = Random.Range(xMin,xMax),
            y = Random.Range(yMin, yMax)
        };
        int m = Random.Range(0 ,Mathf.Min(width, height)/2);
        roads.Add(pos);
        Debug.Log(m + " roads!");
        for(int i = 0; i < m; i++){
            Debug.Log(i + "st!");
            pos += new Vector3Int(){
                x = Random.Range(-1,2),
                y = Random.Range(-1,2)
            };
            if(pos.x > xMax) {
                Debug.Log(pos.x + " " + xMax);
                pos.x--;
            }
            if(pos.x < xMin - 1){ 
                Debug.Log(pos.x + " " + xMin);
                pos.x++;
            }
            if(pos.y > yMax){  
                Debug.Log(pos.y + " " + yMax);
                pos.y--;
            }
            if(pos.y < yMin - 1){ 
                Debug.Log(pos.y + " " + yMin);
                pos.y++;
            }
            roads.Add(pos);
        }
    }

    // public void AlignWith(GroundStruct g){
    //     if(Touchs(g)) return;
    //     Debug.Log(position);
    //     position += DistanceToMove(g);
    //     Debug.Log(position);
    // }
    // public Vector3Int DistanceToMove(GroundStruct g){
    //     if(Intersects(g))
    //         return size - (position - g.position);
    //     Vector3Int res = new Vector3Int();
    //     if(xMax < g.xMin){
    //         res.x = g.xMin - xMax;
    //     }else if(xMin > g.xMax){
    //         res.x = g.xMax - xMin;
    //     }
    //     if(yMax < g.yMin){
    //         res.y = g.yMin - yMax;
    //     }else if(yMin > g.yMax){
    //         res.y = g.yMax - yMin;
    //     }
    //     if(Touchs(g)) return res;
    //     Vector3Int add = new(){
    //         x = Mathf.Clamp(position.x - g.position.x, -1, -1),
    //         y = Mathf.Clamp(position.y - g.position.y, -1, -1),
    //     };
    //     return res + add;
    // }
    // public bool Intersects(GroundStruct g){
    //     return xMin < g.xMax && xMax > g.xMin && yMin < g.yMax && yMax > g.yMin;
    // }
    // public bool Touchs(GroundStruct g){
    //     return xMax == g.xMin || xMin == g.xMax && yMax == g.yMin || yMin == g.yMax;
    // }
}