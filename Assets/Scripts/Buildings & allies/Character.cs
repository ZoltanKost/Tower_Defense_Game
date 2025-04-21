using System;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour, IDamagable
{
    [HeaderAttribute("PrefabData")]
    // Prefab Data
    [SerializeField] private Sprite arrow;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private int MaxHP = 100;
    [SerializeField] public int attackRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float attackPeriod = 5f;
    [SerializeField] private AttackType _attackType;
    [SerializeField] private OnProjectileMeetTargetBehaviour projectileBehaviour;
    [SerializeField] private CharacterType characterType;
    [SerializeField] public int killReward = 5;

    [HeaderAttribute("MonoBeh Dependencies")]
    // Monobehs 
    [SerializeField] public Animator animator;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private HealthBar hpBar;

    [HeaderAttribute("Common Data")]
    // Common Data
    public bool active;
    public int floor;
    public bool ProjectileFlag;
    public bool removeFlag;
    public bool detectFlag;
    public int index;
    public bool shooting;
    public CharState state;
    public Vector3[] movement;
    public List<PathCell> currentPath;

    // Common Events
    Action<int> onKillEvent;
    Action<int, int> onRemoveEvent;
    
    [HeaderAttribute("Projectile Data")]
    // Projectile Data
    [SerializeField] public ProjectileData projectileData;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float ProjectileFlightTime;
    [SerializeField] public int damage;

    [HeaderAttribute("Archer Data")]
    // Archer Data
    public Character target;
    public Vector2Int buildingSize;
    public Vector2Int gridPosition;
    public int buildingID;
    int currentHP;
    public Vector3 destination;

    [HeaderAttribute("Enemy Data")]
    // Enemy Data
    public bool damageCastleFlag;
    public int pointsLeft;
    public int waveIndex;
    public int castleDamage = 10;
    public BuildingObject buildingTarget;
    
    // Getters
    public int HP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }
    public AttackType attackType
    {
        get { return _attackType; }
    }
    public Vector3 position
    {
        get { return transform.position; }
    }
    [HideInInspector]
    public float time;

    public void Init(Character prefab, int friendBuildingID, 
        int friendBuildingWidth, int friendBuildingHeight, int waveIndex, 
        int index, Vector2Int _gridPosition, List<PathCell> path, 
        Action<int, int> onRemoveEvent, Action<int> onKillEvent,
        CharacterType type)
    {
        this.onKillEvent = onKillEvent;
        this.onRemoveEvent = onRemoveEvent;
        this.waveIndex = waveIndex;
        this.index = index;
        
        state = CharState.Idle;
        MaxHP = prefab.MaxHP;
        currentHP = prefab.MaxHP;
        moveSpeed = prefab.moveSpeed;
        damage = prefab.damage;
        attackRange = prefab.attackRange;
        attackPeriod = prefab.attackPeriod;
        killReward = prefab.killReward;
        projectileSpeed = prefab.projectileSpeed;
        _attackType = prefab.attackType;
        projectileData = prefab.projectileData;
        damageClip = prefab.damageClip;
        buildingTarget = null;

        buildingID = friendBuildingID;
        buildingSize = new(friendBuildingWidth, friendBuildingHeight);
        gridPosition = _gridPosition;
        //animator.Init();
        
        projectileData.startPosition = transform.position;
        projectileData.speed = prefab.projectileSpeed;
        projectileData.damage = prefab.damage;
        characterType = type;
        projectileData.behaviour = prefab.projectileBehaviour;
        animator.Play(0);

        if (characterType == CharacterType.Enemy)
        {
            currentPath = new List<PathCell>(path);
            PathCell start = path[path.Count - 1];
            pointsLeft = path.Count;
            transform.position = start.pos;
            destination = start.pos;
            animator.runtimeAnimatorController = prefab.animator.runtimeAnimatorController;
            float degree = Vector2.SignedAngle(Vector2.right, (destination - start.pos).normalized);
            if (degree < 0) degree += 360;
            degree %= 360;
            animator.Play(0);
            hpBar.gameObject.SetActive(true);
            hpBar.Set(1);
        }
        else currentPath = new List<PathCell>();
    }
    // Unity Editor Callback
    public void Attack()
    {
        switch (characterType)
        {
            case CharacterType.Enemy:
                switch (attackType)
                {
                    case AttackType.Melee:
                        buildingTarget.Damage(damage);
                        audioSource.clip = damageClip;
                        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
                        audioSource.Play();
                        break;
                    case AttackType.Projectile:
                        ProjectileFlag = true;
                        projectileData.speed = projectileSpeed;
                        projectileData.target = buildingTarget;
                        //ProjectileData.sprite =
                        projectileData.damage = damage;
                        projectileData.startPosition = transform.position;
                        projectileData.targetPosition = buildingTarget.position;
                        break;
                }
                break;
            case CharacterType.Friend:
                ProjectileFlag = true;
                projectileData.startPosition = transform.position;
                projectileData.target = target;
                projectileData.speed = projectileSpeed;
                projectileData.damage = damage;
                projectileData.flightTime = projectileData.ballistic ?
                    ProjectileFlightTime :
                    (target.transform.position - transform.position).magnitude / projectileData.speed;
                float enemyTimeToDest =
                    (target.destination - target.transform.position).magnitude
                    / target.moveSpeed;
                float time; Vector3 direction; Vector3 start;
                if (enemyTimeToDest < projectileData.flightTime && target.pointsLeft > 0)
                {
                    start = target.destination;
                    var nextDest = target.currentPath[target.pointsLeft];
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
                    target.moveSpeed * time;
                break;
        }
        time = 0;
    }
    public void Damage(int damage)
    {
        if (state == CharState.Dead) return;
        HP -= damage;
        hpBar.Set((float)HP / MaxHP);
        if (HP <= 0)
        {
            Kill();
        }
    }
    public void Kill()
    {
        state = CharState.Dead;
        hpBar.gameObject.SetActive(false);
        animator.Play(0);
        onKillEvent?.Invoke(index);
    }
    public void RemoveInvoke()
    {
        removeFlag = true;
        onRemoveEvent?.Invoke(index, waveIndex);
    }
    public void ResetState()
    {
        state = CharState.Idle;
    }
}
public enum CharState
{
    Idle,
    Attacking,
    Moving,
    Dead
}
public enum CharacterType
{
    Enemy,
    Friend
}