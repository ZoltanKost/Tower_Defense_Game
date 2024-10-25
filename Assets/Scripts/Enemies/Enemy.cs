using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CustomAnimator))]
public class Enemy : MonoBehaviour, IDamagable, IAttacking
{
    Action<int> damageCastleEvent;
    OnKillEvent onKillEvent;
    OnKillEvent onRemoveEvent;
    public bool ProjectileFlag;
    public ProjectileData ProjectileData;
    public int HP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }
    public Vector3 position
    {
        get { return transform.position; }
    }

    public bool active
    {
        get { return _active; }
        set { _active = value; }
    }

    public bool alive { get { return active && state != EnemyState.dead; } }
    [SerializeField] private AttackType _attackType;
    public AttackType attackType
    {
        get { return _attackType; }
    }
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private float projectileSpeed;

    private EnemyState state;

    [SerializeField] private int damage;
    IDamagable[] targets;
    IDamagable currentTarget;
    private CustomAnimator animator;
    Queue<Vector3> currentPath;
    public int index;
    private bool _active;
    [SerializeField] public float speed;
    [SerializeField] private float attackrange = 3f;
    [SerializeField] private int MaxHP = 100;
    [SerializeField] private float attackPeriod = 5f;
    [SerializeField] private int _killReward = 5;
    public int killReward { get => _killReward; }
    float time;
    int currentHP;
    public Vector3 destination;
    void Awake()
    {
        animator = GetComponent<CustomAnimator>();
        animator.Init();
    }
    public void Damage(int damage)
    {
        if (state == EnemyState.dead) return;
        HP -= damage;
        hpBar.Set((float)HP / MaxHP);
        Animate();
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
        onRemoveEvent?.Invoke(index);
    }
    public void Init(Queue<Vector3> path, Vector3 position, bool active, OnKillEvent onRemoveEvent, OnKillEvent onKillEvent, Action<int> damageCastleEvent)
    {
        this.onKillEvent = onKillEvent;
        this.onRemoveEvent = onRemoveEvent;
        this.damageCastleEvent = damageCastleEvent;
        Pathfinding_SetPath(path);
        transform.position = position;
        destination = position;
        _active = active;
        state = EnemyState.run;
        currentHP = MaxHP;
        hpBar.gameObject.SetActive(true);
        hpBar.Set(1);
        animator.SetDirectionAnimation(0, (destination - position).normalized);
    }
    public void Pathfinding_SetPath(Queue<Vector3> path)
    {
        currentPath = path;
    }
    public void Tick(float delta)
    {
        switch (state)
        {
            case EnemyState.dead:
                return;
            case EnemyState.run:
                animator.SetDirectionAnimation(0, (destination - position).normalized);
                Move(delta);
                time += delta;
                if (time < attackPeriod) return;
                Detect();
                return;
            case EnemyState.attack:
                time = 0;
                animator.SetDirectionAnimation(1, (currentTarget.position - position).normalized);
                break;
            default:
                state = EnemyState.run;
                break;
        }
    }
    public void UpdateAnimator(float delta)
    {
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta)
    {
        transform.position += (destination - transform.position).normalized * delta * speed;
        if ((destination - transform.position).magnitude <= .1f)
        {
            if (currentPath.Count > 0) destination = currentPath.Dequeue();
            else DamageCastle();
        }
    }
    public void DamageCastle()
    {
        damageCastleEvent?.Invoke(damage);
        onRemoveEvent?.Invoke(index);
    }

    public void SetEnemyPool(IDamagable[] enemies)
    {
        targets = enemies;
        // Debug.Log($"Targets != null : {targets!=null}; enemies != null {enemies != null};");
    }

    public void Detect()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null || !targets[i].active || !targets[i].alive) continue;
            IDamagable t = targets[i];
            float distance = (t.position - position).magnitude;
            if (distance > attackrange) continue;
            float minDistance;
            if (currentTarget == null || !currentTarget.active || !currentTarget.alive) minDistance = attackrange;
            else minDistance = (currentTarget.position - position).magnitude;
            // Debug.Log($"Min: {minDistance}, distance: {distance};");
            if (distance >= minDistance) continue;
            currentTarget = t;
        }
        bool attack = currentTarget != null && currentTarget.active && currentTarget.alive;
        if (attack) state = EnemyState.attack;
    }

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