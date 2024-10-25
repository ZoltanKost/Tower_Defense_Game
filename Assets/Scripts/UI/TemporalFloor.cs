using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TemporalFloor : Floor
{
    [SerializeField] private float tweenSpeed = 30f;
    [SerializeField] private bool FixedDelta;
    [SerializeField] private float amplitude_overshoot = 1.15f;
    [SerializeField] private Ease moveEase;
    [SerializeField] private Color canPlace;
    [SerializeField] private Color blockPlace;
    private Color currentColor;
    [SerializeField] SpriteRenderer visual;
    private int cellSize;
    Vector3Int currentPosition;
    [SerializeField] Transform[] arrows;
    Vector3Int temp;
    bool activated;
    Tween tween;
    void Start(){
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
        foreach(Vector3Int g in ga.grounds){
            CreateGround(pos + g);
        }
    }
    public void MoveTempFloor(Vector3 position, bool canBuild){
        Vector3 vector = position / cellSize;
        temp.x = Mathf.FloorToInt(f: vector.x);
        temp.y = Mathf.FloorToInt(vector.y);
        temp.z = 0;
        currentColor = canBuild?Color.white: blockPlace;
        UpdateColors();
        if (temp != currentPosition){
            tween.Kill();
            if(activated){
                tween = transform.DOMove(temp, (temp - currentPosition).magnitude * tweenSpeed * (FixedDelta?Time.fixedDeltaTime:Time.deltaTime)).SetEase(moveEase,amplitude_overshoot);
            }else{
                transform.position = temp;
            }
            currentPosition = temp;
        }
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
        activated = true;
        SetGroundArray(ga);
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
    }
    public void ActivateFloor(Building b){
        activated = true;
        ClearAllTiles();
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
        SetBuilding(b);
        visual.gameObject.SetActive(true);
    }
    public void ActivateFloor(ActionMode m){
        activated = true;
        ClearAllTiles();
        visual.gameObject.SetActive(false);
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
        SetObject(m);
    }
    public void ActivateFloor(SpellData data)
    {
        activated = true;
        ClearAllTiles();
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
        activated = false;
    }
    public override void Animate(){
        tweenAnimator.ErrorAnimation();
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
        Vector3 offset = (spellData.radius % 2 == 0 ? (spellData.radius/ 2) : (float)spellData.radius / 2) * Vector3.right;
        visual.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
        visual.color = visuals[0].color;
        arrows[0].localPosition = Vector3.zero * cellSize;
        arrows[1].localPosition = Vector3Int.right * spellData.radius * cellSize;
        arrows[2].localPosition = Vector3Int.up * spellData.radius * cellSize;
        arrows[3].localPosition = new Vector3Int(spellData.radius, spellData.radius) * cellSize;
    }
    public void SetArrows(Vector2Int gridPosition, Vector3Int rectSize)
    {
        foreach (var ar in arrows)
        {
            ar.gameObject.SetActive(true);
        }
        Vector3 position = new Vector3(gridPosition.x, gridPosition.y) * cellSize;
        transform.position = position;
        arrows[0].localPosition = Vector3.zero * cellSize;
        arrows[1].localPosition = Vector3Int.right * rectSize.x * cellSize;
        arrows[2].localPosition = Vector3Int.up * rectSize.y * cellSize;
        arrows[3].localPosition = rectSize * cellSize;
    }
}
