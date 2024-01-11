using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(menuName ="2D/Tiles/Building")]
public class Building : Tile{
    [SerializeField] private float tickTime = 5f;
    [SerializeField] private float detectionRadius;
    float time;
    int posX,posY;
    public void Build(){
    }
    public void Tick(float delta){
        time += delta;
        if(time >= tickTime){
            time = 0;
            Debug.Log("Shoot!");
        }
    }
}