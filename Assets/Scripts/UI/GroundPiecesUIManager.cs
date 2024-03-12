using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GroundPiecesUIManager : MonoBehaviour{
    public delegate void AddArray(GroundUI ui, GroundArray g);
    [SerializeField] private GroundUI prefab;
    [SerializeField] private int groundMaxDimension = 4;
    Canvas canvas;
    public bool hided = false;
    float height;
    Tween currentTween;
    Vector3 downPosition;
    Vector3 upPosition;
    void Awake(){
        height = GetComponent<GridLayoutGroup>().cellSize.y;
        canvas = GetComponentInParent<Canvas>();
        downPosition = Vector3.down * Camera.main.orthographicSize;
        upPosition = downPosition + Vector3.up * height;
    }

    public List<GroundUI> grounds_visuals;
    public void AddGroundArray(AddArray func){
        GroundUI ui = Instantiate(prefab, transform);
        ui.Init(groundMaxDimension);
        ui.CreateGroundArray();
        ui.onClick += func.Invoke;
        grounds_visuals.Add(ui);
    }
    
    public void Reset(){
        foreach(var u in grounds_visuals){
            u.CreateGroundArray();
        }
    }
    public void Hide(){
        Vector3 pos = canvas.transform.parent.position + downPosition;
        pos.z = 0;
        currentTween?.Complete();
        currentTween = canvas.transform.DOMove( pos,1f);
        hided = true;
    }
    public void Show(){
        currentTween?.Kill(false);
        Vector3 pos = canvas.transform.parent.position + upPosition;
        pos.z = 0;
        currentTween = canvas.transform.DOMove( pos,1f);
        hided = false;
    }
}