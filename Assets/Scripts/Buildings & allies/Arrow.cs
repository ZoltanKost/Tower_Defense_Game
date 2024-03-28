using UnityEngine;

public class Arrow : MonoBehaviour {
    [SerializeField] private Transform normal;
    [SerializeField] private Transform cut;
    Vector3 parentPosition;
    Enemy target;
    float speed;
    public bool active;
    public void Init(Vector3 parent){
        parentPosition = parent;
    }
    public void Send(Enemy target, float speed){
        active = true;
        transform.position = parentPosition;
        this.target = target;
        cut.gameObject.SetActive(false);
        normal.gameObject.SetActive(true);
        this.speed = speed;
    }
    public void Move(float delta){
        if(!active) return;
        Vector3 dir = target.transform.position - parentPosition;
        dir.z = 0;
        float angle = Vector2.Angle(Vector2.right,dir);
        if(parentPosition.y > target.transform.position.y) angle = -angle;
        Quaternion rot = Quaternion.Euler(0f,0f,angle);
        normal.rotation = rot;
        transform.position += delta * speed * dir.normalized;
        if((transform.position - target.transform.position).magnitude < .3f){
            active = false;
            normal.gameObject.SetActive(false);
            if(!target.alive) {
                cut.gameObject.SetActive(true);
                cut.transform.SetPositionAndRotation(normal.transform.position, normal.transform.rotation);
            }
            target.Damage();
        }
    }
}