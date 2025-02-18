using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridVisual : MonoBehaviour
{
    public const int basemovecost = 50;
    [SerializeField] Image image;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text leftText;
    [SerializeField] TMP_Text togetherText;
    [SerializeField] TMP_Text indexText;
    public int gridX, gridY, cost, left;
    public int moveCost = 50;
    public GridVisual cameFrom;
    public int index;
    public GridVisual Init(int _gridX, int _gridY, float size, Vector3 position)
    {
        costText.text = "0";
        leftText.text = "0";
        togetherText.text = "0";
        indexText.text = "-1";
        //transform.localScale = new Vector3(size, size, size);
        transform.position = position;
        gridX = _gridX;
        gridY = _gridY;
        Color c = image.color;
        c.a = 0.5f;
        image.color = c;
        return this;
    }
    public void UpdateInfo(Color color, int _cost, int _left, GridVisual _cameFrom)
    {
        image.color = color;
        cost = _cost;
        left = _left;
        costText.text = _cost.ToString();
        leftText.text = _left.ToString();
        togetherText.text = (left * 50 + cost).ToString();
        cameFrom = _cameFrom;
    }
    public void SetColor(Color color)
    {
        image.color = color;
    }
    public void ChangeAlpha(float value)
    {
        Color color = image.color;
        color.a += value;
        image.color = color;
    }
    public void SetIndex(int i)
    {
        index = i;
        indexText.text = i.ToString();
    }
}