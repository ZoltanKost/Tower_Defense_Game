using UnityEngine;
[CreateAssetMenu(menuName = "Buildings")]
public class Building : ScriptableObject{
    public int width = 2;
    public int height = 2;
    public BuildingObject prefab;
}