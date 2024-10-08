using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour{
    public delegate void ClickCallback(Vector3 input);
    public delegate bool CheckCallback(Vector3 intput);
    private CheckCallback checkCallback;
    private ClickCallback clickCallback;
    private ClickCallback holdCallback;
    private Action cancelActionCallback;
    private Action resetAllGroundsCallback;
    private Action<Vector3> rmbCallback;
    protected Camera mCamera;
    protected TemporalFloor temporalFloor;
    protected EventSystem currentEventSystem;
    Vector3 camMovePosition = new();
    Vector3 fixedCameraPosition = new();

    bool active;
    public void Init(TemporalFloor tempFloor, Action resetAllGroundsCallback, Action cancelActionCallback, ClickCallback clickBuildCallback, ClickCallback holdCallback, CheckCallback checkCallback, Action<Vector3> rmbCallback){
        active = true;
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
        temporalFloor = tempFloor;
        this.resetAllGroundsCallback = resetAllGroundsCallback;
        clickCallback = clickBuildCallback;
        this.holdCallback = holdCallback;
        this.checkCallback = checkCallback;
        this.cancelActionCallback = cancelActionCallback;
        this.rmbCallback = rmbCallback;
    }
    public void Update(){
        if(active)Tick();
        if (Input.GetMouseButtonDown(1))
        {
            rmbCallback?.Invoke(mCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        if(Input.GetMouseButtonDown(2)){
            camMovePosition = Input.mousePosition;
            fixedCameraPosition = mCamera.transform.position;
        }else if(Input.GetMouseButton(2)){
            Vector3 pos = Input.mousePosition;
            Vector3 direction = camMovePosition - pos;
            mCamera.transform.position = fixedCameraPosition + direction * Time.fixedDeltaTime;
        }
    }
    public void Tick(){
        Vector3 input = mCamera.ScreenToWorldPoint(Input.mousePosition);
        bool canBuild = checkCallback.Invoke(input);
        temporalFloor.MoveTempFloor(input,canBuild);
        if (canBuild)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentEventSystem.IsPointerOverGameObject())
                {
                    return;
                }
                clickCallback?.Invoke(input);
            }
            else if (Input.GetMouseButton(0))
            {
                holdCallback?.Invoke(input);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            cancelActionCallback?.Invoke();
            resetAllGroundsCallback?.Invoke();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            cancelActionCallback?.Invoke();
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