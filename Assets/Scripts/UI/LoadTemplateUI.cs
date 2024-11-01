using UnityEngine;
using System;
using TMPro;

public class LoadTemplateUI : MonoBehaviour
{
    [SerializeField] private EventSubscribeButton LoadButton;
    [SerializeField] private EventSubscribeButton DeleteButton;
    [SerializeField] private TMP_Text TMP;
    int Id;
    public void Init(Action<int> load, Action<int> remove, int id, string text)
    {
        Id = id;
        gameObject.SetActive(true);
        LoadButton.Init(() => load(Id));
        DeleteButton.Init(() => remove(Id));
        TMP.text = text;
    }
}