using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellSO", menuName = "SpellSO", order = 0)]
public class SpellSO : ScriptableObject {
    [Serializable]
    public struct SpellData
    {
        public int manaCost;
        public int goldCost;
        public int radius;
        public int damage;
        public SpellTarget spellType;
        public Animation animation;
        public Sprite UIicon;
        // animation sprites
        // targeted/area
    }
    public SpellData spellData;
}

public enum SpellTarget
{
    Projectile,
    Area,
    Targeted,
    Untargeted
}