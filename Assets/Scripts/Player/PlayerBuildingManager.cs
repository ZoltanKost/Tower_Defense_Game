using System;
using UnityEngine;
public class PlayerBuildingManager : MonoBehaviour{
    [SerializeField] private FloorManager floor;
    [SerializeField] private TemporalFloor temporalFloor;
    Action buildingFailedCallback;
    Action cancelBuildingActionCallback;
    Action placeCallback;
    Action<int,int> highlightBuildingCallback;
    Action<Resource, int> buyCallback;
    Func<Resource,int, bool> canBuyCallBack;
    GroundArray chosenGround;
    Building chosenBuilding;
    BuildMode mode;
    public void Init(Action buildingFailedCb, TemporalFloor tp, Func<Resource, int, bool> canBuyCb, Action<Resource, int> buyCb, Action<int,int> highlightBuildingCb){
        buildingFailedCallback = buildingFailedCb;
        temporalFloor = tp;
        canBuyCallBack = canBuyCb;
        buyCallback = buyCb;
        highlightBuildingCallback = highlightBuildingCb;
    }
    // Returns True if chosen object can be built once per click
    public void ClickBuild(Vector3 position){
        switch(mode){
            case BuildMode.Ground:
                if(floor.CheckGA(position,chosenGround) && 
                canBuyCallBack(Resource.Gold,chosenGround.price))
                {
                    mode = 0;
                    floor.CreateGroundArray_DontCheck(position,chosenGround);
                    buyCallback?.Invoke(Resource.Gold,chosenGround.price);
                    FinishBuildingAction();
                }
                else
                {
                    buildingFailedCallback?.Invoke();
                }
                break;
            case BuildMode.Road:
                if(!floor.PlaceRoad(position))
                {
                    buildingFailedCallback?.Invoke();
                }
                break;
            case BuildMode.Building:
                if(floor.CheckBuilding(position,chosenBuilding.width, chosenBuilding.height) 
                   && canBuyCallBack(chosenBuilding.resource,chosenBuilding.price))
                {
                    buyCallback(Resource.Gold,chosenBuilding.price);
                    floor.PlaceBuilding_DontCheck(position,chosenBuilding);
                }
                else
                {
                    buildingFailedCallback?.Invoke();
                }
                break;
            case BuildMode.Bridge:
                if(!floor.PlaceBridge(position))
                {
                    buildingFailedCallback?.Invoke();
                }
                break;
            case BuildMode.BridgeSpot:
                if(!floor.PlaceBridgeSpot(position))
                {
                    buildingFailedCallback?.Invoke();
                }
                break;
            case BuildMode.None:
                if(floor.HasBuilding(position,out int X, out int Y)) highlightBuildingCallback?.Invoke(X,Y);
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
    Ground,
}