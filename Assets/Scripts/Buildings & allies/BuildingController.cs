using DG.Tweening;
using UnityEngine;

public class BuildingController : MonoBehaviour{
    [SerializeField] private Animator bowman;
    [SerializeField] private Animator bow;
    [SerializeField] Transform arrow_prefab;
    [SerializeField] private float damageRange;
    bool shooting;
    Enemy target;
    public void Update(){
        if(target == null){
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position + Vector3.one * 2f, damageRange);
            foreach(Collider2D col in cols){
                Enemy enemy = col.GetComponent<Enemy>();
                target = enemy;
                if(target!= null) break;
            }
        }else{
            Shoot();
        }
    }
    public void Shoot(){
        if(shooting) return;
        bowman.Play(0);
        bow.gameObject.SetActive(true);
        bow.Play(1);
        shooting = true;
    }
    public void StopShoot(){
        bow.gameObject.SetActive(false);
    }
    public void InstantiateArrow(){
        Transform arrow = Instantiate(arrow_prefab, transform);
        arrow.DOMove(arrow.forward * 10, 10f);
    }
}