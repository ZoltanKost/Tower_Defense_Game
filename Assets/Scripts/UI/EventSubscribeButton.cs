using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventSubscribeButton : MonoBehaviour{
    [SerializeField] private Button targetButton;
    public void Init(Action start){
        if(targetButton == null) targetButton = GetComponent<Button>();
        if(start == null) return;
        targetButton.onClick.AddListener(start.Invoke);
    }
}