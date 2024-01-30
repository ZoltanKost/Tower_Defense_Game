public struct FloorCell{
    public readonly int x;
    public readonly int y;
    public int currentFloor;
    public FloorCell(int x, int y, int lastFloor = 0){
        this.x = x;
        this.y = y;
        this.currentFloor = lastFloor;
    }
}