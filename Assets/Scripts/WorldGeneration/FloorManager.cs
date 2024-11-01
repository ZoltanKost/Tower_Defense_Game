using UnityEngine;
using System.Collections.Generic;
using System;

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
        floors = new List<Floor>
        {
            Instantiate(floorPrefab, transform)
        };
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
            for(int y = 0; y < height; y++){
                floorCells[x,y] = new FloorCell(x,y,-1);
            }
        }
        foreach(Floor floor in floors){
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
        bm.Build(pos,posX,posY, floor,b, out Func<int> getIndex);
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                floorCells[x,y].GetBuildingIDCallback = getIndex;
            }
        }
        pathfinding.SetCastlePoint(posX, posY, b.width, b.height);
    }
    // public bool CreateGroundArray(Vector3 input, GroundArray groundArray){
    //     Vector3Int pos = floors[0].WorldToCell(input);
    //     pos.z = 0;
    //     if(pos.x < - offset.x || pos.x >= offset.x || pos.y < 1 - offset.y || pos.y >= offset.y) return false;
    //     int posX = pos.x + offset.x;
    //     int posY = pos.y + offset.y;
    //     int currentFloor = -2;
    //     // Debug.Log(groundArray.s);
    //     foreach(Vector3Int g in groundArray.grounds){
    //         if(currentFloor == -2) currentFloor = floorCells[posX + g.x, posY + g.y].currentFloor; 
    //         if(!CheckCell(posX + g.x, posY + g.y,currentFloor,groundArray.targetFloor)) return false;
    //     }
    //     // Debug.Log($"Ground on floor {currentFloor + 1} is successful!");
    //     currentFloor++;
    //     foreach(Vector3Int g in groundArray.grounds){
    //         floors[currentFloor].CreateGround(pos + g);
    //         floorCells[posX + g.x, posY + g.y].currentFloor = currentFloor;
    //         floorCells[posX + g.x, posY + g.y].Reset();
    //     }
    //     floors[currentFloor].Animate();
    //     return true;
    // }
    public void CreateGroundArray_DontCheck(Vector3 input, GroundArray groundArray){
        Vector3Int pos = floors[0].WorldToCell(input);
        pos.z = 0;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int currentFloor = -2;
        foreach(Vector3Int g in groundArray.grounds){
            if(currentFloor == -2) currentFloor = floorCells[posX + g.x, posY + g.y].currentFloor + 1;
            floors[currentFloor].CreateGround(pos + g);
            floorCells[posX + g.x, posY + g.y].currentFloor = currentFloor;
            floorCells[posX + g.x, posY + g.y].Reset();
        }
        floors[currentFloor].Animate();
    }
    public bool PlaceRoad(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        //if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        /*if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied){ 
            // if(floorCells[posX, posY].road && floorCells[posX, posY + 1].currentFloor == floor + 1){
            //     floors[floor + 1].PlaceStairs(pos);
            //     return true;
            // }    
            return false;
        }*/
        if(floorCells[posX, posY + 1].currentFloor == floor + 1){
            floorCells[posX, posY].ladder = true;
            floors[floor + 1].PlaceStairs(pos);
        }
        //else if(floor == 0) return false;
        else floors[floor].PlaceRoad(pos);
        floorCells[posX, posY].road = true;
        return true;
    }
    public bool PlaceBridge(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        //if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor + 1;
        /*if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied) return false;
        bool hasBridgeNeighbour = false;
        // Debug.Log($"Checking bridge's {posX},{posY} neighbours...");
        for(int x = -1; x <= 1; x++){
            // Debug.Log($"Checking {posX + x},{posY + y}");
            FloorCell checking = floorCells[posX + x, posY];
            if(!(checking.bridgeSpot || checking.bridge)) {
                // Debug.Log($"Cell isn't a bridge spot.");
                continue;
            }
            // Debug.Log($"{posX + x},{posY} is a bridge!");
            floor = checking.currentFloor;
            hasBridgeNeighbour = true;
        }
        for(int y = -1; y <= 1; y++){
            // Debug.Log($"Checking {posX + x},{posY + y}");
            FloorCell checking = floorCells[posX, posY + y];
            if(!(checking.bridgeSpot || checking.bridge)) {
                // Debug.Log($"Cell isn't a bridge spot.");
                continue;
            }
            // Debug.Log($"{posX},{posY + y} is a bridge!");
            floor = checking.currentFloor;
            hasBridgeNeighbour = true;
        }
        if(!hasBridgeNeighbour) return false;*/
        floors[floor].PlaceBridge(pos);
        floorCells[posX, posY].currentFloor = floor;
        floorCells[posX, posY].bridge = true;
        // Debug.Log($"New floor: {floor}, on Cell: {posX},{posY}");
        return false;
    }
    // public bool PlaceBuilding(Vector3 input, Building b){
    //     if(b == castle) CreateCastle(input,b);
    //     Vector3Int pos = floors[0].WorldToCell(input);
    //     if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
    //     int posX = pos.x + offset.x;
    //     int posY = pos.y + offset.y;
    //     int floor = floorCells[posX,posY].currentFloor;
    //     if(floor <= 0) return false;
    //     for(int x = posX; x < posX + b.width; x++){
    //         for(int y = posY; y < posY + b.height; y++){
    //             if(floorCells[x, y].occupied || floorCells[x, y].currentFloor != floor) return false;
    //         }
    //     }
    //     bm.Build(pos,posX,posY, floor,b,out Func<int> getIndex);
    //     for(int x = posX; x < posX + b.width; x++){
    //         for(int y = posY; y < posY + b.height; y++){
    //             floorCells[x,y].GetBuildingIDCallback = getIndex;
    //         }
    //     }
    //     return true;
    // }
    public void PlaceBuilding_DontCheck(Vector3 input, Building b){
        Vector3Int pos = floors[0].WorldToCell(input);
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        bm.Build(pos,posX,posY, floor,b,out Func<int> getIndex);
        for(int x = posX; x < posX + b.width; x++){
            for(int y = posY; y < posY + b.height; y++){
                floorCells[x,y].GetBuildingIDCallback = getIndex;
            }
        }
    }
    public void PlaceBuilding_DontCheck(BuildingSaveData data)
    {
        int floor = floorCells[data.gridPosition.x, data.gridPosition.y].currentFloor;
        bm.Build(data, out Func<int> getIndex);
        for (int x = data.gridPosition.x; x < data.gridPosition.x + data.width; x++)
        {
            for (int y = data.gridPosition.y; y < data.gridPosition.y + data.height; y++)
            {
                floorCells[x, y].GetBuildingIDCallback = getIndex;
            }
        }
    }
    public bool PlaceBridgeSpot(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        //if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        FloorCell target = floorCells[posX, posY];
        int floor = target.currentFloor;
        /*if(target.bridge || target.bridgeSpot || target.building) return false;
        bool edge = false;
        List<FloorCell> temp = GetNeighbours4(posX,posY);
        foreach(FloorCell c in temp){
            if(c.currentFloor == floor - 1) edge = true;
            if(c.currentFloor == floor && c.bridgeSpot) return false;
        }
        if(!edge) return false;*/
        floorCells[posX, posY].bridgeSpot = true;
        floors[floor].SetBridgeSpot(pos);
        return true;
    }
    // public void SetBridgeSpot(Vector3Int pos){
    //     FloorCell target = floorCells[pos.x, pos.y];
    //     int floor = target.currentFloor;
    //     target.bridgeSpot = true;
    //     floors[floor].SetBridgeSpot(pos);
    // }
    public void DestroyGround(Vector3 input)
    {
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        FloorCell target = floorCells[posX, posY];
        if(target.occupied) return;
        int floor = target.currentFloor;
        if(floorCells[posX, posY + 1].currentFloor == floor + 1) return;
        bool replaceWithRock = false;
        bool eraseUnder = false;        
        if(floor != 0)
        {
            replaceWithRock = floorCells[posX, posY + 1].currentFloor == floor;
            eraseUnder = floorCells[posX, posY - 1].currentFloor < floor;
        } 
        floors[target.currentFloor].RemoveGround(pos,replaceWithRock,eraseUnder);
        floorCells[posX, posY].currentFloor--;
    }
    public bool CheckBuilding(Vector3 input, int w, int h){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor <= 0) return false;
        for(int x = posX; x < posX + w; x++){
            for(int y = posY; y < posY + h; y++){
                if(floorCells[x, y].occupied || floorCells[x, y].currentFloor != floor) return false;
            }
        }
        return true;
    }
    public bool CheckGA(Vector3 input, GroundArray groundArray){
        Vector3Int pos = floors[0].WorldToCell(input);
        pos.z = 0;
        if(pos.x < - offset.x || pos.x >= offset.x || pos.y < 1 - offset.y || pos.y >= offset.y) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int currentFloor = -2;
        foreach(Vector3Int g in groundArray.grounds){
            if(currentFloor == -2) currentFloor = floorCells[posX + g.x, posY + g.y].currentFloor; 
            if(!CheckCell(posX + g.x, posY + g.y,currentFloor,groundArray.targetFloor)) return false;
        }
        return true;
    }
    public bool CheckBridge(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor + 1;
        if(floor < 0 || floor > floors.Count || (floorCells[posX,posY].occupied && !floorCells[posX,posY].road) 
            || (floorCells[posX,posY].road && !floorCells[posX,posY].ladder)) return false;
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
                // Debug.Log($"{posX + x},{posY + y} is a bridge!");
                hasBridgeNeighbour = true;
            }
        }
        if(!hasBridgeNeighbour) return false;
        return true;
    }
    public bool CheckBridgeSpot(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        FloorCell target = floorCells[posX, posY];
        if(target.bridge || target.bridgeSpot || target.building) return false;
        bool edge = false;
        int floor = target.currentFloor;
        List<FloorCell> temp = GetNeighbours4(posX,posY);
        foreach(FloorCell c in temp){
            if(c.currentFloor == floor - 1) edge = true;
            if(c.currentFloor == floor && c.bridgeSpot) return false;
        }
        if(!edge) return false;
        return true;
    }
    public bool CheckRoad(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX,posY].currentFloor;
        if(floor < 0 || floor > floors.Count || floorCells[posX,posY].occupied){ 
            if(floorCells[posX, posY].road && floorCells[posX, posY + 1].currentFloor == floor + 1){
                floors[floor + 1].PlaceStairs(pos);
                return true;
            }    
            return false;
        }
        return true;
    }
    public bool CheckCell(int x, int y, int placingFloor, int groundTargetFloor){
        FloorCell cell = floorCells[x,y];
        if(
            Mathf.Clamp(placingFloor + 1,0,1) != groundTargetFloor ||
            cell.currentFloor >= floors.Count -1 || // floor is higher then maximum floors
            cell.currentFloor != placingFloor || // floor isn't the same at every cell
            cell.building || // cell has a building
            cell.bridgeSpot || cell.bridge || // cell has a bridge
            (cell.road && !(floorCells[x, y + 1].currentFloor == cell.currentFloor + 1)) || // cell is a road and not a ladder
            cell.currentFloor > floorCells[x, y - 1].currentFloor || // the next cell is under the floor
            (floorCells[x, y - 1].occupied && floorCells[x,y-1].currentFloor < placingFloor))
            {
                return false; 
            }
            
        return true;
    }
    public bool HasBuilding(Vector3 input, out int ID)
    {
        Vector3Int pos = floors[0].WorldToCell(input);
        ID = -1;
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        if(floorCells[posX,posY].building) {
            ID = floorCells[posX,posY].GetBuildingIDCallback.Invoke();
            return true;
        }
        return false;
    }
    public bool HasRoad(Vector3 input, out Vector3Int pos)
    {   
        pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int gridX = pos.x + offset.x;
        int gridY = pos.y + offset.y;
        return floorCells[gridX,gridY].road || floorCells[gridX,gridY].bridge || floorCells[gridX,gridY].bridgeSpot || floorCells[gridX,gridY].ladder;
    }
    public void DestroyRoad(Vector3Int pos)
    {
        int gridX = pos.x + offset.x;
        int gridY = pos.y + offset.y;
        int floor = floorCells[gridX,gridY].currentFloor;
        if(floorCells[gridX, gridY + 1].currentFloor == floor + 1)
        {
            floorCells[gridX, gridY].ladder = false;
            floors[floor + 1].RemoveStairs(pos);
            return;
        }
        if(floorCells[gridX, gridY].road)
        {
            floorCells[gridX,gridY].road = false;
            floors[floor].RemoveRoad(pos);
        }
        if(floorCells[gridX, gridY].bridge)
        {
            floorCells[gridX, gridY].bridge = false;
            floors[floor].RemoveBridge(pos);
            floorCells[gridX, gridY].currentFloor--;
        }
        else if(floorCells[gridX, gridY].bridgeSpot)
        {
            floorCells[gridX, gridY].bridgeSpot = false;
            floors[floor].RemoveBridgeSpot(pos);
        }
    }
    public void DestroyBuilding(int gridX, int gridY, int w, int h){
        for(int x = gridX; x < gridX + w; x++){
            for(int y = gridY; y < gridY + h; y++){
                floorCells[x,y].GetBuildingIDCallback = null;
            }
        }
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
    public Vector3Int WorldToCell(Vector3 position)
    {
        return floors[0].WorldToCell(position);
    }
    public void LoadFloorCells(FloorCellSaveData[] load, Vector3Int offset)
    {
        foreach (Floor floor in floors)
        {
            floor.ClearAllTiles();
        }
        HashSet<Vector3Int> changed = new();
        foreach (FloorCellSaveData data in load)
        {
            floorCells[data.gridX, data.gridY] = new FloorCell
            (
                data.gridX,
                data.gridY, 
                data.currentFloor,
                data.bridgeSpot,
                data.bridge,
                data.road,
                data.ladder
            );
            changed.Add(new Vector3Int(data.gridX, data.gridY));
        }
        foreach(FloorCell cell in floorCells)
        {
            Vector3Int pos = new Vector3Int {x = cell.gridX, y = cell.gridY};
            if (changed.Contains(pos))
            {
                int floor = cell.currentFloor;
                while(floor >= 0)
                {
                    floors[floor--].CreateGround(pos - offset);
                }
                if (cell.road)
                {
                    if (cell.ladder)
                    {
                        floors[cell.currentFloor].PlaceStairs(pos - offset);
                    }
                    else
                    {
                        floors[cell.currentFloor].PlaceRoad(pos - offset);
                    }
                }
                if (cell.bridgeSpot) floors[cell.currentFloor].SetBridgeSpot(pos - offset);
                if (cell.bridge) floors[cell.currentFloor].PlaceBridge(pos - offset);
                Debug.Log("Creating changed location...");
            }
            else
            {
                floorCells[cell.gridX, cell.gridY] = new FloorCell(cell.gridX, cell.gridY);
            }
        }
        foreach (Floor floor in floors)
        {
            floor.Animate();
        }
    }
    public void UpdateFloorCell(FloorCell floorCell)
    {

    }
}