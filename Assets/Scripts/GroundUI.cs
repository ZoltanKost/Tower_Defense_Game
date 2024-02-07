using UnityEngine;

public class GroundUI : MonoBehaviour{
    public delegate void OnClick(GroundUI gUI, GroundArray g);
    public OnClick onClick;
    public Floor floor;
    GroundArray currentGA;
    void Awake(){
        floor.Init(0,GetComponentInParent<Canvas>().sortingOrder + 1);
    }
    public void SetGroundArray(GroundArray ga){
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.layer;
        foreach(var g in ga.grounds){
            floor.CreateGroundArray(g.position, g.width, g.height);
        }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.height)/2 + .5f, z = 0};
        floor.transform.localPosition = pos;
    }
    public void CreateGroundArray(){
        GroundArray ga = new GroundArray(4,2);
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.layer;
        foreach(var g in ga.grounds){
            floor.CreateGroundArray(g.position, g.width, g.height);
        }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.height)/2 + .5f, z = 0};
        floor.transform.localPosition = pos;
    }
    public void ClearAllTiles(){
        floor.ClearAllTiles();
    }
    public GroundArray GetGroundArray(){
        return currentGA;
    }
    public void OnGroundsChoosen(){
        onClick?.Invoke(this, currentGA);
        ClearAllTiles();
    }
}