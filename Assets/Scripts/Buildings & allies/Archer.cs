using DG.Tweening;
using UnityEngine;

public class Archer : MonoBehaviour{
    [SerializeField] private CustomAnimator body;
    [SerializeField] Transform[] arrows;
    int lowestInactiveArrow;
    [SerializeField] Enemy target;
    [SerializeField] private float attackRange;
    bool shooting;
    Enemy[] enemyList;
    public void Start(){
        body.PlayAnimation(0);
        enemyList = EnemyManager.singleton.enemies;
    }
    public void Update(){
        float delta = Time.deltaTime;
        Detect();
        if(shooting) body.SetAnimation(1);
        else body.SetAnimation(0);
        body.UpdateAnimator(delta);
    }
    public void Shoot(){
        Vector3 norm = (transform.position - target.transform.position).normalized;
        norm.z = 0;
        float angle = Vector2.Angle(Vector2.right,norm) + 180;

        if(lowestInactiveArrow >= arrows.Length) lowestInactiveArrow = 0;

        Transform tr = arrows[lowestInactiveArrow++];
        tr.gameObject.SetActive(true);
        tr.localPosition = Vector3.zero;
        tr.rotation = Quaternion.Euler(0f,0f,angle);

        Tween tween = tr.DOMove(target.transform.position,.3f,false);
        tween.onKill += target.Damage;
        tween.onKill += () => {
            tr.gameObject.SetActive(false);
        };
    }
    public void Detect(){
        shooting = false;
        for(int i = 0; i < enemyList.Length; i++){
            if(!enemyList[i].active) continue;
            Vector3 vector = transform.position - enemyList[i].transform.position;
            vector.z = 0;
            float distance = vector.magnitude;
            if(distance > attackRange) continue;
            float minDistance;
            if(target == null || !target.active) minDistance = attackRange; 
            else {
                vector = transform.position - target.transform.position;
                vector.z = 0;
                minDistance = vector.magnitude;
            }
            if(distance < minDistance){
                target = enemyList[i];
            }
            shooting = target!= null && target.active;
        }
    }
}