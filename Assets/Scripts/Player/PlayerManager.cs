using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    Action<float> onHpChange;
    Action<float> onManaChange;
    Action onPlayerLost;
    [SerializeField] float MaxMana = 100f;
    [SerializeField] float MaxHP = 100f;
    [SerializeField] private float hpRegen = 0f;
    [SerializeField] private float manaRegen = 1f;
    public float currentHp;
    public float currentMana;
    public void Init(Action onPlayerLost, Action<float> setHp, Action<float> setMana)
    {
        onHpChange = setHp;
        onManaChange = setMana;
        this.onPlayerLost = onPlayerLost;
        currentHp = MaxHP;
        currentMana = MaxMana;
    }
    public void Damage(int damage){
        currentHp -= damage;
        onHpChange?.Invoke(currentHp/MaxHP);
        if(currentHp <= 0){
            currentHp = MaxHP;
            onPlayerLost?.Invoke();
        }
    }
    public bool RemoveMana(float count)
    {
        if (count > currentMana) return false;
        currentMana -= count;
        //Debug.Log(currentMana);
        onManaChange?.Invoke(currentHp/MaxHP);
        return true;
    }
    private void Update()
    {
        float dt = Time.deltaTime;
        currentMana += manaRegen * dt;
        currentMana = Mathf.Min(currentMana, MaxMana);
        currentHp += hpRegen * dt;
        currentHp = Mathf.Min(currentHp, MaxHP);
        onManaChange?.Invoke(currentMana/MaxMana);
        onHpChange?.Invoke(currentHp / MaxHP);
    }
}