using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DynamicItemPageUI))]
public class PlayerInventoryModel : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SpellSO[] defaultSpells;
    [SerializeField] private DynamicItemPageUI spellsUIView;
    [SerializeField] private PlayerActionManager playerActionManager;
    private List<SpellSO> spells;
    private List<bool> isActiveList;
    private List<CooldownData> cooldowns;

    public void Init()
    {
        //spellsUIView = GetComponent<DynamicItemPageUI>();
        spells = new List<SpellSO>(defaultSpells);
        isActiveList = new List<bool>();
        cooldowns = new();
        spellsUIView.Init(SetPlayerAction, Resource.Mana);
        foreach (var spell in defaultSpells)
        {
            isActiveList.Add(true);
            spellsUIView.AddItem(spell.spellData);
        }
    }
    void Update()
    {
        float dt = Time.deltaTime;
        int l = cooldowns.Count;
        //Debug.Log(l);
        for (int i = 0; i < l; i++)
        {
            var cd = cooldowns[i];
            cd.time -= dt;
            spellsUIView.UpdateCooldown(cooldowns[i].spellID, cd.time/cd.maxTime);

            if (cd.time <= 0)
            {
                Debug.Log("cooldownTime is up!");
                spellsUIView.ActivateVisuals(cd.spellID);
                //spellsUIView
                isActiveList[cd.spellID] = true;
                cooldowns[i] = cooldowns[--l];
                cooldowns.RemoveAt(l);
                i--;
                continue;
            }
            cooldowns[i] = cd;
        }
    }
    public void AddSpell(SpellSO data)
    {
        spells.Add(data);
        isActiveList.Add(true);
        spellsUIView.AddItem(data.spellData);
    }
    public void RemoveSpell(int id)
    {
        int last = spells.Count - 1;
        spells[id] = spells[last];
        cooldowns[id] = cooldowns[last];
        spells.RemoveAt(last);
        cooldowns.RemoveAt(last);
        spellsUIView.RemoveItem(id);
    }
    public void SetPlayerAction(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        if (!isActiveList[uiID])
        {
            // play error sound
            /*audioSource.Stop();
            audioSource.pitch = Random.Range(0.5f, 1f);
            audioSource.Play();*/
            return;
        }
        playerActionManager.ChooseSpell(spells[uiID]);
        spellsUIView.DeactivateVisuals(uiID);
        // play button click sound
        audioSource.Stop();
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.Play();
        playerActionManager.SetPlaceCallback(() =>
        {
            Debug.Log("place callback");
            //spellsUIView.ActivateVisuals(uiID);
            isActiveList[uiID] = false;
            cooldowns.Add(
                new CooldownData()
                {
                    maxTime = spells[uiID].cooldown,
                    time = spells[uiID].cooldown,
                    spellID = uiID
                });
            Debug.Log(cooldowns.Count);
            //RemoveSpell(uiID);
        }
        );
        playerActionManager.SetCancelCallback(() => spellsUIView.ActivateVisuals(uiID));
    }
    public void ResetInventory()
    {
        spellsUIView.ResetVisuals(null);
    }
}
public struct CooldownData
{
    public float maxTime;
    public float time;
    public int spellID;
}