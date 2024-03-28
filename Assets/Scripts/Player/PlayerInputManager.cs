using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour{
    public delegate void Callback();
    public delegate bool BuildCallback(Vector3 input);
    Callback resetCallback;
    BuildCallback buildCallback;
    Callback cancelCallback;
    Callback placeCallback;
    Callback stopLevelCallback;
    bool active;
    Camera mCamera;
    TemporalFloor temporalFloor;
    EventSystem currentEventSystem;
    Vector3 camMovePosition = new();
    Vector3 fixedCameraPosition = new();
    public void Init(TemporalFloor tempFloor, Callback resetCallback, BuildCallback buildCallback, Callback stopLevelCallback){
        active = true;
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
        temporalFloor = tempFloor;
        this.resetCallback = resetCallback;
        resetCallback += tempFloor.DeactivateFloor;
        this.buildCallback = buildCallback;
        this.stopLevelCallback = stopLevelCallback;
    }
    public void Update(){
        DebugActionQueue.Update();
        if(active) Tick();
        else if(Input.GetKeyDown(KeyCode.Escape)){
            stopLevelCallback?.Invoke();
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
        temporalFloor.MoveTempFloor(input);
        if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)){
            if(currentEventSystem.IsPointerOverGameObject()){ 
               // currentEventSystem.currentSelectedGameObject.GetComponentInChildren<Floor>().Animate();
                return;
            }
            if(buildCallback.Invoke(input)){
                placeCallback?.Invoke();
            }
        }else if(Input.GetKeyDown(KeyCode.Space)){
            cancelCallback?.Invoke();
            cancelCallback = null;
            resetCallback?.Invoke();
            Vector3 mid = input + Vector3.up * 5 + Vector3.right * 2.5f;
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            cancelCallback?.Invoke();
        }
    }
    public void Deactivate(){
        active = false;
    }
    public void Activate(){
        active = true;
    }
    public void SetCancelCallback(Callback callback){
        cancelCallback = callback;
        cancelCallback += temporalFloor.DeactivateFloor;
    }
    public void AddCancelCallback(Callback callback){
        cancelCallback += callback;
    }
    public void InvokeCancelCallback(){
        cancelCallback?.Invoke();
    }
    public void SetPlaceCallback(Callback callback){
        placeCallback = callback;
        placeCallback += temporalFloor.DeactivateFloor;
    }
    public void AddPlaceCallback(Callback callback){
        placeCallback += callback;
    }
    public void ActivateTempFloor(GroundArray ga){
        temporalFloor.ActivateFloor(ga);
    }
}