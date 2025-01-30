using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DynamicItemPageUI))]
public class PlayerInventoryModel : MonoBehaviour
{
    [SerializeField] private DynamicItemPageUI inventoryUIView;
    [SerializeField] private PlayerActionManager playerActionManager;
    private List<SpellSO> spells;

    public void Init()
    {
        inventoryUIView = GetComponent<DynamicItemPageUI>();
        spells = new List<SpellSO>();
        inventoryUIView.Init(SetPlayerAction);
    }

    public void AddSpell(SpellSO data)
    {
        spells.Add(data);
        inventoryUIView.AddItem(data.spellData);
    }
    public void RemoveSpell(int id)
    {
        int last = spells.Count - 1;
        spells[id] = spells[last];
        spells.RemoveAt(last);
        inventoryUIView.RemoveItem(id);
    }
    public void SetPlayerAction(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseSpell(spells[uiID]);
        inventoryUIView.DeactivateVisuals(uiID);
        playerActionManager.SetPlaceCallback(() =>
        {
            inventoryUIView.ActivateVisuals(uiID);
            //RemoveSpell(uiID);
        }
        );
        playerActionManager.SetCancelCallback(() => inventoryUIView.ActivateVisuals(uiID));
    }
    public void ResetInventory()
    {
        inventoryUIView.ResetVisuals(null);
    }
}