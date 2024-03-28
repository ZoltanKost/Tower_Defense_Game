using UnityEngine;
using System.Collections.Generic;
public class FloorManager : MonoBehaviour{
    [SerializeField] private BuildingManager bm;
    [SerializeField] private int layers;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Floor floorPrefab; 
    [SerializeField] Pathfinding pathfinding;
    List<Floor> floors;
    public FloorCell[,] floorCells{get;private set;}
    public Vector3Int[] castlePositions{get;private set;}
    public Vector3Int offset{get;private set;}
    [SerializeField] private Building castle;
     
    void Awake(){
        floors = new List<Floor>();
        floors.Add(Instantiate(floorPrefab, transform)); 
        floors[0].Init(0,"0");
        for(int i = 1; i < layers; i++){
            floors.Add(Instantiate(floorPrefab, transform)); 
            floors[i].Init(i,$"{i}");
        }
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < width; y++){
                floorCells[x,y] = new FloorCell(x,y,-1);
            }
        }
        offset = new Vector3Int(width/2, height/2);
    }
    public void ClearFloor(){
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < width; y++){
                floorCells[x,y] = new FloorCell(x,y,-1);
            }
        }
        foreach(var floor in floors){
            floor.ClearAllTiles();
        }
    }
    public void CreateCastle(Vector3 input, Building b){
        Vector3Int pos = floors[0].WorldToCell(input);
        castlePositions = new Vector3Int[b.width * b.height];
        int last = 0;
        for(int x = 0; x < b.width; x++){
            for(int y = 0; y < b.height; y++){
                castlePositions[last++] = new Vector3Int(pos.x + x, pos.y + y);
            }
        }
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor <= 0) return;
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                if(floorCells[x, y].road || floorCells[x, y].occupied) return;
                if(floorCells[x, y].currentFloor != floor) return;
            }
        }
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                floorCells[x,y].building = true;
            }
        }
        bm.Build(pos,floor,b);
        pathfinding.SetCastlePoint(posX, posY, b.width, b.height);
    }
    public bool CreateGroundArray(Vector3 input, GroundArray groundArray){
        Vector3Int pos = floors[0].WorldToCell(input);
        pos.z = 0;
        if(pos.x < - offset.x || pos.x >= offset.x || pos.y < 1 - offset.y || pos.y >= offset.y) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int currentFloor = -2;
        Debug.Log(groundArray.s);
        foreach(Vector3Int g in groundArray.grounds){
            if(currentFloor == -2) currentFloor = floorCells[posX + g.x, posY + g.y].currentFloor; 
            if(!CheckCell(posX + g.x, posY + g.y,currentFloor,groundArray.targetFloor)) return false;
        }
        Debug.Log($"Ground on floor {currentFloor + 1} is successful!");
        currentFloor++;
        foreach(Vector3Int g in groundArray.grounds){
            floors[currentFloor].CreateGround(pos + g);
            floorCells[posX + g.x, posY + g.y].currentFloor = currentFloor;
        }
        return true;
    }
    public void PlaceRoad(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied){ 
            if(floorCells[posX, posY].road && floorCells[posX, posY + 1].currentFloor == floor + 1){
                floors[floor + 1].PlaceStairs(pos);

            }    
            return;
        }
        if(floorCells[posX, posY + 1].currentFloor == floor + 1)
                floors[floor + 1].PlaceStairs(pos);
        else if(floor == 0) return;
        else floors[floor].PlaceRoad(pos);
        floorCells[posX, posY].road = true;
        // Debug.Log($"RoadPlaced: {posX},{posY}");
    }
    public void PlaceBridge(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor + 1;
        if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied) return;
        bool hasBridgeNeighbour = false;
        // Debug.Log($"Checking bridge's {posX},{posY} neighbours...");
        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++){
                // Debug.Log($"Checking {posX + x},{posY + y}");
                FloorCell checking = floorCells[posX + x, posY + y];
                if(!(checking.bridgeSpot || checking.bridge)) {
                    // Debug.Log($"Cell isn't a bridge spot.");
                    continue;
                }
                Debug.Log($"{posX + x},{posY + y} is a bridge!");
                floor = checking.currentFloor;
                hasBridgeNeighbour = true;
            }
        }
        if(!hasBridgeNeighbour) return;
        floors[floor].PlaceBridge(pos);
        floorCells[posX, posY].currentFloor = floor;
        floorCells[posX, posY].bridge = true;
        Debug.Log($"New floor: {floor}, on Cell: {posX},{posY}");
    }
    public bool PlaceBuilding(Vector3 input, Building b){
        if(b == castle) CreateCastle(input,b);
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor <= 0) return false;
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                if(floorCells[x, y].road || floorCells[x, y].occupied) return false;
                if(floorCells[x, y].currentFloor != floor) return false;
            }
        }
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                floorCells[x,y].building = true;
            }
        }
        bm.Build(pos,floor,b);
        return true;
    }
    public void PlaceBridgeSpot(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        FloorCell target = floorCells[posX, posY];
        if(target.bridge || target.bridgeSpot || target.building) return;
        bool edge = false;
        int floor = target.currentFloor;
        List<FloorCell> temp = GetNeighbours4(posX,posY);
        foreach(FloorCell c in temp){
            if(c.currentFloor == floor - 1) edge = true;
            if(c.currentFloor == floor && c.bridgeSpot) return;
        }
        if(!edge) return;
        target.bridgeSpot = true;
        floors[floor].SetBridgeSpot(pos);
    }
    public void SetBridgeSpot(Vector3Int pos){
        FloorCell target = floorCells[pos.x, pos.y];
        int floor = target.currentFloor;
        target.bridgeSpot = true;
        floors[floor].SetBridgeSpot(pos);
    }

    public bool CheckCell(int x, int y, int placingFloor, int groundTargetFloor){
        FloorCell cell = floorCells[x,y];
        if(
            // (placingFloor + 1 != 0 && groundTargetFloor == 0) || // floor isn't the same as on ground
            Mathf.Clamp(placingFloor + 1,0,1) != groundTargetFloor ||
            cell.currentFloor >= floors.Count -1 || // floor is higher then maximum floors
            cell.currentFloor != placingFloor || // floor isn't the same at every cell
            cell.building || // cell has a building
            cell.bridgeSpot || cell.bridge || // cell has a bridge
            // floorCells[x, y - 1].bridgeSpot || floorCells[x, y - 1].bridge ||
            (cell.road && !(floorCells[x, y + 1].currentFloor > cell.currentFloor)) || // cell is a ladder
            cell.currentFloor > floorCells[x, y - 1].currentFloor) // the next cell is under the floor
            {
                Debug.Log("cell " + x + " " + y + " can't be placed");
                Debug.Log("choosenFloor: " + cell.currentFloor + " buildable:" + cell.occupied);
                Debug.Log("placingFloor: " + placingFloor + " targetFloor: " + groundTargetFloor);
                return false; 
            }
            
        return true;
    }
    public FloorCell WorldPositionToCell(Vector3 position){
        Vector3Int pos = floors[0].WorldToCell(position);
        pos += offset;
        return floorCells[pos.x,pos.y];
    }
    public int GetMaxHeapSize(){
        return width*height;
    }
    public List<FloorCell> GetNeighbours4(int gridX, int gridY){
        List<FloorCell> neighbours = new List<FloorCell>();
        if(gridX - 1 >= 0)neighbours.Add(floorCells[gridX - 1, gridY]);
        if(gridX + 1 < width)neighbours.Add(floorCells[gridX + 1, gridY]);
        if(gridY + 1 < height)neighbours.Add(floorCells[gridX, gridY + 1]);
        if(gridY - 1 >= 0)neighbours.Add(floorCells[gridX, gridY - 1]);
        return neighbours;
    }
    public bool IsStarting(int gridX, int gridY){
        return floorCells[gridX, gridY].road && 
            floorCells[gridX, gridY].currentFloor == 0 &&
            floorCells[gridX,gridY - 1].currentFloor == 0;
            
    }
}