using UnityEngine;
using UnityEngine.UI;

public class HighlightUI : MonoBehaviour
{
    [SerializeField] Image m_HighlightUI;
    int _hp;
    public void SetEntity(Sprite sprite, int hp)
    {
        m_HighlightUI.sprite = sprite;
        _hp = hp;
    }
}