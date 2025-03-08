using System;
using UnityEngine;
public class CustomGrid<T>
{
    private T[,] grid;
    public readonly int w, h;
    public readonly float cellsize;
    public readonly Vector2 offset;
    Func<int, int, T> func;
    public CustomGrid(int _w, int _h, float _cellSize, Vector2 _offset, Func<int,int,T> _func)
    {
        w = _w; h = _h;
        grid = new T[w, h]; 
        cellsize = _cellSize;
        offset = _offset;
        func = _func;
        /*for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < w; y++)
            {
                grid[x, y] = null;
            }
        }*/
    }
    public Vector2 GridToWorld(int x, int y)
    {
        if (x < 0 || x >= w || y < 0 || y >= h) return default;
        return new Vector2(x, y) * cellsize - offset + 0.5f * cellsize * Vector2.one;
    }
    public void WorldToGrid(Vector2 pos, out int x, out int y)
    {
        x = Mathf.FloorToInt((pos.x + offset.x) / cellsize);
        y = Mathf.FloorToInt((pos.y + offset.y) / cellsize);
    }
    public Vector3Int WorldToGrid(Vector2 pos)
    {
        WorldToGrid(pos, out int x, out int y);
        return new Vector3Int(x, y);
    }
    public void SetValue(T value, int x, int y)
    {
        if (x < 0 || x >= w || y < 0 || y >= h) return;
        grid[x, y] = value;
    }
    public T GetValue(int x, int y)
    {
        if (x < 0 || x >= w || y < 0 || y >= h) Debug.LogError($"coordinates aare outside of the grid {x} {y} {w} {h}");
        if (grid[x, y] == null) grid[x, y] = func.Invoke(x,y);
        return grid[x, y];
    }
    public T GetValue(Vector2 pos)
    {
        WorldToGrid(pos, out int x, out int y);
        return GetValue(x,y);
    }
    public void GetNeighbours(int gridX, int gridY, T[] result)
    {
        if (gridX < 0 || gridX >= w || gridY < 0 || gridY >= h) Debug.LogError($"coordinates aare outside of the grid {gridX} {gridY} {w} {h}");
        if (gridX - 1 >= 0) result[0] = grid[gridX - 1, gridY];
        if (gridX + 1 < w) result[1] = grid[gridX + 1, gridY];
        else { Debug.Log($"neighbour {gridX + 1} is outside of grid"); result[1] = default; }
        if (gridY + 1 < h) result[2] = grid[gridX, gridY + 1];
        else { Debug.Log($"neighbour {gridY + 1} is outside of grid"); result[2] = default; }
        if (gridY - 1 >= 0) result[3] = grid[gridX, gridY - 1];
    }
}
