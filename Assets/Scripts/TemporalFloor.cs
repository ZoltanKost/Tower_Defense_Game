using UnityEngine;

public class TemporalFloor : Floor
{
    Vector3Int currentPosition;
    Vector3Int temp;
    int w,h;
    GroundArray groundArray;
    // Is used for generating temporal visuals.
    public void CreateGroundArray(Vector3Int start){
        w = groundArray.width;
        h = groundArray.height;
        layer = groundArray.floor;    
        for(int x = start.x; x < start.x + w; x ++){
            for(int y = start.y; y < start.y + h; y++){
                CreateGround(new Vector3Int(x,y,0));
            }
        }
    }
    // Is used for generating temporal visuals.
    public void MoveTempFloor(Vector3 position){
        temp = WorldToCell(position);
        if (temp != currentPosition){
            ClearAllTiles();
            CreateGroundArray(temp);
            currentPosition = temp;
        }
    }
    // Is used for generating temporal visuals.
    public void SetGroundArray(GroundArray array){
        groundArray = array;
        ClearAllTiles();
        CreateGroundArray(currentPosition);
    }
}
