using UnityEngine;

public class ShopTab : EventSubscribeButton {
    [SerializeField] private TweenAnimator tweenAnimator;
    int id;
    public void Init(UI_ID_Callback action, int ID){
        id = ID;
        // GetComponentInChildren<Image>().sprite = sprite;
        Init(() => 
            {
                action?.Invoke(id);
                Animate();
            }
        );
        if(tweenAnimator == null) tweenAnimator = GetComponent<TweenAnimator>();
    }
    public void Animate(){
        tweenAnimator.JellyAnimation();
    }
}