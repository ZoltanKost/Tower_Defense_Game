using UnityEngine;
[CreateAssetMenu(menuName = "Buildings")]
public class Building : ScriptableObject{
    public int width = 2;
    public int height = 2;
    public BuildingObject prefab;
    public Sprite sprite;
    public Resource resource = Resource.Gold;
    public int price = 2;
    // public ArcherSO[] archers;
    // public Vector3 localPositions
}