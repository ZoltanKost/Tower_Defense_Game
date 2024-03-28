using DG.Tweening;
using UnityEngine;

public class TemporalFloor : Floor
{
    [SerializeField] private float lerpSpeed = 30f;
    [SerializeField] private float jumpingSize = 3f;
    private Vector3Int floorStart;
    private int cellSize;
    Vector3Int currentPosition;
    [SerializeField] Transform[] arrows;
    Vector3Int temp;
    GroundArray ground;
    bool activated;
    void Start(){
        floorStart = visuals[0].WorldToCell(transform.position);
        cellSize = Mathf.FloorToInt(visuals[0].cellSize.x);
    }
    public void CreateGroundArray(Vector3Int pos,  GroundArray ga){
        pos.z = 0;
        layer = ga.targetFloor;
        arrows[0].localPosition = pos;
        arrows[1].localPosition = pos + Vector3Int.right * ga.width * cellSize;
        arrows[2].localPosition = pos + Vector3Int.up * ga.height * cellSize;
        arrows[3].localPosition = pos + new Vector3Int(ga.width, ga.height) * cellSize;
        foreach(Vector3Int g in ga.grounds){
            CreateGround(pos + g);
        }
    }
    public void MoveTempFloor(Vector3 position){
        if(!activated) return;
        Vector3 vector = position / cellSize;
        temp.x = Mathf.FloorToInt(vector.x);
        temp.y = Mathf.FloorToInt(vector.y);
        temp.z = 0;
        if (temp != currentPosition){
            if((temp - currentPosition).magnitude > jumpingSize){
                transform.position = temp;
                currentPosition = temp;
                return;
            }
            transform.DOMove(temp,lerpSpeed / (temp - currentPosition).magnitude,false);
            currentPosition = temp;
        }
    }
    public void SetGroundArray(GroundArray array){
        ground = array;
        currentPosition = Vector3Int.zero;
        ClearAllTiles();
        CreateGroundArray(currentPosition, array);
    }
    public void ActivateFloor(GroundArray ga){
        activated = true;
        SetGroundArray(ga);
        foreach(var ar in arrows){
            ar.gameObject.SetActive(true);
        }
    }
    public void DeactivateFloor(){
        // transform.position = floorStart;
        ClearAllTiles();
        foreach(var ar in arrows){
            ar.gameObject.SetActive(false);
        }
        activated = false;
    }
}
