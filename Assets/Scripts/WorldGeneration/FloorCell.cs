using System;

[Serializable]
public struct FloorCell{
    public int currentFloor;
    public bool bridgeSpot;
    public bool bridge;
    public bool road;
    public bool ladder;
    public bool building => GetBuildingIDCallback != null;
    public Func<int> GetBuildingIDCallback;
    public bool occupied => road || building || bridgeSpot || bridge;
    public readonly int gridX;
	public readonly int gridY;

    public FloorCell(int x, int y, int lastFloor = -1){
        gridX = x;
        gridY = y;
        currentFloor = lastFloor;
        bridgeSpot = false;
        bridge = false;
        road = false;
        ladder = false;
        GetBuildingIDCallback = null;
}
    public void Reset()
    {
        road = false;
        ladder = false;
    }
}