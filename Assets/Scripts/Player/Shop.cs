using UnityEngine;

public delegate bool CanBuy(Resource resource, int count);
public class Shop : MonoBehaviour {
    [SerializeField] private GroundPageModel groundPageModel;
    [SerializeField] private BuildingPage buildingPage;
    [SerializeField] private RoadPage roadPage;
    [SerializeField] private MagicPage magicPage;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    bool shown;
    public void Init(int groundsCount){
        groundPageModel.Init(groundsCount);
        buildingPage.Init();
        shopUI.Init();
        roadPage.Init(); 
        magicPage.Init();
    }
    public void ResetGroundArrays(){
        groundPageModel.ResetGroundArrays();
    }
    public void Show(){
        if(shown) return;
        shopUI.ShowUI();
        shown = true;
    }
    public void Hide(){
        if(!shown) return;
        shopUI.HideUI();
        shown = false;
    }
}