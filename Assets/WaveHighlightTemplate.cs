using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHighlightTemplate : MonoBehaviour
{
    public Image image;
    public TMP_Text count;
    public TMP_Text time;
    public float timeValue;
    public void SetWaveData(int _count, Sprite sprite, Vector3 position, int _time)
    {
        count.text = count.ToString();
        time.text = _time.ToString();
        timeValue = _time; 
        gameObject.SetActive(true);
        transform.position = position;
    }
    public void Tick(float delta, Vector3 position)
    {
        
    }
}