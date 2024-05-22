using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour, IHandler {
    List<IProjectile> projectiles = new List<IProjectile>();
    bool active = false;
    public void AddProjectile(IProjectile projectile){
        projectiles.Add(projectile);
    }
    public void RemoveProjectile(IProjectile projectile){
        projectiles.Remove(projectile);
    }
    void Update(){
        if(!active) return;
        float dt = Time.deltaTime;
        Tick(dt);
        AnimatorTick(dt);
    }
    public void Tick(float delta)
    {
        for(int i = 0; i < projectiles.Count; i++){
            if(!projectiles[i].active) continue;
            projectiles[i].Move(delta);
        }
    }

    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < projectiles.Count; i++){
            projectiles[i].UpdateAnimator(delta);
        }
    }

    public void Switch(bool active)
    {
        this.active = active;
    }

    public void DeactivateEntities()
    {
        for(int i = 0; i < projectiles.Count; i++){
            projectiles[i].Deactivate();
        }
    }

    public void ResetEntities()
    {
        for(int i = 0; i < projectiles.Count; i++){
            if(projectiles[i] != null){
                Destroy(projectiles[i].gameObject);
            }
        }
        projectiles.Clear();
    }
}