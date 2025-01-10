using UnityEngine;

public class BuildingObject : MonoBehaviour, IDamagable
{
    OnKillEvent onKillEvent;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private TweenAnimator tweenAnimator;
    [SerializeField] private int maxHP = 100;
    public int AssetID = -1;
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
        animator.animations[0].sprites[0] = b.sprite;
        animator.PlayAnimation(0);
        animator.SetSortingParams(sortingOrder + 1000/gridY, sortingLayer);
        onKillEvent = OnKill;
        active = true;
        Animate();
    }
    public void Init(int sortingOrder, int sortingLayer, int animationToPlay, BuildingSaveData data, Building b, OnKillEvent OnKill)
    {
        currentHP = data.currentHP;
        if (index != 0 && hpBar != null)
        {
            hpBar.gameObject.SetActive(true);
            hpBar.Set(currentHP);
        }
        Activate();
        index = data.index;
        w = b.width;
        h = b.height;
        spriteRenderer.sprite = b.sprite;
        gridPosition = new Vector2Int(data.gridPosition.x, data.gridPosition.y);
        animator.animations[0].sprites[0] = b.sprite;
        animator.PlayAnimation(animationToPlay);
        animator.SetSortingParams(sortingOrder + 1000 / gridPosition.y, sortingLayer);
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
        animator.PlayAnimation(1);
    }
    public void Deactivate(){
        gameObject.SetActive(false);
    }
    public void OnKillCallBack(){
        Debug.Log("Killed Building");
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