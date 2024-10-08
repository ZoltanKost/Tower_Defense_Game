using System;
using UnityEngine;
public class PlayerActionManager : MonoBehaviour{
    const byte BUILDING_STATE = 0;
    const byte MAGIC_STATE = 2;
    [SerializeField] private FloorManager floor;
    [SerializeField] private TemporalFloor temporalFloor;
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private EntitiyHighlighter highlighter;
    Action actionFailedCallback;
    Action cancelActionCallback;
    Action placeCallback;
    Action<int> highlightBuildingCallback;
    Action<Resource, int> buyCallback;
    Func<Resource,int, bool> canBuyCallBack;
    Action<int> destroyBuildingCallback;
    GroundArray chosenGround;
    Building chosenBuilding;
    SpellData chosenSpell;
    ActionMode mode;
    byte gameState_int;
    public void Init(Action buildingFailedCb, TemporalFloor tp, Func<Resource, int, bool> canBuyCb, Action<Resource, int> buyCb, Action<int> highlightBuildingCb, Action<int> destroyBuildingCb){
        actionFailedCallback = buildingFailedCb;
        temporalFloor = tp;
        canBuyCallBack = canBuyCb;
        buyCallback = buyCb;
        highlightBuildingCallback = highlightBuildingCb;
        destroyBuildingCallback = destroyBuildingCb;
    }
    public void Switch(GameState state)
    {
        gameState_int = (byte)state;
    }
    public void HighlightedAction(Vector3 position)
    {
        highlighter.HighlighterCallback(position);
    }
    public void ClickBuild(Vector3 position){
        switch(mode){
            case ActionMode.Ground:
                if(canBuyCallBack(Resource.Gold,chosenGround.price))
                {
                    mode = 0;
                    floor.CreateGroundArray_DontCheck(position,chosenGround);
                    buyCallback?.Invoke(Resource.Gold,chosenGround.price);
                    FinishBuildingAction();
                }
                else
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.CastSpell:
                //mode = 0;
                // ManaCallback?.Invoke(Resource.Mana, chosenSpell.goldCost);
                spellManager.CastSpell(chosenSpell, position);
                FinishBuildingAction();
                break;
            case ActionMode.Road:
                if(!floor.PlaceRoad(position))
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.Building:
                if(canBuyCallBack(chosenBuilding.resource,chosenBuilding.price))
                {
                    buyCallback(Resource.Gold,chosenBuilding.price);
                    floor.PlaceBuilding_DontCheck(position,chosenBuilding);
                }
                else
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.Bridge:
                if(!floor.PlaceBridge(position))
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.BridgeSpot:
                if(!floor.PlaceBridgeSpot(position))
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.None:
                highlighter.TryHighlight(position);
            break;
            case ActionMode.DestroyBuilding:
                if(floor.HasBuilding(position, out int ID)) destroyBuildingCallback?.Invoke(ID);
            break;
            case ActionMode.DestroyGround:
                floor.DestroyGround(position);
            break;
            case ActionMode.DestroyRoad:
                if(floor.HasRoad(position,out Vector3Int pos)) floor.DestroyRoad(pos);
            break;
        }
    }
    public void HoldBuild(Vector3 position){
        switch(mode){
            case ActionMode.Road:
                floor.PlaceRoad(position);
                break;
            case ActionMode.Bridge:
                floor.PlaceBridge(position);
                break;
            case ActionMode.BridgeSpot:
                floor.PlaceBridgeSpot(position);
                break;
            case ActionMode.DestroyRoad:
                if(floor.HasRoad(position,out Vector3Int pos)) floor.DestroyRoad(pos);
            break;
        }
    }
    public void CancelBuildingAction(){
        ResetMode();
        temporalFloor.DeactivateFloor();
        cancelActionCallback?.Invoke();
    }
    public void FinishBuildingAction(){
        ResetMode();
        placeCallback?.Invoke();
        temporalFloor.DeactivateFloor();
    }
    public void ChooseMode(ActionMode m){
        mode = m;
        temporalFloor.ActivateFloor(m);
    }
    public void ResetMode(){
        mode = 0;
    }
    public void ChooseBuilding(Building b){
        mode = ActionMode.Building;
        chosenBuilding = b;
        temporalFloor.ActivateFloor(b);
    }
    public void ChooseSpell(SpellData b)
    {
        mode = ActionMode.CastSpell;
        chosenSpell = b;
    }
    public void ChooseGround(GroundArray g){
        mode = ActionMode.Ground;
        chosenGround = g;
        temporalFloor.ActivateFloor(g);
    }
    public void ChooseDestroyBuildingMode(){
        mode = ActionMode.DestroyBuilding;
        temporalFloor.ActivateFloor(ActionMode.DestroyBuilding);
    }
    public void ChooseDestroyGroundMode(){
        mode = ActionMode.DestroyGround;
        temporalFloor.ActivateFloor(ActionMode.DestroyBuilding);
    }
    public void ChooseDestroyRoadMode(){
        mode = ActionMode.DestroyRoad;
        temporalFloor.ActivateFloor(ActionMode.DestroyBuilding);
    }
    public void SetCancelCallback(Action callback){
        cancelActionCallback = callback;
    }
    public void ClearCancelCallback()
    {
        cancelActionCallback = null;
    }
    public void AddCancelCallback(Action callback){
        cancelActionCallback += callback;
    }
    public void SetPlaceCallback(Action callback){
        placeCallback = callback;
    }
    public bool CanBuild(Vector3 position){
        if (gameState_int % 2 != 0 || gameState_int == 4) return false;
        switch(mode){
            case ActionMode.Ground:
                return gameState_int == BUILDING_STATE && floor.CheckGA(position, chosenGround);
            case ActionMode.CastSpell:
                // mana Check
                return gameState_int == MAGIC_STATE;
            case ActionMode.Road:
                return gameState_int == BUILDING_STATE && floor.CheckRoad(position);
            case ActionMode.Building:
                return gameState_int == BUILDING_STATE && floor.CheckBuilding(position, chosenBuilding.width, chosenBuilding.height);
            case ActionMode.Bridge:
                return gameState_int == BUILDING_STATE && floor.CheckBridge(position);
            case ActionMode.BridgeSpot:
                return gameState_int == BUILDING_STATE && floor.CheckBridgeSpot(position);
        }
        return true;
    }
}
public enum ActionMode{
    None,
    Road,
    Bridge,
    BridgeSpot,
    Building,
    Ground,
    DestroyBuilding,
    DestroyRoad,
    DestroyGround,
    CastSpell,
    Command
}