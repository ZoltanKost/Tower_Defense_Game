using System;
using UnityEngine;
public class PlayerActionManager : MonoBehaviour{
    const byte BUILDING_STATE = 0;
    const byte MAGIC_STATE = 2;
    [SerializeField] private FloorManager floorManager;
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
    SpellSO chosenSpell;
    [SerializeField] ActionMode mode;
    byte gameState_int;
    Vector3 startPosition;
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
                    floorManager.CreateGroundArray_DontCheck(position,chosenGround);
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
                if(!floorManager.PlaceRoad(position))
                {
                    actionFailedCallback?.Invoke();
                    return;
                }
                temporalFloor.JellyAnimation();
            break;
            case ActionMode.Building:
                if(canBuyCallBack(chosenBuilding.resource,chosenBuilding.price))
                {
                    buyCallback(Resource.Gold,chosenBuilding.price);
                    floorManager.PlaceBuilding_DontCheck(position,chosenBuilding);
                }
                else
                {
                    actionFailedCallback?.Invoke();
                }
            break;
            case ActionMode.Bridge:
                if (!floorManager.PlaceBridge_DontCheck(position))
                {
                    actionFailedCallback?.Invoke();
                }
                break;
            case ActionMode.BridgeSpot:
                if (!floorManager.PlaceBridge_DontCheck(position))
                {
                    actionFailedCallback?.Invoke();
                }
                break;
            case ActionMode.None:
                highlighter.TryHighlight(position);
            break;
            case ActionMode.DestroyGround:
                startPosition = position;
                //floorManager.DestroyGround(position);
                break;
            case ActionMode.DestroyBuilding:
                if(floorManager.HasBuilding(position, out int ID)) destroyBuildingCallback?.Invoke(ID);
            break;
            case ActionMode.DestroyRoad:
                if(floorManager.HasRoad(position,out Vector3Int pos)) floorManager.DestroyRoad(pos);
            break;
            case ActionMode.MassGround:
                startPosition = position;
                temporalFloor.StartFlood(position);
                break;
        }
    }
    public void HoldBuild(Vector3 position){
        switch(mode){
            case ActionMode.Road:
                if (floorManager.PlaceRoad(position))
                {
                    temporalFloor.JellyAnimation();
                }
                break;
            case ActionMode.Bridge:
                if (!floorManager.PlaceBridge_DontCheck(position))
                {
                    actionFailedCallback?.Invoke();
                }
                break;
            case ActionMode.BridgeSpot:
                if (!floorManager.PlaceBridge_DontCheck(position))
                {
                    actionFailedCallback?.Invoke();
                }
                break;
            case ActionMode.DestroyGround:
                //floorManager.DestroyGround(position);
                break;
            case ActionMode.DestroyRoad:
                if (floorManager.HasRoad(position, out Vector3Int pos)) 
                { 
                    floorManager.DestroyRoad(pos);
                    temporalFloor.JellyAnimation();
                }
            break;
            case ActionMode.MassGround:
                // mass building
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
    public void ChooseSpell(SpellSO b)
    {
        mode = ActionMode.CastSpell;
        chosenSpell = b;
        temporalFloor.ActivateFloor(b.spellData);
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
    public void ChooseMassGroundMode()
    {
        mode = ActionMode.MassGround;
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
    public void UpBuild(Vector3 position)
    {
        switch (mode)
        {
            case ActionMode.MassGround:
                floorManager.FloodFloor(startPosition,position);
                break;
            case ActionMode.DestroyGround:
                Debug.Log("destroyGround");
                floorManager.MassDestroyGround(startPosition, position);
                break;
        }
    }
    public bool CanBuild(Vector3 position){
        if (gameState_int % 2 != 0 || gameState_int == 4) return false;
        switch(mode){
            case ActionMode.Ground:
                return gameState_int == BUILDING_STATE && floorManager.CheckGA(position, chosenGround);
            case ActionMode.CastSpell:
                // mana Check
                return gameState_int == MAGIC_STATE;
            case ActionMode.Road:
                return gameState_int == BUILDING_STATE && floorManager.CheckRoad(position);
            case ActionMode.Building:
                return gameState_int == BUILDING_STATE && floorManager.CheckBuilding(position, chosenBuilding.width, chosenBuilding.height);
            case ActionMode.Bridge:
                return gameState_int == BUILDING_STATE && floorManager.CheckBridgeOrBridgeSpot(position);
            case ActionMode.BridgeSpot:
                return gameState_int == BUILDING_STATE && floorManager.CheckBridgeOrBridgeSpot(position);
            case ActionMode.MassGround:
                // mass building
                break;
        }
        return true;
    }
}
[Serializable]
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
    MassGround,
    CastSpell,
    Command
}