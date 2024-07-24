using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadPage : MonoBehaviour {
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private GroundUI prefab;
    [SerializeField] private TileBase[] roads;
    private List<GroundUI> buttons;
    public void Init(){
        buttons = new List<GroundUI>();
        foreach(TileBase road in roads){
            GroundUI ui = Instantiate(prefab,transform);
            ui.Init(buttons.Count, OnGroundChosenCallBack);
            ui.SetTile(road);
            buttons.Add(ui);
        }
    }
    public void OnGroundChosenCallBack(int uiID){
        playerBuildingManager.CancelBuildingAction();
        playerBuildingManager.ChooseMode((BuildMode)uiID+1);
        DeactivateVisuals(uiID);
        playerBuildingManager.SetPlaceCallback(() => 
            {
                ActivateVisuals(uiID);
            }
        );
        playerBuildingManager.SetCancelCallback(() => ActivateVisuals(uiID));
    }
    void ActivateVisuals(int id){

    }
    void DeactivateVisuals(int id){

    }

}