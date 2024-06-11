using UnityEngine;
public class PlayerHealthBar : HealthBar {
    [SerializeField] private TweenAnimator tweenAnimator;
    void Awake(){
        if(tweenAnimator == null) tweenAnimator = GetComponent<TweenAnimator>();
    } 
    public override void Set(float value)
    {
        tweenAnimator.JellyAnimation();
        base.Set(value);
    }
}