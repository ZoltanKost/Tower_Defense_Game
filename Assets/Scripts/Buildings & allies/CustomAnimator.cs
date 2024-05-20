using System;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimator : MonoBehaviour{
    
    [SerializeField] public SpriteRenderer spriteRenderer;
    public Animation[] animations;
    private Animation currentAnimation;
    float time = 0;
    public void Init(){
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation(0);
    }
    public void PlayAnimation(Animation animation){
        currentAnimation = animation;
        time = 0;
        currentAnimation.Reset();
    }
    public void PlayAnimation(int id){
        currentAnimation = animations[id];
        time = 0;
        currentAnimation.Reset();
        spriteRenderer.sprite = currentAnimation.sprites[0];
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
    public void SetSortingParams(int order, int layer){
        currentAnimation = animations[0];
        spriteRenderer.sortingOrder = order;
        spriteRenderer.sortingLayerName = $"{layer}";
    }
}
public delegate void AnimationDelegate();
[Serializable]
public class Animation{
    [Serializable]
    public struct Event{
        public UnityEvent action;
        public int actionPoint;
    }
    public Event[] events;
    public Sprite[] sprites;
    public Sprite nextSprite{
        get{
            if(number >= sprites.Length) number = 0;
            CheckEvents();
            return sprites[number++];
        }
    }
    public float duration;
    private int number;
    public float interval{
        get{
            return duration / sprites.Length;
        }
    }
    public void CheckEvents(){
        foreach(Event e in events){
            if(e.actionPoint == number) e.action?.Invoke();
        }
    }
    public void Reset(){
        number = 0;
    }
}