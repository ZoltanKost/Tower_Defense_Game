using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputManager : MonoBehaviour{
    public delegate void CancelCallback();
    public delegate bool BuildCallback(Vector3 input);
    public delegate void ResetCallback();
    public delegate void PlaceCallback();
    ResetCallback resetCallback;
    BuildCallback buildCallback;
    CancelCallback cancelCallback;
    PlaceCallback placeCallback;
    Camera mCamera;
    TemporalFloor temporalFloor;
    EventSystem currentEventSystem;
    Vector3 camMovePosition = new();
    Vector3 fixedCameraPosition = new();
    public void Init(TemporalFloor tempFloor, ResetCallback resetCallback, BuildCallback buildCallback){
        mCamera = Camera.main;
        currentEventSystem = FindObjectOfType<EventSystem>();
        temporalFloor = tempFloor;
        this.resetCallback = resetCallback;
        resetCallback += tempFloor.DeactivateFloor;
        this.buildCallback = buildCallback;
    }
    public void Update(){
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
        }else if(Input.GetMouseButtonDown(2)){
            camMovePosition = Input.mousePosition;
            fixedCameraPosition = mCamera.transform.position;
        }else if(Input.GetMouseButton(2)){
            Vector3 pos = Input.mousePosition;
            mCamera.transform.position = fixedCameraPosition + (camMovePosition - pos) * Time.fixedDeltaTime;
        }else if(Input.GetKeyDown(KeyCode.Space)){
            cancelCallback?.Invoke();
            cancelCallback = null;
            resetCallback?.Invoke();
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            cancelCallback?.Invoke();
        }
    }
    public void SetCancelCallback(CancelCallback callback){
        cancelCallback = callback;
        cancelCallback += temporalFloor.DeactivateFloor;
    }
    public void AddCancelCallback(CancelCallback callback){
        cancelCallback += callback;
    }
    public void InvokeCancelCallback(){
        cancelCallback?.Invoke();
    }
    public void SetPlaceCallback(PlaceCallback callback){
        placeCallback = callback;
        placeCallback += temporalFloor.DeactivateFloor;
    }
    public void AddPlaceCallback(PlaceCallback callback){
        placeCallback += callback;
    }
    public void ActivateTempFloor(GroundArray ga){
        temporalFloor.ActivateFloor(ga);
    }
}