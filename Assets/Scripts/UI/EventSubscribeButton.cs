using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventSubscribeButton : MonoBehaviour{
    [SerializeField] private protected Button targetButton;
    [SerializeField] private AudioSource audioSource;
    public bool playSound;
    Action callback;
    public void Init(Action cb){
        if(targetButton == null) targetButton = GetComponent<Button>();
        if(cb == null) return;
        callback = cb;
        //targetButton.onClick.RemoveAllListeners();
        targetButton.onClick.AddListener(OnClick);
    }
    void OnClick() 
    {
        callback?.Invoke();
        if (playSound)
        {
            audioSource.Stop();
            audioSource.pitch = UnityEngine.Random.Range(0.5f, 1f);
            audioSource.Play();
        }
    }
}