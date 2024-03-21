using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomAnimator : MonoBehaviour{
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Animation[] animations;
    private Animation currentAnimation;
    float time = 0;
    void Awake(){
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void PlayAnimation(Animation animation){
        currentAnimation = animation;
        time = 0;
        currentAnimation.Reset();
    }
    public void PlayAnimation(int id){
        Debug.Log($"Playing Animation: {id}");
        currentAnimation = animations[id];
        time = 0;
        currentAnimation.Reset();
    }
    public void UpdateAnimator(float delta){
        time += delta;
        if(time >= currentAnimation.interval){
            time = 0;
            spriteRenderer.sprite = currentAnimation.nextSprite;
        }
    }
    public void SetAnimation(Animation animation){
        if(currentAnimation == animation) return;
        PlayAnimation(currentAnimation);
    } 
    public void SetAnimation(int animation){
        if(currentAnimation == animations[animation]) return;
        PlayAnimation(animation);
    }
}
public delegate void AnimationDelegate();
[Serializable]
public class Animation{
    public UnityEvent action;
    public int actionPoint;
    public Sprite[] sprites;
    public Sprite nextSprite{
        get{
            if(number >= sprites.Length - 1) number = -1;
            if(number == actionPoint) InvokeAction();
            return sprites[++number];
        }
    }
    public float duration;
    private int number;
    public float interval{
        get{
            return duration / sprites.Length;
        }
    }
    public void Reset(){
        number = -1;
    }
    public void InvokeAction(){
        action?.Invoke();
    }
    public void SetAction(UnityEvent animDelegate){
        action = animDelegate;
    }
}