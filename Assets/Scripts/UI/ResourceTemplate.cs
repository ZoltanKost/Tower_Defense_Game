using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTemplate : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    public Resource resource{get;private set;}
    public void Init(Resource resource, Sprite sprite, int number){
        this.resource = resource;
        image.sprite = sprite;
        text.text = number.ToString();
    }
    public void SetText(int number){
        text.text = number.ToString();
    }
}