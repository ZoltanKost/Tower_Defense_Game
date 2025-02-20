using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHighlightTemplate : MonoBehaviour
{
    public Image image;
    public TMP_Text text;
    public void SetWaveData(int count, Sprite sprite, Vector3 position)
    {
        text.text = count.ToString();
        gameObject.SetActive(true);
        transform.position = position;
    }
}