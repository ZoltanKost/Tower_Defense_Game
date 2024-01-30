using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor {
    private const int FOAMLAYER = 0;
    private const int SHADOWLAYER = 1;
    private const int GROUNDLAYER = 2;
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
    private readonly Vector3Int z1 = Vector3Int.forward;
    private FloorCell[,] floorCells;

    public Floor(Tilemap map, int width, int height){
        this.map = map;
        this.width = width;
        this.height = height;
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                floorCells[x,y] = new FloorCell(x,y);
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
        pos.z =0;
        CellToXY(pos, out int x, out int y);
        if(x < 0 || x>=width || y<0 || y>=height) return;
        int floor = floorCells[x,y].currentFloor;
        if(floor > 2) return;
        Vector3Int downPos = pos + Vector3Int.down + z1 * floor * 4;
        if(floorCells[x, y].currentFloor == 0)
        {
            map.SetTile(pos + z1 * FOAMLAYER + z1 * floor, StaticTiles.GetTile(shadowID | TileID.Alt));
            map.SetTile(pos + z1 * GROUNDLAYER + z1 * floor, StaticTiles.GetTile(TileID.Sand));
            floorCells[x, y].currentFloor++;
            return;
        }
        Debug.Log("HAs Second Floor!");
        TileBase currentTile = map.GetTile(pos + z1 * floor * 4);
        TileBase downTile = map.GetTile(downPos + z1 * GROUNDLAYER);
        TileBase groundAlt = StaticTiles.GetTile(groundID | TileID.Alt);
        TileBase ground = StaticTiles.GetTile(groundID); 
        if(currentTile == ground) return;
        Debug.Log("Placing Stuff!");
        if(downTile != ground)
        {
            map.SetTile(downPos  + z1 * SHADOWLAYER, StaticTiles.GetTile(shadowID));
            map.SetTile(downPos + z1 * GROUNDLAYER, groundAlt);
        }
        map.SetTile(pos + z1 * SHADOWLAYER + z1 * floor * 4,  StaticTiles.GetTile(shadowID));
        map.SetTile(pos + z1 * GROUNDLAYER + z1 * floor * 4, ground);
        floorCells[x, y].currentFloor++;
        // else if(downTile == groundAlt){
        //     map.SetTile(downPos + z1 * GROUNDLAYER, ground);
        //     floorCells[x, y].currentFloor++;
        // }else if(downTile == ground){
        //     map.SetTile(pos + z1 * SHADOWLAYER + z1 * floor,  StaticTiles.GetTile(shadowID));
        //     map.SetTile(pos + z1 * GROUNDLAYER + z1 * floor, ground);
        // }
        // map.SetTile(pos + z1 * 3, StaticTiles.GetTile(grassID));
        // map.SetTile(pos + z1 * SHADOWLAYER, StaticTiles.GetTile(shadowID));
        // map.SetTile(pos + z1 * GROUNDLAYER, StaticTiles.GetTile(groundID));
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
    public void CellToXY(Vector3Int cell, out int x, out int y){
        x = cell.x;
        y = cell.y;
    }
}