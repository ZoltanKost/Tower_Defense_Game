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
        for(int i = 0; i < layers; i++){
            floors.Add(Instantiate(floorPrefab, transform));
            if(i == 0) floors[i].Init(0,TileID.Sand);
            else floors[i].Init(i,TileID.Ground,TileID.Grass);
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
        int w = groundArray.width;
        int h = groundArray.height;
        for(int x = 0; x < w; x++){
            for(int y = 0; y < h; y++){
                if(currentFloor != groundArray.floor - 1) return false;
                if(floorCells[posX + x,posY + y].currentFloor >= floors.Count -1) return false;
                if(floorCells[posX + x,posY + y].currentFloor > floorCells[posX + x,posY-1 + y].currentFloor) return false;
                if(floorCells[posX + x, posY + y].road) return false;
                if(floorCells[posX + x,posY + y].currentFloor != currentFloor) return false;
            }
        }
        floors[++floorCells[posX,posY].currentFloor].CreateGroundArray(pos, w, h);
        for(int x = 0; x < w; x++){
            for(int y = 0; y < h; y++){
                if(y == 0){
                    if(floorCells[posX + x, posY - 1].road){
                        floors[floorCells[posX,posY].currentFloor].PlaceRoad(pos + Vector3Int.down + Vector3Int.right * x);
                    }
                    if(x == 0) continue;
                }
                floorCells[posX + x,posY + y].currentFloor ++;
            }
        }
        Debug.Log("Ground Created!");
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