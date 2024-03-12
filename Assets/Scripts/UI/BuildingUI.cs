using UnityEngine;
public class BuildingUI : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private Building castle;
    [SerializeField] private Building tower;
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
    public void DisableBuilding(){
        buildingManager.ChooseMode(0);
        HideUI();
    }
    public void ShowUI(){
        foreach(Transform c in transform){
            c.gameObject.SetActive(true);
        }
    }
    public void HideUI(){
        foreach(Transform c in transform){
            c.gameObject.SetActive(false);
        }
    }
}