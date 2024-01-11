using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{
    public EventHandler OnEnemyKilled;
    [SerializeField] private float speed;
    [SerializeField] private float _HP; 
    Queue<Vector3> roads;
    Vector3 targetPosition;
    public void Send(Queue<Vector3> roads){
        this.roads = roads;
        targetPosition = transform.position;
    }
    public void Tick(float delta){
        GetMovePosition();
        Move(delta);
    }
    void GetMovePosition(){
        if(Vector3.Distance(transform.position, targetPosition) < .2f){
            GetNextRoad();
        }
    }
    void Move(float delta){
        transform.position += (targetPosition - transform.position).normalized * speed * delta;
    }
    void GetNextRoad(){
        targetPosition = roads.Dequeue();
    }
    public void Damage(float damage){
        _HP -= damage;
        if(_HP <= 0){
            KillEnemy();
        }
    }
    public void KillEnemy(){
        OnEnemyKilled?.Invoke(this,EventArgs.Empty);
    }
}