using System.Collections.Generic;
using UnityEngine;

public class GroundPiecesUIManager : MonoBehaviour{
    public delegate void AddArray(GroundUI ui, GroundArray g);
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private GroundArrayGenerator groundArrayGenerator;
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
        CreateGroundArray(ui);
        ui.onClick += OnGroundUICallBack;
        grounds_visuals.Add(ui);
    }
    
    public void ResetGroundArrays(){
        foreach(var u in grounds_visuals){
            CreateGroundArray(u);
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
        playerBuildingManager.ChooseGround(uI.currentGA);
        uI.DeactivateVisuals();
        playerBuildingManager.SetPlaceCallback(uI.ActivateVisuals);
        playerBuildingManager.AddPlaceCallback(() => CreateGroundArray(uI));
        playerBuildingManager.SetCancelCallback(uI.ActivateVisuals);
    }
    public void CreateGroundArray(GroundUI uI){
        GroundArray ga = groundArrayGenerator.GenerateGA(maxDimensions, maxValue, random, randomReduce, trueCondition);
        uI.SetGroundArray(ga);
    }
}