using UnityEngine;
using System.Collections.Generic;
using System;
public class Pathfinding : MonoBehaviour
{
    //[SerializeField] FloorManager floor;
    //[SerializeField] private EnemyManager enemyManager;

    [SerializeField] private Transform target;
    [SerializeField] private Transform entity;
    [SerializeField] private GridVisual visualPrefab;
    [SerializeField] private Vector2Int offset;
    int baseMoveCost = 50;


    [SerializeField] private int maxPaths = 128;
    [SerializeField] float cellSize;
    [SerializeField] int w,h;

    /*public Dictionary<PathListAccessor, List<List<PathCell>>> start_pathList;
    int pathsCount = 0;
    public int totalCells = 0;
    int currentWaveCells;*/
    [SerializeField] private GridVisual prefab;
    CustomGrid<GridVisual> grid;
    public GridVisual[] openHeap;
    public List<GridVisual> result = new();
    public int heapCount;
    public List<List<PathCell>> paths = new List<List<PathCell>>();
    public List<FloorCell> possibleStarts = new List<FloorCell>();
    public void Awake()
    {
        //floor.Init();
        //floor.CreateCastle(transform.position, b);
        //SetTargetPoint(Mathf.Abs(offset.x), Mathf.Abs(offset.y), 0,0);
        paths = new();
        //cellSize = floor.GetComponent<Grid>().cellSize.x;
        grid = new CustomGrid<GridVisual>(w,h,cellSize,offset, InstantiateGridVisual);
    }
    public GridVisual InstantiateGridVisual(int x, int y)
    {
        return Instantiate(prefab, transform).Init(x,y, cellSize,GridToWorld(x,y));
    }
    public Vector2 GridToWorld(int x, int y)
    {
        if (x < 0 || x >= w || y < 0 || y >= h) return default;
        return new Vector2(x, y) * cellSize - offset;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(grid.GetValue(entity.position), grid.GetValue(target.position), out List<GridVisual> result);
        }
        if (Input.GetMouseButtonDown(0))
        {
            var cell = grid.GetValue(Input.mousePosition);
            cell.moveCost += 10;
            cell.ChangeAlpha(-0.1f);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var cell = grid.GetValue(Input.mousePosition);
            cell.moveCost -= 10;
            cell.ChangeAlpha(0.1f);
        }
    }
    public void SetTargetPoint(int gridX, int gridY, int width, int height)
    {
        //gridX += width / 2;
        //Debug.Log(message: $"Castle: {gridX},{gridY}");
        //floor.floorCells[gridX, gridY].road = true;
        //FloorCell pos = floor.floorCells[gridX, gridY];
        //castlePosition = pos;
        //paths.Add(new Path(new Vector2Int(pos.gridX,pos.gridY)));
    }
    public void ClearCastlePoint()
    {
    }
    public GridVisual TraceStep(GridVisual current)
    {
        current.SetColor(new Color (0.5f,0.5f,1f,1f ));
        result.Add(current);
        return current.cameFrom;
    }
    public void UpdatePaths()
    {
        /*paths.Clear();
        for (int i = 0; i < possibleStarts.Count; i++)
        {
            //if (!FindPath(possibleStarts[i], castlePosition, out List<PathCell> res)) continue;
            paths.Add(res);
        }
        enemyManager.CalculateWave();*/
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
    public unsafe bool FindPath(GridVisual start, GridVisual end, out List<GridVisual> result)
    {
        result = new();
        //FloorCell[,] grid = floor.floorCells;
        HashSet<GridVisual> closed = new();
        GridVisual[] openHeap = new GridVisual[16];
        int heapCount = 0;
        openHeap[heapCount++] = start;
        HashSet<GridVisual> openCell = new();
        GridVisual[] neighbours = new GridVisual[4];
        Debug.Log($"Start Calculating! {start.gridX}, {start.gridY}: {end.gridX}, {end.gridY} ");
        while (heapCount > 0)
        {
            GridVisual current = PeekAndBalance(openHeap, heapCount--);
            current.SetColor(Color.black);
            closed.Add(current);
            //Debug.Log("Adding cell from closed and removing from open");
            openCell.Remove(current);
            //Debug.Log($"Checking! {current.gridX}, {current.gridY} ");
            if (current.gridX == end.gridX && current.gridY == end.gridY)
            {
                // TODO: traceback 
                Debug.Log($"find! {current.gridX}, {current.gridY}");
                return true;
            }
            grid.GetNeighbours(current.gridX, current.gridY, neighbours);
            for (int i = 0; i < 4; i++)
            {
                if (neighbours[i] == null) continue;
                //Debug.Log(i + "th neighbour");
                if (!closed.Contains(neighbours[i]))
                {
                    int stepCost = GetStepCost(neighbours[i]);
                    int leftCost = Mathf.Abs(end.gridX - neighbours[i].gridX)
                            + Mathf.Abs(end.gridY - neighbours[i].gridY);
                    if (openCell.Contains(neighbours[i]))
                    {
                        if (current.cost + stepCost + leftCost * baseMoveCost < GetTogetherCost(neighbours[i]))
                        {
                            neighbours[i].UpdateInfo(Color.green, current.cost + stepCost, leftCost, current);
                            SortUpElement(neighbours[i].index, openHeap);
                        }
                    }
                    else
                    {
                        neighbours[i].UpdateInfo(Color.green, current.cost + stepCost, leftCost, current);
                        if (heapCount >= openHeap.Length)
                        {
                            Array.Resize(ref openHeap, heapCount * 2);
                        }
                        AddToHeap(
                            neighbours[i],
                            openHeap, heapCount++);
                        openCell.Add(neighbours[i]);
                    }
                }
            }
        }
        Debug.Log("No possible paths found...");
        return false;
    }
    bool IsWalkable(GridVisual cell)
    {
        /*FloorCell up = grid[cell.gridX, cell.gridY + 1];
        bool isUp = up.currentFloor == cell.currentFloor + 1;*/
        return true;
    }
    int GetStepCost(GridVisual cell)
    {
        int res = 10;
        /*if (cell.road || cell.bridge)
        {
            res -= 5;
        }else if (cell.GetBuildingIDCallback != null)
        {
            res += 10;
        }*/
        return cell.moveCost;
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
    void AddToHeap(GridVisual element, GridVisual[] heap, int count)
    {
        //Debug.Log($"adding {element.gridX},{element.gridY} to a heap...");
        int index = count;
        heap[index] = element;
        heap[index].SetIndex(index);
        string s = $"adding {element.gridX}, {element.gridY}, {GetTogetherCost(element)}";
        Debug.Log(s);
        SortUpElement(index, heap);
        /*int cost = element.left * baseMoveCost + element.cost;
        int parent = (index - 1) / 2;
        GridVisual parentCell = heap[parent];
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
                //Debug.Log($"{element.gridX},{element.gridY} is now a head");
                break;
            }
        }*/
        
    }
    void SortUpElement(int index, GridVisual[] heap)
    {
        int count = index + 1;
        var temp = heap[index];
        int together = GetTogetherCost(temp);
        int parent = (index - 1) / 2;
        heap[0].SetColor(Color.green);

        string s = $"sorting {temp.gridX}, {temp.gridY}, {together}, {heap[index].left}; \n count: {count} \n";
        while (parent >= 0 && GetTogetherCost(heap[parent]) >= together)
        {
            if (GetTogetherCost(heap[parent]) == together && !(temp.left < heap[parent].left))
                break;
            Swap(index, parent, heap);

            index = parent;
            parent = (index - 1) / 2;
        }
        heap[0].SetColor(Color.red);
        for (int i = 0; i < count; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n";
        }
        Debug.Log(s);
    }
    public int GetTogetherCost(GridVisual cell)
    {
        return cell.left * 50 + cell.cost;
    }
    public void SortDown(int target, GridVisual[] heap, int count)
    {
        int left, right, c1, c2, lowestChild = 0, lowestCost;
        int targetCost = heap[target].left * baseMoveCost + heap[target].cost;
        left = target * 2 + 1;
        right = target * 2 + 2;
        c1 = heap[left].left * baseMoveCost + heap[left].cost;
        c2 = heap[right].left * baseMoveCost + heap[right].cost;
        lowestCost = Mathf.Min(c1, c2);
        while (lowestCost <= targetCost && right < count && left < count)
        {
            c1 = heap[left].left * baseMoveCost + heap[left].cost;
            c2 = heap[right].left * baseMoveCost + heap[right].cost;
            if(c1 == c2)
            {
                lowestChild = c1 < c2 ? left : right;
            }
            else
            {
                lowestChild = heap[left].left < heap[right].left? left : right;
            }
            lowestCost = heap[lowestChild].left * baseMoveCost + heap[left].cost;

            if (lowestCost == targetCost && !(heap[lowestChild].left < heap[target].left))
            {
                break;
            }
            Swap(target,lowestChild, heap);
            target = lowestChild;
            left = target * 2 + 1;
            right = target * 2 + 2;
        }
        Debug.Log($"new Index: {lowestChild}; lowestCost: {lowestCost}; {left}, {right}");
    }
    public void Swap(int first, int second, GridVisual[] heap)
    {
        var temp = heap[first];
        heap[first] = heap[second];
        heap[second] = temp;
        heap[first].SetIndex(first);
        heap[second].SetIndex(second);
    }
    GridVisual PeekAndBalance(GridVisual[] heap, int count)
    {
        GridVisual result = heap[0];
        if (count <= 1) { Debug.Log("Count <= 1, don't balance!"); return result; }
        int target = 0;
        heap[target] = heap[count - 1];
        heap[target].index = target;
        SortDown(target,heap,count - 1);
        string s = $"peeked {result.gridX}, {result.gridY}; {result.moveCost}, {result.left}; \n count: {count - 1} \n";
        for (int i = 0; i < count - 1; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n ";
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
[Serializable]
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


