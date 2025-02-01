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
    public float[] times;
    public int num;
    public int l;
    private void Awake()
    {
        times = new float[l];
        num = 0;
    }
    void Update()
    {
        num++;
        if(num < l)
            return;
        num = 0;
        FPS.text = (Mathf.FloorToInt(1f / Time.deltaTime)).ToString();
        enemy.text = enemyManager.lowestInactive.ToString();
        archer.text = archerManager.archersList.Count.ToString();
        building.text = buildingManager.Count.ToString();
    }
}
