using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DynamicItemPageUI))]
public class PlayerInventoryModel : MonoBehaviour
{
    [SerializeField] private DynamicItemPageUI spellsUIView;
    [SerializeField] private PlayerActionManager playerActionManager;
    private List<SpellSO> spells;

    public void Init()
    {
        spellsUIView = GetComponent<DynamicItemPageUI>();
        spells = new List<SpellSO>();
        spellsUIView.Init(SetPlayerAction, Resource.Mana);
    }

    public void AddSpell(SpellSO data)
    {
        spells.Add(data);
        spellsUIView.AddItem(data.spellData);
    }
    public void RemoveSpell(int id)
    {
        int last = spells.Count - 1;
        spells[id] = spells[last];
        spells.RemoveAt(last);
        spellsUIView.RemoveItem(id);
    }
    public void SetPlayerAction(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseSpell(spells[uiID]);
        spellsUIView.DeactivateVisuals(uiID);
        playerActionManager.SetPlaceCallback(() =>
        {
            spellsUIView.ActivateVisuals(uiID);
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