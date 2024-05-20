using UnityEngine;
using DG.Tweening;

public class TweenAnimator : MonoBehaviour {
    [SerializeField] private protected float animationTime, startSize, amplitude_overshoot;
    [SerializeField] private protected Ease animEase = Ease.InSine;
    Tween errorAnim_left => transform.DOPunchPosition(Vector3.left * .5f,animationTime / 2);
    Tween errorAnim_right => transform.DOPunchPosition(Vector3.right * .5f,animationTime / 2);
    public Tween JellyAnimation(){
        transform.localScale = Vector3.one * startSize;
        // Tween tween = 
        return transform.DOScale(1, animationTime).SetEase(animEase,amplitude_overshoot);
        // tween.onComplete += () => transform.DOScale(1f, animationTime).SetEase(animEase);
    }
    public Tween ErrorAnimation(){
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Random.Range(0,2) == 0 ? errorAnim_left : errorAnim_right);
        return sequence;
    }
}