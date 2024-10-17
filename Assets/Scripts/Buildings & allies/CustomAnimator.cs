using System;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimator : MonoBehaviour{
    public SpriteRenderer spriteRenderer;
    public Animation[] animations;
    public DirectionAnimation[] directionAnimations;
    public UnityEvent[] actions;
    private int currentAnimation;
    private int currentDirAnimation;
    private int currentFrame;
    float time = 0;
    public void Init(){
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation(0);
    }
    public void PlayAnimation(int id){
        currentAnimation = id;
        time = 0;
        currentFrame = 0;
        spriteRenderer.sprite = animations[id].sprites[currentFrame];
        currentDirAnimation = -1;
    }
    public void UpdateAnimator(float delta){
        time += delta;
        Animation tempAnim;
        if (currentAnimation >= 10)
        {
            tempAnim = directionAnimations[currentDirAnimation].animations[currentAnimation - 10];
            spriteRenderer.flipX = tempAnim.flipX;
        }
        else
        {
            tempAnim = animations[currentAnimation];
        }
        if (time >= tempAnim.interval)
        {
            time = 0;
            CheckEvents();
            spriteRenderer.sprite = tempAnim.sprites[currentFrame++];
            if (currentFrame >= tempAnim.sprites.Length)
            {
                currentFrame = 0;
            }
        }
    }
    public void CheckEvents()
    {
        AnimationEvent[] events;
        if (currentAnimation >= 10)
        {
            events = directionAnimations[currentDirAnimation].animations[currentAnimation - 10].events;
        }
        else
        {
            events = animations[currentAnimation].events;
        }
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].frame == currentFrame) actions[events[i].acitonID]?.Invoke();
        }
    }
    public void SetAnimation(int animation){
        if(currentAnimation == animation) return;
        PlayAnimation(animation);
    }
    public void SetDirectionAnimation(int dirID, Vector2 directionNormalized)
    {
        DirectionAnimation temp = directionAnimations[dirID];
        float degree = Vector2.SignedAngle(Vector2.right, directionNormalized);
        degree += temp.offset;
        if (degree < 0) degree += 360;
        degree %= 360;
        int res = (int)(degree / temp.step);
        //Debug.Log($"{degree}, {res}");
        if (currentDirAnimation != dirID) {
            currentFrame = 0;
            time = 0;
            currentAnimation = res + 10;
            spriteRenderer.sprite = directionAnimations[dirID].animations[res].sprites[currentFrame];
        }
        else if (currentAnimation != res)
        {
            currentAnimation = res + 10;
            spriteRenderer.sprite = directionAnimations[dirID].animations[res].sprites[currentFrame];
        }
        currentDirAnimation = dirID ;
    }
    public void SetSortingParams(int order, int layer){
        currentAnimation = 0;
        spriteRenderer.sortingOrder = order;
        spriteRenderer.sortingLayerName = $"{layer}";
    }
}
[Serializable]
public struct Animation{
    public AnimationEvent[] events;
    public Sprite[] sprites;
    public float duration;
    public bool flipX;
    public float interval{
        get{
            return duration / sprites.Length;
        }
    }
}
[Serializable]
public struct DirectionAnimation
{
    public Animation[] animations;
    public readonly int length => animations.Length;
    public readonly float step => 360f / length;
    public float offset;
}
[Serializable]
public struct AnimationEvent
{
    public int acitonID;
    public int frame;
}
/*[Serializable]
public struct AnimationFrame
{
    public Sprite sprite;
    public float duration;
}*/