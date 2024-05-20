using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(GridLayoutGroup))]
public class HideShowUI : MonoBehaviour {
    [SerializeField] private Vector3 hideDeltaPosition = Vector3.right;
    [SerializeField] private Ease ease = Ease.OutElastic;
    [SerializeField] private float amplitude,period,duration;
    [SerializeField] private bool horizontalMove;
    Vector3 hidePosition,showPosition;
    Tween currentTween;
    void Awake(){
        float multiplier;
        if(horizontalMove){
           multiplier = GetComponent<GridLayoutGroup>().cellSize.x;
        }else{
           multiplier = GetComponent<GridLayoutGroup>().cellSize.y;
        }
        hidePosition = transform.localPosition + hideDeltaPosition * multiplier;
        showPosition = transform.localPosition;
    }
    public void ShowUI(){
        currentTween?.Kill();
        transform.DOLocalMove(showPosition,duration).SetEase(ease,amplitude,period);
    }
    public void HideUI(){
        currentTween?.Kill();
        transform.DOLocalMove(hidePosition,duration).SetEase(ease,amplitude,period);
    }
}