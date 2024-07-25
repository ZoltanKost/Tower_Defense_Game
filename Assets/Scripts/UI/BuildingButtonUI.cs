using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuildingButtonUI : EventSubscribeButton {
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    int ID;
    public void Init(Action<int> onPageUIClick, Sprite sprite,int id){
        image.sprite = sprite;
        ID = id;
        Init(() => onPageUIClick?.Invoke(ID));
        text.text = 2.ToString();
    }
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
    public void ActivateVisuals()
    {
        image.enabled = true;
    }
    public void DeactivateVisuals()
    {
        image.enabled = false;
    }
    public void SetID(int id)
    {
        ID = id;
    }
    public void ReInit(Sprite sprite, int ID)
    {
        SetSprite(sprite);
        SetID(ID);
    }
}