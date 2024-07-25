using UnityEngine;
[RequireComponent (typeof(MagicPage))]
public class MagicPageModel : MonoBehaviour
{
    [SerializeField] private SpellSO[] possibleSpells;
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private MagicPage magicPage;
    [SerializeField] private PlayerInventoryModel inventoryModel;

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
        playerBuildingManager.CancelBuildingAction();
        inventoryModel.AddSpell(spells[uiID]);
        magicPage.DeactivateVisuals(uiID);
        CreateSpell(uiID);
        /*playerBuildingManager.SetPlaceCallback(() =>
        {
            magicPage.ActivateVisuals(uiID);
            CreateSpell(uiID);
        }
        );
        playerBuildingManager.SetCancelCallback(() => magicPage.ActivateVisuals(uiID));*/
    }
}