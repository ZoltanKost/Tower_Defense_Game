using System;
using UnityEngine;

public class MagicPage : MonoBehaviour
{
    [SerializeField] private BuildingButtonUI buttonPrefab;
    private BuildingButtonUI[] buttonArray;
    public void Init(SpellData[] spells, int spellCount, Action<int> OnMagicBuyCallBack)
    {
        buttonArray = new BuildingButtonUI[spellCount];
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i] = Instantiate(buttonPrefab, transform);
            buttonArray[i].Init(OnMagicBuyCallBack, spells[i].UIicon, i);
        }
    }

    public void UpdateVisual(int ID, SpellData spell)
    {
        buttonArray[ID].SetSprite(spell.UIicon);
    }
    public void ResetGroundArrays(SpellData[] spells)
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i].SetSprite(spells[i].UIicon);
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
}