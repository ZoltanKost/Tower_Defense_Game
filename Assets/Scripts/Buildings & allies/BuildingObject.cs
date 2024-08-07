using UnityEngine;

public class BuildingObject : MonoBehaviour, IDamagable{
    OnKillEvent onKillEvent;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private TweenAnimator tweenAnimator;
    [SerializeField] private int maxHP = 100;
    public SpriteRenderer spriteRenderer {get {return animator.spriteRenderer;}}    
    Archer[] archers;
    public int index;
    public Vector2Int gridPosition{get;private set;}
    public int w{get;private set;}
    public int h{get;private set;}
    bool _active;
    private int currentHP;
    CustomAnimator animator;
    public int HP { 
        get {return currentHP;} 
        set {currentHP = value;} 
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
    public void Init(int sortingOrder, int sortingLayer, int index,int gridX,int gridY, Building b, OnKillEvent OnKill){
        currentHP = maxHP;
        if(index != 0 && hpBar != null)
        {
            hpBar?.gameObject.SetActive(true);
            hpBar?.Reset();
        }
        Activate();
        this.index = index;
        w = b.width;
        h = b.height;
        spriteRenderer.sprite = b.sprite;
        gridPosition = new Vector2Int(gridX, gridY);
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = $"{sortingLayer}";
        animator.animations[0].sprites[0] = b.sprite;
        animator.PlayAnimation(0);
        onKillEvent = OnKill;
        active = true;
        Animate();
    }
    public void Activate(){
        gameObject.SetActive(true);
        foreach(Archer a in archers){
            a.Activate();
        }
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
        hpBar?.Set((float)HP/maxHP);
        Animate();
        if(HP <= 0) Kill();
    }

    public void Kill()
    {
        hpBar?.gameObject.SetActive(false);
        foreach(Archer a in archers){
            a.Deactivate();
        }
        animator.PlayAnimation(1);
    }
    public void Deactivate(){
        gameObject.SetActive(false);
        foreach (Archer a in archers){
            a.Deactivate();
        }
    }
    public void OnKillCallBack(){
        _active = false;
        onKillEvent?.Invoke(index);
    }
    public void Animate(){
        tweenAnimator.JellyAnimation();
    }
    public void SetColor(Color color){
        spriteRenderer.color = color;
        foreach(Archer archer in archers){
            archer.SetColor(color);
        }
    }
    public int GetIndex()
    {
        return index;
    }
}