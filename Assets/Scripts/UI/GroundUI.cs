using UnityEngine;
using UnityEngine.Tilemaps;

public delegate void OnPageUIClick(int id);
public class GroundUI : MonoBehaviour{
    private OnPageUIClick onClick;
    [SerializeField] private Floor _floor;
    public int uiID{get;private set;}
    void Awake(){
        _floor.Init(0, $"GroundUI");
    }
    public void Init(int ID,OnPageUIClick onClick){
        uiID = ID;
        this.onClick = onClick;
    }
    public void SetGroundArray(GroundArray ga){
        _floor.ClearAllTiles();
        _floor.layer = ga.targetFloor;
        foreach(Vector3Int g in ga.grounds){
            _floor.CreateGround(g);
        }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.width)/2 + .5f, z = 0};
        _floor.transform.localPosition = pos;
    }
    public void SetTile(TileBase tile){
        _floor.ClearAllTiles();
        _floor.transform.localPosition = Vector3.zero;
        _floor.visuals[0].SetTile(Vector3Int.zero,tile);
    }
    public void ActivateVisuals(){
        _floor.gameObject.SetActive(true);
    }
    public void DeactivateVisuals(){
        _floor.gameObject.SetActive(false);
    }
    public void OnGroundsChoosen(){
        onClick?.Invoke(uiID);
        DeactivateVisuals();
    }
}