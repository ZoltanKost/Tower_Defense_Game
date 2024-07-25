using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DynamicItemPageUI))]
public class PlayerInventoryModel : MonoBehaviour
{
    [SerializeField] private DynamicItemPageUI inventoryUIView;
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    private List<SpellData> spells;

    public void Init()
    {
        inventoryUIView = GetComponent<DynamicItemPageUI>();
        spells = new List<SpellData>();
        inventoryUIView.Init(SetPlayerAction);
    }

    public void AddSpell(SpellData data)
    {
        spells.Add(data);
        inventoryUIView.AddItem(data);
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
        playerBuildingManager.CancelBuildingAction();
        playerBuildingManager.ChooseSpell(spells[uiID]);
        inventoryUIView.DeactivateVisuals(uiID);
        playerBuildingManager.SetPlaceCallback(() =>
        {
            inventoryUIView.ActivateVisuals(uiID);
            RemoveSpell(uiID);
        }
        );
        playerBuildingManager.SetCancelCallback(() => inventoryUIView.ActivateVisuals(uiID));
    }
}