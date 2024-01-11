public struct Cell{
    public int x;
    public int y;
    public readonly int FLOOR_LAYER;
    public Cell[] neighbours;
    public bool walkable;
    public bool road;
    public Cell(int x, int y, int layer){
        this.x = x;
        this.y = y;
        FLOOR_LAYER = layer;
        neighbours = new Cell[4];
        walkable = true;
        road = false;
    }
}