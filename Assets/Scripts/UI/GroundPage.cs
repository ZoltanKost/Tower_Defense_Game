using UnityEngine;
using System;

public class GroundPage : MonoBehaviour {
    private GroundUI[] buttons;
    [SerializeField] private GroundUI prefab;
    public void Init(GroundArray[] grounds, int capacity, Action<int> buttonOnClickCallback, int width, int height){
        if (buttons != null)
        {
            foreach(GroundUI g in buttons)
            {
                Destroy(g.gameObject);
            }
        }
        buttons = new GroundUI[capacity];
        for(int i = 0; i < capacity; i++){
            buttons[i] = Instantiate(prefab, transform);
            buttons[i].Init(i,buttonOnClickCallback,width, height);
        }
    }
    public void UpdateVisual(int ID, GroundArray ga){
        buttons[ID].SetGroundArray(ga);
    }
    public void ResetGroundArrays(GroundArray[] grounds){
        for(int i = 0; i < grounds.Length; i++){
            buttons[i].gameObject.SetActive(true);
            buttons[i].ActivateVisuals();
            buttons[i].SetGroundArray(grounds[i]);
        }
    }
    public void ActivateVisuals(int uiID){
        buttons[uiID].ActivateVisuals();
    }
    public void DeactivateVisuals(int uiID){
        buttons[uiID].DeactivateVisuals();
    }
    public void DeactivateButton(int uiID)
    {
        buttons[uiID].gameObject.SetActive(false);
    }
    public void ActivateButton(int uiID)
    {
        buttons[uiID].gameObject.SetActive(true);
    }
}