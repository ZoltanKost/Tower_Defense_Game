using UnityEngine;

[CreateAssetMenu(fileName = "SpellSO", menuName = "SpellSO", order = 0)]
public class SpellSO : ScriptableObject {
    public int manaCost;
    public int goldCost;
    public SpellObject prefab;
}
public enum SpellType{
    Buff,
    Debuff,
    Projectile
}