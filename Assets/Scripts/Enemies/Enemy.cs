using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomAnimator))]
public class Enemy : MonoBehaviour {
    public delegate void OnKillEvent(int index);
    public delegate void DamageCastleEvent(int damage);
    DamageCastleEvent damageCastleEvent;
    OnKillEvent onKillEvent;
    [SerializeField] private int damage;
    private CustomAnimator animator;
    Queue<Vector3> currentPath;
    public int index;
    public bool active;
    public bool alive;
    public float speed;
    public int HP = 100;
    Vector3 destination;
    void Awake(){
        animator = GetComponent<CustomAnimator>();
    }
    public void Damage(){
        if(!alive) return;
        HP -= 10;
        if(HP <= 0){
            KillEnemy();
        }
    }
    public void KillEnemy(){
        alive = false;
        animator.PlayAnimation(1);
    }
    public void OnKillInvoke(){
        onKillEvent?.Invoke(index);
    }
    public void Init(Queue<Vector3> path, Vector3 position, bool active, OnKillEvent onKillEvent, DamageCastleEvent damageCastleEvent){
        this.onKillEvent = onKillEvent;
        this.damageCastleEvent = damageCastleEvent;
        Pathfinding_SetPath(path);
        transform.position = position;
        destination = position;
        this.active = active;
        alive = true;
        HP = 100;
        animator.PlayAnimation(0);
    }
    public void Pathfinding_SetPath(Queue<Vector3> path)
    {
        currentPath = path;
    }
    public void Tick(float delta){
        if(alive) Move(delta);
        UpdateAnimator(delta);
    }
    public void UpdateAnimator(float delta){
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta){
        transform.position += (destination - transform.position).normalized * delta;
        if((destination - transform.position).magnitude <= .1f){ 
            if(currentPath.Count > 0) destination = currentPath.Dequeue();
            else DamageCastle();
        }
    }
    public void DamageCastle(){
        damageCastleEvent?.Invoke(damage);
        onKillEvent?.Invoke(index);
    }
}