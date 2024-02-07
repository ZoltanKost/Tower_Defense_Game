using UnityEngine;
using System.Collections.Generic;
public class FloorManager : MonoBehaviour{
    [SerializeField] private int layers;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Floor floorPrefab; 
    List<Floor> floors;
    private FloorCell[,] floorCells;
    Vector3Int offset;
    
    void Awake(){
        floors = new List<Floor>();
        floors.Add(Instantiate(floorPrefab, transform)); 
        floors[0].Init(0, 0);
        for(int i = 1; i < layers; i++){
            floors.Add(Instantiate(floorPrefab, transform)); 
            floors[i].Init(i, i);
        }
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < width; y++){
                floorCells[x,y] = new FloorCell(-1);
            }
        }
        offset = new Vector3Int(width/2, height/2);
    }
    // Creates ground block on the floor. Refactor.
    // Should use BoundsInt for interacting with floor.
    public bool CreateGround(Vector3 input, GroundArray groundArray){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x < - offset.x || pos.x >= offset.x || pos.y < 1 - offset.y || pos.y >= offset.y) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int currentFloor = floorCells[posX,posY].currentFloor;
        foreach(GroundStruct g in groundArray.grounds){
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    if(currentFloor != groundArray.layer - 1) return false;
                    if(floorCells[posX + x,posY + y].currentFloor >= floors.Count -1) return false;
                    if(floorCells[posX + x,posY + y].currentFloor > floorCells[posX + x,posY-1 + y].currentFloor) return false;
                    if(floorCells[posX + x, posY + y].road) return false;
                    if(floorCells[posX + x,posY + y].currentFloor != currentFloor) return false;
                }
            }
        }
        currentFloor++;
        foreach(GroundStruct g in groundArray.grounds){
            int w = g.width;
            int h = g.height;
            floors[currentFloor].CreateGroundArray(pos + g.position, w, h);
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    floorCells[posX + x,posY + y].currentFloor = currentFloor;
                    if(y != 0 || !floorCells[posX + x, posY - 1].road ) continue;
                    floors[floorCells[posX,posY].currentFloor].PlaceRoad(pos + Vector3Int.down + Vector3Int.right * x);
                }
            }
        }
        // foreach(GroundStruct g in groundArray.roads){
        //     int w = g.width;
        //     int h = g.height;
        //     floors[currentFloor].PlaceRoadArray(pos + g.position, w, h);
        //     floorCells[posX + w, posY + h].road = true;
        // }
        floors[currentFloor].Animate();
        return true;
    }
    // Creates single road. Refactor.
    public void CreateRoad(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor < 0 || floor > floors.Count) return;
        if(floorCells[posX, posY + 1].currentFloor == floor + 1)
            floors[floor + 1].PlaceRoad(pos);
        floors[floor].PlaceRoad(pos);
        floorCells[posX, posY].road = true;
    }
}