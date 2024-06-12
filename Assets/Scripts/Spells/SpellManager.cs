using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {
    [SerializeField] private EventSubscribeButton buttonPrefab;
    public List<EventSubscribeButton> buttons;
    public Dictionary<SpellSO,int> spells_Count;
    public void AddSpell(SpellSO spell, int count){
        if(spells_Count.ContainsKey(spell))
        {
            spells_Count[spell] += count;
        }
        else
        {
            spells_Count.Add(spell,count);
        }
    }
}