using System;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimator : MonoBehaviour{
    public SpriteRenderer spriteRenderer;
    public Animation[] animations;
    public UnityEvent[] actions;

    private Animation currentAnimation;
    private int animID;
    private int currentDirAnimation;
    float time = 0;

    int lastPlayedEvent;

    public void Init(){
        time = 0; animID = 0;
        lastPlayedEvent = 0;
        PlayAnimation(0);
    }
    // TODO: check if first animation s
    public void InitFromPrefab(CustomAnimator animator)
    {
        animations = animator.animations;
        animID = 0;
        time = 0;
        lastPlayedEvent = 0;
        currentAnimation = animations[animID];
        currentDirAnimation = currentAnimation.type == 0? - 1 : 0;
        lastPlayedEvent = 0;
    }
    public void InitFromAnimArray(Animation[] array)
    {
        animations = array;
        animID = 0;
        currentAnimation = animations[animID];
        time = 0;
        lastPlayedEvent = 0;
        //spriteRenderer.sprite = animations[0].sprites[currentFrame];
    }
    public void PlayAnimation(int id, float value = 0f){
        if (animations == null || animations.Length == 0) return;
        if (animID != id) time = 0;
        animID = id;
        currentAnimation = animations[animID];
        if(currentAnimation.type == 0)time = 0;
        value += currentAnimation.dirOffset;
        value %= 360;
        currentDirAnimation =
            currentAnimation.type == 0 ? 0 :
            Mathf.FloorToInt((int)(value/ currentAnimation.dirStep));
    }
    // TODO: wrong time;
    public void UpdateAnimator(float delta){
        if (animations == null || animations.Length == 0) return;
        
        time += delta;
        
        int currentData = 0;
        if (currentAnimation.type != 0) currentData = currentDirAnimation;
        
        var events = currentAnimation.events;
        int l = events.Length;
        if (lastPlayedEvent < l && time >= events[lastPlayedEvent].time)
        {
            actions[events[lastPlayedEvent].acitonID]?.Invoke();
            lastPlayedEvent++;
        }
        if (time > currentAnimation.duration)
        {
            time = 0;
            lastPlayedEvent = 0;
        }
        AnimationData data = currentAnimation.data[currentData];

        int spriteNum = (int)(time / currentAnimation.duration * data.sprites.Length);
        //Debug.Log($"sprite: {spriteNum} l:{data.sprites.Length} t: {time} dur: {currentAnimation.duration}");
        spriteRenderer.sprite = data.sprites[spriteNum];
        spriteRenderer.flipX = currentAnimation.data[currentData].flipX;
    }
    public void SetAnimation(int animation, float value){
        if(animID == animation && (currentAnimation.type == 0)) return;
        PlayAnimation(animation, value);
    }
    // TODO: remove this code to SetAnimation
    public void SetSortingParams(int order, int layer){
        spriteRenderer.sortingOrder = order;
        spriteRenderer.sortingLayerName = $"{layer}";
    }
}
[Serializable]
public struct AnimationData
{
    public Sprite[] sprites;
    public bool flipX;
}
[Serializable]
public struct AnimationEvent
{
    public string name;
    public int acitonID;
    public float time;
}
public enum AnimationType
{
    Default,
    Direction
}
[Serializable]
public struct Animation
{
    public string name;
    public AnimationType type;
    public AnimationEvent[] events;
    public AnimationData[] data;
    public float duration;
    public float dirStep;
    public float dirOffset;
}
/*
 * 1. Set Movement keyframes on individual transforms
 * 2. Set Rotation Keyframes on individual transforms
 * 3. Set Sprite Keyframes on Sprite Renderers
 * 4. Set child Sprite and Movement Keyframes
 * 5. Convert keyframes to f(x);
 * 5. Update everything with easing
 
 
 */