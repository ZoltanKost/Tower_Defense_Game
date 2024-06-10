using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonUI : EventSubscribeButton {
    [SerializeField] private Image image;
    int ID;
    public void Init(UI_ID_Callback onPageUIClick, Sprite sprite,int id){
        image.sprite = sprite;
        ID = id;
        Init(() => onPageUIClick?.Invoke(ID));
    }
}