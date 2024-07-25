using UnityEngine;
using System;

public class GroundPage : MonoBehaviour {
    private GroundUI[] buttons;
    [SerializeField] private GroundUI prefab;
    public void Init(GroundArray[] grounds, int capacity, Action<int> buttonOnClickCallback){
        buttons = new GroundUI[capacity];
        for(int i = 0; i < capacity; i++){
            buttons[i] = Instantiate(prefab, transform);
            buttons[i].Init(i,buttonOnClickCallback);
        }
    }
    public void UpdateVisual(int ID, GroundArray ga){
        buttons[ID].SetGroundArray(ga);
    }
    public void ResetGroundArrays(GroundArray[] grounds){
        for(int i = 0; i < grounds.Length; i++){
            buttons[i].SetGroundArray(grounds[i]);
        }
    }
    public void ActivateVisuals(int uiID){
        buttons[uiID].ActivateVisuals();
    }
    public void DeactivateVisuals(int uiID){
        buttons[uiID].DeactivateVisuals();
    }
}