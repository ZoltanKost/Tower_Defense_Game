using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GroundPage))]
public class GroundPageModel : MonoBehaviour {
    [SerializeField] private GroundArrayGenerator groundArrayGenerator;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private GroundPage groundPage;
    float defaultCooldown;
    private GroundArray[] _groundArrays;
    List<CooldownData> cooldowns = new();
    //private List<bool> isActiveList = new();
    public GroundArray[] groundArrays { get => _groundArrays;}
    int maxGrounds;
    public void Init(int groundsCount, float cooldown){
        maxGrounds = groundsCount;
        groundPage = GetComponent<GroundPage>();
        _groundArrays = new GroundArray[maxGrounds];
        int dims = Mathf.ClosestPowerOfTwo(groundArrayGenerator.maxDimensions) + 1;
        groundPage.Init(groundArrays,groundsCount,OnGroundChosenCallBack, dims, dims);
        ResetGroundArrays();
        defaultCooldown = cooldown;
    }
    public void Init()
    {
        _groundArrays = new GroundArray[maxGrounds];
        int dims = Mathf.ClosestPowerOfTwo(groundArrayGenerator.maxDimensions) + 1;
        groundPage.Init(groundArrays, maxGrounds, OnGroundChosenCallBack, dims, dims);
        ResetGroundArrays();
    }
    private void Update()
    {
        float dt = Time.deltaTime;
        int l = cooldowns.Count;
        //Debug.Log(l);
        for (int i = 0; i < l; i++)
        {
            var cd = cooldowns[i];
            cd.time -= dt;
            //spellsUIView.UpdateCooldown(cooldowns[i].spellID, cd.time / cd.maxTime);

            if (cd.time <= 0)
            {
                //Debug.Log("cooldownTime is up!");
                groundPage.ActivateButton(cd.spellID);
                //spellsUIView
                //isActiveList[cd.spellID] = true;
                CreateGroundArray(cooldowns[i].spellID);
                cooldowns[i] = cooldowns[--l];
                cooldowns.RemoveAt(l);
                i--;
                continue;
            }
            cooldowns[i] = cd;
        }
    }

    void CreateGroundArray(int ID){
        _groundArrays[ID] = groundArrayGenerator.GenerateGA();
        groundPage.UpdateVisual(ID, _groundArrays[ID]);
    }
    void RemoveGroundArray(int ID)
    {
        groundPage.DeactivateButton(ID);
    }
    public void ResetGroundArrays(){
        for(int i = 0; i < _groundArrays.Length;i++){
            _groundArrays[i] = groundArrayGenerator.GenerateGA();
        }
        groundPage.ResetGroundArrays(_groundArrays);
    }
    public void OnGroundChosenCallBack(int uiID){
        Debug.Log("OnGroundChosenCallback");
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseGround(_groundArrays[uiID]);
        groundPage.DeactivateVisuals(uiID);
        playerActionManager.SetPlaceCallback(() => 
            {
                RemoveGroundArray(uiID);
                //isActiveList[uiID] = false;

                cooldowns.Add(
                    new CooldownData()
                    {
                        maxTime = defaultCooldown,
                        time = defaultCooldown,
                        spellID = uiID
                    });
                //CreateGroundArray(uiID);
                //groundPage.ActivateVisuals(uiID);
                playerActionManager.ClearCancelCallback();
            }
        );
        playerActionManager.SetCancelCallback(() => groundPage.ActivateVisuals(uiID));
    }
}