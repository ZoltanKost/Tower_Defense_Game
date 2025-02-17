using UnityEngine;
using System.Collections.Generic;
using System;
public class Pathfinding : MonoBehaviour
{
    [SerializeField] FloorManager floor;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private int maxPaths = 128;
    FloorCell castlePosition;
    int offsetX, offsetY;
    float cellSize;

    /*public Dictionary<PathListAccessor, List<List<PathCell>>> start_pathList;
    int pathsCount = 0;
    public int totalCells = 0;
    int currentWaveCells;*/

    public List<List<PathCell>> paths = new List<List<PathCell>>();
    public List<FloorCell> possibleStarts = new List<FloorCell>();
    public void SetTargetPoint(int gridX, int gridY, int width, int height)
    {
        gridX += width / 2;
        Debug.Log(message: $"Castle: {gridX},{gridY}");
        floor.floorCells[gridX, gridY].road = true;
        FloorCell pos = floor.floorCells[gridX, gridY];
        castlePosition = pos;
        offsetX = floor.offset.x;
        offsetY = floor.offset.y;
        //paths.Add(new Path(new Vector2Int(pos.gridX,pos.gridY)));
    }
    public void ClearCastlePoint()
    {
        castlePosition = default;
    }
    public void Awake()
    {
        /*roads = new Vector2Int[32];
		for (int i = 0; i < 32; i++)
		{
			roads[i] = -Vector2Int.one;
		}*/
        paths = new();
        cellSize = floor.GetComponent<Grid>().cellSize.x;
    }

    public void UpdatePaths()
    {
        paths.Clear();
        for (int i = 0; i < possibleStarts.Count; i++)
        {
            if (!FindPath(possibleStarts[i], castlePosition, out List<PathCell> res)) continue;
            paths.Add(res);
        }
        enemyManager.CalculateWave();
    }
    /*void OnDrawGizmos()
    {
        if (paths == null || paths.Count < 1) { Debug.Log("Nothing to Draw"); return; }
        foreach (var path in paths)
        {
            var offset = new Vector3(0, 0, 5);
            if (path.Count < 1) continue;
            Vector3 prevCell = path[0].pos;
            foreach (var nextCell in path)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(prevCell + offset,nextCell.pos + offset);
                prevCell = nextCell.pos;
            }
        }
    }*/
    public unsafe bool FindPath(FloorCell start, FloorCell end, out List<PathCell> result)
    {
        result = new();
        FloorCell[,] grid = floor.floorCells;
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);
        HashSet<FloorCell> closed = new();
        Path_Cell[] openHeap = new Path_Cell[16];
        int heapCount = 0;
        HashSet<FloorCell> openCell = new();
        Path_Cell startPath = new Path_Cell(start, GetStepCost(start), 
                            Mathf.Abs(end.gridX - start.gridX)
                            + Mathf.Abs(end.gridY - start.gridY), null);
        openHeap[heapCount++] = startPath;
        FloorCell[] neighbours = new FloorCell[4];
        Debug.Log($"Start Calculating! {start.gridX}, {start.gridY}: {end.gridX}, {end.gridY} ");
        int steps = 0;
        Path_Cell current;
        while (heapCount > 0 && steps < 100)
        {
            steps++;
            current = PeekAndBalance(openHeap, heapCount--);
            Debug.Log($"Checking! {current.gridX}, {current.gridY} ");
            if (current.gridX == end.gridX && current.gridY == end.gridY) 
            {
                Debug.Log($"find! {current.gridX}, {current.gridY}");

                int i = 0;
                //construct the path
                /*while (current.cameFrom != null && current.gridX != start.gridX && current.gridY != start.gridY)
                {
                    i++;
                    current = current.cameFrom;
                    Debug.Log(i);

                }*/

                result.Add(new PathCell(current, cellSize, offsetX, offsetY));
                do
                {
                    current = current.cameFrom;
                    result.Add(new PathCell(current, cellSize, offsetX, offsetY));
                } while (current.gridX != start.gridX && current.gridY != start.gridY);
                return true;
            }
            GetNeighbours4(current,grid,w,h,neighbours);

            for (int i = 0; i < 4; i++)
            {
                if (!closed.Contains(neighbours[i]) && !openCell.Contains(neighbours[i])) 
                {
                    //Debug.Log($"{neighbours[i].gridX},{neighbours[i].gridY} is neither in open nor in closed set");
                    if (IsWalkable(neighbours[i], grid))
                    {
                        int stepCost = GetStepCost(neighbours[i]);
                        int leftCost = Mathf.Abs(end.gridX - neighbours[i].gridX)
                            + Mathf.Abs(end.gridY - neighbours[i].gridY);
                        Debug.Log( current);
                        if(heapCount >= openHeap.Length)
                        {
                            Array.Resize(ref openHeap,heapCount * 2);
                        }
                        AddToHeap(
                            new Path_Cell(neighbours[i], current.passedWayCost + stepCost, leftCost,current),
                            openHeap, heapCount++);
                        openCell.Add(neighbours[i]);
                        //Debug.Log($"cell was scheduled in open set");
                    }
                    else
                    {
                        //Debug.Log($"cell was denied as closed");
                        closed.Add(neighbours[i]);
                    }
                }
            }
            closed.Add(grid[current.gridX, current.gridY]);
            openCell.Remove(grid[current.gridX, current.gridY]);
            //Debug.Log($"{current.gridX},{current.gridY} has already been proceed, removing from sets");
        }
        Debug.Log("No possible paths found...");
        return false;
    }
    bool IsWalkable(FloorCell cell, FloorCell[,] grid)
    {
        FloorCell up = grid[cell.gridX, cell.gridY + 1];
        bool isUp = up.currentFloor == cell.currentFloor + 1;
        return cell.currentFloor >= 0 && (!isUp || (isUp && cell.ladder));
    }
    int GetStepCost(FloorCell cell)
    {
        int res = 10;
        if (cell.road || cell.bridge)
        {
            res -= 5;
        }else if (cell.GetBuildingIDCallback != null)
        {
            res += 10;
        }
        return res;
    }
    void GetNeighbours4(Path_Cell cell, FloorCell[,] grid, int w, int h, FloorCell[] result)
    {
        int gridX = cell.gridX;
        int gridY = cell.gridY;
        if (gridX - 1 >= 0) result[0] = grid[gridX - 1, gridY];
        if (gridX + 1 < w) result[1] = grid[gridX + 1, gridY];
        else result[1] = new FloorCell(-1,-1);
        if (gridY + 1 < h) result[2] = grid[gridX, gridY + 1];
        else result[2] = new FloorCell(-1,-1);
        if (gridY - 1 >= 0) result[3] = grid[gridX, gridY - 1];
    }
    void AddToHeap(Path_Cell  element, Path_Cell[] heap, int count)
    {
        //Debug.Log($"adding {element.gridX},{element.gridY} to a heap...");
        int index = count;
        heap[index] = element;
        int cost = element.leftCellsCost + element.passedWayCost;
        int parent = (index - 1) / 2;
        Path_Cell parentCell = heap[parent];
        while (parentCell.leftCellsCost + parentCell.passedWayCost > cost) 
        {
            heap[parent] = heap[index];
            heap[index] = parentCell;
            index = parent;
            parent = (index - 1) / 2;
            if(parent >= 0)
            {
                parentCell = heap[parent];
            }
            else
            {
                //Debug.Log($"{element.gridX},{element.gridY} is now a head");
                break;
            }
        }
        /*string s = $"added {element.gridX}, {element.gridY}; \n {count + 1} ";
        for (int i = 0; i < count + 1; i++)
        {
            s += $"{heap[i].gridX}, {heap[i].gridY};  ";
        }
        Debug.Log(s);*/
    }
    Path_Cell PeekAndBalance(Path_Cell[] heap, int count)
    {
        Path_Cell result = heap[0];
        if (count <= 1) return result;
        int left, right, c1, c2, lowestChild, lowestCost;
        int target = 0;
        heap[target] = heap[count - 1];
        int targetCost = heap[target].leftCellsCost + heap[target].passedWayCost;
        var temp = heap[target];
        left = target * 2 + 1;
        right = target * 2 + 2;
        do
        {
            if (left >= count || right >= count) break;
            c1 = heap[left].leftCellsCost + heap[left].passedWayCost;
            c2 = heap[right].leftCellsCost + heap[right].passedWayCost;
            lowestCost = Mathf.Min(c1, c2);
            lowestChild = c1 < c2 ? left : right;
            
            heap[target] = heap[lowestChild];
            heap[lowestChild] = temp;
            target = lowestChild;
            left = target * 2 + 1;
            right = target * 2 + 2;

        } while (lowestCost < targetCost);

        string s = $"peeked {result.gridX}, {result.gridY};   {count - 1} ";
        for (int i = 0; i < count - 1; i++)
        {
            s += $"{heap[i].gridX}, {heap[i].gridY};  ";
        }
        Debug.Log(s);
        return result;
    }
}
public class Path_Cell 
{
    public int gridY;
    public int gridX;
    public int floor;
    public int passedWayCost;
    public int leftCellsCost;
    public Path_Cell cameFrom;
    public Path_Cell(FloorCell cell, int _passedCost, int leftCost, Path_Cell _cameFrom)
    {
        gridY = cell.gridY;
        gridX = cell.gridX;
        floor = cell.currentFloor;
        passedWayCost = _passedCost;
        leftCellsCost = leftCost;
        cameFrom = _cameFrom;
    }
}
public struct PathCell
{
    public Vector3 pos;
    public int floor;
    public int gridY;
    public PathCell(Path_Cell cell, float cellSize, int offsetX, int offsetY)
    {
        pos = new Vector3(cell.gridX * cellSize - offsetX, cell.gridY * cellSize - offsetY);
        floor = cell.floor;
        gridY = cell.gridY;
    }
}

