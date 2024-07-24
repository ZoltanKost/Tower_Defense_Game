using UnityEngine;
using System;

public class BuildingManager : MonoBehaviour, IHandler {
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private ProjectileManager projectileManager;
    public BuildingObject[] bs;
    int Count = 0;
    bool active;
    private void Awake() {
        bs = new BuildingObject[4];   
    }
    public void Build(Vector3 worldPosition, int gridX,int gridY, int floor, Building building, out Func<int> getID){
        worldPosition.z = 0;
        Vector3 offset =  (building.width % 2 == 0? (building.width / 2) : (float)building.width/2) * Vector3.right;

        // if(bs[Count] != null)
        // {
        //     BuildingObject buildingObject = bs[Count];
        //     buildingObject.Init(6,floor,Count, gridX, gridY, building.width,building.height, RemoveBuilding);
        //     buildingObject.transform.position = worldPosition + offset;
        //     getID = bs[Count].GetIndex;
        // }
        // else
        // {
            BuildingObject s = Instantiate(building.prefab, worldPosition + offset, Quaternion.identity,transform);
            InitArchers(s.GetArchers(), 7,floor);
            s.Init(6,floor,Count, gridX, gridY, building.width,building.height, RemoveBuilding);
            bs[Count] = s;
            getID = s.GetIndex;
        // }
        Count++;
        if(Count >= bs.Length){ 
            Array.Resize(ref bs,Count * 2);
            Debug.Log($"Array resized. new Length is {bs.Length}");
        }
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
        // Debug.Log($"Building{index} removed. Building {Count - 1} is nor building {index}.");
        // BuildingObject temp = bs[index];
        // BuildingObject lastSpawned = bs[--Count];
        // bs[index] = lastSpawned;
        // lastSpawned.index = index;
        // bs[Count] = temp;
        // temp.index = Count;
        // Debug.Log($"Died and lastSpawned are same: {bs[index] == bs[Count]}");
    }
    public void DestroyBuilding(int index, out int gridX, out int gridY, out int w, out int h)
    {
        w = -1;
        h = -1;
        gridX = -1;
        gridY = -1;
        if(index == 0) return;
        Debug.Log($"BuildingManager removing {index}!");
        BuildingObject b = bs[index];
        Debug.Log($"Id in building: {b.index}");
        w = b.w;
        h = b.h;
        gridX = b.gridPosition.x;
        gridY = b.gridPosition.y;
        b.Deactivate();
        b.gameObject.SetActive(false);
        RemoveBuilding(index);
    }

    public void Tick(float delta)
    {
        // insert logic here
    }

    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < Count; i++){
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
        for(int i = 0; i < bs.Length; i++)
        {
            if(bs[i] != null)
            {
                Destroy(bs[i].gameObject);
                bs[i] = null;
            }
        }
        Count = 0;
    }
}