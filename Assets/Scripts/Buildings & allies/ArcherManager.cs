using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour, IHandler {
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
    void Update(){
        float delta = Time.deltaTime;
        Tick(delta);
        AnimatorTick(delta);
    }
    public void Tick(float delta)
    {
        if(!active) return;
        for(int i = 0; i < archersList.Count; i++){
            archersList[i].TickDetection(delta);
        }
    }

    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < archersList.Count; i++){
            archersList[i].TickAnimator(delta);
        }
    }
    public void Switch(bool active)
    {
        this.active = active;
        int n = archersList.Count;
        for(int i = 0; i < n; i++){
            archersList[i].Switch(active);
        }
    }
    public void DeactivateEntities()
    {
        foreach(Archer a in archersList){
            a.gameObject.SetActive(false);
        }
    }

    public void ResetEntities()
    {
        foreach(Archer archer in archersList){
            Destroy(archer.gameObject);
        }
        archersList.Clear();
    }
}