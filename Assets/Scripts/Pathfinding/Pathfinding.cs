﻿using UnityEngine;
using System.Collections.Generic;
public class Pathfinding : MonoBehaviour{
	[SerializeField] FloorManager floor;
	List<FloorCell> castlePositions;
	// List<List<FloorCell>> paths = new();
	List<Queue<Vector3>> vectors = new();
	int offsetX, offsetY;
	float cellSize;
	public void SetCastlePoint(int gridX, int gridY, int width, int height){
		if(castlePositions == null)castlePositions = new List<FloorCell>();
		gridX += width/2;
		FloorCell pos = floor.floorCells[gridX,gridY-1];
		castlePositions.Add(pos);
		Debug.Log(message: $"Castle: {gridX},{gridY}");
	}
	// public bool FindPathToLadder(){

	// }
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
		Debug.Log(vectors.Count);
		return vectors.Count > 0;
	}
	public void BFSearch(FloorCell current, Stack<FloorCell> closedSet, List<Queue<Vector3>> result){
		if(!(current.road || current.bridge || current.bridgeSpot)) return;
		if(floor.IsStarting(current.gridX, current.gridY)){
			if(closedSet.Count == 0) return;
			string s =
			$"Path nr.{result.Count} just finded! Start: {current.gridX},{current.gridY}, Cells:{closedSet.Count}.\n";
			closedSet.Push(current);
			int offsetX = floor.offset.x;
			int offsetY = floor.offset.y;
			Queue<Vector3> res = new Queue<Vector3>();
			foreach(FloorCell cell in closedSet){
				Vector3 pos = new(){
					x = cell.gridX - offsetX + cellSize/2,
					y = cell.gridY - offsetY + cellSize/2
				};
				res.Enqueue(pos);
			}
			closedSet.Pop();
			s += "Path contains following:\n";
			foreach(Vector3 cell in res){
				s += $"Cell: {cell}\n";
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
	public Queue<Vector3> GetRandomPath(){
		int r = Random.Range(0, vectors.Count);
		return new Queue<Vector3>(vectors[r]);
	}
}
