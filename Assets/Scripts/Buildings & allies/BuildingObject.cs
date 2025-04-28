using UnityEngine;

public class BuildingObject : MonoBehaviour, IDamagable
{
    OnKillEvent onKillEvent;
    [SerializeField] CustomAnimator animator;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TweenAnimator tweenAnimator;
    [SerializeField] private int maxHP = 100;
    public int AssetID = -1;
    Character[] archers;
    public int index;
    public Vector2Int gridPosition{get;private set;}
    public int w{get;private set;}
    public int h{get;private set;}
    bool _active;
    private int currentHP;
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
        archers = GetComponentsInChildren<Character>();
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
        gridPosition = new Vector2Int(gridX, gridY);
        //animator.animations[0].data[0].sprites[0][0] = b.sprite;
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
        gridPosition = new Vector2Int(data.gridPosition.x, data.gridPosition.y);
        //animator.animations[0].data[0].sprites[0][0] = b.sprite;
        animator.PlayAnimation(animationToPlay);
        animator.SetSortingParams(sortingOrder + 1000 / gridPosition.y, sortingLayer);
        onKillEvent = OnKill;
        active = true;
        Animate();
    }
    public void Activate(){
        gameObject.SetActive(true);
        foreach(Character a in archers){
            a.gameObject.SetActive(true);
        }
    }
    public Character[] GetArchers(){
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
        if (HP <= 0) Kill();
    }

    public void Kill()
    {
        audioSource.pitch = Random.Range(0.5f, 0.76f);
        audioSource.Play();
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
    public int GetIndex()
    {
        return index;
    }
}