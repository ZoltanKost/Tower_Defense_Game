using UnityEngine;

public class Archer : MonoBehaviour{
    [SerializeField] public CustomAnimator animator;
    [SerializeField] private Sprite arrow;
    [SerializeField] public ProjectileData projectileData;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float ProjectileFlightTime;
    //[SerializeField] private float moveSpeed = 5f;
    public Enemy target;
    [SerializeField] private int damage;
    public Vector2Int buildingSize;
    public Vector2Int gridPosition;
    public int buildingID;
    public bool _active;
    public bool ProjectileFlag;
    public int attackRange;
    public int floor;
    public Vector3 position
    {
        get{return transform.position;}
    }
    [SerializeField] private AttackType _attackType;
    public AttackType attackType{
        get{return _attackType;}
    }
    public bool shooting;
    public Vector3[] movement;
    public ArcherState state;
    public void Init(Vector2Int _gridPosition, int buildingWidth, int buildingHeight, int buildingID){
        this.buildingID = buildingID;
        buildingSize = new(buildingWidth,buildingHeight);
        gridPosition = _gridPosition;
        animator.Init();
        //enemyList = enemies;
        _active = true;
        projectileData.startPosition = transform.position;
        projectileData.speed = projectileSpeed;
        projectileData.damage = damage;
        projectileData.behaviour = OnProjectileMeetTargetBehaviour.Damage | OnProjectileMeetTargetBehaviour.StayIfMissed;
    }
    public void TickAnimator(float delta){
        if (!_active) return;
        animator.UpdateAnimator(delta);
    }
    public void ResetAnimation(){
        animator.SetAnimation(0);
    }
    public void Attack(){
        ProjectileFlag = true;
        projectileData.startPosition = transform.position;
        projectileData.target = target;
        projectileData.speed = projectileSpeed;
        projectileData.damage = damage;
        projectileData.flightTime = projectileData.ballistic?
            ProjectileFlightTime:
            (target.transform.position - transform.position).magnitude / projectileData.speed;
        float enemyTimeToDest = 
            (target.destination - target.transform.position).magnitude 
            / target.speed;
        float time; Vector3 direction; Vector3 start;
        if (enemyTimeToDest < projectileData.flightTime && target.currentPath.TryPeek(out PathCell nextDest))
        {
            start = target.destination;
            direction = nextDest.pos - target.destination;
            time = projectileData.flightTime - enemyTimeToDest;
        }
        else
        {
            start = target.transform.position;
            direction = target.destination - target.transform.position;
            time = projectileData.flightTime;
        }
        projectileData.targetPosition =
            start +
            (direction).normalized *
            target.speed * time;
    }
    public void Switch(bool active){
        _active = active;
    }
    public void Reset(){
        ResetAnimation();
    }
    public void Deactivate(){
        gameObject.SetActive(false);
        _active = false;
    }
    public void Activate(){
        gameObject.SetActive(true);
        _active = true;
    }
    public void SetColor(Color color){
        animator.spriteRenderer.color = color;
    }
    public Sprite GetSprite()
    {
        return animator.spriteRenderer.sprite;
    }
}
public enum ArcherState
{
    Idle,
    Shooting,
    Moving,
    Dead
}