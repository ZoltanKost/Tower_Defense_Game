using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuildingButtonUI : EventSubscribeButton {
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private bool nativeSize = false;
    [SerializeField] private float scaleFactor = 1f;
    int ID;
    public void Init(Action<int> onPageUIClick, Sprite icon, int cost, int id){
        image.sprite = icon;
        text.text = cost.ToString();
        ID = id;
        Init(() => onPageUIClick?.Invoke(ID));
        image.rectTransform.localScale *= scaleFactor;
        if (nativeSize) image.SetNativeSize();
    }
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        image.SetNativeSize();
    }
    /*public void ActivateVisuals()
    {
        image.enabled = true;
    }
    public void DeactivateVisuals()
    {
        //targetButton.image.sprite = targetButton.
        image.enabled = false;
    }*/
    public void SetID(int id)
    {
        ID = id;
    }
    /*public void ReInit(Sprite sprite, int ID)
    {
        SetSprite(sprite);
        SetID(ID);
    }*/
}
