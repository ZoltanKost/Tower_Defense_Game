using UnityEngine;
using System;
using System.Reflection;

public class BuildingManager : MonoBehaviour, IHandler {
    [SerializeField] private BuildingObject prefab;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private ProjectileManager projectileManager;
    public BuildingObject[] bs;
    int Count;
    bool active;
    public void Init()
    {
        Count = 0;
        bs = new BuildingObject[16];
        for(int i = 0; i < bs.Length; i++)
        {
            bs[i] = Instantiate(prefab,transform);
            bs[i].gameObject.SetActive(false);
        }
    }
    public void Build(Vector3 worldPosition, int gridX,int gridY, int floor, Building building, out Func<int> getID){
        worldPosition.z = 0;
        Vector3 WidthAlignmentOffset =  (building.width % 2 == 0? (building.width / 2) : (float)building.width/2) * Vector3.right;

        BuildingObject buildingObject = bs[Count];
        InitArchers(buildingObject.GetArchers());
        buildingObject.Init(6,floor,Count, gridX, gridY, building, KillBuilding);
        buildingObject.transform.position = worldPosition + WidthAlignmentOffset;
        buildingObject.gameObject.SetActive(true);
        getID = bs[Count].GetIndex;

        Count++;
        if(Count >= bs.Length){
            Resize();
        }
    }
    void Resize()
    {
        Array.Resize(ref bs, Count * 2);
        for(int i = 0;i < Count; i++)
        {
            bs[i] = Instantiate(prefab,transform);
        }
        Debug.Log($"Array resized. new Length is {bs.Length}");
    }
    public void InitArchers(Archer[] archers){
        foreach(Archer a in archers){
            archerManager.AddArcher(a);
        }
    }
    void Update(){
        if(!active) return;
        AnimatorTick(Time.deltaTime);
    }
    void KillBuilding(int index)
    {
        archerManager.RemoveArchers(bs[index].GetArchers());
    }
    public void RemoveBuilding(int index, out int gridX, out int gridY, out int w, out int h)
    {
        w = -1;
        h = -1;
        gridX = -1;
        gridY = -1;
        if(index == 0) return;
        BuildingObject b = bs[index];
        bs[index] = bs[--Count];
        bs[index].index = index;
        bs[Count] = b;
        w = b.w;
        h = b.h;
        gridX = b.gridPosition.x;
        gridY = b.gridPosition.y;
        b.Deactivate();
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