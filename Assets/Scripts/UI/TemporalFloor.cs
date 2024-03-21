using DG.Tweening;
using UnityEngine;

public class TemporalFloor : Floor
{
    [SerializeField] private float lerpSpeed = 30f;
    [SerializeField] private float jumpingSize = 3f;
    private Vector3Int floorStart;
    private int cellSize;
    Vector3Int currentPosition;
    Vector3Int temp;
    GroundArray ground;
    bool activated;
    void Start(){
        floorStart = visuals[0].WorldToCell(transform.position);
        cellSize = Mathf.FloorToInt(visuals[0].cellSize.x);
    }
    public void CreateGroundArray(Vector3Int pos,  GroundArray groundArray){
        pos.z = 0;
        foreach(var g in groundArray.grounds){
            layer = groundArray.targetFloor;    
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    CreateGround(pos + new Vector3Int(x,y,0));
                }
            }
            if(!(groundArray.targetFloor == 0)){
                foreach(Vector3Int road in groundArray.roads)
                    PlaceRoad(pos + road);
                foreach(Vector3Int b in groundArray.bridges)
                    SetBridgeSpot(pos + b);
            }
        }
    }
    public void MoveTempFloor(Vector3 position){
        if(!activated) return;
        Vector3 vector = position / cellSize;
        temp.x = Mathf.FloorToInt(vector.x);
        temp.y = Mathf.FloorToInt(vector.y);
        temp.z = 0;
        if (temp != currentPosition){
            if((temp - currentPosition).magnitude > jumpingSize){
                transform.position = temp;
                currentPosition = temp;
                return;
            }
            transform.DOMove(temp,lerpSpeed / (temp - currentPosition).magnitude,false);
            currentPosition = temp;
        }
    }
    public void SetGroundArray(GroundArray array){
        ground = array;
        currentPosition = Vector3Int.zero;
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
