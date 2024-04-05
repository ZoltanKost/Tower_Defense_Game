using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GroundPiecesUIManager : MonoBehaviour{
    public delegate void AddArray(GroundUI ui, GroundArray g);
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GroundUI prefab;
    [SerializeField] private int maxDimensions, maxSeed, maxValue, random, trueCondition;
    [SerializeField] private float randomReduce;
    public bool hided = false;
    Tween currentTween;
    Vector3 downPosition;
    Vector3 upPosition;
    void Start(){
        float height = GetComponent<GridLayoutGroup>().cellSize.y;
        downPosition = transform.localPosition + Vector3.down * height;
        upPosition = transform.localPosition;
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
        currentTween?.Kill(false);
        currentTween = transform.DOLocalMove( downPosition,1f);
        hided = true;
    }
    public void Show(){
        currentTween?.Kill(false);
        currentTween = transform.DOLocalMove(upPosition,1f);
        hided = false;
    }
    void OnGroundUICallBack(GroundUI uI){
        // 1. Call PlayerInput cancel callback
        playerInputManager.InvokeCancelCallback();
        // 2. Set a ground at building Manager
        playerBuildingManager.ChooseGround(uI.currentGA);
        // 3. Activate TempVisual
        playerInputManager.ActivateTempFloor(uI.currentGA);
        // 4. Deactivate ButtonVisual;
        uI.DeactivateVisuals();
        // 5. Set Cancel and Place PlayerInput callback.
        playerInputManager.SetPlaceCallback(uI.ActivateVisuals);
        playerInputManager.AddPlaceCallback(uI.CreateGroundArray);
        // playerInputManager.AddPlaceCallback(tempFloor.Clear);
        playerInputManager.SetCancelCallback(uI.ActivateVisuals);
        // playerInputManager.AddCancelCallback(tempFloor.Clear);
    }
}