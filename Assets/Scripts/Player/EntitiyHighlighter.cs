using UnityEngine;

public class EntitiyHighlighter : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private HighlightUI ui;
    public void TryHighlight(Vector3 position)
    {
        if(floorManager.HasBuilding(position, out int ID))
        {
            BuildingObject b = buildingManager.bs[ID];
            ui.SetEntity(b.spriteRenderer.sprite,b.HP);
        }else
        {
            
        }
    }
}