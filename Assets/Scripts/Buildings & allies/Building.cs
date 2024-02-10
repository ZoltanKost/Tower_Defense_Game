using DG.Tweening;
using UnityEngine;
[CreateAssetMenu(menuName = "Buildings")]
public class Building : ScriptableObject{
    public int width = 2;
    public int height = 2;
    public SpriteRenderer sprite;
    public void Build(Vector3 worldPosition, int floor){
        worldPosition.z = 0;
        Debug.Log("Built: " + worldPosition + " " + floor);
        SpriteRenderer s = Instantiate(sprite, worldPosition, Quaternion.identity);
        s.sortingOrder = floor;
        Animate(s.transform);
    }
    public void Animate(Transform transform){
        Tween tween = transform.DOScale(.99f, .05f);
        tween.onComplete += () => {
            Tween tween1 = transform.DOScale(1.01f, .1f);
            tween1.onComplete += () => transform.DOScale(1, .05f);
        };
    }
}