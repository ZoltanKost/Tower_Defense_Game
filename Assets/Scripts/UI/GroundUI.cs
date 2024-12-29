using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class GroundUI : MonoBehaviour{
    private Action<int> onClick;
    [SerializeField] private UIFloor _floor;
    [SerializeField] private TMP_Text text;
    [SerializeField] private int pixelPerUnit = 32;
    public int uiID{get;private set;}
    public void Init(int ID,Action<int> onClick, int width, int height){
        _floor.Init(pixelPerUnit, width, height);
        uiID = ID;
        this.onClick = onClick;
    }
    public void SetGroundArray(GroundArray ga){
        _floor.ClearAllTiles();
        _floor.floor = ga.targetFloor;
        foreach(Vector3Int v in ga.grounds){
            _floor.CreateGround(v);
        }
        Vector3 pos = new Vector3{x = -ga.width/2f, y = -ga.height / 2f + ga.targetFloor, z = 0} * pixelPerUnit * 3f/ga.width;
        _floor.transform.localPosition = pos;
        _floor.transform.localScale = new Vector3(Mathf.Min(1,3f/ga.width), Mathf.Min(1, 3f / ga.width), 1);
        text.text = ga.price.ToString();
    }
    public void SetTile(TileBase tile){
        _floor.ClearAllTiles();
        _floor.transform.localPosition = Vector3.zero;
        _floor.SetTile(Vector3Int.zero,tile);
    }
    public void ActivateVisuals(){
        _floor.Activate();
    }
    public void DeactivateVisuals(){
        _floor.Deactivate();
    }
    public void OnGroundsChoosen(){
        DeactivateVisuals();
        onClick?.Invoke(uiID);
    }
}