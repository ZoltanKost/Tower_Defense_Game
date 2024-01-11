using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour {
    public static WorldManager singleton{get;private set;}

    [Header("Dependencies")]
    private List<Floor> visuals;
    [SerializeField] private BuildingManager buildingManager;

    [Header("Dimensions")]
    [SerializeField] private int halfWidth;
    [SerializeField] private int halfHeight;

    //[Header("private")] 
    
    Cell[,] cells;
    
    void Start(){
        singleton = this;
        visuals = new List<Floor>();
        Floor[] floors = GetComponentsInChildren<Floor>();
        foreach(var floor in floors){
            visuals.Add(floor);
        }
        cells = new Cell[halfWidth*2, halfHeight * 2];
        GenerateWorld();
    }
    void GenerateWorld(){
        
    }
}