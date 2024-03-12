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
        if (temp != currentPosition){
            ClearAllTiles();
            CreateGroundArray(temp, ground);
            currentPosition = temp;
        }
    }
    // Is used for generating temporal visuals.
    public void SetGroundArray(GroundArray array){
        ground = array;
        ClearAllTiles();
        CreateGroundArray(currentPosition, array);
    }
    public void ActivateFloor(){
        activated = true;
    }
    public void DeactivateFloor(){
        activated = false;
    }
}
