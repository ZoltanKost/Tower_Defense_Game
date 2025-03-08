using System;
using UnityEngine;

[Serializable]
public struct FloorCell{
    public int currentFloor;
    public BridgeData bridgeData;
    public bool bridge;
    public bool road;
    public bool ladder;
    public bool walkable;

    public bool building => GetBuildingIDCallback != null;
    public Func<int> GetBuildingIDCallback;
    public bool occupied => road || building || bridge;
    public readonly int gridX;
	public readonly int gridY;

    public int heapIndex,cost, left;
    public Vector2Int comeFrom;

    public FloorCell(int x, int y, int lastFloor = -1){
        gridX = x;
        gridY = y;
        currentFloor = lastFloor;
        bridgeData = default;
        bridge = false;
        road = false;
        ladder = false;
        GetBuildingIDCallback = null;
        heapIndex = -1;
        cost = -1;
        left = -1;
        comeFrom = -Vector2Int.one;
        walkable = true;
    }
    public FloorCell(
        int x, int y, 
        int currentFloor,
        BridgeData bridgeData,
        bool bridge,
        bool road,
        bool ladder)
    {
        gridX = x;
        gridY = y;
        this.currentFloor = currentFloor;
        this.bridgeData = bridgeData;
        this.bridge = bridge;
        this.road = road;
        this.ladder = ladder;
        GetBuildingIDCallback = null; 
        heapIndex = -1;
        cost = -1;
        left = -1;
        comeFrom = -Vector2Int.one;
        walkable = true;
    }
    public void Reset()
    {
        bridgeData = default;
        bridge = false;
        road = false;
        ladder = false;
        heapIndex = -1;
        cost = -1;
        left = -1;
        comeFrom = -Vector2Int.one;
    }
}
[Serializable]
public struct BridgeData
{
    public BridgeDirection bridgeDirection;
    public bool start;
    public int floor;
}
[Serializable]
public enum BridgeDirection
{
    None,
    Horizontal,
    Vertical
}