using UnityEngine;
using System.Collections.Generic;
public class Pathfinding : MonoBehaviour{
	[SerializeField] FloorManager floor;
	List<FloorCell> castlePositions;
	List<List<FloorCell>> paths = new();
	public void SetCastlePoint(int gridX, int gridY, int width, int height){
		if(castlePositions == null)castlePositions = new List<FloorCell>();
		gridX += width/2;
		FloorCell pos = floor.floorCells[gridX,gridY-1];
		castlePositions.Add(pos);
		Debug.Log($"Castle: {gridX},{gridY}");
	}
	// public bool FindPathToLadder(){

	// }
	public bool FindPathToCastle(){
		paths.Clear();
		Stack<FloorCell> closedSet = new();
		foreach(FloorCell graph in castlePositions){
			int count = paths.Count;
			BFSearch(graph,closedSet,paths);
			if(count >= paths.Count) return false;
		}
		Debug.Log(paths.Count);
		return paths.Count > 0;
	}
	public void BFSearch(FloorCell current, Stack<FloorCell> closedSet, List<List<FloorCell>> result){
		if(!current.road) return;
		if(floor.IsStarting(current.gridX, current.gridY)){
			if(closedSet.Count == 0) return;
			string s =
			$"Path nr.{result.Count} just finded! Start: {current.gridX},{current.gridY}, Cells:{closedSet.Count}.\n";
			closedSet.Push(current);
			List<FloorCell> res = new List<FloorCell>(closedSet);
			closedSet.Pop();
			s += "Path contains following:\n";
			foreach(FloorCell cell in res){
				s += $"Cell: {cell.gridX},{cell.gridY}\n";
			}
			Debug.Log(s);
			result.Add(res);
			return;
		}
		closedSet.Push(current);
		// Debug.Log($"Checking {current.gridX},{current.gridY}...");
		// Debug.Log($"Cell has {neighs.Count} neighbours!");
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
