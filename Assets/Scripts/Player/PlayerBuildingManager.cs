using UnityEngine;
public class PlayerBuildingManager : MonoBehaviour{
    [SerializeField] private FloorManager floor;
    public void Init(FloorManager floorManager){
        floor = floorManager;
    }
    GroundArray chosenGround;
    Building chosenBuilding;
    BuildMode mode;
    // Returns True if chosen object can be built once per click
    public bool Build(Vector3 position){
        switch(mode){
            case BuildMode.Ground:
                if(floor.CreateGroundArray(position,chosenGround)){
                    mode = 0;
                    return true;
                }
                break;
            case BuildMode.Road:
                floor.PlaceRoad(position);
                break;
            case BuildMode.Building:
                if(floor.PlaceBuilding(position,chosenBuilding)){
                    mode = 0;
                    return true;
                }
                break;
            case BuildMode.Bridge:
                floor.PlaceBridge(position);
                break;
            case BuildMode.BridgeSpot:
                floor.PlaceBridgeSpot(position);
                break;
        }
        return false;
    }
    public void ChooseMode(BuildMode m){
        mode = m;
    }
    public void ResetMode(){
        mode = 0;
    }
    public void ChooseBuilding(Building b){
        mode = BuildMode.Building;
        chosenBuilding = b;
    }
    public void ChooseGround(GroundArray g){
        mode = BuildMode.Ground;
        chosenGround = g;
    }
}
public enum BuildMode{
    None,
    Ground,
    Road,
    Building,
    Bridge,
    BridgeSpot
}