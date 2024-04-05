using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSprite : MonoBehaviour {
    [SerializeField] private Button button;
    [SerializeField] private Sprite pressed;
    [SerializeField] private Sprite _default;
    Image s_renderer;
    void Awake(){
        s_renderer = GetComponent<UnityEngine.UI.Image>();
        button.onClick.AddListener(Press);
    }
    public void Press(){
        s_renderer.sprite = pressed;
    }
}