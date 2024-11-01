using UnityEngine;

public class Projectile : MonoBehaviour{
    [SerializeField] public Transform visuals;
    [SerializeField] public CustomAnimator animator;
    public IDamagable target;
    public Vector3 targetPosition;
    public Vector3 startPosition;
    public Vector3 moveVector;
    public int damage;
    public bool meetTarget;
    public OnProjectileMeetTargetBehaviour behaviour;
    public int HitAnimation = 2;
    public bool active;
    public bool enable;
    public float time;
    public float maxTime;
    public bool ballistic;
    void Awake(){
        if(animator == null) animator = visuals.GetComponent<CustomAnimator>();
        animator.Init();
        visuals.gameObject.SetActive(false);
    }
    public void Send(ProjectileData data, float g)
    {
        active = true;
        enable = true;
        transform.position = data.startPosition;
        startPosition = data.startPosition;
        target = data.target;
        animator.animations = data.animations;
        animator.PlayAnimation(0);
        startPosition = data.startPosition;
        damage = data.damage;
        behaviour = data.behaviour;
        ballistic = data.ballistic; 
        visuals.gameObject.SetActive(true);
        targetPosition = data.targetPosition;
        Vector2 dir = targetPosition - startPosition;
        time = 0;
        maxTime = data.flightTime; // data.flightTime;
        if (maxTime == 0) maxTime = 1f;
        if (ballistic)
        {
            float speedY = (dir.y + (g * maxTime * maxTime / 2)) / maxTime;
            float speedX = dir.x / maxTime;
            moveVector = new Vector2(speedX, speedY);
        }
        else
        {
            moveVector = dir.normalized * data.speed;
        }
    }
    public void UpdateAnimator(float delta){
        animator.UpdateAnimator(delta);
    }
    public void Move(float delta){
        
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