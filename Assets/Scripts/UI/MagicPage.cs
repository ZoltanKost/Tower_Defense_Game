using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MagicPage : MonoBehaviour
{
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private BuildingButtonUI buttonPrefab;
    [SerializeField] SpellSO[] spells;
    private List<BuildingButtonUI> buttons;

    public void Init()
    {
        buttons = new List<BuildingButtonUI>();
        foreach (SpellSO spell in spells)
        {
            BuildingButtonUI ui = Instantiate(buttonPrefab, transform);
            ui.Init(OnMagicChosenCallback, spell.spellData.UIicon,buttons.Count);
            buttons.Add(ui);
        }
    }

    void OnMagicChosenCallback(int uiID)
    {
        playerBuildingManager.CancelBuildingAction();
        playerBuildingManager.ChooseSpell(spells[uiID]);
        //DeactivateVisuals(uiID);
        playerBuildingManager.SetPlaceCallback(() =>
        {
            //ActivateVisuals(uiID);
        }
        );
        //playerBuildingManager.SetCancelCallback(() => ActivateVisuals(uiID));
    }
}