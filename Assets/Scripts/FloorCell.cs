public struct FloorCell{
    public int currentFloor;
    public bool road;
    public bool floorEdge;
    public FloorCell(int lastFloor = 0){
        currentFloor = lastFloor;
        road = false;
        floorEdge = false;
    }
}