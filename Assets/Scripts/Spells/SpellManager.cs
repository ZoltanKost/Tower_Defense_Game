using UnityEngine;

public class SpellManager : MonoBehaviour {
    [SerializeField] private EnemyManager enemyManager;
    public void CastSpell(SpellSO spell, Vector3 position)
    {
        switch (spell.spellType) 
        {
            case SpellTarget.Area:
                var g = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Quad));
                g.AddComponent<CustomAnimator>();
                CustomAnimator animator = g.GetComponent<CustomAnimator>();
                animator.animations = new Animation[1];
                animator.animations[0] = spell.animation;
                animator.Init();
                animator.animations[0].events[0].action.AddListener(() => enemyManager.AreaSpell(spell, position));
                break;
            case SpellTarget.Projectile:
                break;
            case SpellTarget.Targeted:
                break;
            case SpellTarget.Untargeted:
                break;
        }
    }
}