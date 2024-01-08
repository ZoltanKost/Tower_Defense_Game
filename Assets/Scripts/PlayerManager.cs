using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{    
    [SerializeField] private TileBase tile;
    [SerializeField] private GridVisual gridVisual;
    Camera main;
    void Awake(){
        main = Camera.main;
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){ 
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = main.ScreenPointToRay(mousePosition);
            float a = -ray.origin.z/ray.direction.z;
            float x = ray.origin.x + ray.origin.x * a;
            float y = ray.origin.y + ray.origin.y * a;
            Vector3 position = new Vector3(x,y);
            gridVisual.SetTile(position,tile,0);
        }
    }
}
