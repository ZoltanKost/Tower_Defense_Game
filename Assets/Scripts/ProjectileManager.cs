using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
    List<IProjectile> projectiles = new List<IProjectile>();
    public void AddProjectile(IProjectile projectile){
        projectiles.Add(projectile);
    }
    public void RemoveProjectile(IProjectile projectile){
        projectiles.Remove(projectile);
    }
    void Update(){
        float delta = Time.deltaTime;
        for(int i = 0; i < projectiles.Count; i++){
            if(!projectiles[i].active) continue;
            projectiles[i].Move(delta);
        }
    }
    public void LateUpdate(){
        float delta = Time.deltaTime;
        for(int i = 0; i < projectiles.Count; i++){
            projectiles[i].UpdateAnimator(delta);
        }
    }
}