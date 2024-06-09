using UnityEngine;
using UnityEngine.Tilemaps;

public class Shop : MonoBehaviour {
    [SerializeField] private GroundPageModel groundPageModel;
    [SerializeField] private BuildingPage buildingPage;
    [SerializeField] private RoadPage roadPage;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Building[] _buildings;
    public Building[] buildings{get => _buildings;}
    
    [SerializeField] private Tile[] _roads;
    public Tile[] roads{get => _roads;}
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private ShopUI ShopUI;
    public void Init(int groundsCount){
        groundPageModel.Init(groundsCount);
        buildingPage.Init();
        shopUI.Init();
        roadPage.Init();
    }
    public void ChooseRoad(int ID){
        playerBuildingManager.ChooseMode((BuildMode)ID+1);
    }
    public void DisableBuilding(){
        playerBuildingManager.ChooseMode(0);
    }
    public void ChooseGround(int ID){
        playerBuildingManager.ChooseGround(groundPageModel.groundArrays[ID]);
    }
    public void ResetGroundArrays(){
        groundPageModel.ResetGroundArrays();
    }
}