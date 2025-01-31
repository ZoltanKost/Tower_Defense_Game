using UnityEngine;
[RequireComponent (typeof(MagicPage))]
public class MagicPageModel : MonoBehaviour
{
    [SerializeField] private SpellSO[] possibleSpells;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private MagicPage magicPage;
    [SerializeField] private PlayerInventoryModel inventoryModel;
    [SerializeField] private PlayerResourceManager playerResourceManager;

    //private SpellSO[] _spells;
    //public SpellSO[] spells { get => _spells; }
    int maxSpells;
    public void Init(int spellCount)
    {
        maxSpells = spellCount;
        magicPage = GetComponent<MagicPage>();
        /*_spells = new SpellSO[maxSpells];
        for (int i = 0; i < _spells.Length; i++)
        {
            _spells[i] = possibleSpells[Random.Range(0, possibleSpells.Length)];
        }*/
        magicPage.Init(possibleSpells, possibleSpells.Length, OnMagicBuyCallBack);
    }
    /*void CreateSpell(int ID)
    {
        _spells[ID] = possibleSpells[Random.Range(0,possibleSpells.Length)];
        magicPage.UpdateVisual(ID, _spells[ID].spellData);
    }*/
    public void ResetSpells()
    {
        /*for (int i = 0; i < possibleSpells.Length; i++)
        {
            _spells[i] = possibleSpells[Random.Range(0, possibleSpells.Length)];
        }*/
        magicPage.ResetSpells(possibleSpells);
    }

    void OnMagicBuyCallBack(int uiID)
    {
        playerActionManager.CancelBuildingAction();
        if (!playerResourceManager.EnoughtResource(Resource.Gold, possibleSpells[uiID].spellData.goldCost)) return;
        playerResourceManager.RemoveResource(Resource.Gold, possibleSpells[uiID].spellData.goldCost);
        inventoryModel.AddSpell(possibleSpells[uiID]);
        magicPage.DeactivateButton(uiID);
        //CreateSpell(uiID);
        /*playerActionManager.SetPlaceCallback(() =>
        {
            magicPage.ActivateVisuals(uiID);
            CreateSpell(uiID);
        }
        );
        playerActionManager.SetCancelCallback(() => magicPage.ActivateVisuals(uiID));*/
    }
}