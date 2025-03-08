using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
public class Pathfinding : MonoBehaviour
{
    [SerializeField] FloorManager floor;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] int defaultMoveCost;
    [SerializeField] int roadMoveBonus;
    [SerializeField] int buildingMovePenalty;
    
    [SerializeField] private GridVisual debug;

    float cellSize;
    int w,h;
    private Vector3Int offset;
    int targetWidth, targetHeight;

    //[SerializeField] private FloorCell prefab;
    FloorCell[,] grid;
    public List<List<PathCell>> paths = new List<List<PathCell>>();
    public List<PossibleStart> possibleStarts = new List<PossibleStart>();
    public FloorCell castlePosition;

    CustomGrid<GridVisual> debugGrid;
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
        debugGrid = new CustomGrid<GridVisual>(w, h, cellSize, new Vector2(offset.x, offset.y), CreateEmptyObject);
    }
    public void SetTargetPoint(int gridX, int gridY, int width, int height)
    {
        gridX += width / 2;
        //Debug.Log(message: $"Castle: {gridX},{gridY}");
        floor.floorCells[gridX, gridY].road = true;
        floor.floorCells[gridX, gridY].walkable = true;
        FloorCell pos = floor.floorCells[gridX, gridY];
        castlePosition = pos;
        targetHeight = height;
        targetWidth = width;
    }
    public void ClearCastlePoint()
    {
        castlePosition = default;        
    }
    public void CreatePossibleStarts(int count)
    {
        possibleStarts.Clear();
        while(count-- > 0)
        {
            possibleStarts.Add(GenereratePossibleStart());
        }
        UpdatePaths();
    }
    public void CheckIfNeedMovePossibleStarts()
    {
        // TODO finish
        //Debug.Log("Checking if need move possible start");
        int l = possibleStarts.Count;
        for (int i = 0; i < l; i++)
        {
            var vec = possibleStarts[i];
            if (vec.vertical)
            {

                if (vec.pos.y < floor.edgeStartY || vec.pos.y >= floor.edgeEndY) 
                {
                    possibleStarts[i] = MovePossibleStart(possibleStarts[i]);
                    continue;
                }
                if (floor.edgeStartX != vec.pos.x && floor.edgeEndX != vec.pos.x)
                {
                    possibleStarts[i] = MovePossibleStart(possibleStarts[i]);
                    continue;
                }
            }
            else
            {
                if (vec.pos.x < floor.edgeStartX || vec.pos.x >= floor.edgeEndX)
                {
                    possibleStarts[i] = MovePossibleStart(possibleStarts[i]);
                    continue;
                }
                if (floor.edgeStartY != vec.pos.y && floor.edgeEndY != vec.pos.y)
                {
                    possibleStarts[i] = MovePossibleStart(possibleStarts[i]);
                    continue;
                }
            }
        }
        UpdatePaths();
    }
    public PossibleStart MovePossibleStart(PossibleStart start )
    {
        //Debug.Log($"Moving Possible Start: {start.pos.x};{start.pos.y}, {start.vertical}, {floor.edgeStartX}, {floor.edgeEndX}");
        int posX = start.pos.x, posY = start.pos.y;
        bool vertical = start.vertical;
        if (vertical)
        {
            int a = Mathf.Abs(posX - floor.edgeStartX);
            int b = Mathf.Abs(posX - floor.edgeEndX);
            if (a < b)
            {
                posX = floor.edgeStartX;
            }
            else
            {
                posX = floor.edgeEndX;
            }
        }
        else
        {
            int a = Mathf.Abs(posY - floor.edgeStartY);
            int b = Mathf.Abs(posY - floor.edgeEndY);
            if (a < b)
            {
                posY = floor.edgeStartY;
            }
            else
            {
                posY = floor.edgeEndY;
            }
        }
        if (grid[posX, posY].currentFloor > 0)
        {
            start.pos = new Vector2Int(posX, posY);
        }
        else
        {
            var cell = castlePosition;
            Vector2 dir = new Vector2Int(cell.gridX, cell.gridY) - new Vector2Int(posX, posY);
            dir.x = Mathf.Clamp(dir.x, -1, 1);
            dir.y = Mathf.Clamp(dir.y, -1, 1);
            int deltaX = (int)dir.x;
            int deltaY = (int)dir.y;
            //UnityEngine.Debug.Log($"{dir} deltaX and deltaY in finding possible start are: {deltaX},{deltaY}");
            int i = 0;
            while (i < 1000 && grid[posX, posY].currentFloor < 1)
            {
                i++;
                posX += deltaX;
                posY += deltaY;
            }
            //Debug.Log(i);
            start.pos = new Vector2Int(posX, posY);
        }
        //UnityEngine.Debug.Log($"{posX}:{posY}");
        return start;
    }
    public PossibleStart GenereratePossibleStart()
    {
        /*
         1. Pick a point
         2. If point is already walkable, set as a wave start         
         3. Else raycast the point towards the castle start
         4. Set the first intersection as a wave start
         */
        //possibleStarts.Clear();
        //FloorCell[,] grid = floorManager.floorCells;
        //Debug.Log("Generating new possible start");
        int posX, posY;
        bool vertical = UnityEngine.Random.Range(0, 2) == 1;
        if (!vertical)
        {
            posX = UnityEngine.Random.Range(floor.edgeStartX, floor.edgeEndX);
            posY = UnityEngine.Random.Range(0, 2) == 1 ? floor.edgeStartY : floor.edgeEndY;
        }
        else
        {
            posY = UnityEngine.Random.Range(floor.edgeStartY, floor.edgeEndY);
            posX = UnityEngine.Random.Range(0, 2) == 1 ? floor.edgeStartX : floor.edgeEndX;
        }
        //UnityEngine.Debug.Log($"Xdism:{floor.edgeStartX},{floor.edgeEndX}, Ydims:{floor.edgeStartY},{floor.edgeEndY}");
        if (grid[posX, posY].currentFloor > 0)
        {
            return new PossibleStart()
            {
                pos = new Vector2Int(posX, posY),
                vertical = vertical
            };
        }
        else
        {
            var cell = castlePosition;
            Vector2 dir = new Vector2Int(cell.gridX, cell.gridY) - new Vector2Int(posX, posY);
            dir.x = Mathf.Clamp(dir.x, -1, 1);
            dir.y = Mathf.Clamp(dir.y, -1, 1);
            int deltaX = (int)dir.x;
            int deltaY = (int)dir.y;
            //UnityEngine.Debug.Log($"{dir} deltaX and deltaY in finding possible start are: {deltaX},{deltaY}");
            int i = 0;
            while (i < 1000 && grid[posX, posY].currentFloor < 1)
            {
                i++;
                posX += deltaX;
                posY += deltaY;
            }
            //UnityEngine.Debug.Log($"{i},{posX}:{posY}");
            return new PossibleStart()
            {
                pos = new Vector2Int(posX, posY),
                vertical = vertical
            };
        }
    }
    public void UpdatePaths()
    {
        StopAllCoroutines();
        paths.Clear();
        //Debug.Log(possibleStarts.Count);
        for (int i = 0; i < possibleStarts.Count; i++)
        {
            //result = new();
            var start = possibleStarts[i];
            var res = new List<PathCell>();
            if (FindPath(grid[start.pos.x, start.pos.y], castlePosition, res)) { paths.Add(res);}
        }
        enemyManager.ShowWave();
    }
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StopAllCoroutines();
            var start = possibleStarts[0];
            var res = new List<PathCell>();
            StartCoroutine(FindPathCoroutine(grid[start.pos.x, start.pos.y], castlePosition, res));
        }
    }*/
    /*public IEnumerator FindPathCoroutine(FloorCell start, FloorCell end, List<PathCell> res)
    {
        bool f = false;
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
            f = Input.GetKeyDown(KeyCode.F);
            while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            f = false;
            FloorCell current = openHeap[0];
            debugGrid.GetValue(current.gridX, current.gridY).SetColor(Color.black);
            //Debug.Log($"Checking! {current.gridX}, {current.gridY} ");
            if (current.gridX == end.gridX && current.gridY == end.gridY)
            {
                ////Debug.Log($"find! {current.gridX}, {current.gridY}");
                // TODO: traceback 
                ////Debug.Log($"{current.gridX},{current.gridY} : {start.gridX},{start.gridY} ");
                //res = new();
                while (current.gridX != start.gridX || current.gridY != start.gridY)
                {
                    while (!f)
                    {
                        yield return null;
                        f = Input.GetKeyDown(KeyCode.F);
                    }
                    f = false;
                    debugGrid.GetValue(current.gridX, current.gridY)
                        .UpdateInfo(Color.blue, current);
                    res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                    current = grid[current.comeFrom.x, current.comeFrom.y];
                }
                //sw.Stop();
                //UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
                res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                yield break; // return true;
            }
            RemoveFirst(openHeap, heapCount--);
            Vector2Int currentPos = new Vector2Int(current.gridX, current.gridY);
            closed.Add(currentPos);
            ////Debug.Log("Adding cell from closed and removing from open");
            openCell.Remove(currentPos);
            GetNeighbours4(current, grid, w, h, neighbours);
            for (int i = 0; i < 4; i++)
            {
                while (!f)
                {
                    yield return null;
                    f = Input.GetKeyDown(KeyCode.F);
                }
                f = false;
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
                            FloorCell cell = neighbours[i];
                            cell = grid[cell.gridX,cell.gridY];
                            cell.cost = stepCost + current.cost;
                            cell.left = left;
                            cell.comeFrom = new Vector2Int(current.gridX, current.gridY);
                            openHeap[cell.heapIndex] = cell;
                            SortUpElement(cell.heapIndex, openHeap);
                            cell = grid[cell.gridX, cell.gridY];
                            debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);
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
                        debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);
                    }
                }
            }
            while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            f = false;
        }
        Debug.Log("No possible paths found...");
        //res = null;
        yield break;  //
        //return false;
    }*/
    /*void OnDrawGizmos()
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
    }*/
    GridVisual CreateEmptyObject(int gridX, int gridY)
    {
        return Instantiate(debug, transform).Init(gridX, gridY, 0f, 
            floor.CellToWorld(new Vector3Int(gridX, gridY) - offset));
    }
    public bool FindPath(FloorCell start, FloorCell end, List<PathCell> res)
    {
        HashSet<Vector2Int> closed = new();
        FloorCell[] openHeap = new FloorCell[16];
        int heapCount = 0;
        openHeap[heapCount++] = start;
        HashSet<Vector2Int> openCell = new();
        FloorCell[] neighbours = new FloorCell[4];
        while (heapCount > 0)
        {
            FloorCell current = openHeap[0];
            if (current.gridX == end.gridX && current.gridY == end.gridY)
            {
                while (current.gridX != start.gridX || current.gridY != start.gridY)
                {
                    res.Add(new PathCell(current,cellSize,offset.x, offset.y));
                    current = grid[current.comeFrom.x, current.comeFrom.y];
                }
                res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                return true;
            }
            RemoveFirst(openHeap, heapCount--);
            Vector2Int currentPos = new Vector2Int(current.gridX, current.gridY);
            closed.Add(currentPos);
            openCell.Remove(currentPos);
            GetNeighbours4(current, grid, w, h, neighbours);
            for (int i = 0; i < 4; i++)
            {
                if (neighbours[i].gridX == -1 || neighbours[i].gridY == -1) 
                { 
                    continue; 
                }
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
                            FloorCell cell = neighbours[i];
                            cell = grid[cell.gridX, cell.gridY];
                            cell.cost = stepCost + current.cost;
                            cell.left = left;
                            cell.comeFrom = new Vector2Int(current.gridX, current.gridY);
                            openHeap[cell.heapIndex] = cell;
                            SortUpElement(cell.heapIndex, openHeap);
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
                    }
                }
            }
        }
        //Debug.Log("No possible paths found...");
        return false;
    }
    bool IsWalkable(FloorCell cell, FloorCell cameFrom)
    {
        if(!cell.walkable) return false;
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
        else result[1] = new FloorCell(-1, -1);
        if (gridX + 1 < w) result[1] = grid[gridX + 1, gridY];
        else result[1] = new FloorCell(-1,-1);
        if (gridY + 1 < h) result[2] = grid[gridX, gridY + 1];
        else result[2] = new FloorCell(-1,-1);
        if (gridY - 1 >= 0) result[3] = grid[gridX, gridY - 1];
        else result[1] = new FloorCell(-1, -1);
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
        //if (index < 0) UnityEngine.Debug.LogError("trying to sort an element not included in heap");
        int count = index + 1;
        var temp = heap[index];
        int together = GetTogetherCost(temp);
        int parent = (index - 1) / 2;

        //string s = $"sorting {index}: {temp.gridX}, {temp.gridY}, {together}, {heap[index].left}; \n count: {count} \n";
        while (index > 0 && GetTogetherCost(heap[parent]) >= together)
        {
            if (GetTogetherCost(heap[parent]) == together && (heap[parent].left <= temp.left))
            {
                //s += "same cost and same cells count left \n";
                break;
            }
            Swap(index, parent, heap);

            index = parent;
            parent = (index - 1) / 2;
        }
        /*for (int i = 0; i < count; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n";
        }
        Debug.Log(s);*/
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
        while (lowestCost <= targetCost && left < count)
        {
            if (right >= count) 
            {
                lowestChild = left;
            }
            else
            {
                c1 = heap[left].left * defaultMoveCost + heap[left].cost;
                c2 = heap[right].left * defaultMoveCost + heap[right].cost;
                if (c1 != c2)
                {
                    lowestChild = c1 < c2 ? left : right;
                }
                else
                {
                    lowestChild = heap[left].left < heap[right].left ? left : right;
                }
            }
            lowestCost = heap[lowestChild].left * defaultMoveCost + heap[lowestChild].cost;

            if (lowestCost == targetCost && !(heap[lowestChild].left < heap[target].left))
            {
                break;
            }
            Swap(target,lowestChild, heap);
            target = lowestChild;
            left = target * 2 + 1;
            right = target * 2 + 2;
        }
        //Debug.Log($"{lowestCost}; {targetCost}");
    }
    public void Swap(int first, int second, FloorCell[] heap)
    {
        //Debug.Log($"Swapping {first} and {second}");
        //int gridX1, gridX2, gridY1, gridY2;
        var temp = heap[first];
        /*gridX1 = temp.gridX;
        gridY1 = temp.gridY;
        gridX2 = heap[second].gridX;
        gridY2 = heap[second].gridY;*/
        /*Debug.Log($"swapping {first} and {second}: " +
            *//*$"{heap[first].gridX},{heap[first].gridY} " +
            $"{heap[second].gridX},{heap[second].gridY} +" +*//*
            $"grid: {grid[gridX1, gridY1].heapIndex} " +
            $"{grid[gridX2, gridY2].heapIndex}");*/
        heap[first] = heap[second];
        heap[second] = temp;
        heap[first].heapIndex = first;
        heap[second].heapIndex = second;
        var cell1 = heap[first];
        var cell2 = heap[second];
        grid[cell1.gridX, cell1.gridY] = cell1;
        grid[cell2.gridX, cell2.gridY] = cell2;
        /*Debug.Log($"swapped {first} and {second}: " +
            *//*$"{heap[first].gridX},{heap[first].gridY} " +
            $"{heap[second].gridX},{heap[second].gridY} +" +*//*
            $"grid: {grid[gridX1, gridY1].heapIndex} " +
            $"{grid[gridX2, gridY2].heapIndex}");
        debugGrid.GetValue(cell1.gridX, cell1.gridY)
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
        /*string s = $"peeked {result.gridX}, {result.gridY}; {result.left}; \n count: {count - 1} \n";
        for (int i = 0; i < count - 1; i++)
        {
            s += $"grid: {heap[i].gridX},{heap[i].gridY}; childOf: {heap[(i - 1) / 2].gridX},{heap[(i - 1) / 2].gridY}; together: {GetTogetherCost(heap[i])}; leftCells: {heap[i].left}; cost:{heap[i].cost}; \n ";
        }
        Debug.Log(s);*/
        return result;
    }
}
public struct PossibleStart
{
    public Vector2Int pos;
    public bool vertical;
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
        if(cell.bridge && !cell.bridgeData.start)
        {
            floor = cell.bridgeData.floor;
        }
        else
        {
            floor = cell.currentFloor;
        }
        gridY = cell.gridY;
    }
}


