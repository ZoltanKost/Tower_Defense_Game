using UnityEngine;

public class Projectile : MonoBehaviour{
    [SerializeField] private Transform visuals;
    [SerializeField] private CustomAnimator animator;
    IDamagable target;
    Vector3 targetPosition;
    int damage;
    float speed;
    public bool meetTarget;
    public OnProjectileMeetTargetBehaviour behaviour;
    public int HitAnimation = 2;
    public bool active;
    public bool enable;
    void Awake(){
        if(animator == null) animator = visuals.GetComponent<CustomAnimator>();
        animator.Init();
        visuals.gameObject.SetActive(false);
    }
    public void Send(ProjectileData data){
        active = true;
        enable = true;
        transform.position = data.startPosition;
        target = data.target;
        animator.animations = data.animations;
        animator.PlayAnimation(0);
        speed = data.speed;
        damage = data.damage;
        behaviour = data.behaviour;
        visuals.gameObject.SetActive(true);
        targetPosition = data.targetPosition;
    }
    public void UpdateAnimator(float delta){
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta){
        Vector3 dir = targetPosition - transform.position;
        dir.z = 0;
        float angle = Vector2.Angle(Vector2.right,dir);
        if(dir.y < 0) angle = -angle;
        Quaternion rot = Quaternion.Euler(0f,0f,angle);
        visuals.rotation = rot;
        transform.position += delta * speed * dir.normalized;
        if(dir.magnitude < .3f){
            active = false;
            enable = false;
            if ((behaviour & OnProjectileMeetTargetBehaviour.StayIfMissed) != 0
                && !target.alive)
            {
                enable = true;
                animator.SetAnimation(1);
            }
            if ((behaviour & OnProjectileMeetTargetBehaviour.Damage) != 0)
            {
                target.Damage(damage);
            }
            if ((behaviour & OnProjectileMeetTargetBehaviour.Animate) != 0)
            {
                animator.SetAnimation(1);
                enable = true;
            }
            if((behaviour & OnProjectileMeetTargetBehaviour.CastSpell) != 0)
            {
                //target.
            }
        }
    }
    public void Disable(){
        active = false;
        enable = false;
    }
    public void HideVisuals(){
        visuals.gameObject.SetActive(false);
    }
    public void Deactivate(){
        Disable();
        HideVisuals();
    }
    public void Activate()
    {
        active = true;
        visuals.gameObject.SetActive(true);
    }
}
public enum TargetType
{
    Building,
    Character,
    Enemy,
    Position
}