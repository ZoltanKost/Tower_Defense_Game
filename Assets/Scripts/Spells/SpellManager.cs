using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {
    [SerializeField] private CustomAnimator prefab;
    [SerializeField] private EnemyManager enemyManager;
    //[SerializeField] private AnimationManager animationManager;
    List<CustomAnimator> spells = new List<CustomAnimator>();
    public void CastSpell(SpellData spell, Vector3 position)
    {
        switch (spell.spellType) 
        {
            case SpellTarget.Area:
                CustomAnimator animator = Instantiate(prefab);
                spells.Add(animator);
                position.z = 0;
                animator.transform.SetPositionAndRotation(position,Quaternion.identity);
                animator.animations = new Animation[1];
                animator.animations[0] = spell.animation;
                animator.Init();
                animator.animations[0].events[0].action.AddListener(() => enemyManager.AreaSpell(spell, position));
                animator.animations[0].onAnimationEnd += () => { spells.Remove(animator); animator.gameObject.SetActive(false);};
                break;
            case SpellTarget.Projectile:
                break;
            case SpellTarget.Targeted:
                break;
            case SpellTarget.Untargeted:
                break;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < spells.Count; i++)
        {
            spells[i].UpdateAnimator(dt);
        }
    }
}