using UnityEngine;

public class EntitiyHighlighter : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private TemporalFloor tempFloor;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private HighlightUI ui;
    HighlightedEntity entity;
    Archer archer;
    public void TryHighlight(Vector3 position)
    {
        if(floorManager.HasBuilding(position, out int ID))
        {
            BuildingObject b = buildingManager.bs[ID];
            ui.SetEntity(b.spriteRenderer.sprite,b.HP,15,0);
            tempFloor.SetMode(ActionMode.Command);
            tempFloor.SetArrows(b.gridPosition,new Vector3Int(b.w,b.h));
            var archer = b.GetArchers()[0];
            tempFloor.SetHighlightedCharacter(archer.gridPosition, archer.buildingSize, archer.attackRange);
            entity = HighlightedEntity.Building;
        }
        /*else if (archerManager.TryHighlightEntity(position,out archer,1f))
        {
            ui.SetEntity(archer.GetSprite(), 100, archer.projectileData.damage, 0);
            entity = HighlightedEntity.Archer;
        }*/
        /*else if (enemyManager.TryHighlightEntity(position, out Character enemy, 1f))
        {
            ui.SetEntity(enemy.GetSprite(), 100, enemy.ProjectileData.damage, 0);
            entity = HighlightedEntity.Enemy;
        }*/
        else
        {
            ui.Close();
            entity = HighlightedEntity.None;
        }
    }
    public void HighlighterCallback(Vector3 position)
    {
        position.z = 0;
        switch (entity)
        {
            case HighlightedEntity.Enemy: break;
            case HighlightedEntity.Building: break;
            case HighlightedEntity.Archer:
                if (floorManager.HasBuilding(position, out int ID))
                {

                }
                /*else if (enemyManager.TryHighlightEntity(position, out Enemy enemy, 1f))
                {
                    ui.SetEntity(enemy.GetSprite(), 100, enemy.ProjectileData.damage, 0);
                    entity = HighlightedEntity.Enemy;
                }*/
                //archer.SetMovement(pathfinding.DijkstraSearch(archer.position, position));
                break;
            default: break;
        }
    }
}
public enum HighlightedEntity
{
    None,
    Building,
    Enemy,
    Archer,
}