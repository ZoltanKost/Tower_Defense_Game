using UnityEngine;
using DG.Tweening;

public class HideShowUI : MonoBehaviour {
    [SerializeField] private Vector3 hideDeltaPosition = Vector3.right;
    [SerializeField] private Ease ease = Ease.OutElastic;
    [SerializeField] private float amplitude,period,duration;
    [SerializeField] private bool horizontalMove;
    [SerializeField] private float xMove;
    Tween currentTween;
    public void ShowUI(){
        currentTween?.Complete();
        currentTween = transform.DOLocalMove(transform.localPosition + xMove * hideDeltaPosition,duration).SetEase(ease,amplitude,period);
    }
    public void HideUI(){
        currentTween?.Complete();
        currentTween = transform.DOLocalMove(transform.localPosition - xMove * hideDeltaPosition,duration).SetEase(ease,amplitude,period);
    }
}