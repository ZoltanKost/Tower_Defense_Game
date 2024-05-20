using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private EnemyManager enemyManager;
    public List<Archer> archersList = new List<Archer>();
    bool active;
    public void AddArcher(Archer archer){
        archersList.Add(archer);
        archer.Init(enemyManager.enemies);
        if(archer.attackType == AttackType.Projectile){
            projectileManager.AddProjectile(archer.GetProjectile());
        }
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
            archersList[i].GetProjectile().Deactivate();
        }
        active = false;
    }
    public void DeactivateArchers(Archer[] archer){
        foreach(Archer a in archer){
            Debug.Log($"Deactivated: {a}");
            archersList.Remove(a);
            a._active = false;
            a.gameObject.SetActive(false);
        }
    }
}