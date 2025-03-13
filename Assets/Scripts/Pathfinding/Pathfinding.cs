using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
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
    public List<List<PathCell>> enemyPaths = new List<List<PathCell>>();
    public List<Vector2Int> possibleStarts = new List<Vector2Int>();

    public List<Vector2Int> ships = new();
    [SerializeField]
    public List<List<PathCell>> shipPaths = new();

    public FloorCell castlePosition;

    CustomGrid<GridVisual> debugGrid;
    public void Start()
    {
        //floor.Init();
        //floor.CreateCastle(transform.position, b);
        //SetTargetPoint(Mathf.Abs(offset.x), Mathf.Abs(offset.y), 0,0);
        enemyPaths = new();
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
    public PossibleStart NeedMovePossibleStart(PossibleStart start, List<PathCell> path)
    {
        /*
         1. Snap start to the correspondent edge(with correspondent coordinate)
         2. if not walkable, find nearest walkable.
         */
        path.Clear();
        if (start.vertical)
        {
            start.pos.x = start.fixedMinimum?floor.edgeStartX:floor.edgeEndX;
        }
        else
        {
            start.pos.y = start.fixedMinimum ? floor.edgeStartY : floor.edgeEndY;
        }
        if (FindPath(grid[start.pos.x, start.pos.y], castlePosition, path))
        {
            return start;
        }
        start.pos = FindFirstwalkable(start.pos.x, start.pos.y, path);
        return start;
    }
    public PossibleStart GenereratePossibleStart(bool randomAxisY, bool fixedAxisMinimal,
        out List<PathCell> enemyPath)
    {

        /*1.Pick a point
         2.If a way can be created from a point, return it
         3.Else find very first walkable point around it
         4.If a way can be created from it, return*/

        int posX, posY;
        if (!randomAxisY)
        {
            posX = UnityEngine.Random.Range(floor.edgeStartX, floor.edgeEndX + 1);
            posY = fixedAxisMinimal ? floor.edgeStartY : floor.edgeEndY;
        }
        else
        {
            posY = UnityEngine.Random.Range(floor.edgeStartY, floor.edgeEndY + 1);
            posX = fixedAxisMinimal ? floor.edgeStartX : floor.edgeEndX;
        }
        //UnityEngine.Debug.Log($"Xdism:{floor.edgeStartX},{floor.edgeEndX}, Ydims:{floor.edgeStartY},{floor.edgeEndY}");
        enemyPath = new();
        Vector2Int pos = new Vector2Int(posX, posY);
        Debug.Log($"{posX}:{posY} edges: {floor.edgeStartX},{floor.edgeEndX}:{floor.edgeStartY},{floor.edgeEndY} ");
        if (!(grid[posX, posY].currentFloor > 0 && FindPath(grid[posX, posY], castlePosition, enemyPath)))
        {
            pos = FindFirstwalkable(posX, posY, enemyPath);
        }
        Debug.Log($"{posX}:{posY} edges: {floor.edgeStartX},{floor.edgeEndX}:{floor.edgeStartY},{floor.edgeEndY} ");
        return new PossibleStart()
        {
            pos = pos,
            vertical = randomAxisY,
            fixedMinimum = fixedAxisMinimal
        };
    }
    public Vector2Int FindFirstwalkable(int posX, int posY, List<PathCell> path)
    {
        int w = grid.GetLength(0), h = grid.GetLength(1);
        FloorCell[] neighs = new FloorCell[4];
        GetNeighbours4(grid[posX, posY], grid, w, h, neighs);
        Queue<FloorCell> open = new(neighs);
        FloorCell current = open.Dequeue();
        HashSet<FloorCell> closed = new() { current };
        // - probably should move only in castle direction.
        // - probably should not check all the cells checked in pathfinding
        while (true)
        {
            if (current.currentFloor < 1)
            {
                GetNeighbours4(current, grid, w, h, neighs);
                foreach (var n in neighs)
                {
                    if (closed.Contains(n) || open.Contains(n)) continue;
                    open.Enqueue(n);
                }
            }
            else
            {
                if (FindPath(current, castlePosition, path))
                {
                    return new Vector2Int(current.gridX, current.gridY);
                }
            }
            current = open.Dequeue();
            closed.Add(current);
        }
    }

    /*public void UpdatePaths()
    {
        enemyPaths.Clear();
        //Debug.Log(possibleStarts.Count);
        for (int i = 0; i < possibleStarts.Count; i++)
        {
            //result = new();
            var start = possibleStarts[i];
            var res = new List<PathCell>();
            //StartCoroutine(FindPathCoroutine(grid[start.x, start.y], castlePosition, res));
            if (FindPath(grid[start.x, start.y], castlePosition, res)) { enemyPaths.Add(res); }
            Debug.Log(enemyPaths.Count);
        }
        enemyManager.ShowWave();
    }*/
    /*public void UpdateShipPaths()
    {
        enemyPaths.Clear();
        shipPaths.Clear();
        possibleStarts.Clear();
        //Debug.Log(possibleStarts.Count);

        for (int i = 0; i < ships.Count; i++)
        {
            //result = new();
            var start = ships[i];
            var shipPath = new List<PathCell>();
            var enemyPath = new List<PathCell>();
            //StartCoroutine(FindPathBoatCoroutine(grid[start.x, start.y], castlePosition, shipPath, enemyPath));
            if (FindPathBoat(grid[start.x, start.y], castlePosition, shipPath, enemyPath, out var enemyStart))
            {
                possibleStarts.Add(enemyStart);
                shipPaths.Add(shipPath);
                enemyPaths.Add(enemyPath);
            }
        }
        enemyManager.ShowWave();
    }*/
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StopAllCoroutines();
            var start = ships[0];
            var res = new List<PathCell>();
            StartCoroutine(FindPathBoatCoroutine(grid[start.x, start.y], castlePosition, res, new()));
        }
    }*/
    public IEnumerator FindPathCoroutine(FloorCell start, FloorCell end, List<PathCell> res)
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
                            cell = grid[cell.gridX, cell.gridY];
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
    }
    public IEnumerator FindPathBoatCoroutine(FloorCell start, FloorCell end, List<PathCell> res)
    {
        //enemyStart = new Vector2Int(-1, -1);
        HashSet<Vector2Int> closed = new();
        FloorCell[] openHeap = new FloorCell[16];
        int heapCount = 0;
        openHeap[heapCount++] = start;
        HashSet<Vector2Int> openCell = new();
        FloorCell[] neighbours = new FloorCell[8];
        while (heapCount > 0)
        {
            bool f = false;
            while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            FloorCell current = openHeap[0];
            debugGrid.GetValue(current.gridX, current.gridY).SetColor(Color.black);
            RemoveFirst(openHeap, heapCount--);
            Vector2Int currentPos = new Vector2Int(current.gridX, current.gridY);
            closed.Add(currentPos);
            openCell.Remove(currentPos);
            GetNeighbours8(current, grid, w, h, neighbours);
            Debug.Log(current.gridX + " " + current.gridY);
            for (int i = 0; i < 8; i++)
            {
                if (neighbours[i].gridX == -1 || neighbours[i].gridY == -1)
                {
                    continue;
                }
                if (!IsWalkableBoat(neighbours[i]))
                {
                    if (Mathf.Abs(current.gridX-end.gridX)<2 
                        && Mathf.Abs(current.gridY - end.gridY)<2)
                    {
                        while (current.gridX != start.gridX || current.gridY != start.gridY)
                        {
                            res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                            current = grid[current.comeFrom.x, current.comeFrom.y];
                        }
                        res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                        yield break;
                    }
                    continue;
                }
                var neighPos = new Vector2Int(neighbours[i].gridX, neighbours[i].gridY);
                if (!closed.Contains(neighPos))
                {
                    int stepCost = (int)(10 * new Vector2(neighbours[i].gridX - current.gridX, neighbours[i].gridY - current.gridY).magnitude);
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
                        debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);
                    }
                }
            }
        }
        //Debug.Log("No possible paths found...");
        yield break;//return false;
    }

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
    public bool FindPathBoat(FloorCell start, FloorCell end, List<PathCell> res)
    {
        //enemyStart = new Vector2Int(-1,-1);
        HashSet<Vector2Int> closed = new();
        FloorCell[] openHeap = new FloorCell[16];
        int heapCount = 0;
        openHeap[heapCount++] = start;
        HashSet<Vector2Int> openCell = new();
        FloorCell[] neighbours = new FloorCell[8];
        while (heapCount > 0)
        {
            /*bool f = Input.GetKeyDown(KeyCode.F);
            while (!f)
            {
                yield return null;
                f = Input.GetKeyDown(KeyCode.F);
            }
            f = false;*/
            FloorCell current = openHeap[0];
            //debugGrid.GetValue(current.gridX, current.gridY).SetColor(Color.black);
            RemoveFirst(openHeap, heapCount--);
            Vector2Int currentPos = new Vector2Int(current.gridX, current.gridY);
            closed.Add(currentPos);
            openCell.Remove(currentPos);
            GetNeighbours8(current, grid, w, h, neighbours);
            for (int i = 0; i < 8; i++)
            {
                if (neighbours[i].gridX == -1 || neighbours[i].gridY == -1)
                {
                    continue;
                }
                if (!IsWalkableBoat(neighbours[i]))
                {
                    if (Mathf.Abs(current.gridX - end.gridX) < 2
                        && Mathf.Abs(current.gridY - end.gridY) < 2)
                    {
                        while (current.gridX != start.gridX || current.gridY != start.gridY)
                        {
                            res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                            current = grid[current.comeFrom.x, current.comeFrom.y];
                        }
                        res.Add(new PathCell(current, cellSize, offset.x, offset.y));
                        return true;
                    }
                    continue;
                }
                var neighPos = new Vector2Int(neighbours[i].gridX, neighbours[i].gridY);
                if (!closed.Contains(neighPos))
                {
                    int stepCost = (int)(10 * new Vector2(neighbours[i].gridX - current.gridX, neighbours[i].gridY - current.gridY).magnitude);
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
                            /*debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);*/
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
                        /*debugGrid.GetValue(cell.gridX, cell.gridY)
                                .UpdateInfo(
                                Color.green, cell);*/
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
    bool IsWalkableBoat(FloorCell cell)
    {
        return !(cell.currentFloor > -1 || cell.bridge); 
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
        else result[0] = new FloorCell(-1, -1);
        if (gridX + 1 < w) result[1] = grid[gridX + 1, gridY];
        else result[1] = new FloorCell(-1,-1);
        if (gridY + 1 < h) result[2] = grid[gridX, gridY + 1];
        else result[2] = new FloorCell(-1,-1);
        if (gridY - 1 >= 0) result[3] = grid[gridX, gridY - 1];
        else result[3] = new FloorCell(-1, -1);
    }
    void GetNeighbours8(FloorCell cell, FloorCell[,] grid, int w, int h, FloorCell[] result8)
    {
        int gridX = cell.gridX;
        int gridY = cell.gridY;
        
        if (gridX - 1 >= 0)
            result8[0] = grid[gridX - 1, gridY];
        else result8[0] = new FloorCell(-1, -1);

        if (gridX - 1 >= 0 && gridY + 1 < h)
            result8[1] = grid[gridX - 1, gridY + 1];
        else result8[1] = new FloorCell(-1, -1);

        if (gridY + 1 < h)
            result8[2] = grid[gridX, gridY + 1];
        else result8[2] = new FloorCell(-1, -1);

        if (gridX + 1 < w && gridY + 1 < h)
            result8[3] = grid[gridX + 1, gridY + 1];
        else result8[3] = new FloorCell(-1, -1);

        if (gridX + 1 < w)
            result8[4] = grid[gridX + 1, gridY];
        else result8[4] = new FloorCell(-1, -1);

        if (gridX + 1 < w && gridY - 1 >= 0)
            result8[5] = grid[gridX + 1, gridY - 1];
        else result8[5] = new FloorCell(-1, -1);

        if (gridY - 1 >= 0)
            result8[6] = grid[gridX, gridY - 1];
        else result8[6] = new FloorCell(-1, -1);

        if (gridX - 1 >= 0 && gridY - 1 >= 0)
            result8[7] = grid[gridX - 1, gridY - 1];
        else result8[7] = new FloorCell(-1, -1);
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
public struct PossibleStart
{
    public bool vertical;
    public bool fixedMinimum;
    public Vector2Int pos;
}


