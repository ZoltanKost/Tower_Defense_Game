using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour{
    
    [SerializeField] public SpriteRenderer spriteRenderer;
    public Animation[] animations;
    private int currentAnimation;
    float time = 0;
    public void Init(){
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation(0);
    }
    public void PlayAnimation(int id){
        currentAnimation = id;
        time = 0;
        animations[currentAnimation].Reset();
        spriteRenderer.sprite = animations[currentAnimation].nextSprite;
    }
    public void UpdateAnimator(float delta){
        time += delta;
        if (time >= animations[currentAnimation].interval){
            time = 0;
            spriteRenderer.sprite = animations[currentAnimation].nextSprite;
        }
    }
    public void SetAnimation(int animation){
        if(currentAnimation == animation) return;
        PlayAnimation(animation);
    }
    public void SetSortingParams(int order, int layer){
        currentAnimation = 0;
        spriteRenderer.sortingOrder = order;
        spriteRenderer.sortingLayerName = $"{layer}";
    }
}
public delegate void AnimationDelegate();
[Serializable]
public struct Animation{
    [Serializable]
    public struct AnimationEvent
    {
        public string name;
        public int frame;
    }
    [Serializable]
    public struct AnimationFrame
    {
        public Sprite sprite;
        public float duration;
    }
    public AnimationEvent onAnimationEnd;
    public AnimationEvent[] events;
    public AnimationFrame[] frames;
    public int currentFrame;
    public List<string> eventsToInvoke;
    public Sprite nextSprite{
        get{
            CheckEvents();
            Sprite sprite = frames[currentFrame++].sprite;
            if (currentFrame >= frames.Length)
            {
                currentFrame = 0;
                eventsToInvoke.Add(onAnimationEnd.name);
            }
            return sprite;
        }
    }
    public float duration;
    public float interval{
        get{
            float res = frames[currentFrame].duration;
            if (res == 0)
            {
                res = duration / frames.Length;
            }
            return res;
        }
    }
    public void CheckEvents(){
        foreach(AnimationEvent e in events){
            if(e.frame == currentFrame) eventsToInvoke.Add(e.name);
        }
    }
    public void Reset(){
        currentFrame = 0;
    }
}