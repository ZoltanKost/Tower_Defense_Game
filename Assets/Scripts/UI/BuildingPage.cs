using System.Collections.Generic;
using UnityEngine;

public class BuildingPage : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
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
        playerBuildingManager.CancelBuildingAction();
        playerBuildingManager.ChooseBuilding(buildings[uiID]);
        DeactivateVisuals(uiID);
        playerBuildingManager.SetPlaceCallback(() => 
            {
                ActivateVisuals(uiID);
            }
        );
        playerBuildingManager.SetCancelCallback(() => ActivateVisuals(uiID));
    }
    public void DeactivateVisuals(int ID){

    }
    public void ActivateVisuals(int ID){

    }
}