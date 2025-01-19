using UnityEngine;
using System.Collections.Generic;

public delegate void ID_Count_Callback(int id, int count);
public class PlayerResourceManager : MonoBehaviour {
    [SerializeField] private ResourceUIManager resourceUIManager;
    [SerializeField] private ResourceSO[] resourceSOs;
    [SerializeField] private int[] counts;
    public Dictionary<Resource,int> storage;
    ID_Count_Callback updateUICallback;
    public void Init(){
        updateUICallback = resourceUIManager.UpdateResource;
        storage = new Dictionary<Resource, int>();
        for(int i = 0; i < resourceSOs.Length; i++){
            storage.Add(resourceSOs[i].ID, counts[i]);
        }
        resourceUIManager.Init(resourceSOs,counts);
    }
    public void Reset(){
        storage = new Dictionary<Resource, int>();
        for(int i = 0; i < resourceSOs.Length; i++){
            storage.Add(resourceSOs[i].ID, counts[i]);
        }
        resourceUIManager.ResetVisuals(resourceSOs,counts);
    }
    public void AddResource(Resource resource, int count){
        if(storage.ContainsKey(resource)){
            storage[resource] += count;
        }else{
            storage.Add(resource, count);
        }
        updateUICallback?.Invoke((int)resource, storage[resource]);
    }
    public bool EnoughtResource(Resource resource, int count){
        if(storage.ContainsKey(resource) && storage[resource] >= count) return true;
        return false;
    }
    public void RemoveResource(Resource resource, int count){
        storage[resource] -= count;
        // Debug.Log($"Remove {count} of {resource}");
        updateUICallback?.Invoke((int)resource, storage[resource]);
    }
    public void SetResource(Resource res, int count)
    {
        storage[res] = count;
        updateUICallback?.Invoke((int)res, storage[res]);
    }
    public void AddGold(int number)
    {
        AddResource(Resource.Gold, number);
    }
}
public enum Resource{
    Gold,
    Wood,
    Meat
}