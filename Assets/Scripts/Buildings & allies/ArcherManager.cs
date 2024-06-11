using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private EnemyManager enemyManager;
    public List<Archer> archersList = new List<Archer>();
    bool active;
    bool animate;
    public void AddArcher(Archer archer){
        archersList.Add(archer);
        archer.Init(enemyManager.enemies);
        if(archer.attackType == AttackType.Projectile){
            projectileManager.AddProjectile(archer.GetProjectile());
        }
    }
    public void RemoveArchers(Archer[] archer){
        foreach(Archer a in archer){
            projectileManager.RemoveProjectile(a.GetProjectile());
            archersList.Remove(a);
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
        if(!animate) return;
        for(int i = 0; i < archersList.Count; i++){
            archersList[i].TickAnimator(delta);
        }
    }
    public void SwitchAnimation(bool animate){
        this.animate = animate;
    }
    public void Switch(bool active)
    {
        this.active = active;
    }
    public void DeactivateEntities()
    {
        foreach(Archer a in archersList){
            a.Deactivate();
        }
    }

    public void ResetEntities()
    {
        foreach(Archer a in archersList){
            a.Reset();
        }
    }

    public void ClearEntities()
    {
        foreach(Archer archer in archersList){
            Destroy(archer.gameObject);
        }
        archersList.Clear();
    }
}