using UnityEngine;
[RequireComponent (typeof(MagicPage))]
public class MagicPageModel : MonoBehaviour
{
    [SerializeField] private SpellSO[] possibleSpells;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private MagicPage magicPage;
    [SerializeField] private PlayerInventoryModel inventoryModel;
    [SerializeField] private PlayerResourceManager playerResourceManager;

private SpellData[] _spells;
    public SpellData[] spells { get => _spells; }
    int maxSpells;
    public void Init(int spellCount)
    {
        maxSpells = spellCount;
        magicPage = GetComponent<MagicPage>();
        _spells = new SpellData[maxSpells];
        magicPage.Init(spells, spellCount, OnMagicBuyCallBack);
        ResetSpells();
    }
    void CreateSpell(int ID)
    {
        _spells[ID] = possibleSpells[Random.Range(0,possibleSpells.Length)].spellData;
        magicPage.UpdateVisual(ID, _spells[ID]);
    }
    public void ResetSpells()
    {
        for (int i = 0; i < _spells.Length; i++)
        {
            _spells[i] = possibleSpells[Random.Range(0, possibleSpells.Length)].spellData;
        }
        magicPage.ResetGroundArrays(_spells);
    }

    void OnMagicBuyCallBack(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        SpellData spellData = spells[uiID];
        if (!playerResourceManager.EnoughtResource(Resource.Gold, spellData.goldCost)) return;
        playerResourceManager.RemoveResource(Resource.Gold, spellData.goldCost);
        inventoryModel.AddSpell(spellData);
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