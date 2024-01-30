using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor {
    private const int FOAMLAYER = -1;
    private const int SHADOWLAYER = 0;
    private const int GROUNDLAYER = 1;
    private const int GRASSLAYER = 3;
    private const int SANDLAYER = 4; 
    private Tilemap map;
    public int floorX{get;private set;}
    public int floorY{get; private set;}
    public int width{get; private set;}
    public int height{get; private set;}
    readonly TileID shadowID = TileID.Shadow;
    private TileID groundID = TileID.Ground;
    private TileID grassID = TileID.Grass;
    private readonly Vector3Int y1 = Vector3Int.up;
    private FloorCell[,] floorCells;

    public Floor(Tilemap map, int width, int height){
        this.map = map;
        this.width = width;
        this.height = height;
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                floorCells[x,y] = new FloorCell();
                if(y < 1) continue;
            }
        }
    }
    // public void SetFloor(int x, int y, int width, int height){
    //     floorX = x;
    //     floorY = y;
    //     floorWidth = width;
    //     floorHeight = height;
    //     for(int k = x; k<x+width; k++){
    //         for(int l = y;l<y+height;l++){
    //             groundID = TileID.Ground;
    //             grassID = TileID.Grass;
    //             Vector3Int pos = new Vector3Int(k,l,0);
    //             if(y - l == 1 || y == l){
    //                 grassID = TileID.None;
    //             }
    //             if(k == x || l == y || k == x + width - 1 || l == y + height - 1){
    //                 if(layer == 0 && l == y) grassID |= TileID.Alt;
    //                 groundID |= TileID.Alt;
    //             }
    //             if(layer == 1) map.SetTile(pos + z1 * FOAMLAYER, StaticTiles.GetTile(shadowID | TileID.Alt)); 
    //             map.SetTile(pos + z1 * SHADOWLAYER, StaticTiles.GetTile(shadowID));
    //             map.SetTile(pos + z1 * GROUNDLAYER, StaticTiles.GetTile(groundID));
    //             map.SetTile(pos + z1 * GRASSLAYER, StaticTiles.GetTile(grassID));
    //         }
    //     }
    // }
    public void ClearAllTiles(){
        map.ClearAllTiles();
    }
    public void CreateGround(Vector3Int pos)
    {
        pos.y = 0;
        CellToXY(pos, out int x, out int z);
        if(x < 0 || x >= width || z < 1 || z >= height) return;
        int floor = floorCells[x,z].currentFloor;
        // if(floor > 2) return;
        Vector3Int downPos = pos + Vector3Int.back + y1 * floor * 4;
        Vector3Int upPos = pos + Vector3Int.forward + y1 * floor * 4;
        if(floorCells[x, z].currentFloor == 0)
        {
            map.SetTile(pos + y1 * FOAMLAYER + y1 * floor, StaticTiles.GetTile(shadowID | TileID.Alt));
            map.SetTile(pos + y1 * SHADOWLAYER + y1 * floor, StaticTiles.GetTile(shadowID));
            map.SetTile(pos + y1 * GROUNDLAYER + y1 * floor, StaticTiles.GetTile(TileID.Sand));
            floorCells[x, z].currentFloor++;
            Debug.Log("SAND SET: " + pos + y1 * floor);
            return;
        }
        TileBase groundAlt = StaticTiles.GetTile(groundID | TileID.Alt);
        TileBase ground = StaticTiles.GetTile(groundID);
        TileBase shadow = StaticTiles.GetTile(shadowID);
        TileBase grass = StaticTiles.GetTile(grassID);
        // if(){
        //     // // Build ground on the upper tile
        //     // map.SetTile(upPos + z1 * SHADOWLAYER + z1 * floor * 4,  shadow);
        //     // map.SetTile(upPos + z1 * GROUNDLAYER + z1 * floor * 4, ground);
        //     // floorCells[x,y + 1].currentFloor ++;
        //     // // Build alternate ground on this tile
        //     // map.SetTile(pos + z1 * SHADOWLAYER + z1 * floor * 4, shadow);
        //     // map.SetTile(pos + z1 * GROUNDLAYER + z1 * floor * 4, groundAlt);
        // }else 
        if(floorCells[x,z].currentFloor < floorCells[x,z-1].currentFloor){
            // Bulid ground on this tile
            map.SetTile(pos + y1 * SHADOWLAYER + y1 * floor * 4, shadow);
            map.SetTile(pos + y1 * GROUNDLAYER + y1 * floor * 4, ground);
            map.SetTile(pos + y1 * GRASSLAYER + y1 * floor * 4, grass);
            floorCells[x,z].currentFloor ++;
        }else if(floorCells[x,z].currentFloor == floorCells[x,z-1].currentFloor){
            // Build ground on this tile;
            map.SetTile(pos + y1 * SHADOWLAYER + y1 * floor * 4,  shadow);
            map.SetTile(pos + y1 * GROUNDLAYER + y1 * floor * 4, ground);
            map.SetTile(pos + y1 * GRASSLAYER + y1 * floor * 4, grass);
            floorCells[x, z].currentFloor ++;
            // Build Rock on down tile 
            map.SetTile(downPos + y1 * SHADOWLAYER, shadow);
            map.SetTile(downPos + y1 * GROUNDLAYER, groundAlt);
        }
    }
    public void CreateGround(Vector3 input){
        Vector3Int pos = map.WorldToCell(input);
        CreateGround(pos);
    }
    public bool HasTile(Vector3Int pos){
        pos.z = GROUNDLAYER;
        if(map.GetTile(pos) != StaticTiles.GetTile(groundID | TileID.Alt))
        return map.HasTile(pos);
        return false;
    }
    public bool HasTile(Vector3 input){
        return HasTile(map.WorldToCell(input));
    }
    public TileBase GetTile(Vector3Int pos){
        pos.z = GRASSLAYER;
        if(!map.HasTile(pos)){
            pos.z = GROUNDLAYER;
        }
        return map.GetTile(pos);
    }
    public void CellToXY(Vector3Int cell, out int x, out int z){
        x = cell.x;
        z = cell.z;
    }
}