using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;

public class GroundUI : MonoBehaviour{
    private Action<int> onClick;
    [SerializeField] private UIFloor[] floors;
    [SerializeField] private Transform FloorsParent;
    [SerializeField] private TMP_Text text;
    [SerializeField] private int pixelPerUnit = 32;
    [SerializeField] private Image coolDownImage;
    [SerializeField] private Color highlightColor;
    public int uiID{get;private set;}
    public void Init(int ID,Action<int> onClick, int width, int height){
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i].Init(pixelPerUnit, width, height);
        }
        uiID = ID;
        this.onClick = onClick;
    }
    public void SetGroundArray(GroundArray ga){
        foreach(UIFloor fl in floors)
        {
            fl.ClearAllTiles();
        }
        foreach(GACell v in ga.grounds){
            int floor = v.floor;
            //Debug.Log(floor);
            while(floor >= 0)
            {
                floors[floor].floor = floor;
                floors[floor--].CreateGround(v.position);
            }
        }
        Vector3 pos = new Vector3{x = -ga.width/2f, y = -ga.height / 2f + ga.targetFloor, z = 0} * pixelPerUnit * 3f/ga.width;
        FloorsParent.transform.localPosition = pos;
        FloorsParent.transform.localScale = new Vector3(Mathf.Min(1,3f/ga.width), Mathf.Min(1, 3f / ga.width), 1);
        text.text = ga.price.ToString();
    }
    public void SetTile(TileBase tile){
        foreach(UIFloor floor in floors) floor.ClearAllTiles();
        FloorsParent.transform.localPosition = Vector3.zero;
        floors[0].SetTile(Vector3Int.zero,tile);
    }
    public void ActivateVisuals(){
        foreach (UIFloor floor in floors) floor.Activate();
    }
    public void DeactivateVisuals(){
        foreach (UIFloor floor in floors) floor.Deactivate();
    }
    public void OnGroundsChoosen(){
        //DeactivateVisuals();
        onClick?.Invoke(uiID);
    }
    public void UpdateValue(float value)
    {
        coolDownImage.fillAmount = value;
        if (coolDownImage.fillAmount >= 1f) coolDownImage.fillAmount = 0;
    }
    public void Highlight()
    {
        coolDownImage.color = highlightColor;
        Debug.Log("highlighted");
    }
    public void Dehighlight()
    {
        Debug.Log("dehighlighted");
        coolDownImage.color = new Color(0,0,0,0);
    }
}