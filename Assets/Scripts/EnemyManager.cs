using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour{
    [SerializeField] Enemy enemy;
    [SerializeField] float rate;
    float time;
    List<Enemy> enemies;
    Queue<Vector3> roads;
    void Awake(){
        roads = new();
    }
    void FixedUpdate(){
        float delta = Time.fixedDeltaTime;
        foreach(Enemy enemy in enemies){
            enemy.Tick(delta);
        }
        
        time += delta;
        if(time >= rate){
            time = 0;
            Enemy newEnemy = Instantiate(enemy);
            enemies.Add(newEnemy);
            newEnemy.Send(roads);
            newEnemy.OnEnemyKilled += OnEnemyKilled;
        }
    }
    void OnEnemyKilled(object enemy, System.EventArgs e){
        Enemy killed = enemy as Enemy;
        if(killed == null) return;
        enemies.Remove(killed);
        killed.OnEnemyKilled -= OnEnemyKilled;
    }
}