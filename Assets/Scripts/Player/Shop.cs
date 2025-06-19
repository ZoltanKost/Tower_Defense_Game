using UnityEngine;

public delegate bool CanBuy(Resource resource, int count);
public class Shop : MonoBehaviour {
    [SerializeField] private GroundPageModel groundPageModel;
    [SerializeField] private BuildingPage buildingPage;
    [SerializeField] private RoadPage roadPage;
    [SerializeField] private MagicPageModel magicPageModel;
    [SerializeField] private PlayerInventoryModel playerInventoryModel;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private PlayerResourceManager playerResourceManager;
    [SerializeField] private float GroundBuyCooldown = 5f;
    bool shown;
    public void Init(int groundsCount, int spellCount = 6){
        groundPageModel.Init(groundsCount, GroundBuyCooldown);
        magicPageModel.Init(spellCount);
        playerInventoryModel.Init();
        buildingPage.Init();
        shopUI.Init();
        roadPage.Init();
    }
    public void ResetGroundArrays(){
        groundPageModel.ResetGroundArrays();
        magicPageModel.ResetSpells();
        //playerInventoryModel.ResetInventory();
    }
    public void Update()
    {
        float dt = Time.deltaTime;
        groundPageModel.Tick(dt);
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