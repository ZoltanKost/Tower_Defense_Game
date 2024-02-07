using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiecesUIManager : MonoBehaviour{
    [SerializeField] private GroundUI prefab;

    public List<GroundUI> grounds_visuals;
    public void AddGroundArray(Action<object,GroundArray> func){
        GroundUI ui = Instantiate(prefab, transform);
        ui.CreateGroundArray();
        ui.onClick += func.Invoke;
        grounds_visuals.Add(ui);
    }
    
    public void Reset(){
        foreach(var u in grounds_visuals){
            u.CreateGroundArray();
        }
    }
}