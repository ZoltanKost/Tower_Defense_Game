using UnityEngine;
using DG.Tweening;

public class BuildingManager : MonoBehaviour {
    [SerializeField] private ArcherManager archerManager;
    public void Build(Vector3 worldPosition, int floor, Building building){
        worldPosition.z = 0;
        BuildingObject s = Instantiate(building.prefab, worldPosition, Quaternion.identity);
        s.Init(6,floor);
        InitArchers(s.GetArchers(), 7,floor);
        // Animate(s.transform);
    }
    public void Animate(Transform transform){
        Tween tween = transform.DOScale(.99f, .05f);
        tween.onComplete += () => {
            Tween tween1 = transform.DOScale(1.01f, .1f);
            tween1.onComplete += () => transform.DOScale(1, .05f);
        };
    }
    
    public void InitArchers(Archer[] archers, int sortingOrder, int sortingLayer){
        foreach(Archer a in archers){
            archerManager.AddArcher(a, sortingOrder, sortingLayer);
        }
    }
}