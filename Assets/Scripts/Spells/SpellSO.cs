using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellSO", menuName = "SpellSO", order = 0)]
public class SpellSO : ScriptableObject {
    public SpellData spellData;
    public Animation[] animations;
    public SpellSpawnData spawnData;
}

[Serializable]
public struct SpellData
{
    public int manaCost;
    public int goldCost;
    public int radius;
    public int damage;
    public bool targeted;
    public SpellAction spellAction;
    public Sprite UIicon;
    // animation sprites
    // targeted/area
}
[Serializable]
public struct SpellSpawnData
{
    public SpellSO spell;
    public ProjectileData projectile;
    public Vector3[] deltaPosToSpawn;
    public int repeat;
}
public enum SpellAction
{
    Damage,
    SpawnProjectiles,
    SpawnSpells
}
