using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellButtonUI : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    [SerializeField] private Image image;
    [SerializeField] private Image coolDownImage;
    public int ID;
    public int param;
    Action<int> callback;
    public void Init(Action<int> _callback, Sprite sprite, int _param, int _ID)
    {
        image.sprite = sprite;
        ID = _ID;
        param = _param;
        if (targetButton == null) targetButton = GetComponent<Button>();
        if (_callback == null) return;
        callback = _callback;
        //targetButton.onClick.RemoveAllListeners();
        targetButton.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        callback?.Invoke(ID);
    }
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        //image.SetNativeSize();
    }
    public void SetID(int id)
    {
        ID = id;
    }
    public void UpdateValue(float value)
    {
        coolDownImage.fillAmount = value;
        if (coolDownImage.fillAmount >= 1f) coolDownImage.fillAmount = 0;
    }
}
