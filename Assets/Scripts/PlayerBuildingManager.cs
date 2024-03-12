using UnityEngine;
public class PlayerBuildingManager : MonoBehaviour{
    [SerializeField] private FloorManager floor;
    public void Init(FloorManager floorManager){
        floor = floorManager;
    }
    GroundArray chosenGround;
    Building chosenBuilding;
    BuildMode mode;
    public void Build(Vector3 position){
        switch(mode){
            case BuildMode.Ground:
                floor.CreateGroundArray(position,chosenGround);
                break;
            case BuildMode.Road:
                floor.PlaceRoad(position);
                break;
            case BuildMode.Building:
                floor.PlaceBuilding(position,chosenBuilding);
                break;
            case BuildMode.Bridge:
                floor.PlaceBridge(position);
                break;
        }
    }
    public void ChooseMode(BuildMode m){
        mode = m;
    }
    public bool ResetMode(){
        if(mode == 0) return false;
        mode = 0;
        return true;
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
    Bridge
}