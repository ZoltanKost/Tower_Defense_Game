using UnityEngine;

public delegate void OnKillEvent(int index);
public interface IDamagable{
    Vector3 position{get;}
    bool active{get;set;}
    bool alive{get;}
    int HP{get;set;}
    public void Damage(int damage);
    public void Kill();
}