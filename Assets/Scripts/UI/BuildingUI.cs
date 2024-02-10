using UnityEngine;
public class BuildingUI : MonoBehaviour {
    [SerializeField] private Building castle;
    [SerializeField] private Building tower;
    [SerializeField] private WorldManager worldManager;
    public void ChooseCastleBuilding(){
        worldManager.building = castle;
        worldManager.choosenBuilding = true;
    }
    public void ChooseTowerBuilding(){
        worldManager.building = tower;
        worldManager.choosenBuilding = true;
    }
}