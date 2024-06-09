using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSprite : MonoBehaviour{
    [SerializeField] private Button button;
    [SerializeField] private Sprite pressed;
    [SerializeField] private Sprite _default;
    Image s_renderer;
    void Awake(){
        s_renderer = GetComponent<Image>();
        // button.onClick.AddListener(Press);
        // button.onClick.AddListener(Unpress);
    }
    public void DownRegister(Action action){
        action += OnPointerDown;
    }
    public void UpRegister(Action action){
        action += OnPointerUp;
    }
    public void OnPointerDown()
    {
        s_renderer.sprite = pressed;
    }

    public void OnPointerUp()
    {
        s_renderer.sprite = _default;
    }
}