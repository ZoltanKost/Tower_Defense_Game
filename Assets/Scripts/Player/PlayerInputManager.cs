using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour{
    public delegate void ClickCallback(Vector3 input);
    public delegate bool CheckCallback(Vector3 intput);
    private CheckCallback checkCallback;
    private ClickCallback clickCallback;
    private ClickCallback holdCallback;
    private Action cancelBuildingActionCallback;
    private Action resetAllGroundsCallback;
    bool active;
    protected Camera mCamera;
    protected TemporalFloor temporalFloor;
    protected EventSystem currentEventSystem;
    Vector3 camMovePosition = new();
    Vector3 fixedCameraPosition = new();
    public void Init(TemporalFloor tempFloor, Action resetAllGroundsCallback, Action cancelBuildingActionCallback, ClickCallback clickBuildCallback, ClickCallback holdCallback, CheckCallback checkCallback){
        active = true;
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
        temporalFloor = tempFloor;
        this.resetAllGroundsCallback = resetAllGroundsCallback;
        clickCallback = clickBuildCallback;
        this.holdCallback = holdCallback;
        this.checkCallback = checkCallback;
        this.cancelBuildingActionCallback = cancelBuildingActionCallback;
    }
    public void Update(){
        if(active) Tick();
        if(Input.GetMouseButtonDown(2)){
            camMovePosition = Input.mousePosition;
            fixedCameraPosition = mCamera.transform.position;
        }else if(Input.GetMouseButton(2)){
            Vector3 pos = Input.mousePosition;
            Vector3 direction = camMovePosition - pos;
            mCamera.transform.position = fixedCameraPosition + direction * Time.fixedDeltaTime;
        }
    }
    public virtual void Tick(){
        Vector3 input = mCamera.ScreenToWorldPoint(Input.mousePosition);
        bool canBuild = checkCallback.Invoke(input);
        temporalFloor.MoveTempFloor(input,canBuild);
        if(Input.GetMouseButtonDown(0)){
            if(currentEventSystem.IsPointerOverGameObject()){ 
                return;
            }
            clickCallback?.Invoke(input);
        }else if(Input.GetMouseButton(0)){
            holdCallback?.Invoke(input);
        }else if(Input.GetKeyDown(KeyCode.Space)){
            cancelBuildingActionCallback?.Invoke();
            resetAllGroundsCallback?.Invoke();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            cancelBuildingActionCallback?.Invoke();
        }
    }
    public void Deactivate(){
        active = false;
    }
    public void Activate(){
        active = true;
    }
    public IEnumerator WaitForActivate(float time){
        yield return new WaitForSeconds(time);
        Activate();
        Debug.Log("Activated");
    }
}