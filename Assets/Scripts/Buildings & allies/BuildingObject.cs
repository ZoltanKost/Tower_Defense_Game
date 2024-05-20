using UnityEngine;

public class BuildingObject : MonoBehaviour, IDamagable{
    OnKillEvent onKillEvent;
    [SerializeField] private TweenAnimator tweenAnimator;
    [SerializeField] private int _hp = 100;
    SpriteRenderer spriteRenderer {get {return animator.spriteRenderer;}}    
    Archer[] archers;
    int index;
    bool _active;
    CustomAnimator animator;
    public int HP { 
        get {return _hp;} 
        set {_hp = value;} 
    }
    public Vector3 position{
        get {return transform.position + 2 * Vector3.up;}
    }
    public bool active{
        get{return _active;}
        set{_active = value;}
    }
    public bool alive{
        get{return HP > 0;}
    }

    void Awake(){
        archers = GetComponentsInChildren<Archer>();
        animator = GetComponent<CustomAnimator>();
        tweenAnimator = GetComponent<TweenAnimator>();
    }
    public void Init(int sortingOrder, int sortingLayer, int index, OnKillEvent OnKill){
        this.index = index;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = $"{sortingLayer}";
        animator.PlayAnimation(0);
        onKillEvent = OnKill;
        active = true;
        // Vector3 temp = Vector3.left * width/2;
        // foreach(Archer archer in archers) archer.transform.position += temp;
        // spriteRenderer.transform.position += temp; 
        Animate();
    }
    public Archer[] GetArchers(){
        return archers;
    }
    public void TickUpdate(float delta){
        animator.UpdateAnimator(delta);
    }

    public void Damage(int damage)
    {
        HP -= damage;
        Animate();
        if(HP <= 0) Kill();
    }

    public void Kill()
    {
        foreach(Archer a in archers){
            a._active = false;
            a.gameObject.SetActive(false);
        }
        animator.PlayAnimation(1);
    }
    public void OnKillCallBack(){
        _active = false;
        onKillEvent?.Invoke(index);
    }
    public void Animate(){
        tweenAnimator.JellyAnimation();
    }
}