using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerInputManager : MonoBehaviour{
    [SerializeField] private PlayerActionManager actionManager;
    [SerializeField] private float MinZoom, MaxZoom;
    [SerializeField] private float ZoomSpeed = .5f;
    [SerializeField] private float ZoomValuePerInput = 50f;
    public delegate void ClickCallback(Vector3 input);
    public delegate bool CheckCallback(Vector3 intput);
    Tween cameraTween;
    private Action cancelActionCallback;
    private Action resetAllGroundsCallback;
    protected Camera mCamera;
    protected TemporalFloor temporalFloor;
    protected EventSystem currentEventSystem;
    Vector3 lastCameraPosition = new();

    bool active;
    public void Init(TemporalFloor tempFloor, Action resetAllGroundsCallback, Action cancelActionCallback){
        active = true;
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
        temporalFloor = tempFloor;
        this.resetAllGroundsCallback = resetAllGroundsCallback;
        this.cancelActionCallback = cancelActionCallback;
    }
    public void Update(){
        if(active)Tick();
        if (Input.GetMouseButtonDown(1))
        {
            actionManager.ResetMode();

            //actionManager.HighlightedAction(mCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        if(Input.GetMouseButtonDown(2)){
            lastCameraPosition = Input.mousePosition;
        }else if(Input.GetMouseButton(2)){
            Vector3 temp = Input.mousePosition;
            Vector3 deltaCameraPosition = lastCameraPosition - temp;
            float Ysize = mCamera.orthographicSize * 2;
            deltaCameraPosition.y /= mCamera.pixelHeight / (Ysize);
            deltaCameraPosition.x /= mCamera.pixelWidth / (Ysize * mCamera.aspect);
            lastCameraPosition = temp;
            mCamera.transform.position += deltaCameraPosition;
        }
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d == 0) return;
        float cameraZoom = mCamera.orthographicSize - d * ZoomValuePerInput;
        cameraTween.Kill();
        if (cameraZoom <= MinZoom)
        {
            mCamera.DOOrthoSize(MinZoom, ZoomSpeed);
        }else if (cameraZoom >= MaxZoom)
        {
            mCamera.DOOrthoSize(MaxZoom, ZoomSpeed);
        }
        else
        {
            mCamera.DOOrthoSize(cameraZoom, ZoomSpeed);
        }
    }
    public void Tick(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cancelActionCallback?.Invoke();
            resetAllGroundsCallback?.Invoke();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            cancelActionCallback?.Invoke();
            return;
        }
        Vector3 input = mCamera.ScreenToWorldPoint(Input.mousePosition);
        bool canBuild = actionManager.CanBuild(input);
        temporalFloor.MoveTempFloor(input,canBuild);
        bool overUI = currentEventSystem.IsPointerOverGameObject();
        if (canBuild)
        {
            if (Input.GetMouseButtonDown(0) && !overUI)
            {
                actionManager.ClickBuild(input);
            }
            else if (Input.GetMouseButton(0))
            {
                actionManager.HoldBuild(input);
            }else if (Input.GetMouseButtonUp(0) && !overUI)
            {
                actionManager.UpBuild(input);
            }
        }
        
    }
    public void Activate()
    {
        active = true;
    }
    public void Deactivate()
    {
        active = false;
    }
}