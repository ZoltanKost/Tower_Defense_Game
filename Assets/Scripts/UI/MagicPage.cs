using System;
using UnityEngine;

public class MagicPage : MonoBehaviour
{
    [SerializeField] private BuildingButtonUI buttonPrefab;
    private BuildingButtonUI[] buttonArray;
    public void Init(SpellSO[] spells, int spellCount, Action<int> OnMagicBuyCallBack)
    {
        buttonArray = new BuildingButtonUI[spellCount];
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i] = Instantiate(buttonPrefab, transform);
            buttonArray[i].Init(OnMagicBuyCallBack, spells[i].spellData.UIicon,spells[i].spellData.goldCost, i);
        }
    }

    public void UpdateVisual(int ID, SpellData spell)
    {
        buttonArray[ID].SetSprite(spell.UIicon);
    }
    public void ResetSpells(SpellSO[] spells)
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i].gameObject.SetActive(true);
            buttonArray[i].SetSprite(spells[i].spellData.UIicon);
        }
    }
    public void ActivateVisuals(int uiID)
    {
        buttonArray[uiID].ActivateVisuals();
    }
    public void DeactivateVisuals(int uiID)
    {
        buttonArray[uiID].DeactivateVisuals();
    }
    public void DeactivateButton(int uiID)
    {
        buttonArray[uiID].gameObject.SetActive(false);
    }
}