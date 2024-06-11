using System;

[Serializable]
public class FloorCell{
    public int currentFloor;
    public bool bridgeSpot;
    public bool bridge;
    public bool road;
    public bool building => buildingData.exist;
    public BuildingData buildingData;
    public bool occupied => road || building || bridgeSpot || bridge;
    public readonly int gridX;
	public readonly int gridY;

    public FloorCell(int x, int y, int lastFloor = -1){
        gridX = x;
        gridY = y;
        currentFloor = lastFloor;
    }
}
public struct BuildingData{
    public bool exist;
    public int ID;
}