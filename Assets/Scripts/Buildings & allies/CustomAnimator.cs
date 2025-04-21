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
    private int currentFrame;
    float time = 0;

    public void Init(){
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation(0);
    }
    // TODO: check if first animation s
    public void InitFromPrefab(CustomAnimator animator)
    {
        animations = animator.animations;
        animID = 0;
        currentAnimation = animations[animID];
        currentDirAnimation = currentAnimation.type == 0? - 1 : 0;
        currentFrame = 0;
        time = 0;
    }
    public void InitFromAnimArray(Animation[] array)
    {
        animations = array;
        animID = 0;
        currentAnimation = animations[animID];
        time = 0;
        currentFrame = 0;
        //spriteRenderer.sprite = animations[0].sprites[currentFrame];
    }
    public void PlayAnimation(int id, float value = 0f){
        animID = id;
        currentAnimation = animations[animID];
        time = 0;
        currentFrame = 0;
        value += currentAnimation.dirOffset;
        currentDirAnimation =
            currentAnimation.type == 0 ? 0 :
            Mathf.FloorToInt((int)(value/ currentAnimation.dirStep));
        
        //spriteRenderer.sprite = 
           // currentAnimation.data[currentDirAnimation].sprites[currentFrame];
    }
    // TODO: rework keyframe updating;
    public void UpdateAnimator(float delta){
        time += delta;
        Sprite[] tempAnim;
        float interval;
        int currentData = 0;
        if (currentAnimation.type != 0) currentData = currentDirAnimation;
        interval = currentAnimation.interval;
        tempAnim = currentAnimation.data[currentData].sprites;
        spriteRenderer.flipX = currentAnimation.data[currentData].flipX;
        
        for (int i = 0; i < currentAnimation.keyFrames.Length; i++)
        {
            var keyFrame = currentAnimation.keyFrames[i];
            if (time > keyFrame.frameTimings[keyFrame.currentFrame])
            {
                if (keyFrame.currentFrame >= keyFrame.length)
                {
                    keyFrame.currentFrame = 0;
                }
                else
                {
                    keyFrame.currentFrame++;
                }
            }
            if (keyFrame.currentFrame < keyFrame.length)
            {
                float dt = keyFrame.frameTimings[keyFrame.currentFrame + 1]
                    - keyFrame.frameTimings[keyFrame.currentFrame];
                var start = keyFrame.framePositions[keyFrame.currentFrame];
                var end = keyFrame.framePositions[keyFrame.currentFrame + 1];
                var result = (end - start) * dt;
                var startR = keyFrame.frameRotations[keyFrame.currentFrame];
                var endR = keyFrame.frameRotations[keyFrame.currentFrame + 1];
                var resultR = (endR - startR) * dt;
                keyFrame.target.position = result;
                keyFrame.target.LookAt(resultR);
            }
        }

        if (time >= interval * currentFrame)
        {
            CheckEvents();
            spriteRenderer.sprite = tempAnim[currentFrame++];
            if (currentFrame >= currentAnimation.length)
            {
                currentFrame = 0;
            }
        }
        
    }
    public void CheckEvents()
    {
        AnimationEvent[] events = currentAnimation.events;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].frame == currentFrame) { 
                actions[events[i].acitonID]?.Invoke();
            }
        }
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
public struct AnimationKeyframes
{
    public Transform target;
    public Vector3[] framePositions;
    public Vector3[] frameRotations;
    public float[] frameTimings;
    public int currentFrame;
    public int length;
}
[Serializable]
public struct DirectionAnimation
{
    public AnimationEvent[] events;
    public AnimationData[] data;
    public float duration;
    public int length;
    public float step;
    public float offset;
}
[Serializable]
public struct AnimationEvent
{
    public int acitonID;
    public int frame;
}
public enum AnimationType
{
    Sprites,
    KeyFrames
}

public struct Animation
{
    public AnimationType type;
    public AnimationEvent[] events;
    public AnimationData[] data;
    public AnimationKeyframes[] keyFrames;
    public float duration;
    public float interval;
    public int length;
    public float dirStep;
    public float dirOffset;
}