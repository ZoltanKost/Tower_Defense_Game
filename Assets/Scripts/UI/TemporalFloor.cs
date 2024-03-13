using System.Data;
using UnityEngine;

public class TemporalFloor : Floor
{
    Vector3Int currentPosition;
    Vector3Int temp;
    GroundArray ground;
    bool activated;
    // Is used for generating temporal visuals.
    public void CreateGroundArray(Vector3Int pos,  GroundArray groundArray){
        pos.z = 0;
        foreach(var g in groundArray.grounds){
            layer = groundArray.layer;    
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    CreateGround(pos + new Vector3Int(x,y,0));
                }
            }
            if(!(groundArray.layer == 0)){
                foreach(Vector3Int road in groundArray.roads)
                    PlaceRoad(pos + road);
                foreach(Vector3Int b in groundArray.bridges)
                    SetBridge(pos + b);
            }
        }
    }
    // Is used for generating temporal visuals.
    public void MoveTempFloor(Vector3 position){
        if(!activated) return;
        temp = WorldToCell(position);
        temp.z = 0; 
        if (temp != currentPosition){
            Vector3Int delta = temp - currentPosition;
            delta *= Mathf.FloorToInt(visuals[0].cellSize.x); 
            transform.position = Vector3.Lerp(transform.position, transform.position + temp,1f);
            currentPosition = temp;
        }
    }
    // Is used for generating temporal visuals.
    public void SetGroundArray(GroundArray array){
        ground = array;
        ClearAllTiles();
        CreateGroundArray(currentPosition, array);
    }
    public void ActivateFloor(GroundArray ga){
        activated = true;
        SetGroundArray(ga);
    }
    public void DeactivateFloor(){
        ClearAllTiles();
        activated = false;
    }
}
