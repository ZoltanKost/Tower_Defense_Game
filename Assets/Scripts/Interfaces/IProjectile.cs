using UnityEngine;

public interface IProjectile{
    public GameObject gameObject{get;}
    IDamagable target{get;}
    public bool active{get;}
    public void Init(IAttacking parent, int damage);
    public void Move(float delta);
    public void UpdateAnimator(float delta);
    public void Send(IDamagable target, float speed);
    public void Activate();
    public void Deactivate();
}