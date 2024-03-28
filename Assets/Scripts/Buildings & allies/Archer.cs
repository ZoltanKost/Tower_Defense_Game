using UnityEngine;

public class Archer : MonoBehaviour{
    [SerializeField] private CustomAnimator body;
    [SerializeField] Arrow arrow;
    [SerializeField] private float shootSpeed = 5f;
    [SerializeField] Enemy target;
    [SerializeField] private float attackRange;
    bool shooting;
    Enemy[] enemyList;
    public void Init(Enemy[] enemies){
        body.PlayAnimation(0);
        enemyList = enemies;
        arrow.Init(transform.position);
    }
    public void TickAnimator(float delta){
        body.UpdateAnimator(delta);
    }
    public void TickDetection(float delta){
        Detect();
        if(arrow.active)arrow.Move(delta);
        if(shooting) {
            body.SetAnimation(1);
        }
        else body.SetAnimation(0);
    }
    public void ResetAnimation(){
        body.SetAnimation(0);
    }
    public void Shoot(){
        arrow.Send(target, shootSpeed);
    }
    public void Detect(){
        shooting = false;
        for(int i = 0; i < enemyList.Length; i++){
            if(!enemyList[i].active || !enemyList[i].alive) continue;
            Vector3 vector = transform.position - enemyList[i].transform.position;
            vector.z = 0;
            float distance = vector.magnitude;
            if(distance > attackRange) continue;
            float minDistance;
            if(target == null || !target.active || !target.alive) minDistance = attackRange; 
            else {
                vector = transform.position - target.transform.position;
                vector.z = 0;
                minDistance = vector.magnitude;
            }
            if(distance < minDistance){
                target = enemyList[i];
            }
        }
        shooting = target != null && target.active && target.alive;
    }
}