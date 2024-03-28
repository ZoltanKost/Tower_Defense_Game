using UnityEngine;

public class BuildingObject : MonoBehaviour {
    SpriteRenderer spriteRenderer;    
    Archer[] archers;
    void Awake(){
        archers = GetComponentsInChildren<Archer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(int sortingOrder, int sortingLayer){
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = $"{sortingLayer}";
    }
    public Archer[] GetArchers(){
        return archers;
    }
}