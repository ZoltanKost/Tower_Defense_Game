using UnityEngine;
using System.Collections.Generic;
public class FloorManager : MonoBehaviour{
    [SerializeField] private int layers;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Floor floorPrefab; 
    [SerializeField] Pathfinding pathfinding;
    List<Floor> floors;
    public FloorCell[,] floorCells{get;private set;}
    public Vector3Int[] castlePositions{get;private set;}
    public Vector3Int offset{get;private set;}
    
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
                floorCells[x,y] = new FloorCell(x,y,-1);
            }
        }
        offset = new Vector3Int(width/2, height/2);
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
        b.Build(pos,floor * 5 + 5);
        pathfinding.SetCastlePoint(posX, posY, b.width, b.height);
    }
    public bool CreateGroundArray(Vector3 input, GroundArray groundArray){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x < - offset.x || pos.x >= offset.x || pos.y < 1 - offset.y || pos.y >= offset.y) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int currentFloor = floorCells[posX,posY].currentFloor;
        if(!CheckGroundArray(groundArray, posX, posY,currentFloor)) return false;
        currentFloor++;
        PlaceGroundArray(groundArray, currentFloor,pos);
        floors[currentFloor].Animate();
        return true;
    }
    public void CreateRoad(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied) return;
        if(floorCells[posX, posY + 1].currentFloor == floor + 1)
            floors[floor + 1].PlaceRoad(pos);
        else if(floor == 0) return;
        floors[floor].PlaceRoad(pos);
        floorCells[posX, posY].road = true;
        Debug.Log($"RoadPlaced: {posX},{posY}");
    }
    public void PlaceGroundArray(GroundArray ga, int currentFloor, Vector3Int pos){
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        ga.layer = ga.layer > 0? 1: 0;
        foreach(GroundStruct g in ga.grounds){
            int w = g.width;
            int h = g.height;
            floors[currentFloor].CreateGroundArray(pos + g.position, w, h);
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    floorCells[posX + x,posY + y].currentFloor = currentFloor;
                    if(y!=g.yMin || !floorCells[x, y].road) continue;
                    floors[floorCells[posX + x,posY + y].currentFloor].PlaceRoad(pos + Vector3Int.down * g.yMin + Vector3Int.right * x);
                }
            }
        }
        if(!(ga.layer == 0))
        foreach(Vector3Int road in ga.roads){
            if(floorCells[(pos + road).x + offset.x, (pos + road).y + offset.y].currentFloor != currentFloor){
                floors[currentFloor].PlaceBridge(pos + road);
            }else{
                floors[currentFloor].PlaceRoad(pos + road);
            }
            floorCells[posX + road.x,posY + road.y].currentFloor = currentFloor;
            floorCells[posX + road.x, posY + road.y].road = true;
        }
    }
    public void PlaceBuilding(Vector3 input, Building b){
        Vector3Int pos = floors[0].WorldToCell(input);
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
        b.Build(pos,floor * 5 + 5);
    }
    public bool CheckGroundArray(GroundArray ga, int posX, int posY, int currentFloor){
        foreach(GroundStruct g in ga.grounds){
            for(int x = g.xMin; x < g.xMax; x++){
                for(int y = g.yMin; y < g.yMax; y++){
                    if(!CheckCell(posX + x, posY + y, currentFloor, ga.layer))
                        return false;
                }
            }
        }
        return true;
    }

    public bool CheckCell(int x, int y, int placingFloor, int groundTargetFloor){
        FloorCell cell = floorCells[x,y];
        if(
            placingFloor + 1 != groundTargetFloor || // floor isn't the same as on ground
            // cell.currentFloor >= floors.Count -1 || // floor is higher then maximum floors
            cell.currentFloor != placingFloor || // floor isn't the same at every cell
            cell.building || // cell has a building
            cell.road && !(floorCells[x, y + 1].currentFloor > cell.currentFloor) ||
            cell.currentFloor > floorCells[x, y - 1].currentFloor) // the next cell is under the floor
            {
                Debug.Log("cell " + x + " " + y + " can't be placed");
                Debug.Log("choosenFloor: " + cell.currentFloor + " buildable" + cell.occupied);
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