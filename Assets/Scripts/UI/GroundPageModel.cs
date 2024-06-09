using UnityEngine;

[RequireComponent(typeof(GroundPage))]
public class GroundPageModel : MonoBehaviour {
    [SerializeField] private GroundArrayGenerator groundArrayGenerator;
    [SerializeField] private PlayerBuildingManager playerBuildingManager;
    [SerializeField] private GroundPage groundPage;
    private GroundArray[] _groundArrays;
    public GroundArray[] groundArrays{get => _groundArrays;}
    int maxGrounds;
    public void Init(int groundsCount){
        maxGrounds = groundsCount;
        groundPage = GetComponent<GroundPage>();
        _groundArrays = new GroundArray[maxGrounds];
        groundPage.Init(groundArrays,groundsCount,OnGroundChosenCallBack);
        ResetGroundArrays();
    }
    void CreateGroundArray(int ID){
        _groundArrays[ID] = groundArrayGenerator.GenerateGA();
        groundPage.UpdateVisual(ID, _groundArrays[ID]);
    }
    public void ResetGroundArrays(){
        for(int i = 0; i < _groundArrays.Length;i++){
            _groundArrays[i] = groundArrayGenerator.GenerateGA();
        }
        groundPage.ResetGroundArrays(_groundArrays);
    }
    public void OnGroundChosenCallBack(int uiID){
        playerBuildingManager.CancelBuildingAction();
        playerBuildingManager.ChooseGround(_groundArrays[uiID]);
        groundPage.DeactivateVisuals(uiID);
        playerBuildingManager.SetPlaceCallback(() => 
            {
                CreateGroundArray(uiID);
                groundPage.ActivateVisuals(uiID);
            }
        );
        playerBuildingManager.SetCancelCallback(() => groundPage.ActivateVisuals(uiID));
    }
}