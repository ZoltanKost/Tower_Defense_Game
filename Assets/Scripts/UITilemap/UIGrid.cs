using System;
using UnityEngine;

public class UIGrid<T> where T : new (){
    T[,] values;
    int width, height;
    float cellSize;
    Vector2 origin;
    Action<T,int,int> onChange;
    public UIGrid(int width, int height, float cellSize, Vector3 origin, Action<T, int, int> onChangeAction)
    {
        values = new T[width,height];
        this.width = width;
        this.height = height;
        this.origin = origin;
        onChange = onChangeAction;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                values[x,y] = new T();
                onChange(values[x,y], x,y);
            }
        }
    }
    public void WordPositionToGrid(Vector2 worldPosition, out int posX, out int posY)
    {
        worldPosition -= origin;
        posX = Mathf.FloorToInt(worldPosition.x / cellSize);
        posY = Mathf.FloorToInt(worldPosition.y / cellSize);
    }
    public void SetValue(int gridX, int gridY, T value)
    {
        if(gridX < 0 || gridX >= width) return;
        if(gridY < 0 || gridY >= height) return;
        values[gridX, gridY] = value;
        onChange(value,gridX,gridY);
    }
    public void SetValue(Vector2 position, T value)
    {
        WordPositionToGrid(position, out int X, out int Y);
        SetValue(X,Y, value);
    }
}