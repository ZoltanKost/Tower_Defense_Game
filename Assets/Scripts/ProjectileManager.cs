using System;
using UnityEngine;
using TMPro;
public class ProjectileManager : MonoBehaviour {
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private Projectile projectilePrefab;
    Projectile[] projectiles;
    [SerializeField] private float FreeFallAccelleration;
    [SerializeField] private TMP_InputField text;
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
        projectiles[index].Send(data, FreeFallAccelleration);
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
        for (int i = 0; i < Count; i++) {
            if (!projectiles[i].enable) RemoveProjectile(i);
            if (!projectiles[i].active) continue;
            Projectile projectile = projectiles[i];
            float angle = Vector2.Angle(Vector2.right, projectile.moveVector);
            if (projectile.moveVector.y < 0) angle = -angle;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            projectile.visuals.rotation = rot;
            projectile.transform.position += projectile.moveVector * delta;
            if(projectile.ballistic)projectile.moveVector.y -= FreeFallAccelleration * delta;
            projectile.time += delta;
            if (projectile.time >= projectile.maxTime)
            {
                projectile.audioSource.Play();
                projectile.active = false;
                projectile.enable = false;
                if ((projectile.behaviour & OnProjectileMeetTargetBehaviour.StayIfMissed) != 0
                    && !(projectile.target.HP > 0))
                {
                    projectile.enable = true;
                    projectile.animator.SetAnimation(1, angle);
                }
                if ((projectile.behaviour & OnProjectileMeetTargetBehaviour.Damage) != 0)
                {
                    projectile.target.Damage(projectile.damage);
                }
                if ((projectile.behaviour & OnProjectileMeetTargetBehaviour.Animate) != 0)
                {
                    projectile.animator.SetAnimation(1, angle);
                    projectile.enable = true;
                }
                if ((projectile.behaviour & OnProjectileMeetTargetBehaviour.CastSpell) != 0)
                {
                    spellManager.CastSpell(projectile.spellSO,projectile.transform.position);
                }
            }
            if (!projectile.enable) RemoveProjectile(i);
        }
    }
    public void AnimatorTick(float delta)
    {
        for(int i = 0; i < Count; i++){
            if (projectiles[i].enable)
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
    public void DestroyEntities()
    {
        for(int i = 0; i < projectiles.Length; i++){
            if(projectiles[i] != null){
                Destroy(projectiles[i].gameObject);
            }
        }
    }
    public void ReadGValue()
    {
        FreeFallAccelleration = float.Parse(text.text);
    }
}
[Serializable]
public struct ProjectileData
{
    public AudioClip hitClip;
    public float audioVolume;
    public SpellSO spell;
    public IDamagable target;
    public Vector3 targetPosition;
    public Vector3 startPosition;
    public float speed;
    public Animation[] animations;
    public int damage;
    public OnProjectileMeetTargetBehaviour behaviour;
    public bool ballistic;
    public float flightTime;
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