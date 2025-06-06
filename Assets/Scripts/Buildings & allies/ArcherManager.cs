using System.Collections.Generic;
using UnityEngine;

public class ArcherManager : MonoBehaviour, IHandler {
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Transform archer;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    public List<Character> archersList = new List<Character>();
    bool active;
    bool animate;
    float cellSize;
    Vector3 offset;
    public void SpawnArcher()
    {
        Archer _archer = Instantiate(archer, transform).GetComponentInChildren<Archer>();
        if (_archer == null) { return; }
        Vector3 temp = Camera.main.transform.position;
        temp.z = 0;
        _archer.transform.position = temp;
        //AddArcher(_archer);
    }
    public void AddArcher(Character archer, Vector2Int gridPosition, int buildingWidth, int buildingHeight, int buildingID, int attackRangeBonus)
    {
        offset = floorManager.offset;
        cellSize = floorManager.CellToWorld(Vector3.one).x;
        archersList.Add(archer);
        archer.Init(archer, buildingID, buildingWidth, 
            buildingHeight,-1,archersList.Count - 1,gridPosition,
            null,null,null, CharacterType.Friend);
        archer.attackRange = attackRangeBonus;
    }
    public void RemoveArchers(Character[] archer) {
        foreach (Character a in archer) {
            archersList.Remove(a);
            a.gameObject.SetActive(false);
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
        Character[] enemyList = enemyManager.enemies;
        float cellSize = floorManager.CellToWorld(Vector3.one).x;
        int count = archersList.Count;
        for (int i = 0; i < count; i++)
        {
            archersList[i].shooting = false;
            archersList[i].target = null;
            int lowestInactive = enemyManager.lowestInactive;
            float minDistance = archersList[i].attackRange;
            int targetPointsLeft = int.MaxValue;
            for (int k = 0; k < lowestInactive; k++)
            {
                if (!(enemyList[k].HP > 0)) continue;
                int currentPointsLeft = enemyList[k].pointsLeft;
                Vector2Int enemyGridPosition = new Vector2Int
                {
                    x = (int)((enemyList[k].transform.position.x + offset.x) / cellSize),
                    y = (int)((enemyList[k].transform.position.y + offset.y) / cellSize)
                };
                Vector2Int buildingPosition = archersList[i].gridPosition;
                Vector2Int res = enemyGridPosition - buildingPosition;
                Vector2Int celloffset = new Vector2Int
                {
                    x = Mathf.Clamp(res.x, 0, archersList[i].buildingSize.x - 1),
                    y = Mathf.Clamp(res.y, 0, archersList[i].buildingSize.y - 1)
                };
                res = res - celloffset;
                //float distance = (new Vector3(pos.x * cellSize, pos.y * cellSize)).magnitude;
                float distance = Mathf.Abs(res.x) + Mathf.Abs(res.y);
                //Debug.Log($"distance: {distance}, diff: {diff}, dBims: {bDims}, buildins: {buildingPosition}, enemy: {enemyGridPosition}");
                if (distance <= minDistance && currentPointsLeft < targetPointsLeft)
                {
                    targetPointsLeft = currentPointsLeft;
                    minDistance = distance;
                    archersList[i].target = enemyList[k];
                }
            }
            if (archersList[i].target == null) archersList[i].state = CharState.Idle;
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
                case CharState.Idle:
                    archersList[i].animator.SetAnimation(0,0);
                    if (archersList[i].target != null && archersList[i].target.index < lowestInactive && archersList[i].target.HP > 0)
                    {
                        archersList[i].state = CharState.Attacking;
                    }
                    break;
                case CharState.Attacking:
                    Vector2Int enemyGridPosition = new Vector2Int
                    {
                        x = Mathf.FloorToInt((archersList[i].target.transform.position.x + offset.x) / cellSize),
                        y = Mathf.FloorToInt((archersList[i].target.transform.position.y + offset.y) / cellSize)
                    };
                    Vector2 direction = (enemyGridPosition - (archersList[i].gridPosition + archersList[i].buildingSize/2));
                    float degree = Vector2.SignedAngle(Vector2.right, direction);
                    if (degree < 0) degree += 360;
                    archersList[i].animator.SetAnimation(1, degree);
                    break;
            }
        }
    }
    public void AnimatorTick(float delta)
    {
        if(!animate) return;
        int count = archersList.Count;
        for (int i = 0; i < count; i++){
            archersList[i].animator.UpdateAnimator(delta);
            if (archersList[i].ProjectileFlag)
            {
                //archersList[i].audioSource.pitch = Random.Range(0.9f, 1.31f);
                //archersList[i].audioSource.Play();
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
        foreach(Character a in archersList){
            a.gameObject.SetActive(false);
        }
    }
    public void ResetEntities()
    {
        foreach(Character a in archersList){
            a.ResetState();
        }
    }
    public void ClearEntities()
    {
        foreach(Character archer in archersList){
            Destroy(archer.gameObject);
        }
        archersList.Clear();
    }
    public bool TryHighlightEntity(Vector3 position, out Character archer, float radius)
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