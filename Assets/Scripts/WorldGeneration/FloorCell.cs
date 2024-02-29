public class FloorCell{
    public int currentFloor;
    public bool road;
    public bool building;
    public bool occupied => road || building;
    public readonly int gridX;
	public readonly int gridY;

    public FloorCell(int x, int y, int lastFloor = -1){
        gridX = x;
        gridY = y;
        currentFloor = lastFloor;
        road = false;
        building = false;
    }
}