using System.Collections.Generic;
using UnityEngine;

public class GroundPiecesUIManager : MonoBehaviour{
    public delegate void AddArray(GroundUI ui, GroundArray g);
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private GroundUI prefab;
    [SerializeField] private int maxDimensions, maxSeed, maxValue, random, trueCondition;
    [SerializeField] private float randomReduce;
    HideShowUI hideShowUI;
    void Start(){
        hideShowUI = GetComponent<HideShowUI>();
        for(int i = 0; i < 5; i++){
            AddGroundArray();
        }
    }

    public List<GroundUI> grounds_visuals;
    public void AddGroundArray(){
        GroundUI ui = Instantiate(prefab, transform);
        ui.Init(maxDimensions, maxSeed, maxValue, random,  randomReduce, trueCondition);
        ui.CreateGroundArray();
        ui.onClick += OnGroundUICallBack;
        grounds_visuals.Add(ui);
    }
    
    public void Reset(){
        foreach(var u in grounds_visuals){
            u.CreateGroundArray();
        }
    }
    public void Hide(){
        hideShowUI.HideUI();
    }
    public void Show(){
        hideShowUI.ShowUI();
    }
    void OnGroundUICallBack(GroundUI uI){
        playerBuildingManager.CancelBuildingAction();
        // 2. Set a ground at building Manager
        playerBuildingManager.ChooseGround(uI.currentGA);
        // 4. Deactivate ButtonVisual;
        uI.DeactivateVisuals();
        // 5. Set Cancel and Place PlayerInput callback.
        playerBuildingManager.SetPlaceCallback(uI.ActivateVisuals);
        playerBuildingManager.AddPlaceCallback(uI.CreateGroundArray);
        playerBuildingManager.SetCancelCallback(uI.ActivateVisuals);
    }
}