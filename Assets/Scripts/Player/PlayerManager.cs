using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public delegate void OnDamage(float HP);
    OnDamage onDamage;
    Action onPlayerLost;
    [SerializeField] int MaxHP = 100;
    int currentHp;
    public void Init(Action onPlayerLost, OnDamage onDamage){
        this.onDamage = onDamage;
        this.onPlayerLost = onPlayerLost;
        currentHp = MaxHP;
    }
    public void Damage(int damage){
        currentHp -= damage;
        onDamage?.Invoke((float)currentHp/MaxHP);
        if(currentHp <= 0){
            currentHp = MaxHP;
            onPlayerLost?.Invoke();
        }
    }
}