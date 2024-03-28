using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour {
    [SerializeField] private EnemyManager enemyManager;
    public List<Archer> archersList = new List<Archer>();
    bool active;
    public void AddArcher(Archer a, int sortingOrder, int sortingLayer){
        archersList.Add(a);
        a.Init(enemyManager.enemies);
    }
    public void ActivateArchers(){
        int n = archersList.Count;
        for(int i = 0; i < n; i++){
            archersList[i].ResetAnimation();
        }
        active = true;
    }
    void Update(){
        int n = archersList.Count;
        float delta = Time.deltaTime;
        if(active){
            for(int i = 0; i < n; i++){
                archersList[i].TickDetection(delta);
            }
        }
        for(int i = 0; i < n; i++){
            archersList[i].TickAnimator(delta);
        }
    }
    public void DeactivateArchers(){
        int n = archersList.Count;
        for(int i = 0; i < n; i++){
            archersList[i].ResetAnimation();
        }
        active = false;
    }

}