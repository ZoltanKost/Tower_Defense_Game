using System.Collections.Generic;
using UnityEngine;

public class BuildingManager: MonoBehaviour{
    List<Building> buildings = new List<Building>();
    public void AddBuilding(Building building){
        buildings.Add(building);
    }
    public void RemoveBuilding(Building building){
        buildings.Remove(building);
    }
    private void FixedUpdate() {
        float delta = Time.deltaTime;
        foreach(Building building in buildings){
            building.Tick(delta);
        }
    }
}