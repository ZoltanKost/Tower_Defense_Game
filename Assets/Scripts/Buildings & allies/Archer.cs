using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour{
    [SerializeField] private CustomAnimator animator;
    [SerializeField] private Sprite arrow;
    [SerializeField] public ProjectileData projectileData;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] IDamagable target;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    public bool _active;
    public bool ProjectileFlag;
    public Vector3 position{
        get{return transform.position;}
    }
    [SerializeField] private AttackType _attackType;
    public AttackType attackType{
        get{return _attackType;}
    }
    bool shooting;
    List<Enemy> enemyList;
    public void Init(List<Enemy> enemies){
        animator.Init();
        enemyList = enemies;
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
    public void TickDetection(float delta){
        if(!_active) return;
        Detect();
        if(shooting) {
            animator.SetAnimation(1);
        }
        else animator.SetAnimation(0);
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
    public void Detect(){
        shooting = false;
        for(int i = 0; i < enemyList.Count; i++){
            if(!enemyList[i].active || !enemyList[i].alive) continue;
            Vector3 vector = transform.position - enemyList[i].position;
            vector.z = 0;
            float distance = vector.magnitude;
            if(distance > attackRange) continue;
            float minDistance;
            if(target == null || !target.active || !target.alive) minDistance = attackRange; 
            else {
                vector = transform.position - target.position;
                vector.z = 0;
                minDistance = vector.magnitude;
            }
            if(distance < minDistance){
                target = enemyList[i];
            }
        }
        shooting = target != null && target.active && target.alive;
    }

    public void SetEnemyPool(List<Enemy> enemies)
    {
        enemyList = enemies;
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