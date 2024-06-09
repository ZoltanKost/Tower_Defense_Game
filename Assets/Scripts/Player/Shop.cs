using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] private GroundPageModel groundPageModel;
    [SerializeField] private BuildingPage buildingPage;
    [SerializeField] private RoadPage roadPage;
    [SerializeField] private ShopUI shopUI;
    public void Init(int groundsCount){
        groundPageModel.Init(groundsCount);
        buildingPage.Init();
        shopUI.Init();
        roadPage.Init();
    }
    public void ResetGroundArrays(){
        groundPageModel.ResetGroundArrays();
    }
    public void Show(){
        shopUI.ShowUI();
    }
    public void Hide(){
        shopUI.HideUI();
    }
}