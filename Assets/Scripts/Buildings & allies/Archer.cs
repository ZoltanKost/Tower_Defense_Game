using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Archer : MonoBehaviour{
    const float cos22_5 = 0.092f;
    const float cos90 = 0;
    const float cos180 = -1;
    const int IDLEANIMATION = 0;
    const int SHOOTANIMATION_TOP = 2;
    const int SHOOTANIMATION_TOP_RIGHT = 3;
    const int SHOOTANIMATION_RIGHT = 1;
    const int SHOOTANIMATION_BOT_RIGHT = 5;
    const int SHOOTANIMATION_BOT = 4;

    [SerializeField] private CustomAnimator animator;
    [SerializeField] private Sprite arrow;
    [SerializeField] public ProjectileData projectileData;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;
    public IDamagable target;
    [SerializeField] private int damage;
    public bool _active;
    public bool ProjectileFlag;
    public float attackRange;
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
    Vector3 moveTarget;
    int currentMovementIndex;
    public ArcherState state;
    //List<Enemy> enemyList;
    public void Init(){
        animator.Init();
        //enemyList = enemies;
        _active = true;
        projectileData.startPosition = transform.position;
        projectileData.target = target;
        projectileData.speed = projectileSpeed;
        projectileData.damage = damage;
        projectileData.behaviour = OnProjectileMeetTargetBehaviour.Damage | OnProjectileMeetTargetBehaviour.StayIfMissed;
    }
    public void TickAnimator(float delta){
        if (!_active) return;
        animator.UpdateAnimator(delta);
    }
    public void TickState(float delta){
        switch (state)
        {
            case ArcherState.Idle:
                animator.SetAnimation(0);
                if (target != null && target.active && target.alive)
                {
                    state = ArcherState.Shooting;
                }
                break;
            case ArcherState.Dead:
                break;
            case ArcherState.Shooting:
                Vector2 direction = (target.position - transform.position).normalized;
                bool y = direction.y > 0;
                float cos = direction.x / direction.magnitude;
                float cosAbs = Mathf.Abs(cos);
                int res = 0;
                Debug.Log(cosAbs);
                if (cosAbs >= 1 - cos22_5)
                {
                    if (cosAbs <= (2 * cos22_5 - 1))
                    {
                        res = SHOOTANIMATION_TOP_RIGHT; // forward
                    }
                    else
                    {
                        res = SHOOTANIMATION_RIGHT; // forward
                    }
                }
                else
                {
                    res = SHOOTANIMATION_TOP;// forward
                }
                if (cos < 0) transform.localScale = new Vector2(-1,1);// forward
                else transform.localScale = Vector2.one;
                if (!y && res != SHOOTANIMATION_RIGHT)
                {
                    res += 2;
                }
                animator.SetAnimation(res);
                break;
            case ArcherState.Moving:
                Vector3 dir = moveTarget - transform.position;
                if (dir.magnitude <= 0.3f)
                {
                    currentMovementIndex++;
                    if (currentMovementIndex >= movement.Length) {state = ArcherState.Idle; break; }
                    moveTarget = movement[currentMovementIndex];
                }
                else
                {
                    Debug.Log(delta);
                    transform.position += dir.normalized * moveSpeed * delta;
                }
                //animator.SetAnimation(2);
                break;
        }
    }
    public void SetMovement(Vector3[] movement)
    {
        this.movement = movement;
        currentMovementIndex = 0;
        moveTarget = movement[0];
        state = ArcherState.Moving;
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