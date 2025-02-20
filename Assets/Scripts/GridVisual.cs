using TMPro;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    public const int basemovecost = 50;
    [SerializeField] SpriteRenderer image;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text leftText;
    [SerializeField] TMP_Text togetherText;
    [SerializeField] TMP_Text indexText;
    [SerializeField] TMP_Text comeFromIndexText;
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
        //comeFromIndexText.text = "-1";
        //transform.localScale = new Vector3(size, size, size);
        transform.position = position;
        gridX = _gridX;
        gridY = _gridY;
        Color c = image.color;
        c.a = 0.5f;
        image.color = c;
        return this;
    }
    public void UpdateInfo(Color color, int _cost, int _left, int heapIndex, Vector2Int comeFrom)
    {
        image.color = color;
        cost = _cost;
        left = _left;
        costText.text = _cost.ToString();
        leftText.text = _left.ToString();
        comeFromIndexText.text = comeFrom.x.ToString()+";"+comeFrom.y.ToString();
        togetherText.text = (left * 50 + cost).ToString();
        indexText.text = heapIndex.ToString();
    }
    public void UpdateInfo(Color color, FloorCell cell)
    {
        if(color != Color.black)
        image.color = color;
        cost = cell.cost;
        left = cell.left;
        costText.text = cost.ToString();
        leftText.text = left.ToString();
        comeFromIndexText.text = cell.comeFrom.x.ToString() + ";" + cell.comeFrom.y.ToString();
        togetherText.text = (left * 50 + cost).ToString();
        indexText.text = cell.heapIndex.ToString();
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