using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TemporalFloor : Floor
{
    [SerializeField] private float tweenSpeed = 30f;
    [SerializeField] private bool FixedDelta;
    [SerializeField] private float amplitude_overshoot = 1.15f;
    [SerializeField] private Ease moveEase;
    [SerializeField] private Ease nonSnapEase;
    [SerializeField] private Color canPlace;
    [SerializeField] private Color blockPlace;
    private Color currentColor;
    [SerializeField] SpriteRenderer visual;
    private int cellSize;
    Vector3 currentCursorPosition;
    [SerializeField] Transform[] arrows;
    Vector3 targetPosition;
    bool activated;
    bool snap;
    Tween tween;
    Vector3 startPosition;
    Vector3 endPosition;
    bool floodBuild;
    ActionMode mode;
    void Start()
    {
        cellSize = Mathf.FloorToInt(visuals[0].cellSize.x);
        currentColor = canPlace;
        foreach(Tilemap tilemap in visuals){
            tilemap.color = currentColor;
        }
    }
    public void CreateGroundArray(Vector3Int pos,  GroundArray ga){
        pos.z = 0;
        floor = ga.targetFloor;
        arrows[0].localPosition = pos;
        arrows[1].localPosition = pos + Vector3Int.right * ga.width * cellSize;
        arrows[2].localPosition = pos + Vector3Int.up * ga.height * cellSize;
        arrows[3].localPosition = pos + new Vector3Int(ga.width, ga.height) * cellSize;
        foreach(GACell g in ga.grounds){
            CreateGround(pos + g.position);
        }
    }
    public void MoveTempFloor(Vector3 position, bool canBuild) {
        if (mode == ActionMode.Command) return;
        if (mode == ActionMode.None)
        {
            position.z = 0;
            transform.position = position;
            return;
        }
        if (mode == ActionMode.MassGround) MoveFloodBuild(position);
        targetPosition = position / cellSize;
        targetPosition.z = 0;
        currentColor = canBuild ? Color.white : blockPlace;
        UpdateColors();
        if (!snap || mode == ActionMode.CastSpell)
        {
            //targetPosition = new Vector2 { x = position.x, y = position.y };
            if((targetPosition - currentCursorPosition).magnitude > .3f)
            {
                tween.Kill();
                tween = transform.DOMove(targetPosition, (targetPosition - currentCursorPosition).magnitude * .5f * tweenSpeed * (FixedDelta ? Time.fixedDeltaTime : Time.deltaTime)).SetEase(nonSnapEase, amplitude_overshoot);
                currentCursorPosition = targetPosition;
            }
            return;
        }
        targetPosition.x = Mathf.FloorToInt(targetPosition.x);
        targetPosition.y = Mathf.FloorToInt(targetPosition.y);
        targetPosition.z = 0;
        if (targetPosition != currentCursorPosition)
        {
            tween.Kill();
            tween = transform.DOMove(targetPosition, (targetPosition - currentCursorPosition).magnitude * tweenSpeed * (FixedDelta ? Time.fixedDeltaTime : Time.deltaTime)).SetEase(moveEase, amplitude_overshoot);
            currentCursorPosition = targetPosition;
        }
    }
    public void MoveFloodBuild(Vector3 position)
    {
        endPosition = position;

    }
    public void SetGroundArray(GroundArray array){
        ClearAllTiles();
        visual.gameObject.SetActive(false);
        CreateGroundArray(Vector3Int.zero, array);
    }
    public void SetBuilding(Building building){
        visual.sprite = building.sprite;
        Vector3 offset = (building.width % 2 == 0? (building.width / 2) : (float)building.width/2) * Vector3.right;
        visual.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
        visual.color = visuals[0].color;
        arrows[0].localPosition = Vector3.zero * cellSize;
        arrows[1].localPosition = Vector3Int.right * building.width * cellSize;
        arrows[2].localPosition = Vector3Int.up * building.height * cellSize;
        arrows[3].localPosition = new Vector3Int(building.width, building.height) * cellSize;
    }
    public void SetObject(ActionMode mode){
        switch(mode){
            case ActionMode.Bridge: 
            PlaceBridge(Vector3Int.zero);
            break;
            case ActionMode.BridgeSpot:
            PlaceBridge(Vector3Int.zero);
            break;
            case ActionMode.Road:
            PlaceRoad(Vector3Int.zero);
            break;
        }
        arrows[0].localPosition = Vector3.zero * cellSize;
        arrows[1].localPosition = Vector3Int.right * cellSize;
        arrows[2].localPosition = Vector3Int.up * cellSize;
        arrows[3].localPosition = Vector3Int.one * cellSize;
    }
    public void ActivateFloor(GroundArray ga){
        mode = ActionMode.Building;
        snap = true;
        SetGroundArray(ga);
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
    }
    public void ActivateFloor(Building b){
        mode = ActionMode.Building;
        snap = true;
        SetHighlightedCharacter(default, new Vector2Int(b.width, b.height),b.attackRange);
        foreach (var ar in arrows){
            ar.gameObject.SetActive(true);
        }
        SetBuilding(b);
        visual.gameObject.SetActive(true);
    }
    public void ActivateFloor(ActionMode m){
        mode = ActionMode.Building;
        snap = true;
        ClearAllTiles();
        visual.gameObject.SetActive(false);
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
        SetObject(m);
    }
    public void ActivateFloor(SpellData data)
    {
        mode = ActionMode.CastSpell;
        snap = false;
        ClearAllTiles();
        Debug.Log("Activated, spell");
        foreach (var ar in arrows)
        {
            ar.gameObject.SetActive(true);
        }
        SetSpell(data);
        visual.gameObject.SetActive(true);
    }
    public void DeactivateFloor(){
        ClearAllTiles();
        visual.gameObject.SetActive(false); 
        foreach(var ar in arrows){
            ar.gameObject.SetActive(false);
        }
        mode = ActionMode.None;
    }
    public void SetMode(ActionMode _mode)
    {
        mode = _mode;
        visual.gameObject.SetActive(true);
        foreach (var ar in arrows)
        {
            ar.gameObject.SetActive(true);
        }
    }
    public override void Animate(){
        tweenAnimator.ErrorAnimation();
    }
    public void JellyAnimation()
    {
        tweenAnimator.JellyAnimation();
    }
    public override Tween GetAnimationTween(){
        return tweenAnimator.ErrorAnimation();
    }
    public void UpdateColors(){
        foreach(Tilemap tilemap in visuals){
            tilemap.color = currentColor;
        }
        visual.color = currentColor;
    }
    public void SetSpell(SpellData spellData)
    {
        visual.sprite = spellData.UIicon;
        float offsetF = spellData.radius / cellSize;
        visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //visual.color = visuals[0].color;
        arrows[0].localPosition = new Vector3(-offsetF, -offsetF);
        arrows[1].localPosition = new Vector3(offsetF, - offsetF);
        arrows[2].localPosition = new Vector3(-offsetF, offsetF);
        arrows[3].localPosition = new Vector3(offsetF, offsetF);
    }
    public void SetArrows(Vector2Int gridPosition, Vector3Int rectSize)
    {
        foreach (var ar in arrows)
        {
            ar.gameObject.SetActive(true);
        }
        Vector3 position = new Vector3(gridPosition.x, gridPosition.y) * cellSize - new Vector3(1,1) * 50;
        transform.position = position;
        arrows[0].localPosition = Vector3.zero * cellSize;
        arrows[1].localPosition = Vector3Int.right * rectSize.x * cellSize;
        arrows[2].localPosition = Vector3Int.up * rectSize.y * cellSize;
        arrows[3].localPosition = rectSize * cellSize;
    }
    public void StartFlood(Vector3 start)
    {
        startPosition = start;
    }
    public void SetHighlightedCharacter(Vector2Int gridPosition, Vector2Int buildingSize, int attackRange)
    {
        foreach (Tilemap map in visuals)
        {
            map.ClearAllTiles();
        }
        visual.gameObject.SetActive(false);
        for (int y = - attackRange; y < attackRange + buildingSize.y; y++)
        {
            for(int x = - attackRange; x < attackRange + buildingSize.x; x++)
            {
                Vector2Int offset = new Vector2Int 
                {
                    x = Mathf.Clamp(x, 0, buildingSize.x - 1),
                    y = Mathf.Clamp(y, 0, buildingSize.y - 1)
                };
                int posX = x - offset.x;
                int posY = y - offset.y;
                if (Mathf.Abs(posX) + Mathf.Abs(posY) > attackRange) continue;
                SetTile(new Vector3Int(x, y), 1, TileID.AttackRangeShadow);
            }
        }
        if(gridPosition != default) transform.position = gridPosition - Vector2.one * 50;
    }
}
