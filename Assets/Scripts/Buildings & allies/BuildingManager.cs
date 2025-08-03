using UnityEngine;
using System;

public class BuildingManager : MonoBehaviour{
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private Building[] buildingData;
    [SerializeField] private BuildingObject prefab;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private AudioSource audioSource;
    public BuildingObject[] bs;
    public int Count;
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
        
        for (int i = 0; i < buildingData.Length; i++)
        {
            if (building == buildingData[i]) buildingObject.AssetID = i;
        }
        int sortingOrder = 6 + 1000 / gridY;
        buildingObject.Init(sortingOrder, floor, Count, gridX, gridY, building, KillBuilding);
        InitArchers(
            buildingObject.GetArchers(), buildingObject.gridPosition, 
            buildingObject.w, buildingObject.h, buildingObject.index, 
            buildingObject.attackRangeBonus, sortingOrder + 1, floor);
        buildingObject.transform.position = worldPosition + WidthAlignmentOffset;
        buildingObject.gameObject.SetActive(true);
        getID = bs[Count].GetIndex;

        Count++;
        if(Count >= bs.Length){
            Resize();
        }
        //audioSource.pitch = UnityEngine.Random.Range(0.7f,1f);
        audioSource.Play();
    }
    public void Build(BuildingSaveData data, out Func<int> getID)
    {
        Debug.LogError("Saving is not implemented");
        BuildingObject buildingObject = bs[Count];
        buildingObject.AssetID = data.AssetID;
        if (data.currentHP <= 0)
        {
            buildingObject.Init(6,
                floorManager.floorCells[data.gridPosition.x, data.gridPosition.y].currentFloor,
                1,data,
                buildingData[data.AssetID], KillBuilding);
            foreach (Character archer in buildingObject.GetArchers())
            {
                archer.gameObject.SetActive(false);
            }
        }
        else
        {
            int layer = 
                floorManager.floorCells[data.gridPosition.x, data.gridPosition.y].currentFloor;
            buildingObject.Init(6, layer, 0, data,
                buildingData[data.AssetID], KillBuilding);
            InitArchers(
                buildingObject.GetArchers(), data.gridPosition, 
                buildingData[data.AssetID].width, buildingData[data.AssetID].height, 
                data.index, buildingObject.attackRangeBonus,
                6, layer);
        }
        buildingObject.transform.position = data.position;
        buildingObject.gameObject.SetActive(true);
        getID = bs[Count].GetIndex;

        Count++;
        if (Count >= bs.Length)
        {
            Resize();
        }
    }
    void Resize()
    {
        Array.Resize(ref bs, Count * 2);
        for(int i = Count; i < bs.Length; i++)
        {
            bs[i] = Instantiate(prefab,transform);
            bs[i].gameObject.SetActive(false);
        }
        Debug.Log($"Array resized. new Length is {bs.Length}");
    }
    public void InitArchers(Character[] archers, Vector2Int gridPosition, int buildingWidth, int buildingHeight, int buildingID, int attackRangeBonus, int sortingOrder,int layer)
    {
        foreach(Character a in archers){
            archerManager.AddArcher(a, gridPosition, buildingWidth, buildingHeight, buildingID, attackRangeBonus, sortingOrder,layer);
        }
    }
    void FixedUpdate(){
        if(!active) return;
        AnimatorTick(Time.fixedDeltaTime);
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
        foreach(Character a in bs[index].GetArchers())
        {
            a.buildingID = index;
        }
        bs[Count] = b;
        b.active = false;
        w = b.w;
        h = b.h;
        gridX = b.gridPosition.x;
        gridY = b.gridPosition.y;
        b.Deactivate();
        archerManager.RemoveArchers(b.GetArchers());
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
        for (int i = 0; i < Count; i++)
        {
            bs[i].Deactivate();
            archerManager.RemoveArchers(bs[i].GetArchers());
        }
        Count = 0;
    }

    public void DeleteEntities()
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