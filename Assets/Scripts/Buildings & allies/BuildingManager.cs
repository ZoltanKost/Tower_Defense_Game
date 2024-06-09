using UnityEngine;

using System.Collections.Generic;

public class BuildingManager : MonoBehaviour, IHandler {
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private ProjectileManager projectileManager;
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
        AnimatorTick(Time.deltaTime);
    }
    public void RemoveBuilding(int index){
        Debug.Log($"Removed: {index}");
    }

    public void Tick(float delta)
    {
        // insert logic here
    }

    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < bs.Count; i++){
            if(!bs[i].active) continue;
            bs[i].TickUpdate(delta);
        }
    }

    public void Switch(bool active)
    {
        this.active = active;
    }

    public void DeactivateEntities()
    {
        foreach(BuildingObject b in bs){
            b.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        
    }

    public void ClearEntities()
    {
        foreach(BuildingObject b in bs){
            Destroy(b.gameObject);
        }
        bs.Clear();
    }
}