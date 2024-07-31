using System.Collections.Generic;
using UnityEngine;

public class BuildingPage : MonoBehaviour {
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private BuildingButtonUI prefab;
    [SerializeField] private Building[] buildings;
    private List<BuildingButtonUI> buttons;
    public void Init(){
        buttons = new List<BuildingButtonUI>();
        for(int i = 0; i < buildings.Length; i++){
            BuildingButtonUI ui = Instantiate(prefab,transform);
            ui.Init(OnGroundChosenCallBack,buildings[i].sprite, i);
            buttons.Add(ui);
        }
    }
    public void OnGroundChosenCallBack(int uiID){
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseBuilding(buildings[uiID]);
        DeactivateVisuals(uiID);
        playerActionManager.SetPlaceCallback(() => 
            {
                ActivateVisuals(uiID);
            }
        );
        playerActionManager.SetCancelCallback(() => ActivateVisuals(uiID));
    }
    public void DeactivateVisuals(int ID){

    }
    public void ActivateVisuals(int ID){

    }
}