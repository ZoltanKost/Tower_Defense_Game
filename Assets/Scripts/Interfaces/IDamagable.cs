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
public interface IBuildable : IDamagable{
    public void Init(int sortingOrder, int sortingLayer, int index, OnKillEvent OnKill);
    public void TickUpdate(float delta);
    public void InitArchers(Enemy[] enemies);
}