using UnityEngine;

public class GroundUI : MonoBehaviour{
    public delegate void OnClick(GroundUI gUI);
    public OnClick onClick;
    public Floor floor;
    public GroundArray currentGA{get;private set;}
    private int groundMaxDimension;
    void Awake(){
        floor.Init(0,GetComponentInParent<Canvas>().sortingOrder + 1);
    }
    public void Init(int groundMaxDimension){
        this.groundMaxDimension = groundMaxDimension;
    }
    public void SetGroundArray(GroundArray ga){
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.layer;
        foreach(var g in ga.grounds){
            floor.CreateGroundArray(g.position, g.width, g.height);
        }
        if(!(ga.layer == 0))
            foreach(Vector3Int road in ga.roads){
                if(!floor.HasTile(road)){
                    floor.PlaceBridge(road);
                }else{
                    floor.PlaceRoad(road);
                }
            }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.height)/2 + .5f, z = 0};
        floor.transform.localPosition = pos;
    }
    public void CreateGroundArray(){
        GroundArray ga = new GroundArray(groundMaxDimension);
        SetGroundArray(ga);
    }
    public void ClearAllTiles(){
        floor.ClearAllTiles();
    }
    public GroundArray GetGroundArray(){
        return currentGA;
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