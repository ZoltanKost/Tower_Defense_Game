using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {
    public const int CALLBACK_REMOVE = 1;
    public const int CALLBACK_CASTAREASPELL = 0;
    public const int AnimationEvent = CALLBACK_REMOVE + CALLBACK_CASTAREASPELL;
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
                animator.actions[CALLBACK_CASTAREASPELL].AddListener(() => enemyManager.AreaSpell(spell, position));
                animator.actions[CALLBACK_REMOVE].AddListener(() => { RemoveSpell(animator); });
                break;
            case SpellTarget.Projectile:
                break;
            case SpellTarget.Targeted:
                break;
            case SpellTarget.Untargeted:
                break;
        }
    }

    void RemoveSpell(CustomAnimator animator)
    {
        spells.Remove(animator); 
        animator.gameObject.SetActive(false);
    }
    void RemoveSpell(int index)
    {
        spells[index].gameObject.SetActive(false);
        spells.RemoveAt(index);
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