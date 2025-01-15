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
        int currentFloor = floorCells[posX + groundArray.grounds[0].position.x, posY + groundArray.grounds[0].position.y].currentFloor + 1;
        if (currentFloor < 1) currentFloor = 1;
        if (groundArray.targetFloor == 0) currentFloor = 0;
        foreach (GACell g in groundArray.grounds) {
            int x = posX + g.position.x;
            int y = posY + g.position.y;
            Vector3Int vec = pos + g.position;
            int placingFLoor = currentFloor;
            int stopFloor = floorCells[x, y].currentFloor >= 0? floorCells[x, y].currentFloor:0;
            if (placingFLoor > 0)
            {
                //if (floorCells[x, y].currentFloor == -1) floors[++floorCells[x, y].currentFloor].CreateGround(vec);
                if (floorCells[x + 1, y].currentFloor == -1) floors[++floorCells[x + 1, y].currentFloor].CreateGround(vec + Vector3Int.right);
                if (floorCells[x + 1, y - 1].currentFloor == -1) floors[++floorCells[x + 1, y - 1].currentFloor].CreateGround(vec + Vector3Int.right + Vector3Int.down);
                if (floorCells[x + 1, y - 2].currentFloor == -1) floors[++floorCells[x + 1, y - 2].currentFloor].CreateGround(vec + Vector3Int.right + Vector3Int.down * 2);
                if (floorCells[x, y - 1].currentFloor == -1) floors[++floorCells[x, y - 1].currentFloor].CreateGround(vec + Vector3Int.down);
                if (floorCells[x, y - 2].currentFloor == -1) floors[++floorCells[x, y - 2].currentFloor].CreateGround(vec + Vector3Int.down * 2);
                if (floorCells[x - 1, y - 1].currentFloor == -1) floors[++floorCells[x - 1, y - 1].currentFloor].CreateGround(vec + Vector3Int.left + Vector3Int.down);
                if (floorCells[x - 1, y - 2].currentFloor == -1) floors[++floorCells[x - 1, y - 2].currentFloor].CreateGround(vec + Vector3Int.left + Vector3Int.down * 2);
                if (floorCells[x - 1, y].currentFloor == -1) floors[++floorCells[x - 1, y].currentFloor].CreateGround(vec + Vector3Int.left);
            }
            //if (currentFloor - floorCells[x, y - 1].currentFloor > 1) floors[++floorCells[x, y - 1].currentFloor].CreateGround(vec + Vector3Int.down);
            while (placingFLoor >= stopFloor)
            {
                floors[placingFLoor--].CreateGround(vec);
            }
            floorCells[x, y].currentFloor = currentFloor;
            floorCells[x, y].bridgeData = default;
            floorCells[x, y].bridge = false;
            floorCells[x, y].road = false;
            floorCells[x, y].ladder = false;
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
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        if (floorCells[posX, posY].building || floorCells[posX, posY].bridge) return false;
        bool hasBridgeNeighbour = false;
        bool horizontal = false;
        bool vertical = false;
        FloorCell left = floorCells[posX - 1, posY];
        FloorCell right = floorCells[posX + 1, posY];
        FloorCell bot = floorCells[posX, posY - 1];
        FloorCell top = floorCells[posX, posY + 1];
        int floor = -1;
        if (left.bridge /* && !(floorCells[posX,posY].road && floorCells[posX, posY].currentFloor >= left.bridgeData.floor)*/)
        {
            hasBridgeNeighbour = true;
            if ((left.bridgeData.start && left.bridgeData.bridgeDirection == 0 || !left.bridgeData.start && left.bridgeData.bridgeDirection == BridgeDirection.Horizontal))
            {
                horizontal = true;
                left.bridgeData.bridgeDirection = BridgeDirection.Horizontal;
                floor = left.bridgeData.floor;
            }
        }
        if (right.bridge)
        {
            if (hasBridgeNeighbour && !horizontal) return false;
            hasBridgeNeighbour = true;
            if ((right.bridgeData.start && right.bridgeData.bridgeDirection == 0 || !right.bridgeData.start && right.bridgeData.bridgeDirection == BridgeDirection.Horizontal))
            {
                horizontal = true;
                right.bridgeData.bridgeDirection = BridgeDirection.Horizontal;
                floor = right.bridgeData.floor;
                if (floor != -1 && floor != right.bridgeData.floor) return false;
            }
        }
        if (bot.bridge)
        {
            if (horizontal || hasBridgeNeighbour) return false;
            if ((bot.bridgeData.start && bot.bridgeData.bridgeDirection == 0 || !bot.bridgeData.start && bot.bridgeData.bridgeDirection == BridgeDirection.Vertical))
            {
                vertical = true;
                bot.bridgeData.bridgeDirection = BridgeDirection.Vertical;
                hasBridgeNeighbour = true;
                floor = bot.bridgeData.floor;
            }
        }
        if (top.bridge)
        {
            if (horizontal || hasBridgeNeighbour && !vertical) return false;
            if ((top.bridgeData.start && top.bridgeData.bridgeDirection == 0 || !top.bridgeData.start && top.bridgeData.bridgeDirection == BridgeDirection.Vertical))
            {
                top.bridgeData.bridgeDirection = BridgeDirection.Vertical;
                hasBridgeNeighbour = true;
                floor = top.bridgeData.floor;
                if (floor != -1 && floor != top.bridgeData.floor) return false;
            }
        }
        if (floor == floorCells[posX, posY].currentFloor) return false;
        if (hasBridgeNeighbour && floor >= 0)
        {
            floorCells[left.gridX, left.gridY] = left;
            floorCells[right.gridX, right.gridY] = right;
            floorCells[bot.gridX, bot.gridY] = bot;
            floorCells[top.gridX, top.gridY] = top;
            floorCells[posX, posY].bridge = true;
            floorCells[posX, posY].bridgeData = new BridgeData {bridgeDirection = (BridgeDirection)(horizontal ? 1:2), floor = floor};
            floors[floor].PlaceBridge(pos);
            Vector3Int offset = horizontal ? Vector3Int.down : default;
            int placeFloor = floorCells[posX, posY].currentFloor >= 0 ? floorCells[posX, posY].currentFloor : 0;
            floors[placeFloor].PlaceBridgeShadow(pos + offset);
            return true;
        }
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
        floorCells[posX, posY].bridge = true;
        floorCells[posX, posY].bridgeData = new BridgeData { start = true, floor = floor };
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
        floors[floor].RemoveGround(pos,replaceWithRock,eraseUnder);
        floorCells[posX, posY].currentFloor--;
        floorCells[posX, posY].Reset();
    }
    public void MassDestroyGround(Vector3 start, Vector3 end)
    {
        Vector3Int startPos = floors[0].WorldToCell(start);
        Vector3Int endPos = floors[0].WorldToCell(end);
        if (startPos.x < -offset.x || startPos.x >= offset.x || startPos.y < 1 - offset.x || startPos.y >= offset.x) return;
        if (endPos.x < -offset.x || endPos.x >= offset.x || endPos.y < 1 - offset.x || endPos.y >= offset.x) return;
        int startPosX = startPos.x + offset.x;
        int startPosY = startPos.y + offset.y;
        int endPosX = endPos.x + offset.x;
        int endPosY = endPos.y + offset.y;
        int targetFloor = floorCells[startPosX, startPosY].currentFloor;
        if (targetFloor < 0) return;
        for (int y = startPosY; y <= endPosY; y++)
        {
            for (int x = startPosX; x <= endPosX; x++)
            {
                if (floorCells[x, y].currentFloor != targetFloor || floorCells[x, y].building) continue;
                if (floorCells[x, y + 1].currentFloor == targetFloor + 1) continue;
                bool replaceWithRock = false;
                bool eraseRoockUnder = false;
                if (targetFloor != 0)
                {
                    replaceWithRock = floorCells[x, y + 1].currentFloor == targetFloor;
                    eraseRoockUnder = floorCells[x, y - 1].currentFloor < targetFloor;
                }
                Vector3Int position = new Vector3Int(x, y) - offset;
                floors[targetFloor].RemoveGround(position, replaceWithRock, eraseRoockUnder);
                if (floorCells[x, y].ladder) floors[targetFloor].RemoveStairs(position);
                if (floorCells[x, y].road) floors[targetFloor].RemoveRoad(position);
                if (floorCells[x, y].bridge) 
                {
                    floors[targetFloor].RemoveBridgeSpot(position);
                }
                floorCells[x, y].currentFloor--;
                floorCells[x, y].ladder = false;
                floorCells[x, y].road = false;
                floorCells[x, y].bridge = false;
                floorCells[x, y].bridgeData = default;
            }
        }
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
        int currentFloor = floorCells[posX + groundArray.grounds[0].position.x, posY + groundArray.grounds[0].position.y].currentFloor;
        if(currentFloor >= floors.Count - 1) return false;
        foreach(GACell ground in groundArray.grounds){
            Vector3Int g = ground.position;
            if(!CheckCell(posX + g.x, posY + g.y,currentFloor,groundArray.targetFloor)) return false;
        }
        return true;
    }
    public bool CheckBridge(Vector3 input) {
        Vector3Int pos = floors[0].WorldToCell(input);
        if (pos.x < -offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        int floor = floorCells[posX, posY].currentFloor + 1;
        if (floor < 0 || floor > floors.Count || floorCells[posX, posY].bridge) return false;
        bool hasBridgeNeighbour = false;
        bool horizontal = false;
        bool vertical = false;
        // Debug.Log($"Checking bridge's {posX},{posY} neighbours...");
        FloorCell left = floorCells[posX - 1, posY];
        FloorCell right = floorCells[posX + 1, posY];
        FloorCell bot = floorCells[posX, posY - 1];
        FloorCell top = floorCells[posX, posY + 1];
        if (left.bridge) 
        { 
            if((left.bridgeData.start && left.bridgeData.bridgeDirection == 0) || !left.bridgeData.start && left.bridgeData.bridgeDirection == BridgeDirection.Horizontal)
            {
                horizontal = true;
            }
            hasBridgeNeighbour = true;
        }
        if (right.bridge) 
        {
            if (hasBridgeNeighbour && !horizontal) return false;
            if((right.bridgeData.start && right.bridgeData.bridgeDirection == 0) || !right.bridgeData.start && right.bridgeData.bridgeDirection == BridgeDirection.Horizontal)
            {
                horizontal = true;
            }
            hasBridgeNeighbour = true;
        }
        if (bot.bridge ) 
        {
            if (horizontal || hasBridgeNeighbour) return false;
            if(((bot.bridgeData.start && bot.bridgeData.bridgeDirection == 0) || !bot.bridgeData.start && bot.bridgeData.bridgeDirection == BridgeDirection.Vertical))
            {
                hasBridgeNeighbour = true;
                vertical = true;
            }
        }
        if (top.bridge )
        {
            if (horizontal || hasBridgeNeighbour && !vertical) return false;
            if(((top.bridgeData.start && top.bridgeData.bridgeDirection == 0) || !top.bridgeData.start && top.bridgeData.bridgeDirection == BridgeDirection.Vertical))
            {
                hasBridgeNeighbour = true;
            }
        }
        return hasBridgeNeighbour;
    }
    public bool CheckBridgeSpot(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x <  - offset.x || pos.x >= offset.x || pos.y < 1 - offset.x || pos.y >= offset.x) return false;
        int posX = pos.x + offset.x;
        int posY = pos.y + offset.y;
        FloorCell target = floorCells[posX, posY];
        if(target.bridge || target.building) return false;
        bool edge = false;
        int floor = target.currentFloor;
        List<FloorCell> temp = GetNeighbours4(posX,posY);
        foreach(FloorCell c in temp){
            if(c.currentFloor < floor) edge = true;
            if(c.bridgeData.floor == floor && c.bridge) return false;
        }
        if (!edge) return false;
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
        /*if (groundTargetFloor == 0) 
        {
            if (
                placingFloor >= 0 ||
                cell.currentFloor != placingFloor) // floor isn't the same at every cell
            {
                return false;
            }
        }
        else */if(((placingFloor >= 1 || cell.currentFloor >= 1)) && 
                    (cell.currentFloor != placingFloor 
                    || cell.currentFloor > floorCells[x, y - 1].currentFloor) || // floor isn't the same at every cell and it's 
                cell.building || // cell has a building
                cell.bridge && (cell.bridgeData.floor <= 1 || cell.bridgeData.floor <= placingFloor + 1) || // cell has a bridge
                (cell.road && !(cell.ladder)) // cell is a road and not a ladder
                || ((floorCells[x, y - 1].road || floorCells[x, y - 1].building) && floorCells[x,y-1].currentFloor == placingFloor))
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
        return floorCells[gridX,gridY].road || floorCells[gridX,gridY].bridge || floorCells[gridX,gridY].ladder;
    }
    public void DestroyRoad(Vector3Int pos)
    {
        int gridX = pos.x + offset.x;
        int gridY = pos.y + offset.y;
        int floor = floorCells[gridX,gridY].currentFloor;
        if (floorCells[gridX, gridY + 1].currentFloor == floor + 1)
        {
            floorCells[gridX, gridY].ladder = false;
            floors[floor + 1].RemoveStairs(pos);
        }
        if(floorCells[gridX, gridY].road)
        {
            floorCells[gridX,gridY].road = false;
            floors[floor].RemoveRoad(pos);
        }
        if(floorCells[gridX, gridY].bridge)
        {
            BridgeData data = floorCells[gridX, gridY].bridgeData;
            floors[data.floor].RemoveBridgeSpot(pos);
            int shadowFloor = floorCells[gridX, gridY].currentFloor >= 0 ? floorCells[gridX, gridY].currentFloor : 0;
            Vector3Int offset = floorCells[gridX, gridY].bridgeData.bridgeDirection == BridgeDirection.Horizontal ? Vector3Int.down : default;
            floors[shadowFloor].RemoveBridgeSpot(pos + offset);
            if (data.bridgeDirection == BridgeDirection.Horizontal)
            {
                if (floorCells[gridX - 1, gridY].bridge && floorCells[gridX - 1, gridY].bridgeData.start)
                {
                    floorCells[gridX - 1, gridY].bridgeData.bridgeDirection = BridgeDirection.None;
                } else if (floorCells[gridX + 1, gridY].bridge && floorCells[gridX + 1, gridY].bridgeData.start)
                {
                    floorCells[gridX + 1, gridY].bridgeData.bridgeDirection = BridgeDirection.None;
                }
            }
            else if(data.bridgeDirection == BridgeDirection.Vertical)
            {
                if (floorCells[gridX, gridY - 1].bridge && floorCells[gridX, gridY - 1].bridgeData.start)
                {
                    floorCells[gridX, gridY - 1].bridgeData.bridgeDirection = BridgeDirection.None;
                }
                else if (floorCells[gridX, gridY + 1].bridge && floorCells[gridX, gridY + 1].bridgeData.start)
                {
                    floorCells[gridX, gridY + 1].bridgeData.bridgeDirection = BridgeDirection.None;
                }
            }
            floorCells[gridX, gridY].bridgeData = default;
            floorCells[gridX, gridY].bridge = false;
            //floorCells[gridX, gridY].currentFloor--;
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
    public Vector3 CellToWorld(Vector3 position)
    {
        return floors[0].WorldToCell(position);
    }
    public void LoadFloorCells(FloorCellSaveData[] load, Vector3Int offset, Vector3Int[] roads, Vector3Int[] ladders, BridgeSaveData[] bridgeStarts, BridgeSaveData[] bridges)
    {
        foreach (Floor floor in floors)
        {
            floor.ClearAllTiles();
        }
        HashSet<Vector3Int> changed = new();
        foreach (FloorCellSaveData data in load)
        {
            FloorCell temp = new FloorCell(data.gridX, data.gridY)
            {
                currentFloor = data.currentFloor,
            };
            floorCells[data.gridX, data.gridY] = temp;
            changed.Add(new Vector3Int(data.gridX, data.gridY));
        }
        foreach (Vector3Int pos in roads)
        {
            floorCells[pos.x, pos.y].road = true;
        }
        foreach (Vector3Int pos in ladders)
        {
            floorCells[pos.x, pos.y].ladder = true;
        }
        foreach (BridgeSaveData pos in bridgeStarts)
        {
            Vector3Int gridPos = new Vector3Int(pos.gridX,pos.gridY);
            floorCells[pos.gridX, pos.gridY].bridge = true;
            floorCells[pos.gridX, pos.gridY].bridgeData.bridgeDirection = (BridgeDirection)pos.direction;
            floorCells[pos.gridX, pos.gridY].bridgeData.start = true;
            floorCells[pos.gridX, pos.gridY].bridgeData.floor = pos.floor;
            if (!changed.Contains(gridPos)) changed.Add(gridPos);
        }
        foreach (BridgeSaveData pos in bridges)
        {
            Vector3Int gridPos = new Vector3Int(pos.gridX,pos.gridY);
            floorCells[pos.gridX, pos.gridY].bridge = true;
            floorCells[pos.gridX, pos.gridY].bridgeData.bridgeDirection = (BridgeDirection)pos.direction;
            floorCells[pos.gridX, pos.gridY].bridgeData.floor = pos.floor;
            if (!changed.Contains(gridPos)) changed.Add(gridPos);
        }
        foreach (FloorCell cell in floorCells)
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
                        floors[cell.currentFloor + 1].PlaceStairs(pos - offset);
                    }
                    else
                    {
                        floors[cell.currentFloor].PlaceRoad(pos - offset);
                    }
                }
                else if (cell.bridge)
                {
                    if (cell.bridgeData.start) floors[cell.bridgeData.floor].SetBridgeSpot(pos - offset);
                    else floors[cell.bridgeData.floor].PlaceBridge(pos - offset);
                }
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
    public void FloodFloor(Vector3 start, Vector3 end)
    {
        Vector3Int startPos = floors[0].WorldToCell(start);
        Vector3Int endPos = floors[0].WorldToCell(end);
        startPos.z = 0;
        endPos.z = 0;
        if (startPos.x < -offset.x || startPos.x >= offset.x || startPos.y < 1 - offset.y || startPos.y >= offset.y) return;
        if (endPos.x < -offset.x || endPos.x >= offset.x || endPos.y < 1 - offset.y || endPos.y >= offset.y) return;
        int startPosX = startPos.x + offset.x;
        int startPosY = startPos.y + offset.y;
        int endPosX = endPos.x + offset.x;
        int endPosY = endPos.y + offset.y;
        int targetFloor = floorCells[startPosX, startPosY].currentFloor;
        for (int y = startPosY; y <= endPosY; y++)
        {
            for (int x = startPosX; x <= endPosX; x++)
            {
                if (floorCells[x, y].currentFloor != targetFloor || floorCells[x, y].building) continue;
                if (floorCells[x, y - 1].currentFloor < targetFloor) continue;
                floorCells[x, y].currentFloor++;
                floorCells[x, y].Reset();
                floors[targetFloor + 1].CreateGround(new Vector3Int(x,y) - offset);
            }
        }
        floors[targetFloor + 1].Animate();
    }
    public void EraseFloor(Vector3 start, Vector3 end)
    {

    }
    public void UpdateFloorCell(FloorCell floorCell)
    {

    }
}