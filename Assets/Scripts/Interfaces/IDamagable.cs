using UnityEngine;

public delegate void OnKillEvent(int index);
public interface IDamagable{
    public int HP{get;set;}
    public void Damage(int damage);
    public void Kill();
}