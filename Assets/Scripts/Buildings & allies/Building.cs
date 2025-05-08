using UnityEngine;
[CreateAssetMenu(menuName = "Buildings")]
public class Building : ScriptableObject{
    public int health = 100;
    public int width = 2;
    public int height = 2;
    public BuildingObject prefab;
    public Sprite sprite;
    public Resource resource = Resource.Gold;
    public int price = 2;
    public int attackRange;
    public Animation[] animations;
    // public ArcherSO[] archers;
    // public Vector3 localPositions
}