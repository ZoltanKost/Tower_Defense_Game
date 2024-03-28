using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class BuildingUI : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager buildingManager;
    [SerializeField] private Building castle;
    [SerializeField] private Building tower;
    Vector3 rightPosition,leftPosition;
    void Awake(){
        float width = GetComponent<GridLayoutGroup>().cellSize.x;
        rightPosition = transform.localPosition + Vector3.right * width;
        leftPosition = transform.localPosition;
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
        HideUI();
    }
    public void ShowUI(){
        transform.DOLocalMove(leftPosition,1f);
    }
    public void HideUI(){
        transform.DOLocalMove(rightPosition,1f);
    }
}