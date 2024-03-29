using UnityEngine;

public class GroundUI : MonoBehaviour{
    public delegate void OnClick(GroundUI gUI);
    public OnClick onClick;
    public Floor floor;
    public GroundArray currentGA{get;private set;}
    private int maxDimensions, maxSeed, maxValue, random,  trueCondition;
    private float randomReduce;
    void Awake(){
        floor.Init(0, $"GroundUI");
    }
    public void Init(int maxDimensions, int maxSeed,int maxValue,int random, float randomReduce, int trueCondition){
        this.maxDimensions = maxDimensions;
        this.maxSeed = maxSeed;
        this.maxValue = maxValue;
        this.random = random; 
        this.randomReduce = randomReduce; 
        this.trueCondition = trueCondition;
    }
    public void SetGroundArray(GroundArray ga){
        currentGA = ga;
        floor.ClearAllTiles();
        floor.layer = ga.targetFloor;
        foreach(Vector3Int g in ga.grounds){
            floor.CreateGround(g);
        }
        Vector3 pos = new Vector3{x = Mathf.Min((-ga.width)/2, -.5f), y = (-ga.width)/2 + .5f, z = 0};
        floor.transform.localPosition = pos;
    }
    public void CreateGroundArray(){
        GroundArray ga = new GroundArray(maxDimensions, maxSeed, maxValue, random, randomReduce, trueCondition);
        SetGroundArray(ga);
    }
    public void ClearAllTiles(){
        floor.ClearAllTiles();
    }
    public GroundArray GetGroundArray(){
        return currentGA;
    }
    public void ActivateVisuals(){
        floor.gameObject.SetActive(true);
    }
    public void DeactivateVisuals(){
        floor.gameObject.SetActive(false);
    }
    public void OnGroundsChoosen(){
        onClick?.Invoke(this);
        DeactivateVisuals();
    }
}