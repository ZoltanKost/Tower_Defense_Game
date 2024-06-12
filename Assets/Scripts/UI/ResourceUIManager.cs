using System.Collections.Generic;
using UnityEngine;

public class ResourceUIManager : MonoBehaviour {
    [SerializeField] private ResourceTemplate prefab;
    List<ResourceTemplate> visuals;
    public void Init(ResourceSO[] resources, int[] counts){
        visuals = new List<ResourceTemplate>();
        for(int i = 0; i < resources.Length; i++){
            ResourceTemplate tmp = Instantiate(prefab, Vector3.zero, Quaternion.identity,transform);
            tmp.Init(resources[i].ID,resources[i].sprite, counts[i]);
            visuals.Add(tmp);
        }
    }
    public void ResetVisuals(ResourceSO[] resources, int[] counts){
        int i = 0;
        for(;i < resources.Length; i++){
            if(i >= visuals.Count) break;
            visuals[i].Init(resources[i].ID,resources[i].sprite, counts[i]);
        }
        while(i < visuals.Count){
            visuals[i].gameObject.SetActive(false);
            i++;
        }
    }
    public void UpdateResource(int ID, int count){
        // Debug.Log($" ID : {ID}, COunt: {count}");
        if(ID >= visuals.Count) return;
        foreach(ResourceTemplate temp in visuals){
            if((int)temp.resource == ID){
                temp.SetText(count);
            } 
        }
    }
}