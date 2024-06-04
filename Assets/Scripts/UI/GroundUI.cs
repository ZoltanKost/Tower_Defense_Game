using UnityEngine;

public class GroundUI : MonoBehaviour{
    public delegate void OnClick(GroundUI gUI);
    public OnClick onClick;
    public Floor floor;
    public GroundArray currentGA{get;private set;}
    void Awake(){
        floor.Init(0, $"GroundUI");
    }
    public void SetGroundArray(GroundArray ga){
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.targetFloor;
        foreach(Vector3Int g in ga.grounds){
            floor.CreateGround(g);
        }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.width)/2 + .5f, z = 0};
        floor.transform.localPosition = pos;
    }
    public void ActivateVisuals(){
        floor.gameObject.SetActive(true);
    }
    public void DeactivateVisuals(){
        floor.gameObject.SetActive(false);
    }
    public void OnGroundsChoosen(){
        onClick?.Invoke(this);
        DeactivateVisuals();
    }
}