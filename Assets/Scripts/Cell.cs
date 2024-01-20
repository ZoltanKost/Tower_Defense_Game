public struct Cell{
    public int x;
    public int y;
    public Cell[] neighbours;
    public int floor;
    public bool walkable;
    public bool road;
    public Cell(int x, int y){
        this.x = x;
        this.y = y;
        neighbours = new Cell[4];
        walkable = true;
        road = false;
        floor = 0;
    }
}