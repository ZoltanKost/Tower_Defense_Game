using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadPage : MonoBehaviour {
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private GroundUI prefab;
    [SerializeField] private TileBase[] roads;
    private List<GroundUI> buttons;
    public void Init(){
        buttons = new List<GroundUI>();
        foreach(TileBase road in roads){
            GroundUI ui = Instantiate(prefab,transform);
            ui.Init(buttons.Count, OnGroundChosenCallBack,1,1);
            ui.SetTile(road);
            buttons.Add(ui);
        }
    }
    public void OnGroundChosenCallBack(int uiID){
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseMode((ActionMode)uiID+1);
        buttons[uiID].Highlight();
        //DeactivateVisuals(uiID);
        /*playerActionManager.SetPlaceCallback(() => 
            {
                ActivateVisuals(uiID);
            }
        );*/
        playerActionManager.SetCancelCallback(() => buttons[uiID].Dehighlight());
    }
    void ActivateVisuals(int id){
        buttons[id].ActivateVisuals();
    }
    void DeactivateVisuals(int id){
        buttons[id].DeactivateVisuals();
    }

}