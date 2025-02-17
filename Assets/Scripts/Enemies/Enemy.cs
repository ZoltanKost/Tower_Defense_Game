using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CustomAnimator))]
public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] public AudioSource audioSource;
    Action<int> damageCastleEvent;
    Action<int> onKillEvent;
    Action<int,int> onRemoveEvent;
    public bool ProjectileFlag;
    public ProjectileData ProjectileData;
    public int HP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }
    [SerializeField] private AttackType _attackType;
    public AttackType attackType
    {
        get { return _attackType; }
    }
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private float projectileSpeed;

    public EnemyState state;

    [SerializeField] private int damage;
    public BuildingObject currentTarget;
    [SerializeField] public CustomAnimator animator;
    public List<PathCell> currentPath;
    public int index;
    [SerializeField] public float speed;
    [SerializeField] public int attackRange = 3;
    [SerializeField] private int MaxHP = 100;
    [SerializeField] public float attackPeriod = 5f;
    [SerializeField] private int _killReward = 5;
    public int killReward { get => _killReward; }
    public float time;
    int currentHP;
    public Vector3 destination;
    int waveIndex;
    public bool detectFlag;
    public int pointsLeft;
    public int pathLength;

    void Awake()
    {
        if(animator == null) animator = GetComponent<CustomAnimator>();
        animator.Init();
    }

    public void Damage(int damage)
    {
        if (state == EnemyState.dead) return;
        HP -= damage;
        hpBar.Set((float)HP / MaxHP);
        Animate();
        audioSource.pitch = UnityEngine.Random.Range(0.6f,1.2f);
        audioSource.Play();
        if (HP <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        state = EnemyState.dead;
        hpBar.gameObject.SetActive(false);
        animator.PlayAnimation(0);
        onKillEvent?.Invoke(index);
    }

    public void RemoveInvoke()
    {
        onRemoveEvent?.Invoke(index,waveIndex);
    }

    public void Init(Enemy prefab, int waveIndex, int index, List<PathCell> path, Action<int, int> onRemoveEvent, Action<int> onKillEvent, Action<int> damageCastleEvent)
    {
        this.onKillEvent = onKillEvent;
        this.onRemoveEvent = onRemoveEvent;
        this.damageCastleEvent = damageCastleEvent;
        this.waveIndex = waveIndex;
        this.index = index;
        currentPath = new List<PathCell>(path);
        PathCell start = path[path.Count - 1];
        pointsLeft = path.Count;
        transform.position = start.pos;
        destination = start.pos;
        state = EnemyState.run;
        MaxHP = prefab.MaxHP;
        currentHP = prefab.MaxHP;
        speed = prefab.speed;
        damage = prefab.damage;
        attackRange = prefab.attackRange;
        attackPeriod = prefab.attackPeriod;
        _killReward = prefab.killReward;
        projectileSpeed = prefab.projectileSpeed;
        _attackType = prefab.attackType;
        ProjectileData = prefab.ProjectileData;
        currentTarget = null;
        animator.InitFromPrefab(prefab.animator);
        animator.SetSortingParams(6 + 1000 / start.gridY,start.floor);
        hpBar.gameObject.SetActive(true);
        hpBar.Set(1);
        animator.SetDirectionAnimation(0, (destination - start.pos).normalized);
    }
    
    public void UpdateAnimator(float delta)
    {
        animator.UpdateAnimator(delta);
    }

    public void DamageCastle()
    {
        damageCastleEvent?.Invoke(damage);
        onRemoveEvent?.Invoke(index, waveIndex);
    }
    
    // UNITY EDITOR CALLBACK
    public void Attack()
    {
        switch (attackType)
        {
            case AttackType.Melee:
                currentTarget.Damage(damage);
                break;
            case AttackType.Projectile:
                ProjectileFlag = true;
                ProjectileData.speed = projectileSpeed;
                ProjectileData.target = currentTarget;
                //ProjectileData.sprite =
                ProjectileData.damage = damage;
                ProjectileData.startPosition = transform.position;
                ProjectileData.targetPosition = currentTarget.position;
                break;
        }
        time = 0;
    }

    //CALLED BY CUSTOMANIMATOR(IN UNITY)
    public void ResetState()
    {
        state = EnemyState.idle;
    }

    public void Animate()
    {
        Tween tween = transform.DOScale(.99f, .05f);
        tween.onComplete += () => {
            Tween tween1 = transform.DOScale(1.01f, .1f);
            tween1.onComplete += () => transform.DOScale(1, .05f);
        };
    }
    
    public Sprite GetSprite()
    {
        return  animator.spriteRenderer.sprite;
    }
}
public enum EnemyState
{
    idle,
    run,
    dead,
    attack
}
