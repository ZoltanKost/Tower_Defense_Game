using UnityEngine;
using System.Collections.Generic;
public class Pathfinding : MonoBehaviour{
	[SerializeField] FloorManager floor;
	[SerializeField] private int maxPaths = 200;
	List<FloorCell> castlePositions;
	// List<List<FloorCell>> paths = new();
	public List<Queue<Vector3>> vectors{get;private set;}
	int offsetX, offsetY;
	float cellSize;
	public void SetCastlePoint(int gridX, int gridY, int width, int height){
		if(castlePositions == null)castlePositions = new List<FloorCell>();
		gridX += width/2;
		floor.floorCells[gridX, gridY].road = true;
        FloorCell pos = floor.floorCells[gridX,gridY];
		castlePositions.Add(pos);
		// Debug.Log(message: $"Castle: {gridX},{gridY}");
	}
	public void ClearCastlePoint(){
		castlePositions.Clear();
	}
	public void Awake(){
		vectors = new List<Queue<Vector3>>();
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
	public void BFSearch(FloorCell current, Stack<FloorCell> closedSet, List<Queue<Vector3>> result){
		if(result.Count > maxPaths) return;
		if(!(current.road || current.bridge || current.bridgeSpot)) return;
		if(closedSet.Count != 0)
		{
			FloorCell prev = closedSet.Peek();
			if((prev.ladder || current.ladder) && prev.gridY == current.gridY) return;
			if(prev.bridge && !(current.bridge || current.bridgeSpot)) return;
			if(current.bridge && !(prev.bridge || prev.bridgeSpot)) return;
			if(current.currentFloor == prev.currentFloor - 1){
				if(!(current.gridY == prev.gridY - 1))return; 
			}else if(current.currentFloor == prev.currentFloor + 1){
				if(!(current.gridY == prev.gridY + 1))return;
			}
		}
		if(floor.IsStarting(current.gridX, current.gridY)){
			if(closedSet.Count == 0) return;
			// string s =
			// $"Path nr.{result.Count} just finded! Start: {current.gridX},{current.gridY}, Cells:{closedSet.Count}.\n";
			closedSet.Push(current);
			Queue<Vector3> res = new Queue<Vector3>();
			foreach(FloorCell cell in closedSet){
				Vector3 pos = new(){
					x = cell.gridX - offsetX + cellSize/2,
					y = cell.gridY - offsetY + cellSize/2
				};
				res.Enqueue(pos);
			}
			closedSet.Pop();
			// s += "Path contains following:\n";
			// foreach(Vector3 cell in res){
			// 	// s += $"Cell: {cell}\n";
			// }
			// Debug.Log(s);
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
	public Queue<Vector3> GetRandomPath(){
		int r = Random.Range(0, vectors.Count);
		return new Queue<Vector3>(vectors[r]);
	}

	public Vector3[] DijkstraSearch(Vector3 start, Vector3 end)
	{
		FloorCell[,] floorCells = floor.floorCells;
		/*Vector3Int gridStart = floor.WorldToCell(start);
        Vector3Int gridEnd = floor.WorldToCell(end);*/
		return new Vector3[] { start, end };
		/*List<FloorCell> openList = new()
        {
            floorCells[gridStart.x, gridStart.y]
        };
		HashSet<FloorCell> closedSet = new();
		while ()
		{

		}*/

        /*Vector3 pos = new()
        {
            x = cell.gridX - offsetX + cellSize / 2,
            y = cell.gridY - offsetY + cellSize / 2
        };*/
    }
}
