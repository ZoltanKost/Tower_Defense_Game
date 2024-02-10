public struct FloorCell{
    public int currentFloor;
    public bool road;
    public bool ocqupied;
    public FloorCell(int lastFloor = 0){
        currentFloor = lastFloor;
        road = false;
        ocqupied = false;
    }
}