using System;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimator : MonoBehaviour{
    public SpriteRenderer spriteRenderer;
    public Animation[] animations;
    public UnityEvent[] actions;
    private int currentAnimation;
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
    }
    public void UpdateAnimator(float delta){
        time += delta;
        if (time >= animations[currentAnimation].interval){
            time = 0;
            CheckEvents();
            spriteRenderer.sprite = animations[currentAnimation].sprites[currentFrame++];
            if (currentFrame >= animations[currentAnimation].sprites.Length)
            {
                currentFrame = 0;
            }
        }
    }
    public void CheckEvents()
    {
        AnimationEvent[] events = animations[currentAnimation].events;
        for(int i = 0; i < events.Length; i++)
        {
            if (events[i].frame == currentFrame) actions[events[i].acitonID]?.Invoke();
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
[Serializable]
public struct Animation{
    public AnimationEvent[] events;
    public Sprite[] sprites;
    public float duration;
    public float interval{
        get{
            return duration / sprites.Length;
        }
    }
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