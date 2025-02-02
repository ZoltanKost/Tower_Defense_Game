﻿using UnityEngine;
using System.Collections.Generic;
public class Pathfinding : MonoBehaviour{
	[SerializeField] FloorManager floor;
	[SerializeField] private int maxPaths = 200;
	public List<FloorCell> castlePositions;
	// List<List<FloorCell>> paths = new();
	public List<Queue<PathCell>> vectors{get;private set;}
	int offsetX, offsetY;
	float cellSize;
	public void SetCastlePoint(int gridX, int gridY, int width, int height){
		if(castlePositions == null)castlePositions = new List<FloorCell>();
		gridX += width/2;
		Debug.Log(message: $"Castle: {gridX},{gridY}");
		floor.floorCells[gridX, gridY].road = true;
        FloorCell pos = floor.floorCells[gridX,gridY];
		castlePositions.Add(pos);
	}
	public void ClearCastlePoint(){
		castlePositions.Clear();
	}
	public void Awake(){
		vectors = new List<Queue<PathCell>>();
	}
	public bool FindPathToCastle(){
		offsetX = floor.offset.x;
		offsetY = floor.offset.y;
		cellSize = floor.GetComponent<Grid>().cellSize.x;
		// paths.Clear();
		vectors.Clear();
		Stack<FloorCell> closedSet = new();
		foreach(FloorCell graph in castlePositions){
			BFSearch(graph,closedSet,vectors);
		}
		// Debug.Log(vectors.Count);
		return vectors.Count > 0;
	}
	public void BFSearch(FloorCell current, Stack<FloorCell> closedSet, List<Queue<PathCell>> result){
		if(result.Count > maxPaths) return;
		if(!(current.road || current.bridge)) return;
		if(closedSet.Count != 0)
		{
			FloorCell prev = closedSet.Peek();
			if((prev.ladder || current.ladder) && prev.gridY == current.gridY) return;
			if (current.bridge)
			{
				if ((prev.bridge && (prev.bridgeData.bridgeDirection != current.bridgeData.bridgeDirection || prev.bridgeData.floor != current.bridgeData.floor)) || (!prev.bridge && !current.bridgeData.start)) return;
			}
			else if (prev.bridge && !prev.bridgeData.start) return;
			else
			{
				if (current.currentFloor == prev.currentFloor - 1)
				{
					Debug.Log($"Current is under: current: {current.gridX}:{current.gridY};{current.currentFloor}; prev: {prev.gridX}:{prev.gridY};{prev.currentFloor}");
					if ((current.gridY != prev.gridY - 1)) {
						Debug.Log("current is not up of previous cell");
						return;
					}
					else
					{
                        Debug.Log("current is up of previous cell");
                    }
                }
				else if (current.currentFloor == prev.currentFloor + 1)
				{
					Debug.Log($"Current is upon: current: {current.gridX}:{current.gridY};{current.currentFloor}; prev: {prev.gridX}:{prev.gridY};{prev.currentFloor}");
					if ((current.gridY != prev.gridY + 1))
                    {
                        Debug.Log("current is not up of previous cell");
                        return;
                    }
                    else
                    {
                        Debug.Log("current is up of previous cell");
                    }
                }
			}
        }
		if(floor.IsStarting(current.gridX, current.gridY)){
			if(closedSet.Count == 0) return;
			string s =
			$"Path nr.{result.Count} just finded! Start: {current.gridX},{current.gridY}, Cells:{closedSet.Count}.\n";
			closedSet.Push(current);
			Queue<PathCell> res = new Queue<PathCell>();
            s += "Path contains following:\n";
            foreach (FloorCell cell in closedSet){
			 	s += $"Cell: {cell.gridX}:{cell.gridY};{cell.currentFloor};\n";
                PathCell pathCell = new PathCell 
				{
                    pos = new Vector3()
                    {
                        x = cell.gridX - offsetX + cellSize / 2,
                        y = cell.gridY - offsetY + cellSize / 2
                    },
					floor = cell.bridge && !cell.bridgeData.start ? cell.bridgeData.floor : cell.currentFloor,
					gridY = cell.gridY
				};
				res.Enqueue(pathCell);
			}
			closedSet.Pop();
			Debug.Log(s);
			result.Add(res);
			return;
		}
		closedSet.Push(current);
		// Debug.Log($"Checking {current.gridX},{current.gridY}...");
		foreach(FloorCell n in floor.GetNeighbours4(current.gridX, current.gridY)){
			if(!closedSet.Contains(n)) {
				BFSearch(n,closedSet,result);
			}
		}
		// Debug.Log($"All the neighbours of {current.gridX},{current.gridY} are checked.");
		closedSet.Pop();
		return;
	}
}

public struct PathCell 
{
	public Vector3 pos;
    public int gridY;
    public int floor;
}
