using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor {
    private Tilemap map;
    public int floorX{get;private set;}
    public int floorY{get; private set;}
    public int floorWidth{get; private set;}
    public int floorHeight{get; private set;}
    
    private readonly Vector3Int z_1 = Vector3Int.forward;
    public const int SHADOW_LAYER = 0; // Shadow only where floor's end
    public const int GROUND_LAYER = 1; // Ground everywhere where there's floor
    public const int GRASS_LAYER = 2; // Grass only inside floor
    public const int ROAD_LAYER = 3; // Road is a certain number of tiles on floor
    public const int ROCK = 4; // Rock only where floor's bottom
    public const int STAIRS = 5; // Stairs where ROCK && ROAD

    public Floor(Tilemap map){
        this.map = map;
    }
    public void SetFloor(int x, int y, int width, int height){
        floorX = x;
        floorY = y;
        floorWidth = width;
        floorHeight = height;
        for(int k = x; k<x+width; k++){
            for(int l = y;l<y+height;l++){
                Vector3Int pos = new Vector3Int(k,l,0);
                if(k == x || l == y || k == x + width - 1 || l == y + height - 1){
                    map.SetTile(pos +  z_1 * SHADOW_LAYER, StaticTiles.GetTile(SHADOW_LAYER));
                    if(l == y){
                        map.SetTile(pos + z_1 * ROAD_LAYER, StaticTiles.GetTile(ROCK));
                    }else{
                        map.SetTile(pos + z_1 * GROUND_LAYER, StaticTiles.GetTile(GROUND_LAYER));
                    }
                    continue;
                }else{
                    map.SetTile(pos +  z_1 * GROUND_LAYER, StaticTiles.GetTile(GROUND_LAYER));
                }
                map.SetTile(pos + z_1 * GRASS_LAYER, StaticTiles.GetTile(GRASS_LAYER));
            }
        }
    }
    public void ClearAllTiles(){
        map.ClearAllTiles();
    }
}
