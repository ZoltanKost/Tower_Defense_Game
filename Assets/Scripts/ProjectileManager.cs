using System;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
    [SerializeField] private Projectile projectilePrefab;
    Projectile[] projectiles;
    int Count;
    bool active = false;
    public void Init()
    {
        projectiles = new Projectile[16];
        for(int i = 0; i < 16; i++)
        {
            projectiles[i] = Instantiate(projectilePrefab, transform);
        }   
    }
    public void SendProjectile(ProjectileData data){
        int index = Count++;
        projectiles[index].Send(data);
        if(Count>= projectiles.Length)
        {
            Resize();
        }
    }
    void Resize()
    {
        Array.Resize(ref projectiles, Count * 2);
        for (int i = Count; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectilePrefab,transform);
        }
    }
    public void RemoveProjectile(int index){
        Projectile b = projectiles[index];
        projectiles[index] = projectiles[--Count];
        //projectiles[index].index = index;
        projectiles[Count] = b;
        b.Deactivate();
    }
    void Update(){
        if(!active) return;
        AnimatorTick(Time.deltaTime);
    }
    private void FixedUpdate() {
        if(!active) return;
        Tick(Time.fixedDeltaTime);
    }
    public void Tick(float delta)
    {
        for(int i = 0; i < Count; i++){
            if(!projectiles[i].active) continue;
            projectiles[i].Move(delta);
            if (!projectiles[i].enable) RemoveProjectile(i);
        }
    }
    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < Count; i++){
            projectiles[i].UpdateAnimator(delta);
        }
    }
    public void Switch(bool active)
    {
        this.active = active;
    }
    public void ResetEntities()
    {
        for(int i = 0; i < Count; i++){
           projectiles[i].Deactivate();
        }
    }
    public void ClearEntities()
    {
        for(int i = 0; i < projectiles.Length; i++){
            if(projectiles[i] != null){
                Destroy(projectiles[i].gameObject);
            }
        }
    }
}
[Serializable]
public struct ProjectileData
{
    public IDamagable target;
    public Vector3 startPosition;
    public float speed;
    public Sprite sprite;
    public int damage;
    public OnProjectileMeetTargetBehaviour behaviour;
}
[Flags]
public enum OnProjectileMeetTargetBehaviour
{
    None = 0,
    Animate = 1, // 0x0001
    CastSpell = 2, // 0x0010
    Damage = 4, // 0x0100
    StayIfMissed = 8 // 0x1000
}