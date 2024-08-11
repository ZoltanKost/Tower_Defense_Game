using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighlightUI : MonoBehaviour
{
    [SerializeField] Image m_HighlightUI;
    [SerializeField] TMP_Text damage;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text health;
    [SerializeField] private HideShowUI hideShow;
    bool open;
    public void SetEntity(Sprite sprite, int hp, int dmg, int spd)
    {
        if (!open)
        {
            hideShow.ShowUI();
            open = true;
        }
        m_HighlightUI.sprite = sprite;
        health.text = hp.ToString();
        damage.text = dmg.ToString();
        speed.text = spd.ToString();
    }
    public void ResetEntity()
    {
        if (!open) return;
        hideShow.HideUI();
        open = false;
    }
    public void Close()
    {
        if (!open) return;
        hideShow.HideUI();
        open = false;
    }
}