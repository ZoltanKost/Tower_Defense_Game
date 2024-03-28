using UnityEngine;

public class GroundUI : MonoBehaviour{
    public delegate void OnClick(GroundUI gUI);
    public OnClick onClick;
    public Floor floor;
    public GroundArray currentGA{get;private set;}
    private int groundMaxDimension, groundMinDimensions;
    private int groundMaxPieces, groundMinPieces;
    void Awake(){
        floor.Init(0, $"GroundUI");
    }
    public void Init(int groundMaxPieces, int groundMaxDimension,int groundMinPieces,int groundMinDimensions){
        this.groundMaxPieces = groundMaxPieces;
        this.groundMaxDimension = groundMaxDimension;
        this.groundMinPieces = groundMinPieces;
        this.groundMinDimensions = groundMinDimensions;
    }
    public void SetGroundArray(GroundArray ga){
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.targetFloor;
        foreach(var g in ga.grounds){
            floor.CreateGroundArray(g.position, g.width, g.height);
        }
        if(!(ga.targetFloor == 0))
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
        GroundArray ga = new GroundArray(groundMaxPieces, groundMaxDimension,groundMinPieces,groundMinDimensions);
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