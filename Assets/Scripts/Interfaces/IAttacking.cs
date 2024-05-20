using UnityEngine;
using System.Collections.Generic;

public enum AttackType{
    Melee,
    Projectile
}
public interface IAttacking{
    public AttackType attackType{get;}
    public Vector3 position{get;}
    public void SetEnemyPool(IDamagable[] enemies);
    public void Detect();
    public void Attack();
}