using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
public class Pathfinding : MonoBehaviour
{
    [SerializeField] FloorManager floor;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] int defaultMoveCost;
    [SerializeField] int roadMoveBonus;
    [SerializeField] int buildingMovePenalty;
    //[SerializeField] private GridVisual debug;

    float cellSize;
    int w,h;
    private Vector3Int offset;

    //[SerializeField] private FloorCell prefab;
    FloorCell[,] grid;
    public List<List<PathCell>> paths = new List<List<PathCell>>();
    public List<Vector2Int> possibleStarts = new List<Vector2Int>();
    FloorCell castlePosition;

    //CustomGrid<GridVisual> debugGrid;
    public void Start()
    {
        //floor.Init();
        //floor.CreateCastle(transform.position, b);
        //SetTargetPoint(Mathf.Abs(offset.x), Mathf.Abs(offset.y), 0,0);
        paths = new();
        //cellSize = floor.GetComponent<Grid>().cellSize.x;
        grid = floor.floorCells;
        cellSize = floor.cellSize;
        offset = floor.offset;
        w = grid.GetLength(0);
        h = grid.GetLength(1);
        //debugGrid = new CustomGrid<GridVisual>(w, h, cellSize, new Vector2(offset.x, offset.y), CreateEmptyObject);
    }
    public void SetTargetPoint(int gridX, int gridY, int width, int height)
    {
        gridX += width / 2;
        //Debug.Log(message: $"Castle: {gridX},{gridY}");
        floor.floorCells[gridX, gridY].road = true;
        FloorCell pos = floor.floorCells[gridX, gridY];
        castlePosition = pos;
    }
    public void ClearCastlePoint()
    {
        
    }
    public void UpdatePaths()
    {
        StopAllCoroutines();
        paths.Clear();
        //Debug.Log(possibleStarts.Count);
        for (int i = 0; i < possibleStarts.Count; i++)
        {
            //result = new();
            var vector = possibleStarts[i];
            var res = new List<PathCell>();
            if (FindPath(grid[vector.x, vector.y], castlePosition, res)) paths.Add(res);
        }
        enemyManager.CalculateWave();
    }
    void OnDrawGizmos()
    {
        if (paths == null || paths.Count < 1) { return; }
        foreach (var path in paths)
        {
            var offset = new Vector3(0, 0, 5);
            if (path.Count < 1) continue;
            Vector3 prevCell = path[0].pos;
            foreach (var nextCell in path)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(prevCell + offset, nextCell.pos + offset);
                prevCell = nextCell.pos;
            }
        }
    }
    /*GridVisual CreateEmptyObject(int gridX,int gridY)
    {
        return Instantiate(////////Debug., transform).Init(gridX, gridY, 0f, 
            floor.CellToWorld(new Vector3Int(gridX, gridY) - offset));
    }*/
    public bool FindPath(FloorCell start, FloorCell end, List<PathCell> res)
    {
        //bool f = false;
        //FloorCell[,] grid = floor.floorCells;
        //Stopwatch sw = Stopwatch.StartNew();
        HashSet<Vector2Int> closed = new();
        FloorCell[] openHeap = new FloorCell[16];
        int heapCount = 0;
        openHeap[heapCount++] = start;
        HashSet<Vector2Int> openCell = new();
        FloorCell[] neighbours = new FloorCell[4];
        //////Debug.Log($"Start Calculating! {start.gridX}, {start.gridY}: {end.gridX}, {end.gridY} ");
        while (heapCount > 0)
        {
            /*f = Input.GetKeyDown(KeyCode.F);
            while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            f = false;*/
            FloorCell current = openHeap[0];
            //////DebugGrid.GetValue(current.gridX, current.gridY).SetColor(Color.black);
            //////Debug.Log($"Checking! {current.gridX}, {current.gridY} ");
            if (current.gridX == end.gridX && current.gridY == end.gridY)
            {
                ////Debug.Log($"find! {current.gridX}, {current.gridY}");
                // TODO: traceback 
                ////Debug.Log($"{current.gridX},{current.gridY} : {start.gridX},{start.gridY} ");
                //res = new();
                while (current.gridX != start.gridX || current.gridY != start.gridY)
                {
                    /*while (!f)
                    {
                        yield return null;
                        f = Input.GetKeyDown(KeyCode.F);
                    }
                    f = false;*/
                    //////DebugGrid.GetValue(current.gridX, current.gridY)
                        //.UpdateInfo(Color.blue,current);
                    res.Add(new PathCell(current,cellSize,offset.x, offset.y));
                    current = grid[current.comeFrom.x, current.comeFrom.y];
                }
                //sw.Stop();
                //UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
                res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                return true;
            }
            RemoveFirst(openHeap, heapCount--);
            Vector2Int currentPos = new Vector2Int(current.gridX, current.gridY);
            closed.Add(currentPos);
            ////Debug.Log("Adding cell from closed and removing from open");
            openCell.Remove(currentPos);
            GetNeighbours4(current, grid, w, h, neighbours);
            for (int i = 0; i < 4; i++)
            {
                /*while (!f)
                {
                    yield return null;
                    f = Input.GetKeyDown(KeyCode.F);
                }
                f = false;*/
                // TODO bug
                if (neighbours[i].gridX == -1 || neighbours[i].gridY == -1) 
                { 
                    //Debug.Log("empty neighbour");
                    continue; 
                }
                ////Debug.Log(i + "th neighbour");
                var neighPos = new Vector2Int(neighbours[i].gridX, neighbours[i].gridY);
                if (!closed.Contains(neighPos) && IsWalkable(neighbours[i], current))
                {
                    int stepCost = GetStepCost(neighbours[i]);
                    int left = Mathf.Abs(end.gridX - neighbours[i].gridX)
                            + Mathf.Abs(end.gridY - neighbours[i].gridY);
                    if (openCell.Contains(neighPos))
                    {
                        if (current.cost + stepCost + left * defaultMoveCost < GetTogetherCost(neighbours[i]))
                        {
                            var cell = neighbours[i];
                            cell.cost = stepCost + current.cost;
                            cell.left = left;
                            cell.comeFrom = new Vector2Int(current.gridX,current.gridY);
                            openHeap[cell.heapIndex] = cell;
                            SortUpElement(neighbours[i].heapIndex, openHeap);
                            cell = grid[cell.gridX, cell.gridY];
                            /*debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green,cell);*/
                        }
                    }
                    else
                    {
                        var cell = neighbours[i];
                        grid[cell.gridX, cell.gridY].heapIndex = heapCount;
                        grid[cell.gridX, cell.gridY].cost = stepCost + current.cost;
                        grid[cell.gridX, cell.gridY].left = left;
                        grid[cell.gridX, cell.gridY].comeFrom = new Vector2Int(current.gridX, current.gridY);
                        if (heapCount >= openHeap.Length)
                        {
                            Array.Resize(ref openHeap, heapCount * 2);
                        }
                        AddToHeap(
                            grid[cell.gridX, cell.gridY],
                            openHeap, heapCount++);
                        openCell.Add(neighPos);
                        cell = grid[cell.gridX, cell.gridY];
                        /*debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);*/
                    }
                }
            }
            /*while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            f = false;*/
        }
        //Debug.Log("No possible paths found...");
        res = null;
        return false;
    }
    bool IsWalkable(FloorCell cell, FloorCell cameFrom)
    {
        FloorCell up = grid[cell.gridX, cell.gridY + 1];
        bool isArock = up.currentFloor == cell.currentFloor + 1;
        return cell.currentFloor > 0 && (!isArock || isArock && cell.road) || (cell.bridge && (cameFrom.bridge || cell.bridgeData.start));
    }
    int GetStepCost(FloorCell cell)
    {
        int res = defaultMoveCost;
        if (cell.road || cell.bridge)
        {
            res -= roadMoveBonus;
        }
        else if (cell.GetBuildingIDCallback != null)
        {
            res += buildingMovePenalty;
        }
        return res;
    }
    void GetNeighbours4(FloorCell cell, FloorCell[,] grid, int w, int h, FloorCell[] result)
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
    void AddToHeap(FloorCell element, FloorCell[] heap, int count)
    {
        ////Debug.Log($"adding {element.gridX},{element.gridY} to a heap...");
        int index = count;
        heap[index] = element;
        
        heap[index].heapIndex = index;
        //Debug.Log($"adding {element.gridX}, {element.gridY}, {GetTogetherCost(element)}");
        SortUpElement(index, heap);
        
        /*int cost = element.left * baseMoveCost + element.cost;
        int parent = (index - 1) / 2;
        FloorCell parentCell = heap[parent];
        while (parentCell.left * baseMoveCost + parentCell.cost > cost) 
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
                ////Debug.Log($"{element.gridX},{element.gridY} is now a head");
                break;
            }
        }*/

    }
    void SortUpElement(int index, FloorCell[] heap)
    {
        if (index < 0) UnityEngine.Debug.LogError("trying to sort an element not included in heap");
        int count = index + 1;
        var temp = heap[index];
        int together = GetTogetherCost(temp);
        int parent = (index - 1) / 2;

        string s = $"sorting {temp.gridX}, {temp.gridY}, {together}, {heap[index].left}; \n count: {count} \n";
        while (parent >= 0 && GetTogetherCost(heap[parent]) >= together)
        {
            if (GetTogetherCost(heap[parent]) == together && !(temp.left < heap[parent].left))
                break;
            Swap(index, parent, heap);

            index = parent;
            parent = (index - 1) / 2;
        }
        for (int i = 0; i < count; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n";
        }
        //Debug.Log(s);
    }
    public int GetTogetherCost(FloorCell cell)
    {
        return cell.left * defaultMoveCost + cell.cost;
    }
    public void SortDown(int target, FloorCell[] heap, int count)
    {
        int left, right, c1, c2, lowestChild = 0, lowestCost;
        int targetCost = heap[target].left * defaultMoveCost + heap[target].cost;
        left = target * 2 + 1;
        right = target * 2 + 2;
        c1 = heap[left].left * defaultMoveCost + heap[left].cost;
        c2 = heap[right].left * defaultMoveCost + heap[right].cost;
        lowestCost = Mathf.Min(c1, c2);
        while (lowestCost <= targetCost && right < count && left < count)
        {
            c1 = heap[left].left * defaultMoveCost + heap[left].cost;
            c2 = heap[right].left * defaultMoveCost + heap[right].cost;
            if(c1 == c2)
            {
                lowestChild = c1 < c2 ? left : right;
            }
            else
            {
                lowestChild = heap[left].left < heap[right].left? left : right;
            }
            lowestCost = heap[lowestChild].left * defaultMoveCost + heap[left].cost;

            if (lowestCost == targetCost && !(heap[lowestChild].left < heap[target].left))
            {
                break;
            }
            Swap(target,lowestChild, heap);
            target = lowestChild;
            left = target * 2 + 1;
            right = target * 2 + 2;
        }
        //Debug.Log($"new Index: {target};{left}, {right}");
    }
    public void Swap(int first, int second, FloorCell[] heap)
    {
        //Debug.Log($"Swapping {first} and {second}");
        var temp = heap[first];
        heap[first] = heap[second];
        heap[second] = temp;
        heap[first].heapIndex = first;
        heap[second].heapIndex = second;
        var cell1 = heap[first];
        var cell2 = heap[second];
        grid[cell1.gridX, cell1.gridY] = cell1;
        grid[cell2.gridX, cell2.gridY] = cell2;
        /*debugGrid.GetValue(cell1.gridX, cell1.gridY)
            .UpdateInfo(Color.black, cell1);
        debugGrid.GetValue(cell2.gridX, cell2.gridY)
            .UpdateInfo(Color.black, cell2);*/
    }
    FloorCell RemoveFirst(FloorCell[] heap, int count)
    {
        FloorCell result = heap[0];
        if (count <= 1) { 
            //Debug.Log("Count <= 1, don't balance!");
            return result; 
        }
        int target = 0;
        Swap(target, count - 1, heap);
        SortDown(target,heap,count - 1);
        string s = $"peeked {result.gridX}, {result.gridY}; {result.left}; \n count: {count - 1} \n";
        for (int i = 0; i < count - 1; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n ";
        }
        //Debug.Log(s);
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
    public FloorCell cameFrom;
    public Path_Cell(FloorCell cell, int _passedCost, int leftCost, FloorCell _cameFrom)
    {
        gridY = cell.gridY;
        gridX = cell.gridX;
        floor = cell.currentFloor;
        passedWayCost = _passedCost;
        leftCellsCost = leftCost;
        cameFrom = _cameFrom;
    }
}
[Serializable]
public struct PathCell
{
    public Vector3 pos;
    public int floor;
    public int gridY;
    public PathCell(FloorCell cell, float cellSize, int offsetX, int offsetY)
    {
        pos = new Vector3(cell.gridX * cellSize - offsetX, cell.gridY * cellSize - offsetY) + new Vector3(1,1) * 0.5f * cellSize;
        floor = cell.currentFloor;
        gridY = cell.gridY;
    }
}


