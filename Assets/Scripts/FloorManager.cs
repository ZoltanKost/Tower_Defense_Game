using UnityEngine;
using System.Collections.Generic;
public class FloorManager : MonoBehaviour{
    [SerializeField] private int layers;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Floor floorPrefab; 
    List<Floor> floors;
    private FloorCell[,] floorCells;
    void Awake(){
        floors = new List<Floor>();
        for(int i = 0; i < layers; i++){
            floors.Add(Instantiate(floorPrefab, transform));
            if(i == 0) floors[i].Init(0,TileID.Sand);
            else floors[i].Init(i,TileID.Ground,TileID.Grass);
        }
        floorCells = new FloorCell[width,height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < width; y++){
                floorCells[x,y] = new FloorCell();
            }
        }
    }
    public void CreateGround(Vector3 input){
        Vector3Int pos = floors[0].WorldToCell(input);
        if(pos.x < 0 || pos.x >= width || pos.y < 1 || pos.y >= height) return;
        for(int x = 0; x < 2; x++){
            for(int y = 0; y < 2; y++){
                if(floorCells[pos.x + x,pos.y + y].currentFloor >= floors.Count) return;
                if(floorCells[pos.x + x,pos.y + y].currentFloor > floorCells[pos.x + x,pos.y-1 + y].currentFloor) return;
            }
        }
        if(floors[floorCells[pos.x,pos.y].currentFloor].CreateGroundArray(pos, 2, 2)) {
            for(int x = 0; x < 2; x++){
                for(int y = 0; y < 2; y++){
                    floorCells[pos.x + x, pos.y + y].currentFloor++;
                }
            }
        }
    }
}