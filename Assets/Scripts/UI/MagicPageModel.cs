using UnityEngine;
[RequireComponent (typeof(MagicPage))]
public class MagicPageModel : MonoBehaviour
{
    [SerializeField] private SpellSO[] possibleSpells;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private MagicPage magicPage;
    [SerializeField] private PlayerInventoryModel inventoryModel;
    [SerializeField] private PlayerResourceManager playerResourceManager;

private SpellSO[] _spells;
    public SpellSO[] spells { get => _spells; }
    int maxSpells;
    public void Init(int spellCount)
    {
        maxSpells = spellCount;
        magicPage = GetComponent<MagicPage>();
        _spells = new SpellSO[maxSpells];
        for (int i = 0; i < _spells.Length; i++)
        {
            _spells[i] = possibleSpells[Random.Range(0, possibleSpells.Length)];
        }
        magicPage.Init(spells, spellCount, OnMagicBuyCallBack);
    }
    void CreateSpell(int ID)
    {
        _spells[ID] = possibleSpells[Random.Range(0,possibleSpells.Length)];
        magicPage.UpdateVisual(ID, _spells[ID].spellData);
    }
    public void ResetSpells()
    {
        for (int i = 0; i < _spells.Length; i++)
        {
            _spells[i] = possibleSpells[Random.Range(0, possibleSpells.Length)];
        }
        magicPage.ResetGroundArrays(_spells);
    }

    void OnMagicBuyCallBack(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        if (!playerResourceManager.EnoughtResource(Resource.Gold, spells[uiID].spellData.goldCost)) return;
        playerResourceManager.RemoveResource(Resource.Gold, spells[uiID].spellData.goldCost);
        inventoryModel.AddSpell(spells[uiID]);
        //magicPage.DeactivateVisuals(uiID);
        CreateSpell(uiID);
        /*playerActionManager.SetPlaceCallback(() =>
        {
            magicPage.ActivateVisuals(uiID);
            CreateSpell(uiID);
        }
        );
        playerActionManager.SetCancelCallback(() => magicPage.ActivateVisuals(uiID));*/
    }
}