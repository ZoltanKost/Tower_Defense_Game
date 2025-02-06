using UnityEngine;
using System.Collections.Generic;
using System;
public class Pathfinding : MonoBehaviour{
	[SerializeField] FloorManager floor;
	[SerializeField] private int maxPaths = 1000;
	public List<FloorCell> castlePositions;
	public List<Queue<PathCell>> vectors{get;private set;}
	int offsetX, offsetY;
	float cellSize;

	// Heap stuff
	public List<Path> paths = new List<Path>();
	public List<int> finishedPaths = new List<int>();
	public void SetCastlePoint(int gridX, int gridY, int width, int height){
		if(castlePositions == null)castlePositions = new List<FloorCell>();
		gridX += width/2;
		Debug.Log(message: $"Castle: {gridX},{gridY}");
		floor.floorCells[gridX, gridY].road = true;
        FloorCell pos = floor.floorCells[gridX,gridY];
		castlePositions.Add(pos);
		paths.Add(new Path(new Vector2Int(pos.gridX,pos.gridX)));
    }
	public void ClearCastlePoint(){
		castlePositions.Clear();
	}
	public void Awake(){
		/*roads = new Vector2Int[32];
		for (int i = 0; i < 32; i++)
		{
			roads[i] = -Vector2Int.one;
		}*/
		vectors = new List<Queue<PathCell>>();
        cellSize = floor.GetComponent<Grid>().cellSize.x;
    }
	public void PlaceRoad(int x, int y, bool spawnPoint)
	{
		Vector2Int newRoad = new(x, y);
		int Count = paths.Count;
		bool addedToPath = false;
		for(int i = 0; i < Count; i++)
		{
			int res = paths[i].TryAddCell(newRoad);
			Debug.Log($"path: {i} result: {res} pos: {x},{y} ");
			if (res == 0) 
			{
				addedToPath = true;
                continue; 
			}
			if (res > 0)
			{
				addedToPath = true;
                paths.Add(new Path(paths[i], newRoad, res));
				if (spawnPoint) finishedPaths.Add(i);
			}
		}
		if (!addedToPath)
		{
	        paths.Add(new Path(newRoad));
        }
        if (spawnPoint) finishedPaths.Add(paths.Count);
    }

	public bool FindPathToCastle(){
		offsetX = floor.offset.x;
		offsetY = floor.offset.y;
		// paths.Clear();
		/*vectors.Clear();
		Stack<FloorCell> closedSet = new();
		foreach(FloorCell graph in castlePositions){
			DFSearch(graph,closedSet,vectors);
		}*/

		Debug.Log($"Count: {paths.Count}, finished: {finishedPaths.Count}");
		// Debug.Log(vectors.Count);
		return false;
		//return vectors.Count > 0;
	}
	public void DFSearch(FloorCell current, Stack<FloorCell> closedSet, List<Queue<PathCell>> result){
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
				DFSearch(n,closedSet,result);
			}
		}
		// Debug.Log($"All the neighbours of {current.gridX},{current.gridY} are checked.");
		closedSet.Pop();
		return;
	}
}
public struct Path
{
	public Vector2Int start;
    public Vector2Int end;
    public Vector2Int[] cells;
    public int Count;
	public Path(Vector2Int _start)
	{
		start = _start;
		end = default;
		cells = new Vector2Int[16];
		cells[0] = start;
        Count = 0;
	}
    public Path(Path path, Vector2Int add, int endIndex)
    {
        start = path.start;
        end = path.cells[endIndex];
        cells = new Vector2Int[endIndex * 2];
        Array.Copy(path.cells,cells,endIndex);
        cells[endIndex + 1] = add;
		Count = endIndex + 2;
    }
    public void SortUp(int index)
	{
        int parent = (index - 1) / 2;
        while ((cells[parent] - start).magnitude >= (cells[index] - start).magnitude)
        {
            var temp = cells[parent];
            cells[parent] = cells[index];
            cells[index] = temp;
            index = parent;
            parent = (parent - 1) / 2;
        }
    }
	public bool SearchDesiredPlace(Vector2Int target, out int index)
	{
        index = 0;
        int magnitude = Mathf.Abs(target.x - start.x) + Mathf.Abs(target.y - start.y);
		while(magnitude != 1)
		{
            index++;
            if (index >= Count) return false;
            magnitude = Mathf.Abs(target.x - cells[index].x) + Mathf.Abs(target.y - cells[index].y);
        }
		return true;
	}
	public int TryAddCell(Vector2Int cell)
	{
        cells[Count++] = cell;
		if ((int)(end - cell).magnitude == 1)
		{
			end = cell;
			return 0;
		}
		if(SearchDesiredPlace(cell, out int index))
		{
			return index;
		}
		else
		{
			return -1;
		}
    }
}

public struct PathCell 
{
	public Vector3 pos;
    public int gridY;
    public int floor;
}
