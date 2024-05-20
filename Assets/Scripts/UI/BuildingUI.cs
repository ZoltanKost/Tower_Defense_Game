using UnityEngine;
public class BuildingUI : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private Building castle;
    [SerializeField] private Building tower;
    HideShowUI hideShowUI;
    void Awake(){
        hideShowUI = GetComponent<HideShowUI>();
    }
    public void ChooseCastleBuilding(){
        buildingManager.ChooseBuilding(castle);
    }
    public void ChooseTowerBuilding(){
        buildingManager.ChooseBuilding(tower);
    }
    public void ChooseRoad(){
        buildingManager.ChooseMode(BuildMode.Road);
    }
    public void ChooseBridge(){
        buildingManager.ChooseMode(BuildMode.Bridge);
    }
    public void ChooseBridgeSpot(){
        buildingManager.ChooseMode(BuildMode.BridgeSpot);
    }
    public void DisableBuilding(){
        buildingManager.ChooseMode(0);
        hideShowUI.HideUI();
    }
    public void ShowUI(){
        hideShowUI.ShowUI();
    }
    public void HideUI(){
        hideShowUI.HideUI();
    }
}