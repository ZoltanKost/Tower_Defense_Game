using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Transform archer;
    public List<Archer> archersList = new List<Archer>();
    bool active;
    bool animate;
    public void SpawnArcher()
    {
        Archer _archer = Instantiate(archer,transform).GetComponentInChildren<Archer>();
        if (_archer == null) { return; }
        Vector3 temp = Camera.main.transform.position;
        temp.z = 0;
        _archer.transform.position = temp;
        AddArcher(_archer);
    }
    public void AddArcher(Archer archer){
        archersList.Add(archer);
        archer.Init();
    }
    public void RemoveArchers(Archer[] archer){
        foreach(Archer a in archer){
            archersList.Remove(a);
            a.Deactivate();
        }
    }
    void Update(){
        float delta = Time.deltaTime;
        Tick(delta);
        AnimatorTick(delta);
    }
    public void Tick(float delta)
    {
        //if(!active) return;
        TickDetection();
        for (int i = 0; i < archersList.Count; i++){
            archersList[i].TickState(delta);
            if (archersList[i].ProjectileFlag)
            {
                projectileManager.SendProjectile(archersList[i].projectileData);
                archersList[i].ProjectileFlag = false;
            }
        }
    }
    public void TickDetection()
    {
        for (int i = 0; i < archersList.Count; i++)
        {
            archersList[i].shooting = false;
            archersList[i].target = null;
            float attackRange = archersList[i].attackRange;
            List<Enemy> enemyList = enemyManager.enemies;
            for (int k = 0; k < enemyManager.lowestInactive; k++)
            {
                if (!enemyList[k].alive) continue;
                Vector3 vector = archersList[i].transform.position - enemyList[k].position;
                vector.z = 0;
                float distance = vector.magnitude;
                if (distance > attackRange) continue;
                float minDistance;
                if (archersList[i].target == null || !archersList[i].target.active || !archersList[i].target.alive) minDistance = attackRange;
                else
                {
                    vector = archersList[i].transform.position - archersList[i].target.position;
                    vector.z = 0;
                    minDistance = vector.magnitude;
                }
                if (distance < minDistance)
                {
                    archersList[i].target = enemyList[k];
                }
            }
            if (archersList[i].target == null) archersList[i].state = ArcherState.Idle;
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
    public bool TryHighlightEntity(Vector3 position, out Archer archer, float radius)
    {
        archer = null;
        for (int i = 0; i < archersList.Count; i++)
        {
            var temp = archersList[i];
            Vector3 pos = temp.position - position;
            pos.z = 0;
            if (pos.magnitude > radius) continue;
            archer = temp;
            return true;
        }
        return false;
    }
}