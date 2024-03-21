using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomAnimator))]
public class Enemy : MonoBehaviour {
    public delegate void OnKillEvent(int index);
    OnKillEvent onKillEvent;
    private CustomAnimator animator;
    Queue<Vector3> currentPath;
    public int index;
    public bool active;
    public float speed;
    public int HP = 100;
    Vector3 destination;
    void Awake(){
        animator = GetComponent<CustomAnimator>();
    }
    public void Damage(){
        HP -= 10;
        if(HP <= 0){
            KillEnemy();
        }
    }
    public void KillEnemy(){
        onKillEvent?.Invoke(index);
    }
    public void Init(Queue<Vector3> path, Vector3 position, bool active, OnKillEvent onKillEvent){
        this.onKillEvent = onKillEvent;
        Pathfinding_SetPath(path);
        transform.position = position;
        destination = position;
        this.active = active;
        animator.PlayAnimation(0);
    }
    public void Pathfinding_SetPath(Queue<Vector3> path)
    {
        currentPath = path;
    }
    public void Move(float delta){
        animator.UpdateAnimator(delta);
        transform.position += (destination - transform.position).normalized * delta;
        if((destination - transform.position).magnitude <= .1f){ 
            if(currentPath.Count > 0) destination = currentPath.Dequeue();
            else KillEnemy();
        }
    }
}