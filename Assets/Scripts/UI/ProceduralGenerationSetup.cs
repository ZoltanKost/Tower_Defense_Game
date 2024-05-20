using UnityEngine;

public class ProceduralGenerationSetup : MonoBehaviour {
    [SerializeField] private Floor[] visuals;
    int maxDimensions, maxValue, random, trueCondition;
    float randomMultiplier; 
    GroundArray ga;
    Camera cam;
    void Awake(){
        cam = Camera.main;
        ga = new GroundArray(maxDimensions,maxValue,random,randomMultiplier,trueCondition);
    }

    public void Activate(){
        foreach(var f in visuals){
            f.gameObject.SetActive(true);
            f.transform.position = cam.transform.position;
        }
    }
    public void GenerateGA(){
        ga = new GroundArray(maxDimensions,maxValue,random,randomMultiplier,trueCondition);
    }
    public void DrawGA(){
        Debug.Log(ga.s);
        foreach(Vector3Int g in ga.grounds){
            visuals[1].CreateGround(g);
            visuals[0].CreateGround(g);
        }
    }
}