using UnityEngine;

[RequireComponent(typeof(GroundPage))]
public class GroundPageModel : MonoBehaviour {
    [SerializeField] private GroundArrayGenerator groundArrayGenerator;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private GroundPage groundPage;
    private GroundArray[] _groundArrays;
    public GroundArray[] groundArrays{get => _groundArrays;}
    int maxGrounds;
    public void Init(int groundsCount){
        maxGrounds = groundsCount;
        groundPage = GetComponent<GroundPage>();
        _groundArrays = new GroundArray[maxGrounds];
        int dims = Mathf.ClosestPowerOfTwo(groundArrayGenerator.maxDimensions) + 1;
        groundPage.Init(groundArrays,groundsCount,OnGroundChosenCallBack, dims, dims);
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
        Debug.Log("OnGroundChosenCallback");
        playerActionManager.CancelBuildingAction();
        playerActionManager.ChooseGround(_groundArrays[uiID]);
        groundPage.DeactivateVisuals(uiID);
        playerActionManager.SetPlaceCallback(() => 
            {
                CreateGroundArray(uiID);
                groundPage.ActivateVisuals(uiID);
                playerActionManager.ClearCancelCallback();
            }
        );
        playerActionManager.SetCancelCallback(() => groundPage.ActivateVisuals(uiID));
    }
}