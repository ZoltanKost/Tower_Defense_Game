using UnityEngine;

public class Projectile : MonoBehaviour{
    [SerializeField] private Transform visuals;
    [SerializeField] private CustomAnimator animator;
    IDamagable target;
    int damage;
    void Awake(){
        if(animator == null) animator = visuals.GetComponent<CustomAnimator>();
        animator.Init();
        visuals.gameObject.SetActive(false);
    }
    float speed;
    public bool active;
    public bool enable;
    public void Send(ProjectileData data){
        active = true;
        enable = true;
        transform.position = data.startPosition;
        target = data.target;
        visuals.gameObject.SetActive(true);
        animator.SetAnimation(0);
        animator.animations[0].sprites[0] = data.sprite;
        speed = data.speed;
        damage = data.damage;
    }
    public void UpdateAnimator(float delta){
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta){
        Vector3 dir = target.position - transform.position;
        dir.z = 0;
        float angle = Vector2.Angle(Vector2.right,dir);
        if(transform.position.y > target.position.y) angle = -angle;
        Quaternion rot = Quaternion.Euler(0f,0f,angle);
        visuals.rotation = rot;
        transform.position += delta * speed * dir.normalized;
        if((transform.position - target.position).magnitude < .3f){
            animator.SetAnimation(1);
            Disable();
            if(!target.alive) {
                animator.SetAnimation(2);
            }
            else
            {
                enable = false;
            }
            target.Damage(damage);
        }
    }
    public void Disable(){
        active = false;
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