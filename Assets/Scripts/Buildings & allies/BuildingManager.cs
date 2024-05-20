using UnityEngine;

using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {
    [SerializeField] private ArcherManager archerManager;
    public List<BuildingObject> bs = new List<BuildingObject>();
    bool active;
    public void Build(Vector3 worldPosition, int floor, Building building){
        worldPosition.z = 0;
        Vector3 offset =  (building.width % 2 == 0? (building.width / 2) : (float)building.width/2) * Vector3.right;
        BuildingObject s = Instantiate(building.prefab, worldPosition + offset, Quaternion.identity,transform);
        s.Init(6,floor, bs.Count, RemoveBuilding);
        bs.Add(s);
        InitArchers(s.GetArchers(), 7,floor);
    }
    public void InitArchers(Archer[] archers, int sortingOrder, int sortingLayer){
        foreach(Archer a in archers){
            archerManager.AddArcher(a);
        }
    }
    void Update(){
        if(!active) return;
        float delta = Time.deltaTime;
        for(int i = 0; i < bs.Count; i++){
            if(!bs[i].active) continue;
            bs[i].TickUpdate(delta);
        }
    }
    public void Reset(){
        int b = transform.childCount;
        for(int i = 0; i < b; i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void RemoveBuilding(int index){
        Debug.Log($"Removed: {index}");
        if(index == 0) return;
    }
    public void Activate(){
        active = true;
    }
    public void Deactivate(){
        active = false;
        archerManager.DeactivateArchers();
    }
}