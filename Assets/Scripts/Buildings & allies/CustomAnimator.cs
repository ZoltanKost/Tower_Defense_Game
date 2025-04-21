using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

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
    public void UpdateAnimator(float delta){
        time += delta;
        Sprite[] tempAnim;
        float interval;
        if (currentAnimation.type != 0)
        {
            tempAnim = currentAnimation.data[currentDirAnimation].sprites;
            spriteRenderer.flipX = currentAnimation.data[currentDirAnimation].flipX;
            interval = currentAnimation.duration / currentAnimation.length;
        }
        else
        {
            tempAnim = currentAnimation.data[0].sprites;
            interval = currentAnimation.interval;
        }
        if (time >= interval)
        {
            time = 0;
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
    public void SetDirectionAnimation(int dirID, Vector2 directionNormalized)
    {
        if (dirID >= currentAnimation.length) Debug.Log("dirID " + dirID + " is outside of array");
        float degree = Vector2.SignedAngle(Vector2.right, directionNormalized);
        if (degree < 0) degree += 360;
        degree %= 360;


        
        /*//Debug.Log($"{degree}, {res}");
        if (currentDirAnimation != dirID) {
            currentFrame = 0;
            time = 0;
            currentDirAnimation = res;
            spriteRenderer.sprite = currentAnimation.data[res].sprites[currentFrame];
        }
        else if (animID != res)
        {
            animID = res;
            currentAnimation = animations[animID];
            spriteRenderer.sprite = currentAnimation.data[res].sprites[currentFrame];
        }
        currentDirAnimation = dirID ;*/
    }
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
    public Vector3[] framePositions;
    public float[] frameTimings;
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