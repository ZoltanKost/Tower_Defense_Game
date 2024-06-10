using System;
using UnityEngine;
public class PlayerBuildingManager : MonoBehaviour{
    [SerializeField] private FloorManager floor;
    [SerializeField] private TemporalFloor temporalFloor;
    Action buildingFailed;
    Action cancelBuildingActionCallback;
    Action placeCallback;
    Action<Resource, int> buyCallback;
    Func<Resource,int, bool> canBuyCallback;
    GroundArray chosenGround;
    Building chosenBuilding;
    BuildMode mode;
    public void Init(Action buildingFailed, TemporalFloor tp, Func<Resource, int, bool> canBuyCallback, Action<Resource, int> buyCallback){
        this.buildingFailed = buildingFailed;
        temporalFloor = tp;
        this.canBuyCallback = canBuyCallback;
        this.buyCallback = buyCallback;
    }
    // Returns True if chosen object can be built once per click
    public void ClickBuild(Vector3 position){
        switch(mode){
            case BuildMode.Ground:
                if(floor.CheckGA(position,chosenGround) && 
                canBuyCallback(Resource.Gold,chosenGround.price))
                {
                    mode = 0;
                    floor.CreateGroundArray_DontCheck(position,chosenGround);
                    buyCallback?.Invoke(Resource.Gold,chosenGround.price);
                    FinishBuildingAction();
                }
                else
                {
                    buildingFailed?.Invoke();
                }
                break;
            case BuildMode.Road:
                if(!floor.PlaceRoad(position))
                {
                    buildingFailed?.Invoke();
                }
                break;
            case BuildMode.Building:
                if(floor.CheckBuilding(position,chosenBuilding.width, chosenBuilding.height) 
                   && canBuyCallback(chosenBuilding.resource,chosenBuilding.price))
                {
                    buyCallback(Resource.Gold,chosenBuilding.price);
                    floor.PlaceBuilding_DontCheck(position,chosenBuilding);
                }
                else
                {
                    buildingFailed?.Invoke();
                }
                break;
            case BuildMode.Bridge:
                if(!floor.PlaceBridge(position))
                {
                    buildingFailed?.Invoke();
                }
                break;
            case BuildMode.BridgeSpot:
                if(!floor.PlaceBridgeSpot(position))
                {
                    buildingFailed?.Invoke();
                }
                break;
        }
    }
    public void HoldBuild(Vector3 position){
        switch(mode){
            case BuildMode.Road:
                floor.PlaceRoad(position);
                break;
            case BuildMode.Bridge:
                floor.PlaceBridge(position);
                break;
            case BuildMode.BridgeSpot:
                floor.PlaceBridgeSpot(position);
                break;
        }
    }
    public void CancelBuildingAction(){
        ResetMode();
        temporalFloor.DeactivateFloor();
        cancelBuildingActionCallback?.Invoke();
    }
    public void FinishBuildingAction(){
        ResetMode();
        placeCallback?.Invoke();
        temporalFloor.DeactivateFloor();
    }
    public void ChooseMode(BuildMode m){
        mode = m;
        temporalFloor.ActivateFloor(m);
    }
    public void ResetMode(){
        mode = 0;
    }
    public void ChooseBuilding(Building b){
        mode = BuildMode.Building;
        chosenBuilding = b;
        temporalFloor.ActivateFloor(b);
    }
    public void ChooseGround(GroundArray g){
        mode = BuildMode.Ground;
        chosenGround = g;
        temporalFloor.ActivateFloor(g);
    }
    public void SetCancelCallback(Action callback){
        cancelBuildingActionCallback = callback;
    }
    public void AddCancelCallback(Action callback){
        cancelBuildingActionCallback += callback;
    }
    public void SetPlaceCallback(Action callback){
        placeCallback = callback;
    }
    public void AddPlaceCallback(Action callback){
        placeCallback += callback;
    }
    public bool CanBuild(Vector3 position){
        switch(mode){
            case BuildMode.Ground:
                if(!floor.CheckGA(position,chosenGround)){
                    return false;
                }
                break;
            case BuildMode.Road:
                if(!floor.CheckRoad(position)){
                    return false;
                }
                break;
            case BuildMode.Building:
                if(!floor.CheckBuilding(position,chosenBuilding.width, chosenBuilding.height)){
                    return false;
                }
                break;
            case BuildMode.Bridge:
                if(!floor.CheckBridge(position)){
                    return false;
                }
                break;
            case BuildMode.BridgeSpot:
                if(!floor.CheckBridgeSpot(position)){
                    return false;
                }
                break;
        }
        return true;
    }
}
public enum BuildMode{
    None,
    Road,
    Bridge,
    BridgeSpot,
    Building,
    Ground
}