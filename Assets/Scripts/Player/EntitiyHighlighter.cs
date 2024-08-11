using UnityEngine;

public class EntitiyHighlighter : MonoBehaviour
{
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private TemporalFloor tempFloor;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private HighlightUI ui;
    public void TryHighlight(Vector3 position)
    {
        if(floorManager.HasBuilding(position, out int ID))
        {
            BuildingObject b = buildingManager.bs[ID];
            ui.SetEntity(b.spriteRenderer.sprite,b.HP,15,0);
            //tempFloor.SetArrows(b.gridPosition,new Vector3Int(b.w,b.h));
        }else if (archerManager.TryHighlightEntity(position,out Archer archer,1f))
        {
            ui.SetEntity(archer.GetSprite(), 100, archer.projectileData.damage, 0);
        }else if (enemyManager.TryHighlightEntity(position, out Enemy enemy, 1f))
        {
            ui.SetEntity(enemy.GetSprite(), 100, enemy.ProjectileData.damage, 0);
        }
        else
        {
            ui.Close();
        }
    }
}