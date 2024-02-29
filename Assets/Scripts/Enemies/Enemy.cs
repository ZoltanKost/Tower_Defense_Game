using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    // public Pathfinding.PathfindingCallBack pathfindingCallBack;
    List<FloorCell> currentPath;
    public float speed;
    public int HP = 100;
    public void Damage(int damage){
        HP -= damage;
    }
    void Awake(){
        // pathfindingCallBack = Pathfinding_SetPath;
    }

    private void Pathfinding_SetPath(List<FloorCell> path)
    {
        currentPath = path;
    }
}