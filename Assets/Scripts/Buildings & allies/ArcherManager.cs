using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Transform archer;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    public List<Archer> archersList = new List<Archer>();
    bool active;
    bool animate;
    float cellSize;
    public void SpawnArcher()
    {
        Archer _archer = Instantiate(archer, transform).GetComponentInChildren<Archer>();
        if (_archer == null) { return; }
        Vector3 temp = Camera.main.transform.position;
        temp.z = 0;
        _archer.transform.position = temp;
        //AddArcher(_archer);
    }
    public void AddArcher(Archer archer, Vector2Int gridPosition, int buildingWidth, int buildingHeight, int buildingID)
    {
        cellSize = floorManager.CellToWorld(Vector3.one).x;
        archersList.Add(archer);
        archer.Init(gridPosition, buildingWidth, buildingHeight, buildingID);
    }
    public void RemoveArchers(Archer[] archer) {
        foreach (Archer a in archer) {
            archersList.Remove(a);
            a.Deactivate();
        }
    }
    void Update() {
        float delta = Time.deltaTime;
        Tick(delta);
    }
    void FixedUpdate()
    {
        TickDetection();
        TickState();
        AnimatorTick(Time.fixedDeltaTime);
    }
    public void Tick(float delta)
    {
        if(!active) return;
    }
    public void TickDetection()
    {
        Enemy[] enemyList = enemyManager.enemies;
        float cellSize = floorManager.CellToWorld(Vector3.one).x;
        Vector3 offset = floorManager.offset;
        int count = archersList.Count;
        for (int i = 0; i < count; i++)
        {
            archersList[i].shooting = false;
            archersList[i].target = null;
            int lowestInactive = enemyManager.lowestInactive;
            float minDistance = archersList[i].attackRange;
            for (int k = 0; k < lowestInactive; k++)
            {
                if (!(enemyList[k].HP > 0)) continue;
                Vector3Int enemyGridPosition = new Vector3Int { 
                    x = Mathf.FloorToInt((enemyList[k].transform.position.x + offset.x )/ cellSize), 
                    y = Mathf.FloorToInt((enemyList[k].transform.position.y + offset.y) / cellSize) 
                };
                Vector3 cellCenter = archersList[i].gridPosition + new Vector2(archersList[i].buildingSize.x / 2f, archersList[i].buildingSize.y / 2f);
                Vector3 diff = enemyGridPosition - cellCenter;
                float CellDistance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
                if(i == 1)Debug.Log($"CellDistance: {CellDistance}, diff: {diff}, cellCenter: {cellCenter}, enemy: {enemyGridPosition}");
                if (CellDistance > archersList[i].attackRange) continue;
                if (CellDistance <= minDistance)
                {
                    minDistance = CellDistance;
                    archersList[i].target = enemyList[k];
                }
            }
            if (archersList[i].target == null) archersList[i].state = ArcherState.Idle;
        }
    }
    public void TickState()
    {
        int count = archersList.Count;
        int lowestInactive = enemyManager.lowestInactive;
        for (int i = 0; i < count; i++)
        {
            switch (archersList[i].state)
            {
                case ArcherState.Idle:
                    archersList[i].animator.SetAnimation(0);
                    if (archersList[i].target != null && archersList[i].target.index < lowestInactive && archersList[i].target.HP > 0)
                    {
                        archersList[i].state = ArcherState.Shooting;
                    }
                    break;
                case ArcherState.Shooting:
                    Vector2 direction = (archersList[i].target.transform.position - transform.position).normalized;
                    archersList[i].animator.SetDirectionAnimation(0, direction);
                    break;
            }
        }
    }
    public void AnimatorTick(float delta)
    {
        if(!animate) return;
        int count = archersList.Count;
        for (int i = 0; i < count; i++){
            archersList[i].TickAnimator(delta);
            if (archersList[i].ProjectileFlag)
            {
                projectileManager.SendProjectile(archersList[i].projectileData);
                archersList[i].ProjectileFlag = false;
            }
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
        int count = archersList.Count;
        for (int i = 0; i < count; i++)
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