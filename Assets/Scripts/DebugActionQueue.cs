using UnityEngine;
using System.Collections.Generic;

public static class DebugActionQueue{
    public delegate void DebugAction();
    static Queue<DebugAction> actions = new();
    public static void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            actions.Dequeue()?.Invoke();
        }
    }
    public static void AddAction(DebugAction a){
        actions.Enqueue(a);
    }
}