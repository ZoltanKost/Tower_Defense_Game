using System;
using UnityEngine;
using UnityEngine.Events;

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
        spriteRenderer.sprite = animations[currentAnimation].sprites[0];
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
    public struct Event{
        public UnityEvent action;
        public int actionPoint;
    }
    public event Action onAnimationEnd;
    public Event[] events;
    public Sprite[] sprites;
    public Sprite nextSprite{
        get{
            if (number >= sprites.Length) { 
                number = 0;
                onAnimationEnd?.Invoke();
            }
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