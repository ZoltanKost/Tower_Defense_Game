using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    Queue<Vector3> currentPath;
    public int index;
    public bool active;
    public float speed;
    public int HP = 100;
    Vector3 destination;
    public void Damage(int damage){
        HP -= damage;
    }
    public void KillEnemy(){
        active = false;
        gameObject.SetActive(false);
        Debug.Log($"Enemy number {index} just killed!");
    }
    public void Init(Queue<Vector3> path, Vector3 position, bool active){
        Pathfinding_SetPath(path);
        transform.position = position;
        destination = position;
        this.active = active;
    }
    public void Pathfinding_SetPath(Queue<Vector3> path)
    {
        currentPath = path;
    }
    public void Move(float delta){
        transform.position += (destination - transform.position).normalized * delta;
        if((destination - transform.position).magnitude <= .1f){ 
            if(currentPath.Count > 0) destination = currentPath.Dequeue();
            else KillEnemy();
        }
    }
}