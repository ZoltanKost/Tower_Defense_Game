using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GroundPiecesUIManager : MonoBehaviour{
    public delegate void AddArray(GroundUI ui, GroundArray g);
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GroundUI prefab;
    [SerializeField] private int groundMaxDimension = 4;
    Canvas canvas;
    public bool hided = false;
    float height;
    Tween currentTween;
    Vector3 downPosition;
    Vector3 upPosition;
    GroundArray chosen;
    void Start(){
        height = GetComponent<GridLayoutGroup>().cellSize.y;
        canvas = GetComponentInParent<Canvas>();
        downPosition = Vector3.down * Camera.main.orthographicSize;
        upPosition = downPosition + Vector3.up * height;
        for(int i = 0; i < 5; i++){
            AddGroundArray();
        }
    }

    public List<GroundUI> grounds_visuals;
    public void AddGroundArray(){
        GroundUI ui = Instantiate(prefab, transform);
        ui.Init(groundMaxDimension);
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
        Vector3 pos = canvas.transform.parent.position + downPosition;
        pos.z = 0;
        currentTween?.Complete();
        currentTween = canvas.transform.DOMove( pos,1f);
        hided = true;
    }
    public void Show(){
        currentTween?.Kill(false);
        Vector3 pos = canvas.transform.parent.position + upPosition;
        pos.z = 0;
        currentTween = canvas.transform.DOMove( pos,1f);
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