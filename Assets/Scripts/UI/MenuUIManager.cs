using System;
using UnityEngine;

public class MenuUIManager : MonoBehaviour {
    [SerializeField] EventSubscribeButton[] buttons;
    public void Init(Action[] events){
        for(int i = 0; i < events.Length; i++){
            if(i >= buttons.Length) break;
            buttons[i].Init(events[i]);
        } 
    }
}