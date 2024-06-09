using UnityEngine;

public class Projectile : MonoBehaviour, IProjectile{
    [SerializeField] private Transform visuals;
    [SerializeField] private int damage;
    [SerializeField] private CustomAnimator animator;
    IAttacking _parent;
    IDamagable _target; 
    public IDamagable target{get{return _target;}}
    public bool _alive;
    public bool active{
        get {return _active;}
    }
    public bool alive{get => _alive; set {_alive = value;}}
    void Awake(){
        if(animator == null) animator = visuals.GetComponent<CustomAnimator>();
        animator.Init();
        visuals.gameObject.SetActive(false);
    }

    float speed;
    public bool _active;
    public void Init(IAttacking parent, int damage){
        _parent = parent;
        this.damage = damage;
        visuals.gameObject.SetActive(false);
        _alive = true;
    }
    public void Send(IDamagable target, float speed){
        if(!alive) return;
        _active = true;
        transform.position = _parent.position;
        _target = target;
        visuals.gameObject.SetActive(true);
        animator.SetAnimation(0);
        this.speed = speed;
    }
    public void UpdateAnimator(float delta){
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta){
        Vector3 dir = target.position - _parent.position;
        dir.z = 0;
        float angle = Vector2.Angle(Vector2.right,dir);
        if(_parent.position.y > target.position.y) angle = -angle;
        Quaternion rot = Quaternion.Euler(0f,0f,angle);
        visuals.rotation = rot;
        transform.position += delta * speed * dir.normalized;
        if((transform.position - target.position).magnitude < .3f){
            animator.SetAnimation(1);
            Disable();
            if(!target.alive) {
                animator.SetAnimation(2);
            }
            target.Damage(damage);
        }
    }
    public void Disable(){
        _active = false;
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
        _active = true;
        visuals.gameObject.SetActive(true);
    }
}