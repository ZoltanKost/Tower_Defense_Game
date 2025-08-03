using DG.Tweening;
using System;
using UnityEngine;

public class SpellManager : MonoBehaviour {
    public const int CALLBACK_REMOVE = 0;
    public const int CALLBACK_DAMAGE_AREA= 1;
    public const int CALLBACK_SPAWN_SPELL = 2;
    public const int CALLBACK_SPAWN_PPROJECTILE= 3;
    public const int CALLBACK_DAMAGE_TARGET= 4;
    [SerializeField] private SpellObject prefab;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private AudioPooling audioPooling;
    //[SerializeField] private AnimationManager animationManager;
    //List<CustomAnimator> spells = new List<CustomAnimator>();
    public SpellObject[] spellAnimators;
    public SpellData[] spellDatas;
    public SpellSpawnData[] spawnDatas;
    int count = 0;
    public void Awake()
    {
        spellAnimators = new SpellObject[8];

        for (int i = 0; i < 8; i++)
        {
            spellAnimators[i] = Instantiate(prefab, transform);
            spellAnimators[i].gameObject.SetActive(false);
        }
    }
    public void CastSpell(SpellSO spell, Vector3 position)
    {
        SpellData data = spell.spellData;
        switch (data.spellAction)
        {
            case SpellAction.Damage:
                { 
                    int i = count++;
                    //Debug.Log("Simple spell casting " + i);
                    if (count >= spellAnimators.Length) Resize();
                    position.z = 0;
                    spellAnimators[i].Init(spell.animations, i, position, data);
                    break;
                }
            case SpellAction.SpawnSpells:
                { 
                    int i = count++;
                    //Debug.Log("Multiple spell's Parent casting " + i);
                    if (count >= spellAnimators.Length) Resize();
                    position.z = 0;
                    spellAnimators[i].Init(spell.animations, i, position,data, spell.spawnData);
                    break;
                }
            case SpellAction.SpawnProjectiles:
                {
                    int i = count++;
                    //Debug.Log(i);
                    if (count >= spellAnimators.Length) Resize();
                    position.z = 0;
                    spellAnimators[i].Init(spell.animations, i, position,data, spell.spawnData);
                    break;
                }
        }
        audioPooling.PlaySoundNormalPitch(spell.audioClip);
    }
    public void CastSpell(SpellData data, SpellSpawnData spawnData, Animation[] animations, Vector3 position)
    {
        switch (data.spellAction)
        {
            case SpellAction.Damage:
                {
                    int i = count++;
                    Debug.Log("Simple spell casting " + i);
                    if (count >= spellAnimators.Length) Resize();
                    position.z = 0;
                    spellAnimators[i].Init(animations, i, position, data);
                    break;
                }
            case SpellAction.SpawnSpells:
                {
                    int i = count++;
                    Debug.Log("Multiple spell's Parent casting " + i);
                    if (count >= spellAnimators.Length) Resize();
                    position.z = 0;
                    spellAnimators[i].Init(animations, i, position, data, spawnData);
                    break;
                }
                /*case SpellAction.SpawnSpells:
                    {
                        *//*int i = count++;
                        Debug.Log(i);
                        if (count >= spells.Length) Resize();
                        position.z = 0;
                        spells[i].transform.SetPositionAndRotation(position, Quaternion.identity);
                        spells[i].gameObject.SetActive(true);
                        spells[i].animations = spell.animations;
                        spells[i].Init();
                        spells[i].actions[CALLBACK_REMOVE].AddListener(() => { RemoveSpell(i); });
                        SpellSO[] spellsToSpawn = spell.spellsToSpawn;
                        Vector3[] positions = spell.deltaPosToSpawn;
                        for (int l = 0; l < spellsToSpawn.Length; i++)
                        {
                            spells[i].actions[CALLBACK_SPAWN_SPELL].AddListener(() => { CastSpell(spellsToSpawn[i].spellData, position + positions[i]); });
                        }*//*
                        break;
                    }*/
        }

    }
    void RemoveSpell(int index)
    {
        //Debug.Log("Removing spell " + index);
        spellAnimators[index].gameObject.SetActive(false);
        var temp = spellAnimators[index];
        spellAnimators[index] = spellAnimators[--count];
        spellAnimators[index].id = index;
        spellAnimators[count] = temp;
        temp.gameObject.SetActive(false);
    }
    public void Resize()
    {
        Array.Resize(ref spellAnimators, count * 2);
        for (int i = count; i < spellAnimators.Length; i++)
        {
            spellAnimators[i] = Instantiate(prefab, transform);
        }
    }
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        for (int i = 0; i < count; i++)
        {
            spellAnimators[i].animator.UpdateAnimator(dt);
        }
        for(int i = 0; i < count; i++)
        {
            if (spellAnimators[i].damage)
            {
                spellAnimators[i].damage = false;
                enemyManager.AreaSpell(spellAnimators[i].data, spellAnimators[i].position);
            }
            if (spellAnimators[i].spawn && spellAnimators[i].spawnData.repeat > 0)
            {
                Debug.Log($"SpawnFlag detected, spawning... id:{i};more times:{spellAnimators[i].spawnData.repeat > 0}");
                spellAnimators[i].spawn = false;
                spellAnimators[i].spawnData.repeat--;
                int num = spellAnimators[i].spawnData.deltaPosToSpawn.Length;
                for (int l = 0; l < num; l++)
                {
                    CastSpell(spellAnimators[i].data, spellAnimators[i].spawnData, spellAnimators[i].spawnData.spell.animations, spellAnimators[i].position + spellAnimators[i].spawnData.deltaPosToSpawn[l]);
                }
            }
            if (spellAnimators[i].spawnProjectile)
            {
                Character[] targets = enemyManager.enemies;
                int l = targets.Length;
                for (int k =0; k < l; k++)
                {
                    float distance = (spellAnimators[i].transform.position - targets[k].transform.position).magnitude;
                    if (spellAnimators[i].data.radius > distance)
                    {
                        spellAnimators[i].spawnData.projectile.targetPosition = targets[k].transform.position;
                        spellAnimators[i].spawnData.projectile.target = targets[k];
                        projectileManager.SendProjectile(spellAnimators[i].spawnData.projectile);
                    }
                }
                spellAnimators[i].spawnProjectile = false;
            }
            if (spellAnimators[i].remove)
            {
                spellAnimators[i].remove = false;
                RemoveSpell(i);
                i--;
            }
        }
    }
}