using DG.Tweening;
using UnityEngine;

public class BuildingController : MonoBehaviour{
    [SerializeField] private Animator bowman;
    [SerializeField] private Animator bow;
    [SerializeField] Transform arrow_prefab;
    public void Shoot(){
        bowman.Play(0);
        bow.gameObject.SetActive(true);
        bow.Play(1);
    }
    public void StopShoot(){
        bow.gameObject.SetActive(false);
    }
    public void InstantiateArrow(){
        Transform arrow = Instantiate(arrow_prefab, transform);
        arrow.DOMove(arrow.forward * 10, 10f);
    }
}