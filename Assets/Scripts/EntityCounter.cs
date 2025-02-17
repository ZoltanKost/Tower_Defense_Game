using TMPro;
using UnityEngine;

public class EntityCounter : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ArcherManager archerManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private TMP_Text FPS;
    [SerializeField] private TMP_Text enemy;
    [SerializeField] private TMP_Text archer;
    [SerializeField] private TMP_Text building;
    public int num = 0;
    public int l;
    float fps;
    void Update()
    {
        num++;
        fps += 1f / Time.deltaTime;
        if(num < l)
            return;
        num = 0;
        FPS.text = Mathf.FloorToInt(fps/l).ToString();
        fps = 0;
        enemy.text = enemyManager.lowestInactive.ToString();
        archer.text = archerManager.archersList.Count.ToString();
        //building.text = buildingManager.Count.ToString();
    }
}
